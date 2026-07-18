using System;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private MyPlayerInput _myPlayerInput;
    [SerializeField] private AgentMover _agentMover;

    [SerializeField] private InteractionDetector _interactionDetector;
    [SerializeField] private  Animator _animator;
    [SerializeField] private  AnimationEvents _animationEvents;


    private bool _isInteracting;
    [SerializeField] private GameObject _axeModel, _picaxeModel, _woodModel, _stoneModel;
    private NetworkVariable<ulong>_heldNetworkObjectId = new(ulong.MaxValue);
    private NetworkVariable<ObjectType> _heldObjectType = new(ObjectType.None);

    private void OnEnable()
    {
        _myPlayerInput.OnPickUpPressed += HandlePickUpPressed;
    }

     private void HandlePickUpPressed()
    {
        if (_isInteracting)
        {
            return;
        }

        if(_interactionDetector.ClosestInteractable == null)
        {
            return;
        }
        _animator.SetBool("Interact", true);
        _isInteracting = true;
    }
    private void OnDisable()
    {
        _myPlayerInput.OnPickUpPressed -= HandlePickUpPressed;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkDespawn();
        _interactionDetector.Initialize(IsOwner);
        _heldObjectType.OnValueChanged += HandleHeldItemChance;
        HandleItemOnJoin();
        if (IsOwner)
        {
            _animationEvents.OnInteract += HandleInteractActions;
            _animationEvents.OnAnimationDone += HandleAnimationDone;
        }
    }

    private void HandleItemOnJoin()
    {
        if(_heldObjectType.Value != ObjectType.None)
        {
            HandleHeldItemChance(ObjectType.None, _heldObjectType.Value);
        }
    }

    private void HandleHeldItemChance(ObjectType previousValue, ObjectType newValue)
    {
        _axeModel.SetActive(newValue == ObjectType.Axe);
        _picaxeModel.SetActive(newValue == ObjectType.PickAxe);
        _woodModel.SetActive(newValue == ObjectType.Wood);
        _stoneModel.SetActive(newValue == ObjectType.Stone);
    }

    private void HandleAnimationDone()
    {
        _isInteracting = false;
    }

    private void HandleInteractActions()
    {
        if(_interactionDetector.ClosestInteractable is PickableBase)
        {
            RequestPickUpServerRpc(_interactionDetector.ClosestInteractable.NetworkObject.NetworkObjectId);
        }
    }

    [Rpc(SendTo.Server)]
    private void RequestPickUpServerRpc(ulong networkObjectId)
    {
        if(!NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out NetworkObject target))
        {
            return;
        }
        if(!target.TryGetComponent(out PickableBase pickableItem))
        {
            return;
        }
        if (!pickableItem.CanBePickedUp)
        {
            return;
        }
        if(_heldObjectType.Value != ObjectType.None)
        {
            DropCurrentItem();
        }
        if(pickableItem is PickableTool)
        {
            _heldNetworkObjectId.Value = networkObjectId;
        }

        _heldObjectType.Value = pickableItem.ObjectType;
        pickableItem.PickUp();
    }

    private void DropCurrentItem()
    {
        if(IsServer == false)
        {
            return;
        }
        if(_heldObjectType.Value == ObjectType.None)
        {
            _heldNetworkObjectId.Value = ulong.MaxValue;
            return;
        }
        if(_heldObjectType.Value is ObjectType.Axe or ObjectType.PickAxe)
        {
            if(NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(_heldNetworkObjectId.Value, out NetworkObject target))
            {
                if(target.TryGetComponent(out PickableTool pickableItem))
                {
                    pickableItem.Drop(transform.position);
                }
            }
        }
        _heldObjectType.Value = ObjectType.None;
        _heldNetworkObjectId.Value = ulong.MaxValue;
    }

    public override void OnNetworkDespawn()
    {
        _heldObjectType.OnValueChanged -= HandleHeldItemChance;
        if (IsOwner)
        {
            RequestDropServerRpc();
            _animationEvents.OnInteract -= HandleInteractActions;
            _animationEvents.OnAnimationDone -= HandleAnimationDone;
        }
        base.OnNetworkDespawn();
    }

    [Rpc(SendTo.Server)]
    private void RequestDropServerRpc()
    {
        DropCurrentItem();
    }

    void Update()
    {
        if(IsOwner == false)
        {
            return;
        }
        Vector2 movementInput = _myPlayerInput.MovementInput;
        _agentMover.Move(movementInput);
    }
}
