using AetherEvents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostProcessEvents : MonoBehaviour
{
    [SerializeField]
    private PostProcessController controller;
    [Header("On Player Damaged")]
    [SerializeField]
    private float vignetteJumpDuration = 0.2f;
    [SerializeField]
    private float vignetteJumpValue = 0.3f;
    [SerializeField]
    private float vignetteFallingDuration = 1f;

    private Coroutine vignetteDelayedReturningToCurrentValueCoroutine;

    private void Awake()
    {
        OnPlayerDamaged.AddListener(OnPlayerDamagedAction);
    }

    private void OnPlayerDamagedAction(OnPlayerDamaged eventData)
    {
        float hpPercentage = eventData.player.CurrentHp / eventData.player.StartingHp;
        float vignettePercentage = 1f - hpPercentage;
        controller.SetVignetteSmoothlyAsMaxPercentage(vignettePercentage + vignetteJumpValue, vignetteJumpDuration);
        
        if(vignetteDelayedReturningToCurrentValueCoroutine != null)
        {
            StopCoroutine(vignetteDelayedReturningToCurrentValueCoroutine);
        }
        vignetteDelayedReturningToCurrentValueCoroutine = StartCoroutine(GeneralUtils.InvokeDelayed(() => controller.SetVignetteSmoothlyAsMaxPercentage(vignettePercentage, vignetteFallingDuration), vignetteJumpDuration));
    }
}
