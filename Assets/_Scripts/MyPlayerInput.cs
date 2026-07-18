using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class MyPlayerInput : NetworkBehaviour
{
    [SerializeField] private  InputActionReference _movementReference;

    public Vector2 MovementInput {get; private set;}

    public event Action OnPickUpPressed;
    public event Action OnInteractPressed;

    private Vector2 _rawInput;
    [SerializeField] private float _smoothTime = 0.1f;


    void Update()
    {

        if(IsOwner == false)
        {
            return;
        }
        _rawInput = _movementReference.action.ReadValue<Vector2>();
        MovementInput = Vector2.MoveTowards(MovementInput, _rawInput, Time.deltaTime / _smoothTime);

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            OnPickUpPressed?.Invoke();
        }
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            OnInteractPressed?.Invoke();
        }
    }
}
