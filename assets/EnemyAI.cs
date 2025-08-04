using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Target (usually XR Rig Head or Body)")]
    public Transform playerTarget; // Assign the XR Rig's camera or body collider

    [Header("Ranges")]
    public float detectionRange = 15f;
    public float shootingRange = 5f;

    private NavMeshAgent agent;
    private Animator animator;

    [Header("Optional: LookAt Settings")]
    public Transform headBone; // For head rotation toward player (optional)
    public float lookAtSpeed = 5f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (playerTarget == null && Camera.main != null)
        {
            playerTarget = Camera.main.transform; // fallback to main camera
        }
    }

    void Update()
    {
        if (playerTarget == null) return;

        float distance = Vector3.Distance(transform.position, playerTarget.position);

        if (distance <= shootingRange)
        {
            // Stop and shoot
            agent.isStopped = true;
            animator.SetBool("isRunning", false);
            animator.SetTrigger("isShooting");

            FaceTarget();

            // TODO: Add shooting logic (raycast or projectile toward head)
        }
        else if (distance <= detectionRange)
        {
            // Chase player
            agent.isStopped = false;
            agent.SetDestination(playerTarget.position);
            animator.SetBool("isRunning", true);

            FaceTarget();
        }
        else
        {
            // Idle
            agent.isStopped = true;
            animator.SetBool("isRunning", false);
        }
    }

    void FaceTarget()
    {
        Vector3 direction = (playerTarget.position - transform.position).normalized;
        direction.y = 0f; // keep rotation horizontal
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * lookAtSpeed);
        }

        if (headBone != null)
        {
            Vector3 lookDir = playerTarget.position - headBone.position;
            Quaternion headRot = Quaternion.LookRotation(lookDir);
            headBone.rotation = Quaternion.Slerp(headBone.rotation, headRot, Time.deltaTime * lookAtSpeed);
        }
    }
}