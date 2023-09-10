using DG.Tweening;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class FishMonster : MonoBehaviour, IPlayer
{
    [BoxGroup("Movement")]
    [SerializeField]
    private float movementMultiplier = 1f;
    [BoxGroup("Movement")]
    [SerializeField]
    private float speedBoostMovementMultiplier = 0.5f;
    [SerializeField]
    private Collider interactionTrigger;

    public Rigidbody Rigid { get; private set; }
    public bool IsUnderWater => envPhysicsHandler.IsCurrentEnvironmentWater;

    public bool BlockInput 
    {
        get => blockInput;
        private set
        {
            blockInput = value;
            ResetMovement();
        }
    }

    private TwoEnvironmentsPhysicsHandler envPhysicsHandler;
    private Vector2 currentMovementRawInput;
    private Vector3 currentMovement;
    private bool blockInput;
    private bool noEatingAction;

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

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(Tags.Water.ToString()))
        {
            ChangeEnvironment(true);
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

    public void InteractableCatched(Collider interactableCollider)
    {
        if(noEatingAction)
            return;

        IInteractable interactableComponent = interactableCollider.GetComponent<IInteractable>();
        if(interactableComponent.JumpToThisInteractable)
        {
            noEatingAction = true;
            BlockInput = true;
            transform
                .DOMove(interactableCollider.transform.position, 0.25f)
                .SetEase(Ease.OutSine)
                .OnComplete(() =>
                {
                    BlockInput = false;
                    interactableComponent.Interact(this);
                    noEatingAction = false;
                });
        }
        else
        {
            interactableComponent.Interact(this);
        }
    }

    public void OnMovement(CallbackContext context)
    {
        if (context.performed && !BlockInput)
        {
            currentMovementRawInput = context.ReadValue<Vector2>();
            currentMovement = TransposeInputValuesToMovement(currentMovementRawInput * movementMultiplier);
        }
        else if(context.canceled)
        {
            ResetMovement();
        }
    }

    public void OnInteract(CallbackContext context)
    {
        if (context.performed && !BlockInput)
        {
            interactionTrigger.gameObject.SetActive(true);
        }
        else if(context.canceled)
        {
            interactionTrigger.gameObject.SetActive(false);
        }
    }

    public void OnSpeedBoost(CallbackContext context)
    {
        if(context.started && !BlockInput)
        {
            Vector3 speedBoostMovement = TransposeInputValuesToMovement(currentMovementRawInput * speedBoostMovementMultiplier);
            Move(speedBoostMovement);
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

    private void ResetMovement()
    {
        currentMovementRawInput = Vector2.zero;
        currentMovement = Vector3.zero;
    }
}
