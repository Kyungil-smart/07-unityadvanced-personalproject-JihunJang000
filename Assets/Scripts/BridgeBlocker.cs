using UnityEngine;

public class BridgeBlocker : MonoBehaviour
{
    private Collider2D blockCollider;

    void Start()
    {
        blockCollider = GetComponent<Collider2D>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // 계속 부딪히는 것을 방지
            PlayerMovement player = collision.gameObject.GetComponent<PlayerMovement>();
            if (player != null && player.isDialogueActive) return;

            if (FishingManager.Instance != null && DialogueManager.Instance != null)
            {
                bool isComplete = FishingManager.Instance.IsFreshWaterEncyclopediaComplete();

                if (isComplete)
                {
                    // 완료했을 때의 대화창
                    DialogueManager.Instance.ShowDialogue("민물고기 마스터!\n바다로 통하는 길이 열렸습니다.");
                    blockCollider.enabled = false;
                }
                else
                {
                    // 아직 못 모았을 때의 대화창
                    int current = FishingManager.Instance.GetUnlockedFreshWaterCount();
                    int total = FishingManager.Instance.freshWaterFishDB.Count;
                    DialogueManager.Instance.ShowDialogue($"아직 바다로 갈 수 없습니다.\n민물고기 도감을 완성하세요!\n(현재 도감: {current} / {total})");
                }
            }
        }
    }
}