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
    [SerializeField] [Tooltip("How fast the fish turns to face movement direction")]
    private float rotationSpeed = 300f;
    [SerializeField]
    private AnimationCurve jumpCurve;
    [SerializeField]
    private AnimationCurve diveCurve;

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

    /// <summary>
    /// Target planar movement in XZ axes relative to the camera.
    /// </summary>
    public Vector3 CurrentPlanarMovement => TransposeInputValuesToMovement(currentMovementRawInput * movementMultiplier);
    /// <summary>
    /// Movement force additional X axis rotation given by jumping or diving input. If player jumps and dives at the same time, then jumping is applied and diving is ignored.
    /// </summary>
    public float CurrentMovementAdditionalXAngle => !Mathf.Approximately(currentJumpAngleRawInput, 0f) ? -jumpCurve.Evaluate(currentJumpAngleRawInput) : diveCurve.Evaluate(currentDiveAngleRawInput);

    private TwoEnvironmentsPhysicsHandler envPhysicsHandler;
    private Vector2 currentMovementRawInput;
    private float currentJumpAngleRawInput;
    private float currentDiveAngleRawInput;
    private bool blockInput;
    private bool noEatingActionLeft;
    private bool isMovingToPrey;
    private BoatHook hookHookedOnCurrently;
    private Camera mainCamera;
    private Vector3 currentRotationSpeed;
    private Vector3 previousRotationOnStop;

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
        Move(CurrentPlanarMovement);
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

    private void Move(Vector3 planarMovementForceToAdd)
    {
        Vector3 targetRotation = previousRotationOnStop;
        
        // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is a move input rotate player when the player is moving
        if(planarMovementForceToAdd != Vector3.zero || isMovingToPrey)
        {
            // normalise input direction
            Vector3 forceDirection = planarMovementForceToAdd.normalized;
            targetRotation = new Vector3(
                Mathf.Lerp(-MathUtils.RecalculateAngleToBetweenMinus180And180(mainCamera.transform.eulerAngles.x), MathUtils.RecalculateAngleToBetweenMinus180And180(mainCamera.transform.eulerAngles.x), Mathf.InverseLerp(-1f, 1f, forceDirection.z)), // the more a player character is moving straight forward/backward, the more make the x rotation the same as of camera. smoothly decrease that dependence, when a player is going more to the left/right than straight forward/backward
                Mathf.Atan2(forceDirection.x, forceDirection.z) * Mathf.Rad2Deg + mainCamera.transform.eulerAngles.y, // keep y rotation the same as camera when going forward, but rotate y rotation if direction of moving is changing, so a player character is always facing a moving direction
                mainCamera.transform.eulerAngles.z); // keep z rotation the same as camera no matter what

            targetRotation.x += CurrentMovementAdditionalXAngle;

            Vector3 worldPlanarMovementForceToAddRelativeToCamera = mainCamera.transform.rotation * planarMovementForceToAdd;
            Vector3 cameraAxisToRotateMovementForceAround = Vector3.Cross(mainCamera.transform.up, worldPlanarMovementForceToAddRelativeToCamera);
            Vector3 finalWorldForceToAddRelativeToCamera = worldPlanarMovementForceToAddRelativeToCamera.RotateAroundAxis(cameraAxisToRotateMovementForceAround, CurrentMovementAdditionalXAngle);            ;
            Rigid.AddForce(finalWorldForceToAddRelativeToCamera * envPhysicsHandler.CurrentEnvParams.movementMultiplier * Time.fixedDeltaTime * 100f, ForceMode.Acceleration);

            ApplyRotation();
            previousRotationOnStop = transform.rotation.eulerAngles;
        }
        else if(!Mathf.Approximately(CurrentMovementAdditionalXAngle, 0f))
        {
            targetRotation.x += CurrentMovementAdditionalXAngle;
            ApplyRotation();
        }

        void ApplyRotation()
        {
            // XY
            Vector3 torqueYawnPitch = CalculateTorque(transform.forward, targetRotation, Vector3.forward);
            // Z
            Vector3 torqueRoll = CalculateTorque(transform.up, targetRotation, Vector3.up);
            Vector3 finalTorque = torqueYawnPitch + torqueRoll;
            Rigid.AddTorque(finalTorque * rotationSpeed);

            Vector3 CalculateTorque(Vector3 currentRotationDirection, Vector3 targetRotation, Vector3 axis)
            {
                Vector3 targetRotationDirection = Quaternion.Euler(targetRotation) * axis;
                Vector3 crossProduct = -Vector3.Cross(targetRotationDirection, currentRotationDirection);
                float angle = Vector3.Angle(targetRotationDirection, currentRotationDirection);
                return angle * angle * Mathf.Deg2Rad * Mathf.Deg2Rad * crossProduct.normalized; // TODO: this square function could be extracted into AnimationCurve
            }
        }
    }

    public void InteractableCatched(Collider interactableCollider)
    {
        if(noEatingActionLeft)
            return;

        IInteractable interactableComponent = interactableCollider.GetComponent<IInteractable>();
        if(interactableComponent.JumpToThisInteractable)
        {
            noEatingActionLeft = true;
            isMovingToPrey = true;
            BlockInput = true;
            transform
                .DOMove(interactableCollider.transform.position, 0.25f)
                .SetEase(Ease.OutSine)
                .OnComplete(() =>
                {
                    BlockInput = false;
                    interactableComponent.Interact(this);
                    noEatingActionLeft = false;
                    isMovingToPrey = false;
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

    public void OnJump(CallbackContext context)
    {
        if(BlockInput)
            return;

        currentJumpAngleRawInput = context.ReadValue<float>();
    }

    public void OnDive(CallbackContext context)
    {
        if(BlockInput)
            return;

        currentDiveAngleRawInput = context.ReadValue<float>();
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
    }

    [Button(enabledMode: EButtonEnableMode.Playmode)]
    private void GetTest10Damage() => ReceiveDamage(10f);
}
