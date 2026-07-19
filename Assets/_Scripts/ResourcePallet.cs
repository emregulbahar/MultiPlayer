using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class ResourcePallet : NetworkBehaviour, IInteractable
{
    [SerializeField] private SelectionOutline _selectionOutline;

    [SerializeField]
    private List<ComponentController> _componentControllers;

    [SerializeField]
    private ObjectType _acceptedObjectType;

    private NetworkVariable<int> _stackedResources = new(0);
    public int StackedResoruces => _stackedResources.Value;
    public event Action OnPalletFilled;

    [SerializeField] private InteractAudio _interactAudio;

    public bool Interact(ObjectType objectType)
    {
        if(IsServer == false)
        {
            return false;
        }
        if(objectType != _acceptedObjectType)
        {
            return false;
        }
        if(_stackedResources.Value >= _componentControllers.Count)
        {
            return false;
        }
        PlayerAudioRpc();
        _componentControllers[_stackedResources.Value].SetEnabled(true);
        _stackedResources.Value++;
        if(_stackedResources.Value >= _componentControllers.Count)
        {
            OnPalletFilled?.Invoke();
        }
        return true;
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void PlayerAudioRpc()
    {
        if(_interactAudio != null)
        {
            _interactAudio.PlaySound();
        }
    }
    public void ToggleSelection(bool isSelected)
    {
        if(_selectionOutline != null)
        {
            _selectionOutline.ToggleOutline(isSelected);
        }
    }
}
