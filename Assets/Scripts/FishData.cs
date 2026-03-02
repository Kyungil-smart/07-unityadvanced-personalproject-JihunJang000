using UnityEngine;


[CreateAssetMenu(fileName = "New Fish", menuName = "Fishing System/Fish Data")]
public class FishData : ScriptableObject
{
    [Header("물고기 기본 정보")]
    public string fishName;         // 물고기 이름
    public Sprite fishIcon;         // 물고기 이미지 
    [TextArea]
    public string description;      // 물고기 설명

    [Header("낚시 수치")]
    public int price;               // 상점에 팔 때 가격
    public float Probability;       // 뜰 확률
    public float catchDifficulty;   // 낚시 난이도 (게이지가 움직이는 속도 등) (1~10f)
    public float minSize;           // 최소 크기
    public float maxSize;           // 최대 크기
}