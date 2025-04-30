using UnityEngine;

/// <summary>
/// Resources 폴더에서 리소스를 로드하는 유틸리티 매니저.
/// 프리팹 및 에셋 로딩을 통일된 인터페이스로 제공.
/// </summary>
public class ResourcesManager : MonoBehaviour
{
    /// <summary>
    /// 지정된 경로의 리소스를 로드한다. 실패 시 null 반환.
    /// </summary>
    /// 
    
    /// 
        public void Init()
    {
        // 리소스 로딩 관련 초기 설정이 필요하다면 여기에 작성
        //Debug.Log("[ResourcesManager] Init 호출됨");
    }

    public T Load<T>(string path) where T : Object
    {
        T resource = Resources.Load<T>(path);
        if (resource == null)
        {
            Debug.LogError($"[ResourcesManager] {path} 에 해당하는 리소스를 찾을 수 없습니다.");
        }
        return resource;
    }

    /// <summary>
    /// 리소스를 로드한 뒤 인스턴스를 생성한다.
    /// </summary>
    public GameObject Instantiate(string path, Vector3 pos, Quaternion rot, Transform parent = null)
    {
        GameObject prefab = Load<GameObject>(path);
        if (prefab == null) return null;

        return Object.Instantiate(prefab, pos, rot, parent);
    }
}
