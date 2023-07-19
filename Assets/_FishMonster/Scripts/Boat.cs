using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boat : MonoBehaviour
{
    [SerializeField]
    private AnimationCurve buoyancyTorqueCurve;

    private Rigidbody rigid;
    private ThreeEnvironmentsPhysicsHandler envPhysicsHandler;
    private ConstantForce constForce;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        envPhysicsHandler = GetComponent<ThreeEnvironmentsPhysicsHandler>();
        constForce = GetComponent<ConstantForce>();
    }

    private void Start()
    {
        TryToAirGlide();
    }

    private void FixedUpdate()
    {
        if(envPhysicsHandler.IsCurrentEnvironmentWater)
        {
            float xRotationMinus180To180 = MathUtils.RecalculateAngleToBetweenMinus180And180(transform.rotation.eulerAngles.x);
            constForce.torque = new Vector3(buoyancyTorqueCurve.Evaluate(Mathf.Abs(xRotationMinus180To180) / 90f) * rigid.mass * -Mathf.Sign(xRotationMinus180To180), 0f, 0f);
        }
    }

    public void TryToAirGlide()
    {
        if(envPhysicsHandler.IsCurrentEnvironmentDrowning)
            return;

        envPhysicsHandler.ChangeEnvironmentToAir();
        ResetTorque();
    }


    public void TryToWaterDrift()
    {
        if(envPhysicsHandler.IsCurrentEnvironmentDrowning)
            return;

        envPhysicsHandler.ChangeEnvironmentToWater();
    }

    public void Drown()
    {
        envPhysicsHandler.ChangeEnvironmentToDrowning();
        ResetTorque();
    }

    private void ResetTorque()
    {
        constForce.torque = Vector3.zero;
    }
}
