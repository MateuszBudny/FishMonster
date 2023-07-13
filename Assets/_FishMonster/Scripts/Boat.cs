using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boat : MonoBehaviour
{
    private Rigidbody rigid;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    public void Drown()
    {
        rigid.isKinematic = false;
        rigid.useGravity = true;
        rigid.constraints &= ~RigidbodyConstraints.FreezeRotationX;
    }
}
