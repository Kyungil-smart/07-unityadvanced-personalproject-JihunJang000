using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FishingSystem : MonoBehaviour
{
    [Header("낚시 설정")]
    public LayerMask freshWaterLayer; // 민물 레이어
    public LayerMask saltWaterLayer;  // 바닷물 레이어
    public CameraFollow cameraFollow; 

    public FishingMinigameUI minigameUI; 

    public bool isFishing = false;
    private bool isSaltWaterFishing = false; // 바닷물인지 체크
    private FishData currentTargetFish;
    private Vector2 debugTargetPos;

    private Animator animator;
    private Rigidbody2D rb;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void StartFishing(bool isFacingLeft)
    {
        isFishing = true;
        rb.linearVelocity = Vector2.zero;
        
        animator.SetBool("isWalking", false);
        animator.SetBool("isFishing", true); 

		//에셋의 pivot위치가 0,0이 아니라서 보정
        Vector2 visualCenter = (Vector2)transform.position + new Vector2(cameraFollow.offset.x, cameraFollow.offset.y);

		//낚시 미끼 떨어지는 곳 계산.
        float targetX = isFacingLeft ? -2f : 2f;
        Vector2 targetPos = visualCenter + new Vector2(targetX, -2f);
        debugTargetPos = targetPos;
        
		//민물 레이어 인식하는 것과 바닷물레이어 인식하는 두개
        Collider2D freshHit = Physics2D.OverlapCircle(targetPos, 0.1f, freshWaterLayer);
        Collider2D saltHit = Physics2D.OverlapCircle(targetPos, 0.1f, saltWaterLayer);
        
        if (freshHit != null)
        {
            isSaltWaterFishing = false; 
            Debug.Log("민물 발견! 미끼를 던집니다");
            StartCoroutine(CastingRoutine(false));
        }
        else if (saltHit != null)
        {
            isSaltWaterFishing = true; 
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
        yield return new WaitForSeconds(waitTime);
        
        // 입질 시 물고기 결정
        if (FishingManager.Instance != null)
        {
            List<FishData> targetDB = isSaltWaterFishing ? FishingManager.Instance.saltWaterFishDB : FishingManager.Instance.freshWaterFishDB;
            if (targetDB.Count > 0)
            {
                currentTargetFish = FishingManager.Instance.GetRandomFish(targetDB);
            }
        }

        Debug.Log($"물고기 입질 시작 [{currentTargetFish.fishName}]");
        
        if (minigameUI != null)
        {
            minigameUI.StartMinigame(currentTargetFish, this);
        }
    }

    public void FinishFishing(bool isSuccess)
    {
        animator.SetBool("isFishing", false);
        
        // 나중에 여기서 if(isSuccess) 로 성공, 실패 사운드 구분.
        animator.SetTrigger("isCaught");
        Invoke("EnableMovement", 0.75f); 
    }

    void EnableMovement()
    {
        isFishing = false;
    }

    void OnDrawGizmos() //낚시 찌 보이게
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(debugTargetPos, 0.1f);
        }
    }
}