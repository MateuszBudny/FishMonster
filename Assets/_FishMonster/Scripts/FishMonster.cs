using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    public Rigidbody Rigid { get; private set; }
    public bool IsUnderWater => envPhysicsHandler.IsCurrentEnvironmentWater;

    private TwoEnvironmentsPhysicsHandler envPhysicsHandler;
    private Vector3 movement;
    private IInteractable readyForInteraction;

    private void Awake()
    {
        Rigid = GetComponent<Rigidbody>();
        envPhysicsHandler = GetComponent<TwoEnvironmentsPhysicsHandler>();
    }

    private void Start()
    {
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
        Rigid.AddForce(movement * envPhysicsHandler.CurrentEnvParams.movementMultiplier, ForceMode.Acceleration);
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
        if(toUnderWater)
        {
            envPhysicsHandler.ChangeEnvironmentToWater();
        }
        else
        {
            envPhysicsHandler.ChangeEnvironmentToAir();
        }
    }

    private Vector3 TransposeInputValuesToMovement(Vector2 inputValues) => new Vector3(0f, inputValues.y, inputValues.x);
}
