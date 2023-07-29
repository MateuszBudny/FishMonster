using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileCanvasHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject mobileUI;

    private void Awake()
    {
#if UNITY_ANDROID
        Application.targetFrameRate = (int)Screen.currentResolution.refreshRateRatio.value;
        mobileUI.SetActive(true);
#else
        mobileUI.SetActive(false);
#endif
    }
}
