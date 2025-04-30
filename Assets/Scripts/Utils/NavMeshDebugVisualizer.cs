using UnityEngine;
using UnityEngine.AI;

public class NavMeshDebugVisualizer : MonoBehaviour
{
    public float radius = 0.3f;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 1.0f, NavMesh.AllAreas))
        {
            Gizmos.DrawSphere(hit.position, radius);
        }
        else
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, radius); // NavMesh가 없는 위치 표시
        }
    }
}
