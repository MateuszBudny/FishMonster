using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GeneralUtils
{
    public static IEnumerator InvokeDelayed(Action action, float delay, bool realtime = false)
    {
        if(realtime)
        {
            yield return new WaitForSecondsRealtime(delay);
        }
        else
        {
            yield return new WaitForSeconds(delay);
        }

        action();
    }
}
