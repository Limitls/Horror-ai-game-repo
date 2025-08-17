using System.Collections;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [Header("FOV Settings")]
    public float radius = 10f;             
    [Range(0, 360)]
    public float angle = 90f;              

    [Header("Target Detection")]
    public LayerMask targetMask;         
    public LayerMask obstructionMask;

    [Header("Flags")]
    public bool canSeePlayer;
    public Transform visibleTarget;

    private void Start()
    {
        StartCoroutine(FOVRoutine());
    }

    private IEnumerator FOVRoutine()
    {
        float delay = 0.2f; 
        WaitForSeconds wait = new WaitForSeconds(delay);

        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    canSeePlayer = true;
                    visibleTarget = target;
                }
                else
                {
                    canSeePlayer = false;
                    visibleTarget = null;
                }
            }
            else
            {
                canSeePlayer = false;
                visibleTarget = null;
            }
        }
        else if (canSeePlayer)
        {
            canSeePlayer = false;
            visibleTarget = null;
        }
    }
}
