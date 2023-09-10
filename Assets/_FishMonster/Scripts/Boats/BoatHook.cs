using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatHook : MonoBehaviour, IInteractable
{
    private FixedJoint joint;

    public bool JumpToThisInteractable => !joint;

    public void Interact(IPlayer player)
    {
        if(!joint)
        {
            joint = gameObject.AddComponent<FixedJoint>();
            joint.connectedBody = (player as FishMonster).Rigid;
        }
        else
        {
            Destroy(joint);
        } 
    }
}
