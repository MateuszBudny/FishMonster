using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boat : MonoBehaviour
{
    [SerializeField]
    private BoatSO startingParams;
    [SerializeField]
    private GameObject fuselageGO;
    [SerializeField]
    private AnimationCurve buoyancyTorqueCurve;
    [Header("Crew")]
    [SerializeField]
    private int crewNum = 5;
    [SerializeField]
    [MinMaxSlider(0f, 2f)]
    private Vector2 crewDroppingIntervalOnDrowning;

    private Rigidbody rigid;
    private ThreeEnvironmentsPhysicsHandler envPhysicsHandler;
    private ConstantForce constForce;
    private InstantiateWithForce dropCrewMember;
    private int startingCrewNum;
    private int currentCrewNum;

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
    }

    private void FixedUpdate()
    {
        if(envPhysicsHandler.IsCurrentEnvironmentWater)
        {
            float xRotationMinus180To180 = MathUtils.RecalculateAngleToBetweenMinus180And180(transform.rotation.eulerAngles.x);
            constForce.torque = new Vector3(buoyancyTorqueCurve.Evaluate(Mathf.Abs(xRotationMinus180To180) / 90f) * rigid.mass * -Mathf.Sign(xRotationMinus180To180), 0f, 0f);
        }
    }

    public void Init(BoatSO boatSO)
    {
        rigid.mass = boatSO.rootMassMinMax.RandomRangeMinMax();
        fuselageGO.GetComponent<Rigidbody>().mass = boatSO.fuselageMassMinMax.RandomRangeMinMax();
        startingCrewNum = (int)boatSO.crewNumdMinMax.RandomRangeMinMax();
        currentCrewNum = startingCrewNum;
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

    public void FishMonsterCollidedStrongly()
    {
        if(crewNum > 0)
        {
            dropCrewMember.InstantiateAndAddForce();
            crewNum--;
        }
    }

    public void Drown()
    {
        envPhysicsHandler.ChangeEnvironmentToDrowning();
        ResetTorque();
        StartCoroutine(DropAllCrewMembers());
    }

    private void ResetTorque()
    {
        constForce.torque = Vector3.zero;
    }

    private IEnumerator DropAllCrewMembers()
    {
        while(crewNum > 0)
        {
            dropCrewMember.InstantiateAndAddForce();
            crewNum--;
            yield return new WaitForSeconds(UnityEngine.Random.Range(crewDroppingIntervalOnDrowning.x, crewDroppingIntervalOnDrowning.y));
        }
    }
}
