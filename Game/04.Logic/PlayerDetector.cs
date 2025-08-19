using UnityEngine;


public class PlayerDetector : MonoBehaviour
{
    public NPCRevealController npcController;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            npcController.PlayerEnteredTrigger();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            npcController.PlayerExitedTrigger();
        }
    }
}