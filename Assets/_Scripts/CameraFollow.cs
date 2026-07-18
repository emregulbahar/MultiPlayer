using JetBrains.Annotations;
using Unity.Netcode;
using UnityEngine;

public class CameraFollow : NetworkBehaviour
{
    private Camera _camera;

    Vector3 _offsetFromPlayer;
    Vector3 _originPosition;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if(IsOwner == false)
        {
            return;
        }
        _camera = Camera.main;
        _originPosition = _camera.transform.position;
        _offsetFromPlayer = transform.position - _camera.transform.position;
    }

    public override void OnNetworkDespawn()
    {
        if(IsOwner && _camera != null)
        {
            _camera.transform.position = _originPosition;
        }
        base.OnNetworkDespawn();
    }

    void LateUpdate()
    {
        if(IsOwner && _camera != null)
        {
            _camera.transform.position = transform.position - _offsetFromPlayer;
        }
    }


}
