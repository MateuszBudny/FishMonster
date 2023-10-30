using AetherEvents;
using DG.Tweening;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;
using static UnityEngine.InputSystem.InputAction;

public class FishMonster : MonoBehaviour, IPlayer
{
    [Header("Movement")]
    [SerializeField]
    private float movementMultiplier = 1f;
    [SerializeField]
    private float speedBoostMovementMultiplier = 0.5f;
    [Tooltip("How fast the fish turns to face movement direction")]
    [Range(0.0f, 0.3f)]
    public float rotationSmoothTime = 0.12f;

    [Header("Other")]
    [SerializeField]
    private Collider interactionTrigger;
    [SerializeField]
    private float startingHp = 100f;

    public Rigidbody Rigid { get; private set; }
    public bool IsUnderWater => envPhysicsHandler.IsCurrentEnvironmentWater;
    public float CurrentHp { get; private set; }
    public float StartingHp => startingHp;

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
    private BoatHook hookHookedOnCurrently;
    private Camera mainCamera;
    private Vector3 currentRotationSpeed;

    private void Awake()
    {
        Rigid = GetComponent<Rigidbody>();
        envPhysicsHandler = GetComponent<TwoEnvironmentsPhysicsHandler>();
        CurrentHp = startingHp;
        mainCamera = Camera.main;
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
        // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is a move input rotate player when the player is moving
        if(new Vector2(forceToAdd.x, forceToAdd.z) != Vector2.zero)
        {
            // normalise input direction
            Vector3 forceDirection = forceToAdd.normalized;
            Vector3 targetRotation = new Vector3(
                Mathf.Atan2(forceDirection.x, forceDirection.z) * Mathf.Rad2Deg + mainCamera.transform.eulerAngles.x,
                Mathf.Atan2(forceDirection.x, forceDirection.z) * Mathf.Rad2Deg + mainCamera.transform.eulerAngles.y,
                0f);

            Vector3 rotation = new Vector3(
                Mathf.SmoothDampAngle(transform.eulerAngles.x, targetRotation.x, ref currentRotationSpeed.x, rotationSmoothTime),
                Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation.y, ref currentRotationSpeed.y, rotationSmoothTime),
                Mathf.SmoothDampAngle(transform.eulerAngles.z, targetRotation.z, ref currentRotationSpeed.z, rotationSmoothTime));

            // rotate to face input direction relative to camera position
            transform.rotation = Quaternion.Euler(rotation);
            
            Vector3 forceToAddRelativeToCamera = mainCamera.transform.rotation * forceToAdd;
            Rigid.AddForce(forceToAddRelativeToCamera * envPhysicsHandler.CurrentEnvParams.movementMultiplier * Time.fixedDeltaTime * 100f, ForceMode.Acceleration);
        }
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

        if(interactableComponent is BoatHook hook)
        {
            if(hook.JumpToThisInteractable)
            {
                hookHookedOnCurrently = hook;
            }
            else
            {
                hookHookedOnCurrently = null;
            }
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
        if(context.started && !BlockInput && envPhysicsHandler.IsCurrentEnvironmentWater)
        {
            Vector3 speedBoostMovement = TransposeInputValuesToMovement(currentMovementRawInput * speedBoostMovementMultiplier);
            Move(speedBoostMovement);

            if(hookHookedOnCurrently)
            {
                hookHookedOnCurrently.PlayerBoostImpulse();
            }
        }
    }

    public void ReceiveDamage(float damage)
    {
        CurrentHp -= damage;
        if(CurrentHp < 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        new OnPlayerDamaged(damage, this).Invoke();
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

    private Vector3 TransposeInputValuesToMovement(Vector2 inputValues) => new Vector3(inputValues.x, 0f, inputValues.y);

    private void ResetMovement()
    {
        currentMovementRawInput = Vector2.zero;
        currentMovement = Vector3.zero;
    }

    [Button(enabledMode: EButtonEnableMode.Playmode)]
    private void GetTest10Damage() => ReceiveDamage(10f);
}
