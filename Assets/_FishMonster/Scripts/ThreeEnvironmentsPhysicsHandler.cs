using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreeEnvironmentsPhysicsHandler : TwoEnvironmentsPhysicsHandler
{
    [SerializeField]
    private EnvironmentParams drowningEnvParams;

    public bool IsCurrentEnvironmentDrowning => CurrentEnvParams == drowningEnvParams;

    public void ChangeEnvironmentToDrowning()
    {
        CurrentEnvParams = drowningEnvParams;
        SetEnvironmentParamsFromCurrentEnvParams();
    }
}
