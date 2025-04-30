using UnityEngine;

/// <summary>
/// 모든 씬의 공통 기반 클래스.
/// 씬별 초기화/정리와 씬 타입 구분을 위한 인터페이스를 제공한다.
/// </summary>
public abstract class BaseScene : MonoBehaviour
{
    /// <summary>
    /// 해당 씬이 로드될 때 호출됨. 초기 설정 및 로직 실행.
    /// </summary>
    public abstract void InitScene();

    /// <summary>
    /// 씬 전환 직전에 호출됨. 리소스 정리 등 클리어 처리 가능.
    /// </summary>
    public abstract void ClearScene();

    /// <summary>
    /// 해당 씬의 타입을 반환 (Title, Game, Result 등)
    /// </summary>
    public abstract Define.SceneType SceneType { get; }
}
