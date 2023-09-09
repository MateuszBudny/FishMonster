using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnCollisionTagAndForce : MonoBehaviour
{
    [SerializeField]
    private List<OnCollisionTagAndForceRecord> onCollisionEnterRecords;
    [SerializeField]
    [Tooltip("Collision force is not checked in on exit collisions (ofc).")]
    private List<OnCollisionTagAndForceRecord> onCollisionExitRecords;

    private Rigidbody rigid;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        onCollisionEnterRecords.ForEach(record =>
        {
            if(collision.gameObject.CompareTag(record.tag.ToString()))
            {
                float collisionForce = 1f;
                if(record.useMassToCalculateForce)
                {
                    collisionForce *= collision.rigidbody.mass;
                }
                if(record.useVelocityToCalculateForce)
                {
                    float velocityDiff;
                    if(rigid)
                    {
                        velocityDiff = (collision.rigidbody.velocity - rigid.velocity).magnitude;
                    }
                    else
                    {
                        velocityDiff = collision.rigidbody.velocity.magnitude;
                    }
                    collisionForce *= velocityDiff;
                }

                if(collisionForce >= record.minCollisionForce)
                {
                    record.onCollisionEvent.Invoke();
                }
            }
        });
    }

    private void OnCollisionExit(Collision collision)
    {
        onCollisionExitRecords.ForEach(record =>
        {
            if(collision.gameObject.CompareTag(record.tag.ToString()))
            {
                record.onCollisionEvent.Invoke();
            }
        });
    }

    [Serializable]
    public class OnCollisionTagAndForceRecord
    {
        public Tags tag;
        [Tooltip("Collision force is not checked in on exit collisions (ofc).")]
        public float minCollisionForce;
        public bool useMassToCalculateForce = true;
        public bool useVelocityToCalculateForce = true;
        public UnityEvent onCollisionEvent;
    }
}
