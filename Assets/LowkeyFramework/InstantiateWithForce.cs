using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InstantiateWithForce : MonoBehaviour
{
    [SerializeField]
    private GameObject prefabToInstantiate;
    [SerializeField]
    private List<Transform> instantiatePosRange;
    [SerializeField]
    private Transform instanceParent;
    [Header("Force")]
    [SerializeField]
    private Vector3 minForceOnInstantiate;
    [SerializeField]
    private Vector3 maxForceOnInstantiate;
    [SerializeField]
    private bool useLocalCoordinatesForForceAdding;
    [SerializeField] [ShowIf(nameof(useLocalCoordinatesForForceAdding))] [Tooltip("If null, then transform of an instantiated object is used.")]
    private Transform transformToUseLocalCoordinatesFrom;
    [Header("Rotation")]
    [SerializeField]
    private bool applyRandomRotation;
    [SerializeField] [ShowIf(nameof(applyRandomRotation))]
    private Vector3 axisToRandomRotate = Vector3.up;
    [SerializeField] [ShowIf(nameof(applyRandomRotation))]
    private RandomRotationType randomRotationType = RandomRotationType.Range;
    [SerializeField] [ShowIf(nameof(IsRandomRangeRotationType))] [MinMaxSlider(0f, 360f)]
    private Vector2 rotationRandomAngle;
    [SerializeField] [ShowIf(nameof(IsRandomFromValuesRotationType))]
    private List<float> angleValuesToDrawFrom;

    private bool IsRandomRangeRotationType => applyRandomRotation && randomRotationType == RandomRotationType.Range;
    private bool IsRandomFromValuesRotationType => applyRandomRotation && randomRotationType == RandomRotationType.FromValues;

    public void InstantiateAndAddForce()
    {
        Vector3 instantiatePos = Vector2.zero;
        if(instantiatePosRange.Count == 0 || instantiatePosRange.Count > 2)
        {
            Debug.LogError("instantiatePosRange cannot have value equal to 0 (at least one instantiate pos transform is needed) or greater than 2 (it is not supported currently).");
            return;
        }
        else if(instantiatePosRange.Count == 1)
        {
            instantiatePos = instantiatePosRange[0].position;
        }
        else if(instantiatePosRange.Count == 2)
        {
            instantiatePos = instantiatePosRange[0].position.RandomRange(instantiatePosRange[1].position);
        }

        GameObject instantiatedGO = Instantiate(prefabToInstantiate, instantiatePos, Quaternion.identity, instanceParent);
        
        Rigidbody rigidbodyToAddForceTo;
        if(instantiatedGO.TryGetComponent(out Rigidbody instantiatedRigidbody))
        {
            rigidbodyToAddForceTo = instantiatedRigidbody;
        }
        else
        {
            NpcCharacterBehaviour npcBehaviour = instantiatedGO.GetComponent<NpcCharacterBehaviour>();
            rigidbodyToAddForceTo = npcBehaviour.mainRigidbody;
            npcBehaviour.SetAsRagdoll();
        }
        if(applyRandomRotation)
        {
            float randomAngleToRotateBy = 0f;
            if(IsRandomRangeRotationType)
            {
                randomAngleToRotateBy = Random.Range(rotationRandomAngle.x, rotationRandomAngle.y);
            }
            else if(IsRandomFromValuesRotationType)
            {
                randomAngleToRotateBy = angleValuesToDrawFrom.GetRandomElement();
            }

            instantiatedGO.transform.RotateAround(instantiatedGO.transform.position, axisToRandomRotate, randomAngleToRotateBy);
        }
        
        Vector3 forceOnInstantiate = minForceOnInstantiate.RandomRange(maxForceOnInstantiate);
        
        if(useLocalCoordinatesForForceAdding)
        {
            if(!transformToUseLocalCoordinatesFrom)
            {
                rigidbodyToAddForceTo.AddRelativeForce(forceOnInstantiate, ForceMode.Impulse);
                return;
            }

            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, transformToUseLocalCoordinatesFrom.up);
            forceOnInstantiate = rotation * forceOnInstantiate;
        }

        rigidbodyToAddForceTo.AddForce(forceOnInstantiate, ForceMode.Impulse);
    }

    private enum RandomRotationType
    {
        Range,
        FromValues,
    }
}
