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
    private float movementMultiplier = 1f;
    [BoxGroup("Movement")]
    [SerializeField]
    private float speedBoostMovementMultiplier = 0.5f;

    public Rigidbody Rigid { get; private set; }
    public bool IsUnderWater => envPhysicsHandler.IsCurrentEnvironmentWater;

    private TwoEnvironmentsPhysicsHandler envPhysicsHandler;
    private Vector2 currentMovementRawInput;
    private Vector3 currentMovement;
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
        Move(currentMovement);
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

    private void Move(Vector3 forceToAdd)
    {
        Rigid.AddForce(forceToAdd * envPhysicsHandler.CurrentEnvParams.movementMultiplier, ForceMode.Acceleration);
    }

    public void OnMovement(CallbackContext context)
    {
        if (context.performed)
        {
            currentMovementRawInput = context.ReadValue<Vector2>();
            currentMovement = TransposeInputValuesToMovement(currentMovementRawInput * movementMultiplier);
        }
        else if(context.canceled)
        {
            currentMovement = Vector3.zero;
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

    public void OnSpeedBoost(CallbackContext context)
    {
        if(context.started)
        {
            Vector3 speedBoostMoveent = TransposeInputValuesToMovement(currentMovementRawInput * speedBoostMovementMultiplier);
            Move(speedBoostMoveent);
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
