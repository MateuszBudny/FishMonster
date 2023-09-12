using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnCollisionTagAndForce : MonoBehaviour
{
    [SerializeField] [Tooltip("Collision force is not checked for on exit collisions (ofc).")]
    private List<OnCollisionTagAndForceRecord> onCollisionRecords;

    private void OnCollisionEnter(Collision collision)
    {
        onCollisionRecords.ForEach(record => record.CollisionEntered(collision.collider, gameObject));
    }

    private void OnCollisionExit(Collision collision)
    {
        onCollisionRecords.ForEach(record => record.CollisionExited(collision.collider, gameObject));
    }

    [Serializable]
    public class OnCollisionTagAndForceRecord : OnTriggerTagCollisions.OnTriggerTagCollisionsRecord
    {
        [SerializeField] [Tooltip("Collision force is not checked for on exit collisions (ofc).")]
        private float minCollisionForce;
        [SerializeField]
        private bool useMassToCalculateForce = true;
        [SerializeField]
        private bool useVelocityToCalculateForce = true;

        public OnCollisionTagAndForceRecord(Tags tag, Action<Collider> onEveryCollisionEnter = null, Action<Collider> onEveryCollisionExit = null, Action<Collider> onFirstCollisionEnteredWhenNoOtherCollisionsIsCurrently = null, Action<Collider> onLastCollisionExitedSoNoOtherCollisionIsCurrently = null) : base(tag, onEveryCollisionEnter, onEveryCollisionExit, onFirstCollisionEnteredWhenNoOtherCollisionsIsCurrently, onLastCollisionExitedSoNoOtherCollisionIsCurrently)
        {
        }

        protected override bool AdditionalConditionsForCollisionEnter(Collider collider, GameObject collisionInvoker)
        {
            Rigidbody collidedRigid = collider.GetComponent<Rigidbody>();
            float collisionForce = 1f;
            if(useMassToCalculateForce)
            {
                collisionForce *= collidedRigid.mass;
            }
            if(useVelocityToCalculateForce)
            {
                float velocityDiff;
                if(collisionInvoker.TryGetComponent(out Rigidbody collisionInvokerRigid))
                {
                    velocityDiff = (collidedRigid.velocity - collisionInvokerRigid.velocity).sqrMagnitude;
                }
                else
                {
                    velocityDiff = collidedRigid.velocity.sqrMagnitude;
                }
                collisionForce *= velocityDiff;
            }

            return collisionForce >= minCollisionForce;
        }
    }
}
