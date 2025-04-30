using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

/// <summary>
/// 시민 프리팹을 현재 스테이지의 범위에 맞춰 자동으로 스폰하는 매니저.
/// 프리팹은 Resources에서 자동 로드되며, 바닥 오브젝트는 필요 없다.
/// </summary>
public class SpawnManager : MonoBehaviour
{
    private GameObject _citizenPrefab;   
    private Transform _humanParent;     // 시민들의 부모 오브젝트

    private GameObject _zombiePrefab;
    private Transform _zombieParent;

    private Vector2 _spawnCenter;      
    private Vector2 _spawnArea;        
    //private int testnumb = 1;

    private List<Vector3> _usedPositions = new(); 
    private const float MIN_DISTANCE_BETWEEN_UNITS = 1.0f;

void Awake()
{
}
    public void Init()
    {
        _citizenPrefab = Managers.Resource.Load<GameObject>(Define.PATH_CITIZEN_PREFAB);

        if (_citizenPrefab == null)
            Debug.LogError("[SpawnManager] 시민 프리팹을 불러올 수 없습니다.");

        // 하이라키에 시민의 부모 오브젝트 생성
        GameObject humanParentObj = new GameObject("Citizens");
        _humanParent = humanParentObj.transform;

        if (_zombiePrefab == null)
        {
            _zombiePrefab = Managers.Resource.Load<GameObject>(Define.PATH_ZOMBIE_PREFAB);
            if (_zombiePrefab == null)
            {
                Debug.LogError("[SpawnManager] 좀비 프리팹을 불러올 수 없습니다.");
                return;
            }
        }

        if (_zombieParent == null)
        {
            // 하이라키에 좀비의 부모 오브젝트 생성
            GameObject zombieParentObj = new GameObject("Zombies");
            _zombieParent = zombieParentObj.transform;
        }
    }

    public void InitSpawnArea(Vector2 center, Vector2 size)
    {
        _spawnCenter = center;
        _spawnArea = size;
    }

    public void SpawnCitizens()
    {
//        Debug.Log($"[SpawnManager] 스폰 중심 좌표: {_spawnCenter}, 스폰 영역 크기: {_spawnArea}");
        if (_citizenPrefab == null)
        {
            Debug.LogError("[SpawnManager] 시민 프리팹이 없습니다.");
            return;
        }

        int count = Managers.Stage.CurrentStage.humanCount;
        _usedPositions.Clear();

        for (int i = 0; i < count; i++)
        {
            Vector3 randPos = GetRandomSpawnPosition();
            GameObject go = Instantiate(_citizenPrefab, randPos, Quaternion.identity, _humanParent);
            go.name = $"Human_{i}";
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        for (int attempt = 0; attempt < 100; attempt++)
        {
            float x = Random.Range(_spawnCenter.x - _spawnArea.x / 2f, _spawnCenter.x + _spawnArea.x / 2f);
            float y = Random.Range(_spawnCenter.y - _spawnArea.y / 2f, _spawnCenter.y + _spawnArea.y / 2f);
            Vector3 pos = new Vector3(x, y, 0);

            bool isTooClose = false;
            foreach (var existing in _usedPositions)
            {
                if (Vector3.Distance(existing, pos) < MIN_DISTANCE_BETWEEN_UNITS)
                {
                    isTooClose = true;
                    break;
                }
            }

            if (!isTooClose)
            {
                _usedPositions.Add(pos);
                return pos;
            }
        }

        Debug.LogWarning("[SpawnManager] 충분한 간격을 가진 위치를 찾지 못했습니다. 기본 위치로 생성됩니다.");
        return _spawnCenter;
    }

    public void SpawnZombie(Vector2? position = null)
    {
        Vector3 spawnPos = position.HasValue ? (Vector3)position.Value : GetRandomSpawnPosition();
        GameObject zombie = Instantiate(_zombiePrefab, spawnPos, Quaternion.identity, _zombieParent);
        if(zombie == null)
        {
        Debug.Log("좀비가 널임");}
        zombie.name = $"Zombie_{_zombieParent.childCount - 1}";
    }

}
