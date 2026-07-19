using UnityEngine;

public class PickableResource : PickableBase
{
    protected override void ApplyAvailabilityState(bool newValue)
    {
        //no code
    }

    protected override void OnPickedUP()
    {
        if(IsServer == false)
        {
            return;
        }
        NetworkObject.Despawn();
    }
}
