// Talkable.cs
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Talkable : MonoBehaviour

{
    [Header("NPC身份信息")]
    public string npcName = "默认NPC"; // NPC显示名称
    public Sprite npcImage;           // NPC头像
    [Header("返回位置")]
    [Tooltip("玩家完成小游戏后的返回位置")]

    public static Vector3 GlobalReturnPosition;   // 记录玩家触发对话时的位置z


    public enum NPCType { VillageChief, ShuiCheNPC, KuangShiNPC, ChuanNPC, PulleyNPC }
    public static Talkable currentTalker; // 当前对话的NPC

    [Header("基础配置")]
    [Tooltip("设置NPC类型\n村长需要特殊对话逻辑")]
    public NPCType npcType = NPCType.VillageChief;

    [Header("对话内容配置")]
    [TextArea(1, 3), Tooltip("初始对话内容（首次交互时显示）")]
    public string[] initialDialogue = { "默认初始对话" };

    [TextArea(1, 3), Tooltip("小游戏完成后的对话内容（仅普通NPC有效）")]
    public string[] postGameDialogue = { "默认完成对话" };

    [TextArea(1, 3), Tooltip("村长的最终对话（完成所有普通NPC后显示）")]

    public string[] finalChiefDialogue = { "最终祝贺对话" };

    private bool playerInRange;

    public static int TotalcompletedNPCs;


    private void Update()
    {

        if (playerInRange && Input.GetKeyDown(KeyCode.F))
        {

            currentTalker = this;

            if (DialogueControl.Instance.dialogueBox.activeInHierarchy) return;
            switch (npcType)
            {
                case NPCType.VillageChief:
                    if (TotalcompletedNPCs < 3)
                    {
                        DialogueControl.Instance.ShowDialogue(initialDialogue);
                    }
                    else
                    {
                        DialogueControl.Instance.ShowDialogue(finalChiefDialogue);
                        TaskManager.Instance.HeshenTask.SetActive(true);
                        TaskManager.Instance.UpdateTaskState(4);
                    }

                    break;
                case NPCType.ShuiCheNPC:
                    if (PuzzleMG.PuzzlegameComplete == false)
                    {
                        DialogueControl.Instance.ShowDialogue(initialDialogue);

                    }
                    else
                    {
                        DialogueControl.Instance.ShowDialogue(postGameDialogue);
                    }
                    break;
                case NPCType.KuangShiNPC:
                    if (MineralUIManager.KuangshigameComplete == false)
                    {
                        DialogueControl.Instance.ShowDialogue(initialDialogue);

                    }
                    else
                    {
                        DialogueControl.Instance.ShowDialogue(postGameDialogue);
                    }
                    break;
                case NPCType.ChuanNPC:
                    if (UI.ChuangameComplete == false)
                    {
                        DialogueControl.Instance.ShowDialogue(initialDialogue);

                    }
                    else
                    {
                        DialogueControl.Instance.ShowDialogue(postGameDialogue);
                    }
                    break;
                case NPCType.PulleyNPC:
                    if (BucketRopeController.PulleygameComplete == false)
                    {
                        DialogueControl.Instance.ShowDialogue(initialDialogue);
                    }
                    else
                    {
                        DialogueControl.Instance.ShowDialogue(postGameDialogue);
                        TaskManager.Instance.UpdateTaskState(6);
                    }
                    break;
                default:
                    break;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            GlobalReturnPosition = other.transform.position;

        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

        }
    }

}