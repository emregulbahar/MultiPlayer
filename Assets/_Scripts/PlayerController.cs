using System;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private MyPlayerInput _myPlayerInput;
    [SerializeField] private AgentMover _agentMover;

    [SerializeField] private InteractionDetector _interactionDetector;
    [SerializeField] private  Animator _animator;
    [SerializeField] private  AnimationEvents _animationEvents;


    private bool _isInteracting, _isChooping;
    [SerializeField] private GameObject _axeModel, _picaxeModel, _woodModel, _stoneModel;
    private ResourceSpawner _resourceSpawner;
    private NetworkVariable<ulong>_heldNetworkObjectId = new(ulong.MaxValue);
    private NetworkVariable<ObjectType> _heldObjectType = new(ObjectType.None);

    private void Awake()
    {
        _resourceSpawner = FindAnyObjectByType<ResourceSpawner>();
    }

    private void OnEnable()
    {
        _myPlayerInput.OnPickUpPressed += HandlePickUpPressed;
        _myPlayerInput.OnInteractPressed += HandheActionsPressed;
    }

    private void HandheActionsPressed()
    {
        if(IsOwner == false)
        {
            return;
        }
        if(_isChooping || _isInteracting)
        {
            return;
        }
        if(_heldObjectType.Value is ObjectType.Axe or ObjectType.PickAxe)
        {
            _animator.SetTrigger("Chop");
            _isChooping = true;
        }
    }

    private void HandlePickUpPressed()
    {
        if (_isInteracting || _isChooping)
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
        _myPlayerInput.OnInteractPressed -= HandheActionsPressed;
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
            _animationEvents.OnChop += HandleChopAction;
        }
    }

    private void HandleChopAction()
    {
        if(_heldObjectType.Value is ObjectType.Axe or ObjectType.PickAxe)
        {
            if(_interactionDetector.ClosestInteractable is ResourceNode)
            {
                RequestResourceNodeInteractionServerRpc(_interactionDetector.ClosestInteractable.NetworkObject.NetworkObjectId);
            }
        }
    }

    [Rpc(SendTo.Server)]
    private void RequestResourceNodeInteractionServerRpc(ulong networkObjectId)
    {
        if(!NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out NetworkObject target))
        {
            return;
        }
        if(!target.TryGetComponent(out ResourceNode node))
        {
            return;
        }

        node.Harvest(_heldObjectType.Value);
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
        _isChooping = false;
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
        else
        {
            _resourceSpawner.SpawnResource(_heldObjectType.Value, transform.position);
        }

        _heldObjectType.Value = ObjectType.None;
        _heldNetworkObjectId.Value = ulong.MaxValue;
    }

    public override void OnNetworkDespawn()
    {
        _heldObjectType.OnValueChanged -= HandleHeldItemChance;
    
        if (IsServer && NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening)
        {
            DropCurrentItem();
        }

        if (IsOwner)
        {
            // DİKKAT: Buradaki RequestDropServerRpc() satırını sildik!
            _animationEvents.OnInteract -= HandleInteractActions;
            _animationEvents.OnAnimationDone -= HandleAnimationDone;
            _animationEvents.OnChop -= HandleChopAction;
        }
        
        base.OnNetworkDespawn();
    }

    
    void Update()
    {
        if(IsOwner == false)
        {
            return;
        }
        Vector2 movementInput = _myPlayerInput.MovementInput;
        if(_isChooping || _isInteracting)
        {
            movementInput = Vector2.zero;
        }
        _agentMover.Move(movementInput);
    }
}
