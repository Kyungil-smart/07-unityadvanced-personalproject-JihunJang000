using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class FishRecord
{
    public bool isUnlocked = false; // 도감 해금 여부
    public int catchCount = 0;      // 잡은 횟수
    public float maxSize = 0f;      // 최고 크기 기록
}

public class FishingManager : MonoBehaviour
{
    public static FishingManager Instance { get; private set; }
    
    [Header("민물고기 데이터")]
    public List<FishData> freshWaterFishDB;
    [Header("바다 물고기 데이터")]
    public List<FishData> saltWaterFishDB;
	
	// 물고기 이름= Key, 물고기 잡은 기록 정보 = value
	public Dictionary<string, FishRecord> myFishRecords = new Dictionary<string, FishRecord>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
	public void AddFishRecord(FishData caughtFish, float size)
    {
        string fishName = caughtFish.fishName;

        // 한 번도 잡은 적 없는 물고기라면 딕셔너리에 추가.
        if (!myFishRecords.ContainsKey(fishName))
        {
            myFishRecords[fishName] = new FishRecord();
        }

        FishRecord record = myFishRecords[fishName];
        
        record.isUnlocked = true; // 도감 해금 (검은 실루엣 벗겨짐)
        record.catchCount++;      // 잡은 횟수 1 증가

        // 방금 잡은 크기가 기존 최고 기록보다 크면 갱신
        if (size > record.maxSize)
        {
            record.maxSize = size;
            Debug.Log($"{fishName} 최대 크기 갱신 ({size:F1}cm)");
        }
        else
        {
            Debug.Log($"{fishName} 도감에 기록됨. ({record.catchCount}마리째)");
        }
    }

    // 확률에 맞게 랜덤 물고기를 뽑는 함수
    public FishData GetRandomFish(List<FishData> fishDatabase)
    {
        float totalWeight = 0;
        foreach (FishData fish in fishDatabase)
        {
            totalWeight += fish.Probability;
        }
        
        float randomValue = Random.Range(0, totalWeight);
        
        float currentWeight = 0;
        foreach (FishData fish in fishDatabase)
        {
            currentWeight += fish.Probability;
            // 랜덤 값이 현재 누적 확률 안쪽이라면 그 물고기가 당첨
            if (randomValue <= currentWeight)
            {
                return fish;
            }
        }

        
        return fishDatabase[0];
    }

	// 민물 도감 다채웠는지
	public bool IsFreshWaterEncyclopediaComplete()
    {
        int unlockedCount = 0;
        foreach (FishData fish in freshWaterFishDB)
        {
            // 도감 기록에 이 물고기가 있고, 해금(isUnlocked) 상태라면 카운트 증가
            if (myFishRecords.ContainsKey(fish.fishName) && myFishRecords[fish.fishName].isUnlocked)
            {
                unlockedCount++;
            }
        }
        // 모은 개수가 민물고기 DB 전체 개수와 같거나 크면 true 반환
        return unlockedCount >= freshWaterFishDB.Count;
    }

    // 현재까지 모은 민물고기 개수를 숫자로 알려주는 함수
    public int GetUnlockedFreshWaterCount()
    {
        int count = 0;
        foreach (FishData fish in freshWaterFishDB)
        {
            if (myFishRecords.ContainsKey(fish.fishName) && myFishRecords[fish.fishName].isUnlocked)
            {
                count++;
            }
        }
        return count;
    }
}