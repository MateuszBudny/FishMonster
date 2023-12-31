using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ConstantForce))]
public class TwoEnvironmentsPhysicsHandler : MonoBehaviour
{
    [SerializeField]
    private EnvironmentParams waterEnvParams;
    [SerializeField]
    private EnvironmentParams airEnvParams;
    [SerializeField]
    private bool autoSetupOnTriggerTagComponentOnAwake = true;

    public EnvironmentParams CurrentEnvParams { get; protected set; }
    public bool IsCurrentEnvironmentWater => CurrentEnvParams == waterEnvParams;
    public bool IsCurrentEnvironmentAir => CurrentEnvParams == airEnvParams;

    protected Rigidbody rigid;
    protected ConstantForce constForceComp;

    protected void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        constForceComp = GetComponent<ConstantForce>();

        if(autoSetupOnTriggerTagComponentOnAwake)
        {
            OnTriggerTagCollisions newOnTriggerTagComponent = gameObject.AddComponent<OnTriggerTagCollisions>();
            newOnTriggerTagComponent.OnTriggerTagRecords.Add(new OnTriggerTagCollisions.OnTriggerTagCollisionsRecord(Tags.Water, ChangeEnvironmentToWater, ChangeEnvironmentToAir));
        }
    }

    private void Start()
    {
        ChangeEnvironmentToAir();
    }

    public void ChangeEnvironmentToWater(Collider _ = null)
    {
        CurrentEnvParams = waterEnvParams;
        SetEnvironmentParamsFromCurrentEnvParams();
    }

    public void ChangeEnvironmentToAir(Collider _ = null)
    {
        CurrentEnvParams = airEnvParams;
        SetEnvironmentParamsFromCurrentEnvParams();
    }

    protected void SetEnvironmentParamsFromCurrentEnvParams()
    {
        rigid.drag = CurrentEnvParams.drag;
        rigid.angularDrag = CurrentEnvParams.angularDrag;
        float constantForceY = CurrentEnvParams.gravity;
        constForceComp.force = new Vector3(constForceComp.force.x, constantForceY * rigid.mass, constForceComp.force.z);
    }
}
