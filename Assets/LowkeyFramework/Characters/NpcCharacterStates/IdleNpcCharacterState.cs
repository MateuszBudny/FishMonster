using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleNpcCharacterState : NpcCharacterState
{
    public IdleNpcCharacterState(NpcCharacterBehaviour npcCharacter) : base(npcCharacter) {}

    public override Vector2 GetMovement()
    {
        return Vector2.zero;
    }

    public override Quaternion GetRotation()
    {
        return NpcCharacterBehaviour.transform.rotation;
    }

    //public override void OnTriggerEnter(Collider collider)
    //{
    //    if (collider.CompareTag(Tags.PlayerVehicle.ToString()) || collider.CompareTag(Tags.NpcVehicle.ToString()))
    //    {
    //        NpcCharacterBehaviour.ChangeState(new DeadNpcCharacterState(NpcCharacterBehaviour));
    //    }

    //    if(collider.CompareTag(Tags.MeatShop.ToString()))
    //    {
    //        NpcCharacterBehaviour.ChangeState(new OnRouteToMeatShopNpcCharacterState(NpcCharacterBehaviour, MeatShopManager.Instance.customerEntryWaypoint, true));
    //    }
    //}
}
