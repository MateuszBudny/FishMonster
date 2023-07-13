using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatHook : MonoBehaviour, IInteractable
{
    private FixedJoint joint;

    public void Interact(FishMonster fishMonster)
    {
        if(!joint)
        {
            joint = gameObject.AddComponent<FixedJoint>();
            joint.connectedBody = fishMonster.Rigid;
        }
        else
        {
            Destroy(joint);
        } 
    }
}
