using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private MyPlayerInput _myPlayerInput;
    [SerializeField] private AgentMover _agentMover;

    [SerializeField] private InteractionDetector _interactionDetector;
    [SerializeField] private  Animator _animator;
    [SerializeField] private  AnimationEvents _animationEvents;


    private bool _isInteracting;

    private void OnEnable()
    {
        _myPlayerInput.OnPickUpPressed += HandlePickUpPressed;
    }

    private void OnDisable()
    {
        _myPlayerInput.OnPickUpPressed -= HandlePickUpPressed;
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

    public override void OnNetworkSpawn()
    {
        base.OnNetworkDespawn();
        _interactionDetector.Initialize(IsOwner);
        if (IsOwner)
        {
            _animationEvents.OnInteract += HandleInteractActions;
            _animationEvents.OnAnimationDone += HandleAnimationDone;
        }
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
        pickableItem.PickUp();
    }

    public override void OnNetworkDespawn()
    {
        
        if (IsOwner)
        {
            _animationEvents.OnInteract += HandleInteractActions;
            _animationEvents.OnAnimationDone += HandleAnimationDone;
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
        _agentMover.Move(movementInput);
    }
}
