using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnTriggerTagCollisions : MonoBehaviour
{
    [SerializeField]
    private List<OnTriggerTagCollisionsRecord> onTriggerTagEnterRecords = new List<OnTriggerTagCollisionsRecord>();
    [SerializeField]
    private List<OnTriggerTagCollisionsRecord> onTriggerTagExitRecords = new List<OnTriggerTagCollisionsRecord>();

    public List<OnTriggerTagCollisionsRecord> OnTriggerTagEnterRecords { get => onTriggerTagEnterRecords; private set => onTriggerTagEnterRecords = value; }
    public List<OnTriggerTagCollisionsRecord> OnTriggerTagExitRecords { get => onTriggerTagExitRecords; private set => onTriggerTagExitRecords = value; }

    private void OnTriggerEnter(Collider other)
    {
        OnTriggerTagEnterRecords.ForEach(record =>
        {
            if(other.CompareTag(record.tag.ToString()))
            {
                record.eventOnCollision.Invoke();
            }
        });
    }

    private void OnTriggerExit(Collider other)
    {
        OnTriggerTagExitRecords.ForEach(record =>
        {
            if(other.CompareTag(record.tag.ToString()))
            {
                record.eventOnCollision.Invoke();
            }
        });
    }

    [Serializable]
    public class OnTriggerTagCollisionsRecord
    {
        public Tags tag;
        public UnityEvent eventOnCollision;

        public OnTriggerTagCollisionsRecord(Tags tag, Action onCollision) 
        {
            this.tag = tag;
            eventOnCollision = new UnityEvent();
            eventOnCollision.AddListener(() => onCollision());
        }
    }
}
