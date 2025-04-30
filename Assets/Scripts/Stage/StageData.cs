using UnityEngine;

/// <summary>
/// 스테이지에 대한 메타 정보 및 설정 데이터를 ScriptableObject 형태로 보관한다.
/// </summary>
[CreateAssetMenu(menuName = "Data/StageData", fileName = "Stage_")]
public class StageData : ScriptableObject
{
    [Header("스테이지 기본 정보")]
    public int stageId;                    // 고유 스테이지 ID
    public int humanCount = 20;            // 초기 시민 수

    [Header("맵 범위")]
    public Vector2 spawnAreaSize = new Vector2(10, 10);     // 스폰 영역 크기
    public Vector2 spawnAreaCenter = Vector2.zero;          // 스폰 중심 좌표

    //[Header("기타 설정 (확장용)")]
    // 향후 특수 조건, BGM, 날씨 효과 등 추가 가능
}
