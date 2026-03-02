using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Vector2 moveInput;

    // 낚시 상태 변수
    private bool isFishing = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // 낚시 중이 아닐 때만 이동 입력 처리
        if (!isFishing)
        {
            HandleMovementInput();
        }

        // F키를 눌러 낚시 시작 또는 종료
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (!isFishing)
            {
                StartFishing();
            }
            else
            {
                FinishFishing();
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
        rb.linearVelocity = Vector2.zero; // 물리 이동 즉시 정지 (유니티 6 기준 linearVelocity)
        animator.SetBool("isWalking", false);
        animator.SetBool("isFishing", true); // Casting -> Waiting으로 이어짐
    }

    void FinishFishing()
    {
        animator.SetBool("isFishing", false);
        // Caught 애니메이션 재생 트리거
        animator.SetTrigger("isCaught");
        
        // 애니메이션이 완전히 끝난 뒤에 움직이게 하려면 
        // 애니메이션 이벤트(Animation Event)를 쓰거나 코루틴을 사용합니다.
        Invoke("EnableMovement", 0.75f); // 1.5초(애니메이션 길이) 뒤에 이동 가능하게 설정
    }

    void EnableMovement()
    {
        isFishing = false;
        //animator.SetBool("isFishing", false);
    }

    void FixedUpdate()
    {
        if (!isFishing)
        {
            rb.MovePosition(rb.position + moveInput.normalized * speed * Time.fixedDeltaTime);
        }
    }
}