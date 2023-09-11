using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LoopingInvoker : MonoBehaviour
{
    [SerializeField]
    private float interval = 1f;
    [SerializeField]
    private float firstInvokeDelay;
    [SerializeField]
    private UnityEvent eventToInvoke;

    private void OnEnable()
    {
        Invoke(nameof(StartLooping), firstInvokeDelay);
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(InvokeEvent));
    }

    private void StartLooping()
    {
        InvokeRepeating(nameof(InvokeEvent), 0f, interval);
    }

    private void InvokeEvent() => eventToInvoke.Invoke();
}
