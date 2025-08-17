using UnityEngine;

public class WaypointCircuit : MonoBehaviour
{
    public Transform[] waypoints;

    private void OnDrawGizmos()
    {
        for (int i = 0; i < waypoints.Length; i++)
        {
            Gizmos.color = Color.gray;
            Gizmos.DrawSphere(waypoints[i].position, 0.25f);

            int index = i + 1 < waypoints.Length ? i++ : 0;
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(waypoints[i].position, waypoints[index].transform.position);
        }
    }
}