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
}
