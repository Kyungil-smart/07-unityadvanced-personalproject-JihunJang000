using UnityEngine;
using System.Collections; 
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    
    [Header("낚시 설정")]
    public LayerMask freshWaterLayer; // 민물 레이어
    public LayerMask saltWaterLayer;  // 바닷물 레이어

    public CameraFollow cameraFollow; 
    
    public RectTransform successZoneRect; //성공 범위

    [Header("UI 설정")]
    public GameObject minigamePanel; //  UI  끄고 키는 거
    public Slider needleSlider; // 바늘 위치
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Vector2 moveInput;

    private bool isFishing = false;
    private bool isCasting = false; 
    
	private bool isSaltWaterFishing = false; // 바닷물인지 체크

    // 낚시 미니게임 관련 변수
    private bool isMinigameActive = false; 
    private FishData currentTargetFish; 
    
	public bool isDialogueActive = false; //대화창 켜져있는지 확인

    // 시계바늘 타이밍 시스템
    private float needlePos = 0f;       // 바늘의 현재 위치 (0.0 ~ 1.0)
    private float needleDir = 1f;       // 바늘의 이동 방향 (1=오른쪽, -1=왼쪽)
    private float needleSpeed = 1f;     // 바늘이 움직이는 속도
    private float successZoneMin = 0f;  // 성공 구간 시작점
    private float successZoneMax = 0f;  // 성공 구간 끝점
    
    private Vector2 debugTargetPos;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        
        if (minigamePanel != null) minigamePanel.SetActive(false); //시작시에 UI 숨기기.
    }

    void Update()
    {
		if (isDialogueActive)
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("isWalking", false);
            return;
        }

        if (!isFishing)
        {
            HandleMovementInput();
        }

        // F키: 낚시 시작
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (!isFishing)
            {
                StartFishing();
            }
        }

        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isMinigameActive) 
            {
                AttemptCatch(); // 타이밍 체크 함수 실행
            }
        }

        // 미니게임이 켜져 있을 때 바늘을 왕복 이동시킵니다.
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
        }
    }

    void HandleMovementInput()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        if (moveInput.x < 0) spriteRenderer.flipX = true;
        else if (moveInput.x > 0) spriteRenderer.flipX = false;

        animator.SetBool("isWalking", moveInput.magnitude > 0);
    }

    void StartFishing()
    {
        isFishing = true;
        isCasting = true;
        isMinigameActive = false; 
        currentTargetFish = null; 
        rb.linearVelocity = Vector2.zero; 
        
        animator.SetBool("isWalking", false);
        animator.SetBool("isFishing", true); 

        Vector2 visualCenter = (Vector2)transform.position + new Vector2(cameraFollow.offset.x, cameraFollow.offset.y);
        float targetX = spriteRenderer.flipX ? -2f : 2f;
        Vector2 targetPos = visualCenter + new Vector2(targetX, -2f);
        debugTargetPos = targetPos;
        
        Collider2D freshHit = Physics2D.OverlapCircle(targetPos, 0.1f, freshWaterLayer);
        Collider2D saltHit = Physics2D.OverlapCircle(targetPos, 0.1f, saltWaterLayer);
        
        if (freshHit != null)
        {
            isSaltWaterFishing = false; // 민물 낚시 모드
            Debug.Log("민물 발견! 미끼를 던집니다");
            StartCoroutine(CastingRoutine(false));
        }
        else if (saltHit != null)
        {
            isSaltWaterFishing = true; // 바다 낚시 모드
            Debug.Log("바다 발견! 미끼를 던집니다");
            StartCoroutine(CastingRoutine(false));
        }
        else
        {
            Debug.Log("물이 없음 (위치: " + targetPos + ")");
            StartCoroutine(CastingRoutine(true));
        }
        
    }

    IEnumerator CastingRoutine(bool isMissed)
    {
        yield return new WaitForSeconds(1.12f);
        isCasting = false;

        if (isMissed)
        {
            FinishFishing(false); 
        }
        else
        {
            StartCoroutine(WaitBiteRoutine());
        }
    }

    IEnumerator WaitBiteRoutine()
    {
        float waitTime = Random.Range(1.0f, 3.0f);
        Debug.Log($"{waitTime:F1}초 후에 입질이 옵니다");
        yield return new WaitForSeconds(waitTime);
        
        // 1. 입질 시 물고기 결정
        if (FishingManager.Instance != null)
        {
            List<FishData> targetDB = isSaltWaterFishing ? FishingManager.Instance.saltWaterFishDB : FishingManager.Instance.freshWaterFishDB;

            if (targetDB.Count > 0)
            {
                currentTargetFish = FishingManager.Instance.GetRandomFish(targetDB);
            }
        }

        // 난이도에 따라 구간과 속도가 변함
        float diff = currentTargetFish != null ? currentTargetFish.catchDifficulty : 1f;
        
        // 난이도가 높을수록 바늘이 빨라집니다.
        needleSpeed = 1f + (diff * 0.15f); 
        
        // 난이도가 높을수록 성공 구간(정중앙 기준)이 좁아집니다.
        float zoneWidth = Mathf.Clamp(0.5f - (diff * 0.04f), 0.1f, 0.5f);
        successZoneMin = 0.5f - (zoneWidth / 2f);
        successZoneMax = 0.5f + (zoneWidth / 2f);

        if (successZoneRect != null)
        {
            // 네모의 왼쪽 끝을 successZoneMin,Max 비율에 맞춤
            successZoneRect.anchorMin = new Vector2(successZoneMin, successZoneRect.anchorMin.y);
            successZoneRect.anchorMax = new Vector2(successZoneMax, successZoneRect.anchorMax.y);
            
            // 여백(Offset)을 0으로 만들어 부모 영역에 딱 맞게 채움
            successZoneRect.offsetMin = new Vector2(0f, successZoneRect.offsetMin.y);
            successZoneRect.offsetMax = new Vector2(0f, successZoneRect.offsetMax.y);
        }

        needlePos = 0f;
        
        // 변수 초기화 및 미니게임 시작
        needlePos = 0f;
        needleDir = 1f;
        isMinigameActive = true;

        Debug.Log($"물고기 입질 시작 [{currentTargetFish.fishName}]");
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

            // 🌟 Debug.Log 대신 대화창 호출 (물고기 이미지도 함께 넘겨줍니다!)
            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.ShowCatchDialogue(
                    $"와! {currentTargetFish.fishName}을(를) 낚았다!\n크기: {caughtSize:F1}cm", 
                    currentTargetFish.fishIcon
                );
            }
            
            if (FishingManager.Instance != null)
            {
                FishingManager.Instance.AddFishRecord(currentTargetFish, caughtSize);
            }

            FinishFishing(true);
        }
        else
        {
            // 실패했을때 대화창 띄우기 (이미지 없이 일반 대화창으로)
            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.ShowDialogue("앗 타이밍을 놓쳐서 물고기가 도망갔다...");
            }

            FinishFishing(false);
        }
    }
    
    void FinishFishing(bool isSuccess)
    {
        isMinigameActive = false;
        animator.SetBool("isFishing", false);
        
        if (minigamePanel != null) minigamePanel.SetActive(false); //UI 끄기 확실하게.
        
        // 나중에 여기서 if(isSuccess) 로 성공, 실패 사운드 구분.
        animator.SetTrigger("isCaught");
        Invoke("EnableMovement", 0.75f); 
    }

    void EnableMovement()
    {
        isFishing = false;
    }

    void FixedUpdate()
    {
        if (!isFishing && !isDialogueActive)
        {
            rb.MovePosition(rb.position + moveInput.normalized * speed * Time.fixedDeltaTime);
        }
    }

    void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(debugTargetPos, 0.1f);
        }
    }
}