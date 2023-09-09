using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateWithForce : MonoBehaviour
{
    [SerializeField]
    private GameObject prefabToInstantiate;
    [SerializeField]
    private Transform instantiatePos;
    [SerializeField]
    private Transform instanceParent;
    [Header("Force")]
    [SerializeField]
    private Vector3 minForceOnInstantiate;
    [SerializeField]
    private Vector3 maxForceOnInstantiate;
    [Header("Rotation")]
    [SerializeField]
    private bool applyRandomRotation;
    [SerializeField] [ShowIf(nameof(applyRandomRotation))]
    private Vector3 axisToRandomRotate = Vector3.up;
    [SerializeField] [ShowIf(nameof(applyRandomRotation))] [MinMaxSlider(0f, 360f)]
    private Vector2 rotationRandomAngle;

    public void InstantiateAndAddForce()
    {
        GameObject instantiatedGO = Instantiate(prefabToInstantiate, instantiatePos.position, Quaternion.identity, instanceParent);
        
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
            instantiatedGO.transform.RotateAround(instantiatedGO.transform.position, axisToRandomRotate, Random.Range(rotationRandomAngle.x, rotationRandomAngle.y));
        }
        
        Vector3 forceOnInstantiate = minForceOnInstantiate.RandomRange(maxForceOnInstantiate);
        rigidbodyToAddForceTo.AddForce(forceOnInstantiate, ForceMode.Impulse);
    }
}
