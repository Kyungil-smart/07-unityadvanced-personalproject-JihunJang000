using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("UI 연결")]
    public GameObject dialoguePanel;     // 대화창 패널
    public TextMeshProUGUI dialogueText; // 대화창 텍스트
    public Image fishIconImage; //띄울 물고기 이미지 

    private PlayerMovement player;
    
    // 대화창이 열린 시간을 기억할 변수
    private int openedFrame;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        
        // 씬에 있는 플레이어 찾기 (나중에 FindFirstObjectByType으로)
        player = FindAnyObjectByType<PlayerMovement>(); 
    }

    private void Update()
    {
        // 대화창이 켜져 있고 && 열린 지 1프레임 이상 지났을 때만 꺼지도록 방어
        if (dialoguePanel != null && dialoguePanel.activeSelf && Time.frameCount > openedFrame)
        {
            // 스페이스바를 누르거나 마우스 왼쪽 버튼 클릭하면 창 닫음
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                CloseDialogue();
            }
        }
    }

    // 외부에서 대화창을 띄울 때 호출하는 함수
    public void ShowDialogue(string message)
    {
        if (dialoguePanel == null) return;

        dialogueText.text = message;
        
        // 일반 대화에서는 그림을 숨김.
        if (fishIconImage != null)
        {
            fishIconImage.gameObject.SetActive(false);
        }
        
        dialoguePanel.SetActive(true);

        // 플레이어의 움직임을 멈춤
        if (player != null)
        {
            player.isDialogueActive = true; 
        }

        // 창이 열린 현재 프레임을 기록
        openedFrame = Time.frameCount;
    }

    // 물고기 잡았을때 물고기 스프라이트까지 보여주는 텍스트창
    public void ShowCatchDialogue(string message, Sprite fishSprite)
    {
        if (dialoguePanel == null) return;

        dialogueText.text = message;

        // 물고기 이미지를 교체하고 화면에 보이게
        if (fishIconImage != null && fishSprite != null)
        {
            fishIconImage.sprite = fishSprite;
            fishIconImage.gameObject.SetActive(true); 
        }

        dialoguePanel.SetActive(true);
        if (player != null) player.isDialogueActive = true;

        // 창이 열린 현재 프레임을 기록
        openedFrame = Time.frameCount;
    }
    
    // 대화창을 닫는 함수
    public void CloseDialogue()
    {
        dialoguePanel.SetActive(false);
  
        if (player != null)
        {
            player.isDialogueActive = false;
        }
    }
}