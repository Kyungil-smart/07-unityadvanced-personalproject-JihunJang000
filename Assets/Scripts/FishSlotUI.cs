using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class FishSlotUI : MonoBehaviour
{
    public Image fishIcon;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI infoText;

    // 매니저에서 UI세팅
    public void Setup(FishData data, FishRecord record)
    {
        // 도감이 해금된(잡은 적 있는) 물고기
        if (record != null && record.isUnlocked)
        {
            fishIcon.sprite = data.fishIcon;
            fishIcon.color = Color.white; // 원래 색상 보여주기
            nameText.text = data.fishName;
            infoText.text = $"포획: {record.catchCount}마리\n최대: {record.maxSize:F1}cm";
        }
        else
        {
            // 아직 못 잡은 물고기
            fishIcon.sprite = data.fishIcon;
            fishIcon.color = Color.black; // 검은색으로 덮어서 실루엣만 표시
            nameText.text = "???";
            infoText.text = "포획: 0마리\n최대: 0.0cm";
        }
    }
}