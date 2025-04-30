using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using NavMeshPlus.Components;

/// <summary>
/// NavMeshSurface의 Build()를 1프레임 뒤에 실행하도록 처리하는 컴포넌트.
/// </summary>
public class NavMeshBakeStarter : MonoBehaviour
{
    private NavMeshSurface _surface;

    public void Init(NavMeshSurface surface)
    {
        _surface = surface;
        StartCoroutine(BakeNextFrame());
    }

    private IEnumerator BakeNextFrame()
    {
        yield return new WaitForEndOfFrame();
        _surface.BuildNavMesh();
    }
}
