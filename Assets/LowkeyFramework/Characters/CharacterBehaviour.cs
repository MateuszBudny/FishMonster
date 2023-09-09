using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterBehaviour : MonoBehaviour
{
    public float walkSpeed = 0.85f;
    public float rotationSpeed = 1f;
    public Rigidbody mainRigidbody;

    protected CharacterState characterGenericState;

    /// <summary>
    /// In world space.
    /// </summary>
    /// <returns></returns>
    public abstract Vector2 GetMovement();

    /// <summary>
    /// In world space.
    /// </summary>
    /// <returns></returns>
    public abstract Quaternion GetRotation();
}
