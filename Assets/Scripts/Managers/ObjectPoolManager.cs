using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 오브젝트 풀링을 관리하는 매니저.
/// 같은 프리팹을 반복해서 사용할 때 성능을 위해 사용.
/// </summary>
public class ObjectPoolManager : MonoBehaviour
{
    private Dictionary<string, Queue<GameObject>> _poolDict = new();

    /// <summary>
    /// 프리팹 경로에 해당하는 오브젝트를 풀에서 가져오거나 새로 생성한다.
    /// </summary>
    /// 
    public void Init()
    {
        // 오브젝트 풀 초기화 로직 필요시 여기에 작성
       // Debug.Log("[ObjectPoolManager] Init 호출됨");
    }
    /// 
    public GameObject Spawn(string prefabPath, Vector3 pos, Quaternion rot, Transform parent = null)
    {
        GameObject go;

        if (_poolDict.TryGetValue(prefabPath, out Queue<GameObject> pool) && pool.Count > 0)
        {
            go = pool.Dequeue();
            go.transform.SetPositionAndRotation(pos, rot);
            go.transform.SetParent(parent, false);
            go.SetActive(true);
        }
        else
        {
            GameObject prefab = Managers.Resource.Load<GameObject>(prefabPath);
            if (prefab == null) return null;

            go = Instantiate(prefab, pos, rot, parent);
        }

        return go;
    }

    /// <summary>
    /// 오브젝트를 풀에 반환한다.
    /// </summary>
    public void Despawn(string prefabPath, GameObject go)
    {
        go.SetActive(false);
        go.transform.SetParent(null);

        if (!_poolDict.ContainsKey(prefabPath))
            _poolDict[prefabPath] = new Queue<GameObject>();

        _poolDict[prefabPath].Enqueue(go);
    }
}
