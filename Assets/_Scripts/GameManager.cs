using UnityEngine;
using System;
using Unity.Netcode;

public class GameManager : NetworkBehaviour
{
    [SerializeField] private MultiplayerUI _multiPlayerUI;


    private void Start() {
        if(_multiPlayerUI != null)
        {
            _multiPlayerUI.OnStartHost += StartHost;
            _multiPlayerUI.OnStartClient += StartClient;
            _multiPlayerUI.OnDiconnectClient += DisconnectClient;
        }
    }

    private void StartHost()
    {
        _multiPlayerUI.DisableButtons();
        NetworkManager.StartHost();
    }

    private void StartClient()
    {
        _multiPlayerUI.DisableButtons();
        NetworkManager.StartClient();
    }

    private void DisconnectClient()
    {
        _multiPlayerUI.EnableButtons();
        NetworkManager.Shutdown();
    }
}
