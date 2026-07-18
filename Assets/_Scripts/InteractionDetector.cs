using UnityEngine;
using System;

public class InteractionDetector : MonoBehaviour
{
    
    [SerializeField] private float _detectionRadius = 3f;
    [SerializeField] private float _detectionsAngle = 60f;
    [SerializeField] private  LayerMask _pickupLayer;

    private IInteractable _closestInteractable;
    public IInteractable ClosestInteractable => _closestInteractable;
    private bool _isOwner;


    public void Initialize(bool isOwner)
    {
        _isOwner = isOwner;
    }

    private void Update() {
        if(_isOwner == false)
        {
            return;
        }
        DetectInteractables();
    }

    private void DetectInteractables()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, _detectionRadius, _pickupLayer);

        float closestDistance = float.MaxValue;
        IInteractable candidate = null;

        foreach (Collider hit in hits)
        {
            IInteractable pickable = hit.GetComponent<IInteractable>();
            if (pickable == null)
                continue;

            Vector3 directionToPickable
                = (hit.transform.position - transform.position).normalized;
            float angleToPickable = Vector3.Angle(transform.forward, directionToPickable);

            if (angleToPickable > _detectionsAngle * 0.5f)
                continue;

            float distance = Vector3.Distance(transform.position, hit.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                candidate = pickable;
            }
        }

        if (candidate != _closestInteractable)
        {
            if (_closestInteractable != null)
                _closestInteractable.ToggleSelection(false);
            _closestInteractable = candidate;
            if (_closestInteractable != null)
                _closestInteractable.ToggleSelection(true);

        }
    }
}
