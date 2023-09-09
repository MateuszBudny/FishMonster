using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterState
{
    protected CharacterBehaviour characterBehaviour;

    public abstract Vector2 GetMovement();

    public abstract Quaternion GetRotation();

    public virtual void OnStateEnter(CharacterState previousState) {}

    public virtual void OnStateExit(CharacterState nextState) {}

    public virtual void OnTriggerEnter(Collider collider) {}

    public virtual void OnTriggerExit(Collider collider) {}

    public virtual void OnUpdate() {}
}
