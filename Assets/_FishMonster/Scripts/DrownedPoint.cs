using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrownedPoint : MonoBehaviour
{
    [SerializeField]
    private Boat boat;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(Tags.Water.ToString()))
        {
            boat.Drown();
        }
    }
}
