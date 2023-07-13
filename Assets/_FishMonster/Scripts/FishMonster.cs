using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class FishMonster : MonoBehaviour
{
    [BoxGroup("Movement")]
    [SerializeField]
    private float startedMovementMultiplier = 1f;
    [BoxGroup("Movement")]
    [SerializeField]
    private float stayingMovementMultiplier = 0.5f;

    [BoxGroup("Under Water Environment")]
    [SerializeField]
    private float dragUnderWater = 1f;
    [BoxGroup("Under Water Environment")]
    [SerializeField]
    private float angularDragUnderWater = 0.5f;
    [BoxGroup("Under Water Environment")]
    [SerializeField]
    private float gravityUnderWater = -0.5f;


    [BoxGroup("In Air Environment")]
    [SerializeField]
    private float dragInAir = 0.2f;
    [BoxGroup("In Air Environment")]
    [SerializeField]
    private float angularDragInAir = 0.05f;
    [BoxGroup("In Air Environment")]
    [SerializeField]
    private float gravityInAir = -3f;
    [BoxGroup("In Air Environment")]
    [SerializeField]
    private float movementMultiplierInAir = 0.3f;

    public Rigidbody Rigid { get; private set; }

    private ConstantForce constantForceComp;
    private Vector3 movement;
    private bool isUnderWater = true;
    private IInteractable readyForInteraction;

    private void Awake()
    {
        Rigid = GetComponent<Rigidbody>();
        constantForceComp = GetComponent<ConstantForce>();

        ChangeEnvironment(true);
    }

    private void FixedUpdate()
    {
        Move();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.CompareTag(Tags.Interactable.ToString()))
        {
            readyForInteraction = collision.collider.GetComponent<IInteractable>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(Tags.Water.ToString()))
        {
            ChangeEnvironment(true);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if(collision.collider.CompareTag(Tags.Interactable.ToString()))
        {
            if(readyForInteraction == collision.collider.GetComponent<IInteractable>())
            {
                readyForInteraction = null;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag(Tags.Water.ToString()))
        {
            ChangeEnvironment(false);
        }
    }

    private void Move()
    {
        float movementMultiplier = isUnderWater ? 1f : movementMultiplierInAir;
        Rigid.AddForce(movement * movementMultiplier, ForceMode.Acceleration);
        //Debug.Log(rigid.velocity);
    }

    public void OnMovement(CallbackContext context)
    {
        if (context.started)
        {
            movement = TransposeInputValuesToMovement(context.ReadValue<Vector2>() * startedMovementMultiplier);
            Move();
        }
        else if(context.performed)
        {
            movement = TransposeInputValuesToMovement(context.ReadValue<Vector2>() * stayingMovementMultiplier);
        }
        else if(context.canceled)
        {
            movement = Vector3.zero;
        }
    }

    public void OnInteract(CallbackContext context)
    {
        if (context.performed)
        {
            if(readyForInteraction != null)
            {
                readyForInteraction.Interact(this);
            }
        }
    }

    private void ChangeEnvironment(bool toUnderWater)
    {
        isUnderWater = toUnderWater;
        Rigid.drag = toUnderWater ? dragUnderWater : dragInAir;
        Rigid.angularDrag = toUnderWater ? angularDragUnderWater : angularDragInAir;
        float constantForceY = toUnderWater ? gravityUnderWater : gravityInAir;
        constantForceComp.force = new Vector3(0f, constantForceY * Rigid.mass, 0f);
    }

    private Vector3 TransposeInputValuesToMovement(Vector2 inputValues) => new Vector3(0f, inputValues.y, inputValues.x);
}
