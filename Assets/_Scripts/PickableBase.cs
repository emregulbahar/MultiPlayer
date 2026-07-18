using System;
using Unity.Netcode;
using UnityEngine;

public abstract class PickableBase : NetworkBehaviour, IInteractable
{

    protected NetworkVariable<bool> _isAvaible = new(true, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    [SerializeField] private SelectionOutline _outline;
    [SerializeField] private  ObjectType _objectType;

    public bool CanBePickedUp => _isAvaible.Value;
    public ObjectType ObjectType => _objectType;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        _isAvaible.OnValueChanged += OnAvailabilityChanged;
        ApplyAvailabilityState(_isAvaible.Value);
    }

    public override void OnNetworkDespawn()
    {
        _isAvaible.OnValueChanged += OnAvailabilityChanged;
        base.OnNetworkDespawn();
        
    }

    private void OnAvailabilityChanged(bool previousValue, bool newValue)
    {
        ApplyAvailabilityState(newValue);
    }

    protected abstract void ApplyAvailabilityState(bool newValue);

    public void PickUp()
    {
        if(IsServer == false)
        {
            return;
        }

        _isAvaible.Value = false;
        OnPickedUP();
    }

    protected abstract void OnPickedUP();

    public void ToggleSelection(bool isSelected)
    {
        if(_outline != null)
        {
            _outline.ToggleOutline(isSelected);
        }
    }
}
