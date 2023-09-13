using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class OnTriggerTagCollisions : MonoBehaviour
{
    [SerializeField]
    private List<OnTriggerTagCollisionsRecord> onTriggerTagRecords = new List<OnTriggerTagCollisionsRecord>();

    public List<OnTriggerTagCollisionsRecord> OnTriggerTagRecords { get => onTriggerTagRecords; set => onTriggerTagRecords = value; }

    private void OnTriggerEnter(Collider other)
    {
        OnTriggerTagRecords.ForEach(record => record.CollisionEntered(other, gameObject));
    }

    private void OnTriggerExit(Collider other)
    {
        OnTriggerTagRecords.ForEach(record => record.CollisionExited(other, gameObject));
    }

    [Serializable]
    public class OnTriggerTagCollisionsRecord
    {
        [SerializeField]
        protected Tags tag;
        [SerializeField]
        protected UnityEvent<Collider> eventOnEveryCollisionEnter = new UnityEvent<Collider>();
        [SerializeField]
        protected UnityEvent<Collider> eventOnEveryCollisionExit = new UnityEvent<Collider>();
        [SerializeField]
        protected UnityEvent<Collider> eventOnFirstCollisionEnteredWhenNoOtherCollisionsIsCurrently = new UnityEvent<Collider>();
        [SerializeField]
        protected UnityEvent<Collider> eventOnLastCollisionExitedSoNoOtherCollisionIsCurrently = new UnityEvent<Collider>();

        public bool IsCollidersContainerEmpty => !collidersInsideTriggerTag.Any();

        [SerializeField] [HideInInspector]
        protected List<Collider> collidersInsideTriggerTag = new List<Collider>();

        public OnTriggerTagCollisionsRecord(Tags tag, Action<Collider> onEveryCollisionEnter = null, Action<Collider> onEveryCollisionExit = null, Action<Collider> onFirstCollisionEnteredWhenNoOtherCollisionsIsCurrently = null, Action<Collider> onLastCollisionExitedSoNoOtherCollisionIsCurrently = null) 
        {
            this.tag = tag;
            TryToAddListenerToNewUnityEvent(eventOnEveryCollisionEnter, onEveryCollisionEnter);
            TryToAddListenerToNewUnityEvent(eventOnEveryCollisionExit, onEveryCollisionExit);
            TryToAddListenerToNewUnityEvent(eventOnFirstCollisionEnteredWhenNoOtherCollisionsIsCurrently, onFirstCollisionEnteredWhenNoOtherCollisionsIsCurrently);
            TryToAddListenerToNewUnityEvent(eventOnLastCollisionExitedSoNoOtherCollisionIsCurrently, onLastCollisionExitedSoNoOtherCollisionIsCurrently);
        }

        public void CollisionEntered(Collider collider, GameObject collisionInvoker)
        {
            if(!collider.CompareTag(tag.ToString()))
                return;
            if(!AdditionalConditionsForCollisionEnter(collider, collisionInvoker))
                return;

            eventOnEveryCollisionEnter.Invoke(collider);
            if(IsCollidersContainerEmpty)
            {
                eventOnFirstCollisionEnteredWhenNoOtherCollisionsIsCurrently.Invoke(collider);
            }
            collidersInsideTriggerTag.Add(collider);
        }

        public void CollisionExited(Collider collider, GameObject collisionInvoker)
        {
            if(!collider.CompareTag(tag.ToString()))
                return;
            if(!AdditionalConditionsForCollisionExit(collider, collisionInvoker))
                return;

            eventOnEveryCollisionExit.Invoke(collider);
            collidersInsideTriggerTag.Remove(collider);
            if(IsCollidersContainerEmpty)
            {
                eventOnLastCollisionExitedSoNoOtherCollisionIsCurrently.Invoke(collider);
            }
        }

        protected virtual bool AdditionalConditionsForCollisionEnter(Collider collider, GameObject collisionInvoker) => true;

        protected virtual bool AdditionalConditionsForCollisionExit(Collider collider, GameObject collisionInvoker) => true;

        protected void TryToAddListenerToNewUnityEvent(UnityEvent<Collider> unityEvent, Action<Collider> listener)
        {
            if(listener != null)
            {
                unityEvent.AddListener(collider => listener(collider));
            }
        }
    }
}
