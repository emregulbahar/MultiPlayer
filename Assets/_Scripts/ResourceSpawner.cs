using Unity.Netcode;
using UnityEngine;

public class ResourceSpawner : NetworkBehaviour
{
   [SerializeField] private NetworkObject _woodPrefab, _stonePrefab;


   public void SpawnResource(ObjectType type, Vector3 position)
   {
        if(IsServer == false){return;}
        NetworkObject resource = type == ObjectType.Wood ? _woodPrefab : _stonePrefab;
        GameObject isntance = Instantiate(resource.gameObject, position, Quaternion.Euler(0, UnityEngine.Random.Range(0, 360),0));
        isntance.GetComponent<NetworkObject>().Spawn();

   }
}
