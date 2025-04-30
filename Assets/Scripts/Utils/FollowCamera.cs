using UnityEngine;

/// <summary>
/// 월드 스페이스 캔버스를 메인 카메라 위치에 따라가게 만드는 스크립트.
/// </summary>
public class FollowCamera : MonoBehaviour
{
    private Transform _cameraTransform;

    private void Start()
    {
        _cameraTransform = Camera.main.transform;
    }

    private void LateUpdate()
    {
        if (_cameraTransform != null)
            transform.position = _cameraTransform.position;
    }
}
