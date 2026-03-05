using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [Header("메뉴 UI 연결")]
    public GameObject menuPanel;
    public GameObject controlsPanel; 

    void Start()
    {
        // 시작할 때 메뉴와 조작법 창은 숨겨둡니다
        if (menuPanel != null) menuPanel.SetActive(false);
        if (controlsPanel != null) controlsPanel.SetActive(false); 
    }

    void Update()
    {
        // ESC 키
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // 조작법 창이 열려있다면 메뉴창보다 먼저 닫기
            if (controlsPanel != null && controlsPanel.activeSelf)
            {
                CloseControls();
            }
            // 메뉴창이 열려있다면 메뉴를 닫습니다.
            else if (menuPanel.activeSelf)
            {
                CloseMenu();
            }
            // 둘 다 안 열려있으면 메뉴를 엽니다.
            else
            {
                OpenMenu();
            }
        }
    }

    public void OpenMenu()
    {
        menuPanel.SetActive(true);
        Time.timeScale = 0f; // 일시정지
    }

    public void CloseMenu()
    {
        menuPanel.SetActive(false);
        Time.timeScale = 1f; // 일시정지 해제
    }
    
    public void OpenControls()
    {
        if (controlsPanel != null) controlsPanel.SetActive(true);
    }
    
    public void CloseControls()
    {
        if (controlsPanel != null) controlsPanel.SetActive(false);
    }

    public void QuitGame()
    {
        Debug.Log("게임을 종료합니다!");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}