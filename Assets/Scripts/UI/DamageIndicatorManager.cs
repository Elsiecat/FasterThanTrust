using UnityEngine;

public class DamageIndicatorManager : MonoBehaviour
{
    private DamageIndicatorRoot _root;
    private GameObject _parentPrefab;
    private GameObject _valuePrefab;

    public void Init()
    {
        _root = Instantiate(Resources.Load<DamageIndicatorRoot>("Prefabs/UI/DamageIndicatorRoot"));
        _parentPrefab = Resources.Load<GameObject>("Prefabs/UI/DamageIndicatorParent");
        _valuePrefab = Resources.Load<GameObject>("Prefabs/UI/DamageIndicatorValue");
    }

    public void SpawnDamageIndicator(Vector3 worldPosition, int damageAmount, bool isCritical)
    {
        if (_root == null)
        {
            Debug.LogError("[DamageIndicatorManager] Root가 존재하지 않습니다.");
            return;
        }

        GameObject parentObj = Instantiate(_parentPrefab, _root.transform);
        parentObj.transform.position = worldPosition;

        GameObject valueObj = Instantiate(_valuePrefab, parentObj.transform);
        valueObj.transform.localPosition = Vector3.zero;

        DamageIndicatorValue value = valueObj.GetComponent<DamageIndicatorValue>();
        if (value != null)
            value.SetDamage(damageAmount, isCritical);
    }
}
