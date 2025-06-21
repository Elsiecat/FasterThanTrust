using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 좀비 유닛 스폰 전담 매니저.
/// 시민은 맵에 직접 배치되며, 좀비는 클릭 감염 또는 기타 조건에 따라 생성된다.
/// </summary>
public class SpawnManager : MonoBehaviour
{
    private GameObject _zombiePrefab;     // 좀비 프리팹
    private Transform _zombieParent;      // 좀비 부모 오브젝트

    private List<Vector3> _usedPositions = new(); // 랜덤 스폰 위치 중복 방지
    private const float MIN_DISTANCE_BETWEEN_UNITS = 1.0f;

    /// <summary>
    /// 좀비 프리팹 로드 및 부모 오브젝트 초기화
    /// </summary>
    public void Init()
    {
        // 좀비 프리팹 로드
        _zombiePrefab = Managers.Resource.Load<GameObject>(Define.PATH_ZOMBIE_PREFAB);
        if (_zombiePrefab == null)
        {
            Debug.LogError("[SpawnManager] 좀비 프리팹을 불러올 수 없습니다.");
            return;
        }

        // 좀비 부모 오브젝트 생성
        if (_zombieParent == null)
        {
            GameObject zombieParentObj = new GameObject("Zombies");
            _zombieParent = zombieParentObj.transform;
        }
    }

    /// <summary>
    /// 감염된 시민의 위치 또는 랜덤 위치에 좀비를 생성한다.
    /// Init()이 호출되지 않은 경우에도 지연 로딩으로 안전하게 처리된다.
    /// </summary>
    /// <param name="position">좀비를 생성할 위치. null이면 랜덤 위치 사용</param>
    public void SpawnZombie(Vector2? position = null)
    {
        // ✅ Init()이 누락된 경우를 대비한 지연 로딩 처리
        if (_zombiePrefab == null)
        {
            _zombiePrefab = Managers.Resource.Load<GameObject>(Define.PATH_ZOMBIE_PREFAB);
            if (_zombiePrefab == null)
            {
                Debug.LogError("[SpawnManager] 좀비 프리팹 로드 실패 (지연 로딩)");
                return;
            }
        }

        if (_zombieParent == null)
        {
            GameObject zombieParentObj = new GameObject("Zombies");
            _zombieParent = zombieParentObj.transform;
        }

        Vector3 spawnPos = position.HasValue ? (Vector3)position.Value : GetRandomSpawnPosition();

        GameObject zombie = Instantiate(_zombiePrefab, spawnPos, Quaternion.identity, _zombieParent);
        if (zombie == null)
        {
            Debug.LogError("[SpawnManager] 좀비 생성 실패 (null)");
            return;
        }

        zombie.name = $"Zombie_{_zombieParent.childCount - 1}";
    }

    /// <summary>
    /// 범위 내 랜덤 위치를 반환한다. (중복 방지 포함)
    /// </summary>
    private Vector3 GetRandomSpawnPosition()
    {
        for (int attempt = 0; attempt < 100; attempt++)
        {
            float x = Random.Range(-5f, 5f);
            float y = Random.Range(-5f, 5f);
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

        Debug.LogWarning("[SpawnManager] 충분한 랜덤 위치를 찾지 못했습니다. (0,0) 사용");
        return Vector3.zero;
    }
}
