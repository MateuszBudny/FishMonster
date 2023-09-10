using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTimer : MonoBehaviour
{
    [SerializeField]
    private GameObject toBeDestroyed;
    [SerializeField]
    private float destroyDelay;
    [SerializeField]
    private bool startTimerOnStart;

    private void Start()
    {
        if (startTimerOnStart)
        {
            StartDestroyerTimer();
        }
    }

    private void Reset()
    {
        if(!toBeDestroyed)
        {
            toBeDestroyed = gameObject;
        }
    }

    public void StartDestroyerTimer()
    {
        StartCoroutine(DestroyAfterDelay());
    }

    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(destroyDelay);
        Destroy(toBeDestroyed);
    }
}
