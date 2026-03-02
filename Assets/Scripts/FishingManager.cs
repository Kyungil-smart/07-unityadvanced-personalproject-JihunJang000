using UnityEngine;
using System.Collections.Generic;

public class FishingManager : MonoBehaviour
{
    public static FishingManager Instance { get; private set; }
    
    [Header("민물고기 데이터")]
    public List<FishData> freshWaterFishDB;
    [Header("바다 물고기 데이터")]
    public List<FishData> saltWaterFishDB;

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
}