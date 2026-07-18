using System.Drawing;
using Unity.Netcode.Components;
using UnityEngine;

public class PickableTool : PickableBase
{

    [SerializeField] private ComponentController _componentController;
    protected override void ApplyAvailabilityState(bool newValue)
    {
        if (IsServer)
        {
            _componentController.SetEnabled(newValue);
        }
    }

    protected override void OnPickedUP()
    {
        //NO CODE
    }

    public void Drop(Vector3 position)
    {
        if(IsServer == false)
        {
            return;
        }
        transform.position = new Vector3(position.x, transform.position.y, position.z);
        _isAvaible.Value = true;
    }

}
