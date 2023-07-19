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

    public EnvironmentParams CurrentEnvParams { get; protected set; }
    public bool IsCurrentEnvironmentWater => CurrentEnvParams == waterEnvParams;
    public bool IsCurrentEnvironmentAir => CurrentEnvParams == airEnvParams;

    protected Rigidbody rigid;
    protected ConstantForce constForceComp;

    protected void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        constForceComp = GetComponent<ConstantForce>();
    }

    public void ChangeEnvironmentToWater()
    {
        CurrentEnvParams = waterEnvParams;
        SetEnvironmentParamsFromCurrentEnvParams();
    }

    public void ChangeEnvironmentToAir()
    {
        CurrentEnvParams = airEnvParams;
        SetEnvironmentParamsFromCurrentEnvParams();
    }

    protected void SetEnvironmentParamsFromCurrentEnvParams()
    {
        rigid.drag = CurrentEnvParams.drag;
        rigid.angularDrag = CurrentEnvParams.angularDrag;
        float constantForceY = CurrentEnvParams.gravity;
        constForceComp.force = new Vector3(0f, constantForceY * rigid.mass, 0f);
    }
}
