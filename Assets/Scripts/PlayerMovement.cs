using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public bool isDialogueActive = false; //대화창 켜져있는지 확인

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Vector2 moveInput;

    private FishingSystem fishingSystem;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        fishingSystem = GetComponent<FishingSystem>();
    }

    void FixedUpdate()
    {
        if (fishingSystem != null && !fishingSystem.isFishing && !isDialogueActive) //낚시중이랑 대화창 떳을떄 안움직이게
        {
            rb.MovePosition(rb.position + moveInput.normalized * speed * Time.fixedDeltaTime);
        }
    }
    
    void Update()
    {
        if (Time.timeScale == 0f) return; // 메뉴 열면 멈추게
        
        //대화창 on이면 움직이기 불가능
        if (isDialogueActive)
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("isWalking", false);
            return;
        }

        
        // 낚시 하고 있지 않을때 움직이기 가능
        if (fishingSystem != null && !fishingSystem.isFishing)
        {
            HandleMovementInput();

            // F키 낚시 시작
            if (Input.GetKeyDown(KeyCode.F))
            {
                fishingSystem.StartFishing(spriteRenderer.flipX);
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

    
}