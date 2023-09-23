using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBoatSO", menuName = "FishMonster/BoatSO")]
public class BoatSO : ScriptableObject
{
    [MinMaxSlider(0f, 10000f)]
    public Vector2 rootMassMinMax = new Vector2(800f, 1200f);
    [MinMaxSlider(0f, 10000f)]
    public Vector2 fuselageMassMinMax = new Vector2(800f, 1200f);
    [MinMaxSlider(0f, 1000f)]
    public Vector2 accelerationMinMax = new Vector2(80f, 120f);
    [MinMaxSlider(0f, 1000f)]
    public Vector2 maxSpeedMinMax = new Vector2(80f, 120f);
    [MinMaxSlider(0f, 100f)]
    public Vector2 crewNumMinMax = new Vector2(2f, 10f);

    [Header("Hook")]
    [InfoBox("Maybe instead of randomizing every parameter, there should be just one, HookStrength, and depending on HookStrength all values are set?")]
    [MinMaxSlider(0f, 100f)]
    public Vector2 damageOnHookStarted = new Vector2(3f, 6f);
    [MinMaxSlider(0f, 100f)]
    public Vector2 damageOnEveryTickWhileHooked = new Vector2(0.5f, 2f);
    [MinMaxSlider(0f, 10f)]
    public Vector2 tickDuration = new Vector2(0.8f, 2f);
    [MinMaxSlider(0f, 100f)]
    public Vector2 damageOnPlayerBoostWhileHooked = new Vector2(3f, 6f);
}
