using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
[RequireComponent(typeof(RagdollCharacterControllerExtension), typeof(CollidersHandler))]
public class NpcCharacterBehaviour : CharacterBehaviour
{
    public NpcCharacter npcCharacter;

    public NpcCharacterState State { get => characterGenericState as NpcCharacterState; private set => characterGenericState = value; }
    public RagdollCharacterControllerExtension Controller { get; private set; }
    public CollidersHandler CollidersHandler { get; private set; }

    public bool IsGatherable => State is DeadNpcCharacterState;

    protected void Awake()
    {
        State = new IdleNpcCharacterState(this);
        Controller = GetComponent<RagdollCharacterControllerExtension>();
        CollidersHandler = GetComponent<CollidersHandler>();
    }

    private void Start()
    {
        SetAsRagdoll();
    }

    private void Update()
    {
        State.OnUpdate();
    }

    private void OnTriggerEnter(Collider collider)
    {
        State.OnTriggerEnter(collider);
    }

    private void OnTriggerExit(Collider collider)
    {
        State.OnTriggerExit(collider);
    }

    public override Vector2 GetMovement() => State.GetMovement();

    public override Quaternion GetRotation() => State.GetRotation();

    public void ChangeState(NpcCharacterState newState)
    {
        NpcCharacterState previousState = State;
        State.OnStateExit(newState);
        State = newState;
        State.OnStateEnter(previousState);
    }

    public void SetAsRagdoll() => ChangeState(new DeadNpcCharacterState(this));
}
