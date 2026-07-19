using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class ResourceNode : NetworkBehaviour, IInteractable
{
    [SerializeField] private  SelectionOutline _selectionOutline;
    [SerializeField] private ComponentController _componentController;
    [SerializeField] private List<ObjectType> _toolTytpeRequired;
    [SerializeField] private  ObjectType _producedObjectType;
    [SerializeField] private int _amountSpawn;
    [SerializeField] private InteractAnimation _interactAnimation;
    [SerializeField] private InteractAudio _interactAudio;

    private NetworkVariable<int> _health = new(3);

    private ResourceSpawner _resourceSpawner;

    private void Awake()
    {
        _resourceSpawner = FindAnyObjectByType<ResourceSpawner>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer)
        {
            _health.Value = 3;
        }
    }

    public void Harvest(ObjectType toolType)
    {
        if (!IsServer)
        {
            return;
        }
        if (_toolTytpeRequired.Contains(toolType))
        {
            _health.Value--;
            PlayAudioClientRpc();
            if(_health.Value > 0)
            {
                PlayAnimationClientRpc();
            }
            else
            {
                _componentController.SetEnabled(false);
                for(int i = 0; i < _amountSpawn; i++)
                {
                    Vector3 position = transform.position;
                    position.y = 0;
                    Vector2 offset = UnityEngine.Random.insideUnitCircle;
                    position.x += offset.x;
                    position.z += offset.y;
                    _resourceSpawner.SpawnResource(_producedObjectType, position);
                }
            }
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void PlayAnimationClientRpc()
    {
        if(_interactAnimation != null)
        {
            _interactAnimation.Shake();
        }
    }


    [Rpc(SendTo.ClientsAndHost)]
    private void PlayAudioClientRpc()
    {
        if(_interactAudio != null)
        {
            if(_health.Value > 0)
            {
                _interactAudio.PlaySound();
            }
            else
            {
                _interactAudio.PlaySoundSeparate();
            }
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