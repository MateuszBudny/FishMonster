using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;
using static UnityEngine.InputSystem.InputAction;

public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField] [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
    public GameObject cinemachineCameraTarget;
    [SerializeField]
    private float lookInputThreshold = 0.01f;
    [SerializeField] [Tooltip("How far in degrees can you move the camera up")]
    private float topClamp = 70.0f;
    [SerializeField] [Tooltip("How far in degrees can you move the camera down")]
    private float bottomClamp = -30.0f;
    [SerializeField] [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
    private float cameraAngleOverride = 0.0f;
    [SerializeField]
    private PlayerInput playerInput;
    [SerializeField] [ShowIf(nameof(IsPlayerInputNotNull))] [Dropdown(nameof(ControlSchemesInCurrentInputAsset))]
    private string keyboardMouseControlScheme;

    private List<string> ControlSchemesInCurrentInputAsset => playerInput.actions.controlSchemes.Select(inputAsset => inputAsset.name).ToList();
    private bool IsPlayerInputNotNull => playerInput != null;
    private bool IsCurrentDeviceMouse => playerInput.currentControlScheme == keyboardMouseControlScheme;

    private float cinemachineTargetYaw;
    private float cinemachineTargetPitch;

    private void Start()
    {
        cinemachineTargetYaw = cinemachineCameraTarget.transform.rotation.eulerAngles.y;
    }

    private void LateUpdate()
    {
        // Cinemachine will follow this target
        cinemachineCameraTarget.transform.rotation = Quaternion.Euler(cinemachineTargetPitch + cameraAngleOverride, cinemachineTargetYaw, 0.0f);
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        SetCursorState(true);
    }

    public void OnLook(CallbackContext context)
    {
        CameraRotation(context.ReadValue<Vector2>());
    }

    private void CameraRotation(Vector2 lookInput)
    {
        // if there is an input
        if(lookInput.sqrMagnitude >= lookInputThreshold)
        {
            //Don't multiply mouse input by Time.deltaTime if mouse is used (mmultiply if controller is used)
            float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

            cinemachineTargetYaw += lookInput.x * deltaTimeMultiplier;
            cinemachineTargetPitch += lookInput.y * deltaTimeMultiplier;
        }

        // clamp our rotations so our values are limited 360 degrees
        cinemachineTargetYaw = ClampAngle(cinemachineTargetYaw, float.MinValue, float.MaxValue);
        cinemachineTargetPitch = ClampAngle(cinemachineTargetPitch, bottomClamp, topClamp);
    }

    private float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if(lfAngle < -360f)
            lfAngle += 360f;
        if(lfAngle > 360f)
            lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    private void SetCursorState(bool lockCursor)
    {
        Cursor.lockState = lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
    }
}
