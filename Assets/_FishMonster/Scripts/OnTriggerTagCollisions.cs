using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnTriggerTagCollisions : MonoBehaviour
{
    [SerializeField]
    private List<OnTriggerTagCollisionsRecord> onTriggerTagEnterRecords;
    [SerializeField]
    private List<OnTriggerTagCollisionsRecord> onTriggerTagExitRecords;

    private void OnTriggerEnter(Collider other)
    {
        onTriggerTagEnterRecords.ForEach(record =>
        {
            if(other.CompareTag(record.tag.ToString()))
            {
                record.eventOnCollision.Invoke();
            }
        });
    }

    private void OnTriggerExit(Collider other)
    {
        onTriggerTagExitRecords.ForEach(record =>
        {
            if(other.CompareTag(record.tag.ToString()))
            {
                record.eventOnCollision.Invoke();
            }
        });
    }

    [Serializable]
    private class OnTriggerTagCollisionsRecord
    {
        public Tags tag;
        public UnityEvent eventOnCollision;
    }
}
