using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Floater : MonoBehaviour
{
    [SerializeField]
    private float floatingObjectHeight = 1f;
    [SerializeField]
    private float floatingForce = 10f;

    public bool IsTurnedOn { get; set; } = true;

    private Rigidbody rigid;

    private void Awake()
    {
        rigid = GetComponentInParent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if(!IsTurnedOn)
            return;

        float waterHeight = 0f;
        if(transform.position.y < waterHeight)
        {
            float submergedAmount = Mathf.Clamp(waterHeight - transform.position.y, 0f, floatingObjectHeight);
            rigid.AddForceAtPosition(Vector3.up * submergedAmount * floatingForce, transform.position, ForceMode.Acceleration);
        }
    }
}
