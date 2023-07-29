using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(CharacterBehaviour))]
public class RagdollCharacterControllerExtension : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    [Header("Ragdoll Rigidbodies Properties")]
    [SerializeField]
    private int rigidbodySolverIterations = 12;
    [SerializeField]
    private int rigidbodySolverVelocityIterations = 12;
    [SerializeField]
    private int rigidbodyMaxAngularSpeed = 30;
    
    public CharacterController CoreController { get; private set; }

    private List<Collider> ragdollColliders = new List<Collider>();
    private List<Rigidbody> ragdollRigidbodies;

    private void Awake()
    {
        CoreController = GetComponent<CharacterController>();

        GetComponentsInChildren<Collider>().ToList().ForEach(collider =>
            {
                if(!(collider is CharacterController))
                {
                    ragdollColliders.Add(collider);
                }
            });
        ragdollRigidbodies = new List<Rigidbody>(GetComponentsInChildren<Rigidbody>());
        ragdollRigidbodies.ForEach(ragdollRigidbody =>
        {
            ragdollRigidbody.solverIterations = rigidbodySolverIterations;
            ragdollRigidbody.solverVelocityIterations = rigidbodySolverVelocityIterations;
            ragdollRigidbody.maxAngularVelocity = rigidbodyMaxAngularSpeed;
        });

        SetAsAnimated();
    }

    public void SimpleMove(Vector3 speed, Quaternion rotation)
    {
        if (CoreController.enabled)
        {
            transform.rotation = rotation;
            CoreController.SimpleMove(speed);
        }
    }

    public void SetAsAnimated()
    {
        animator.enabled = true;
        CoreController.enabled = true;
        ragdollColliders.ForEach(ragdollCollider => ragdollCollider.enabled = false);
        ragdollRigidbodies.ForEach(ragdollRigidbody => ragdollRigidbody.isKinematic = true);
    }

    public void SetAsRagdoll()
    {
        animator.enabled = false;
        CoreController.enabled = false;
        ragdollColliders.ForEach(ragdollCollider => ragdollCollider.enabled = true);
        ragdollRigidbodies.ForEach(ragdollRigidbody => ragdollRigidbody.isKinematic = false);
    }
}
