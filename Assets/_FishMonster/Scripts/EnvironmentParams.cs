using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnvironmentParams
{
    public float drag = 0.2f;
    public float angularDrag = 0.05f;
    public float gravity = -5f;
    public float movementMultiplier = 1f;
}
