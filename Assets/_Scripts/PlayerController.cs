using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private MyPlayerInput _myPlayerInput;
    [SerializeField] private AgentMover _agentMover;


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
