using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Boat : MonoBehaviour
{
    [SerializeField] [Expandable]
    private BoatSO startingParams;
    [SerializeField]
    private GameObject fuselageGO;
    [SerializeField]
    private BoatHook boatHook;
    [Header("Crew")]
    [SerializeField] [MinMaxSlider(0f, 2f)]
    private Vector2 crewDroppingIntervalOnDrowning;

    private Rigidbody rigid;
    private ThreeEnvironmentsPhysicsHandler envPhysicsHandler;
    private ConstantForce constForce;
    private InstantiateWithForce dropCrewMember;
    private List<Floater> floaters;

    private bool isFrontOfTheBoatClear = true;
    private bool feelsEndangered;
    private int startingCrewNum;
    private int currentCrewNum;
    private float maxSpeed;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        envPhysicsHandler = GetComponent<ThreeEnvironmentsPhysicsHandler>();
        constForce = GetComponent<ConstantForce>();
        dropCrewMember = GetComponent<InstantiateWithForce>();
        floaters = GetComponentsInChildren<Floater>().ToList();

        Init(startingParams);
    }

    private void Start()
    {
        TryToAirGlide();
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
        SetFloatersActive(false);
        ResetTorque();
    }


    public void TryToWaterDrift()
    {
        if(envPhysicsHandler.IsCurrentEnvironmentDrowning)
            return;

        envPhysicsHandler.ChangeEnvironmentToWater();
        AdjustEnginesStatus();
        SetFloatersActive(true);
    }

    public void FishMonsterCollidedStrongly()
    {
        feelsEndangered = true;
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

    public void SetFloatersActive(bool active)
    {
        floaters.ForEach(floater => floater.IsTurnedOn = active);
    }

    public void Drown()
    {
        envPhysicsHandler.ChangeEnvironmentToDrowning();
        ResetTorque();
        TurnOffEngines();
        SetFloatersActive(false);
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
