using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Boat : MonoBehaviour
{
    [SerializeField] [Expandable]
    private BoatSO startingParams;
    [SerializeField]
    private GameObject fuselageGO;
    [SerializeField]
    private BoatHook boatHook;
    [SerializeField]
    private float buoyancyTorqueMultiplier = 10000f;
    [SerializeField]
    private AnimationCurve buoyancyTorqueCurve;
    [Header("Crew")]
    [SerializeField] [MinMaxSlider(0f, 2f)]
    private Vector2 crewDroppingIntervalOnDrowning;

    private Rigidbody rigid;
    private ThreeEnvironmentsPhysicsHandler envPhysicsHandler;
    private ConstantForce constForce;
    private InstantiateWithForce dropCrewMember;

    private bool isFrontOfTheBoatClear = true;
    private int startingCrewNum;
    private int currentCrewNum;
    private float maxSpeed;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        envPhysicsHandler = GetComponent<ThreeEnvironmentsPhysicsHandler>();
        constForce = GetComponent<ConstantForce>();
        dropCrewMember = GetComponent<InstantiateWithForce>();

        Init(startingParams);
    }

    private void Start()
    {
        TryToAirGlide();
        constForce.relativeForce = new Vector3(constForce.relativeForce.x, constForce.relativeForce.y, maxSpeed);
    }

    private void FixedUpdate()
    {
        if(envPhysicsHandler.IsCurrentEnvironmentWater)
        {
            float xRotationMinus180To180 = MathUtils.RecalculateAngleToBetweenMinus180And180(transform.rotation.eulerAngles.x);
            float isBoatYRotated = Mathf.Approximately(transform.rotation.eulerAngles.y, 0f) ? 1f : -1f;
            constForce.torque = new Vector3(buoyancyTorqueCurve.Evaluate(Mathf.Abs(xRotationMinus180To180) / 90f) * rigid.mass * rigid.angularDrag * buoyancyTorqueMultiplier * -Mathf.Sign(xRotationMinus180To180) * isBoatYRotated, 0f, 0f);
        }
    }

    public void Init(BoatSO boatSO)
    {
        rigid.mass = boatSO.rootMassMinMax.RandomRangeMinMax();
        fuselageGO.GetComponent<Rigidbody>().mass = boatSO.fuselageMassMinMax.RandomRangeMinMax();
        startingCrewNum = (int)boatSO.crewNumMinMax.RandomRangeMinMax();
        currentCrewNum = startingCrewNum;
        maxSpeed = boatSO.maxSpeedMinMax.RandomRangeMinMax() * rigid.mass;

        boatHook.DamageOnHookStarted = boatSO.damageOnHookStarted.RandomRangeMinMax();
        boatHook.DamageOnEveryTickWhileHooked = boatSO.damageOnEveryTickWhileHooked.RandomRangeMinMax();
        boatHook.TickDuration = boatSO.tickDuration.RandomRangeMinMax();
        boatHook.DamageOnPlayerBoostWhileHooked = boatSO.damageOnPlayerBoostWhileHooked.RandomRangeMinMax();
    }
    
    public void SetIsFrontOfTheBoatClear(bool isClear)
    {
        isFrontOfTheBoatClear = isClear;
        AdjustEnginesStatus();
    }

    public void TryToAirGlide()
    {
        if(envPhysicsHandler.IsCurrentEnvironmentDrowning)
            return;

        envPhysicsHandler.ChangeEnvironmentToAir();
        AdjustEnginesStatus();
        ResetTorque();
    }


    public void TryToWaterDrift()
    {
        if(envPhysicsHandler.IsCurrentEnvironmentDrowning)
            return;

        envPhysicsHandler.ChangeEnvironmentToWater();
        AdjustEnginesStatus();
    }

    public void FishMonsterCollidedStrongly()
    {
        if(currentCrewNum > 0)
        {
            dropCrewMember.InstantiateAndAddForce();
            currentCrewNum--;
        }
    }

    public void TurnOnEngines()
    {
        constForce.relativeForce = new Vector3(constForce.relativeForce.x, constForce.relativeForce.y, maxSpeed);
    }

    public void TurnOffEngines()
    {
        constForce.relativeForce = new Vector3(constForce.relativeForce.x, constForce.relativeForce.y, 0f);
    }

    public void Drown()
    {
        envPhysicsHandler.ChangeEnvironmentToDrowning();
        ResetTorque();
        TurnOffEngines();
        StartCoroutine(DropAllCrewMembers());
    }

    private void AdjustEnginesStatus()
    {
        if(isFrontOfTheBoatClear && envPhysicsHandler.IsCurrentEnvironmentWater)
        {
            TurnOnEngines();
        }
        else
        {
            TurnOffEngines();
        }
    }

    private void ResetTorque()
    {
        constForce.torque = Vector3.zero;
    }

    private IEnumerator DropAllCrewMembers()
    {
        while(currentCrewNum > 0)
        {
            dropCrewMember.InstantiateAndAddForce();
            currentCrewNum--;
            yield return new WaitForSeconds(UnityEngine.Random.Range(crewDroppingIntervalOnDrowning.x, crewDroppingIntervalOnDrowning.y));
        }
    }
}
