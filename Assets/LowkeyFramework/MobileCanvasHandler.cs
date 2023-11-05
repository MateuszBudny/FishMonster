using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MobileCanvasHandler : MonoBehaviour
{
    [SerializeField]
    private UnityEvent onMobileEvent;
    [SerializeField]
    private UnityEvent onPCEvent;

    private void Awake()
    {
#if UNITY_ANDROID
        Application.targetFrameRate = (int)Screen.currentResolution.refreshRateRatio.value;
        onMobileEvent.Invoke();
#else
        onPCEvent.Invoke();
#endif
    }
}
