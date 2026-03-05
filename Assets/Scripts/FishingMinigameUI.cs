using UnityEngine;
using UnityEngine.UI;

public class FishingMinigameUI : MonoBehaviour
{
    [Header("UI 설정")]
    public GameObject minigamePanel; //  UI  끄고 키는 거
    public Slider needleSlider; // 바늘 위치
    public RectTransform successZoneRect; //성공 범위

    private bool isMinigameActive = false; 
    private FishData currentTargetFish; 
    private FishingSystem currentFishingSystem; 
    
    // 시계바늘 타이밍 시스템
    private float needlePos = 0f;       // 바늘의 현재 위치 (0 ~ 1)
    private float needleDir = 1f;       // 바늘의 이동 방향 (1=오른쪽, -1=왼쪽)
    private float needleSpeed = 1f;     // 바늘이 움직이는 속도
    private float successZoneMin = 0f;  // 성공 구간 시작점
    private float successZoneMax = 0f;  // 성공 구간 끝점

    void Start()
    {
        if (minigamePanel != null) minigamePanel.SetActive(false); //시작시에 UI 숨기기.
    }

    void Update()
    {
        if (Time.timeScale == 0f) return; // 메뉴 열면 멈추게

        // 미니게임이 켜져 있을 때 바늘을 왕복 이동
        if (isMinigameActive)
        {
            // 속도와 방향에 맞춰 바늘 위치 이동
            needlePos += needleDir * needleSpeed * Time.deltaTime;

            // 양쪽 끝(0.0 또는 1.0)에 도달하면 튕겨서 반대로 돌아감
            if (needlePos >= 1f)
            {
                needlePos = 1f;
                needleDir = -1f; // 왼쪽으로 방향 전환
            }
            else if (needlePos <= 0f)
            {
                needlePos = 0f;
                needleDir = 1f; // 오른쪽으로 방향 전환
            }
            
            // 슬라이더 UI에 위치 적용
            if (needleSlider != null)
            {
                needleSlider.value = needlePos;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                AttemptCatch(); // 타이밍 체크 함수 실행
            }
        }
    }

    public void StartMinigame(FishData fish, FishingSystem system)
    {
        currentTargetFish = fish;
        currentFishingSystem = system;

        // 난이도에 따라 구간과 속도가 변함
        float diff = currentTargetFish != null ? currentTargetFish.catchDifficulty : 1f;
        
        // 난이도가 높을수록 바늘이 빨라짐
        needleSpeed = 1f + (diff * 0.15f); 
        
        // 난이도가 높을수록 성공 구간(정중앙 기준)이 좁아짐
        float zoneWidth = Mathf.Clamp(0.5f - (diff * 0.04f), 0.1f, 0.5f);
        successZoneMin = 0.5f - (zoneWidth / 2f);
        successZoneMax = 0.5f + (zoneWidth / 2f);

        if (successZoneRect != null)
        {
            // 네모의 왼쪽 끝을 successZoneMin,Max 비율에 맞춤
            successZoneRect.anchorMin = new Vector2(successZoneMin, successZoneRect.anchorMin.y);
            successZoneRect.anchorMax = new Vector2(successZoneMax, successZoneRect.anchorMax.y);
            
            // 여백을 0으로 만들어서 부모 영역에 딱 맞게 채움
            successZoneRect.offsetMin = new Vector2(0f, successZoneRect.offsetMin.y);
            successZoneRect.offsetMax = new Vector2(0f, successZoneRect.offsetMax.y);
        }

        // 변수 초기화 및 미니게임 시작
        needlePos = 0f;
        needleDir = 1f;
        isMinigameActive = true;

        if (minigamePanel != null) minigamePanel.SetActive(true); //UI에 띄워줌.
    }

    // 스페이스바를 눌렀을 때 실행되는 판정 함수
    void AttemptCatch()
    {
        isMinigameActive = false; 
        if (minigamePanel != null) minigamePanel.SetActive(false);

        // 바늘 위치가 성공 구간 안에 있는지 검사
        if (needlePos >= successZoneMin && needlePos <= successZoneMax)
        {
            // 물고기 크기를 랜덤으로 결정
            float caughtSize = Random.Range(currentTargetFish.minSize, currentTargetFish.maxSize);
            
            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.ShowCatchDialogue($"와! {currentTargetFish.fishName}을(를) 낚았다!\n크기: {caughtSize:F1}cm", currentTargetFish.fishIcon);
            }
            
            if (FishingManager.Instance != null)
            {
                FishingManager.Instance.AddFishRecord(currentTargetFish, caughtSize);
            }

            currentFishingSystem.FinishFishing(true);
        }
        else
        {
            // 실패했을때 대화창 띄우기 (이미지 없이 일반 대화창으로)
            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.ShowDialogue("앗 타이밍을 놓쳐서 물고기가 도망갔다...");
            }
            currentFishingSystem.FinishFishing(false);
        }
    }
}