using UnityEngine;
using System.Collections.Generic;

public class EncyclopediaUI : MonoBehaviour
{
    [Header("UI 연결")]
    public GameObject encyclopediaPanel; // 도감 창 전체
    public Transform contentTransform;   // Grid Layout Group이 붙어있는 Content
    public GameObject fishSlotPrefab;    // FishSlot  프리팹

    void Start()
    {
        // 시작할 때 도감 창은 숨기기
        encyclopediaPanel.SetActive(false);
    }

    void Update()
    {
        // 도감 온오프
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (encyclopediaPanel.activeSelf)
            {
                encyclopediaPanel.SetActive(false);
            }
            else
            {
                OpenEncyclopedia();
            }
        }
    }

    public void OpenEncyclopedia()
    {
        encyclopediaPanel.SetActive(true);
        RefreshUI();
    }

    // 도감 내용을 최신화해서 그려주는 함수
    void RefreshUI()
    {
        // 기존에 남아있는 슬롯 삭제
        foreach (Transform child in contentTransform)
        {
            Destroy(child.gameObject);
        }

        // 민물고기 DB와 바다물고기 DB를 하나의 리스트로 합침
        List<FishData> allFish = new List<FishData>();
        allFish.AddRange(FishingManager.Instance.freshWaterFishDB);
        allFish.AddRange(FishingManager.Instance.saltWaterFishDB);

        // 합쳐진 모든 물고기를 순회하며 도감 슬롯을 생성
        foreach (FishData fish in allFish)
        {
            GameObject slotGO = Instantiate(fishSlotPrefab, contentTransform);
            FishSlotUI slotUI = slotGO.GetComponent<FishSlotUI>();

            FishRecord record = null;
            if (FishingManager.Instance.myFishRecords.ContainsKey(fish.fishName))
            {
                record = FishingManager.Instance.myFishRecords[fish.fishName];
            }

            slotUI.Setup(fish, record);
        }
    }

	public void CloseEncyclopedia()
    {
		Debug.Log("닫기 버튼이 눌렸습니다.");
        encyclopediaPanel.SetActive(false);
    }
}