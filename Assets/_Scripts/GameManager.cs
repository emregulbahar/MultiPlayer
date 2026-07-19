using UnityEngine;
using System;
using Unity.Netcode;
using Unity.Services.Matchmaker.Models;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : NetworkBehaviour
{
    [SerializeField] private MultiplayerUI _multiPlayerUI;
    [SerializeField] private  GameObject _PlayerPrefab;

    [SerializeField] private List<ResourcePallet> _pallets;


    private void Start() {
        if(_multiPlayerUI != null)
        {
            _multiPlayerUI.OnStartHost += StartHost;
            _multiPlayerUI.OnStartClient += StartClient;
            _multiPlayerUI.OnDiconnectClient += DisconnectClient;
        }
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkDespawn();
        if(IsServer == false)
        {
            return;
        }
        NetworkManager.OnClientConnectedCallback += SpawnPlayer;
        NetworkManager.SceneManager.OnLoadEventCompleted += HandleSceneLoadCompleted;
        foreach(ResourcePallet pallet in _pallets)
        {
            pallet.OnPalletFilled += CheckWinConditional;
        }
    }

    private void CheckWinConditional()
    {
        int points = 0;
        foreach(ResourcePallet pallet in _pallets)
        {
            points += pallet.StackedResoruces;
        }
        if(points >= _pallets.Count * 3)
        {
            NetworkManager.SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
        }
    }

    private void HandleSceneLoadCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        foreach(ulong clientId in clientsCompleted)
        {
            SpawnPlayer(clientId);
        }
    }

    private void SpawnPlayer(ulong clientID)
    {
        if(NetworkManager.ConnectedClients[clientID].PlayerObject != null)
        {
            return;
        }
        GameObject player = Instantiate(_PlayerPrefab);
        player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientID, true);
    }

    public override void OnNetworkDespawn()
    {
        if(IsServer == false)
        {
           NetworkManager.OnClientConnectedCallback += SpawnPlayer;
           NetworkManager.SceneManager.OnLoadEventCompleted -= HandleSceneLoadCompleted;
           foreach(ResourcePallet pallet in _pallets)
        {
            pallet.OnPalletFilled -= CheckWinConditional;
        }
        }
        base.OnNetworkDespawn();
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
