using UnityEngine;
using System.Collections;
using TMPro;
using MyGame.Inventory;
using System.Collections.Generic;

public class NPCRevealController : MonoBehaviour
{
    [Header("检测设置")]
    public int requiredItemID;  // 需要检测的物品ID
    public float fadeDuration = 1.5f; // 渐变时间
    public float checkInterval = 0.2f; // 检测间隔

    [Header("组件引用")]
    public SpriteRenderer npcSpriteRenderer;
    public PlayerDetector playerDetector;

    private bool playerInRange;
    private bool hasRevealed;
    private float fadeProgress;
    private Coroutine checkCoroutine;

    public SoundName soundEffect;

    private List<Collider2D> colliders; // 存储所有碰撞体和触发器

    private void Awake()
    {
        // 检查 npcSpriteRenderer 是否为 null
        if (npcSpriteRenderer == null)
        {
            Debug.LogError("npcSpriteRenderer 未设置！");
            return;
        }

        // 获取所有碰撞体和触发器
        colliders = new List<Collider2D>();
        Collider2D[] allColliders = GetComponents<Collider2D>();
        foreach (Collider2D collider in allColliders)
        {
            colliders.Add(collider);
        }

        if (colliders.Count == 0)
        {
            Debug.LogError("NPC 上没有碰撞体或触发器！");
        }

        // 初始化透明度
        SetAlpha(0f);

        // 检查玩家对象中的状态
        Player player = FindObjectOfType<Player>();
        if (player != null && player.npcStates.ContainsKey(gameObject.name) && player.npcStates[gameObject.name])
        {
            SetAlpha(1f); // 直接显示 NPC
            hasRevealed = true;
        }
    }

    public void PlayerEnteredTrigger()
    {
        if (!hasRevealed)
        {
            playerInRange = true;
            checkCoroutine = StartCoroutine(CheckPlayerInventory());
        }
    }

    public void PlayerExitedTrigger()
    {
        playerInRange = false;
        if (checkCoroutine != null)
        {
            StopCoroutine(checkCoroutine);
        }
    }

    private IEnumerator CheckPlayerInventory()
    {
        while (playerInRange && !hasRevealed)
        {
            // 检测背包是否包含指定物品
            bool hasItem = InventoryManager.Instance.IsItemInBag(requiredItemID);

            if (hasItem)
            {
                StartCoroutine(FadeInNPC());
                yield break; // 停止检测循环
            }

            yield return new WaitForSeconds(checkInterval);
        }
    }

    private IEnumerator FadeInNPC()
    {
        // 播放音效
        if (soundEffect != SoundName.none)
        {
            var soundDetails = AudioManager.Instance.soundDetailsData.GetSoundDetails(soundEffect);
            EventHandler.CallInitSoundEffect(soundDetails);
        }
        hasRevealed = true;
        float elapsedTime = 0f;
        Color startColor = npcSpriteRenderer.color;

        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            SetAlpha(alpha);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        SetAlpha(1f); // 确保最终完全显示

        // 更新玩家对象中的NPC状态
        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            player.npcStates[gameObject.name] = true;
        }
    }

    private void SetAlpha(float alpha)
    {
        // 设置NPC自身透明度
        if (npcSpriteRenderer == null)
        {
            Debug.LogError("npcSpriteRenderer 为 null！");
            return;
        }

        Color color = npcSpriteRenderer.color;
        color.a = alpha;
        npcSpriteRenderer.color = color;

        // 设置所有子物体的透明度
        foreach (SpriteRenderer childRenderer in GetComponentsInChildren<SpriteRenderer>())
        {
            if (childRenderer != npcSpriteRenderer)
            {
                Color childColor = childRenderer.color;
                childColor.a = alpha;
                childRenderer.color = childColor;
            }
        }

        // 控制碰撞体和触发器的启用和禁用
        if (colliders != null)
        {
            foreach (Collider2D collider in colliders)
            {
                collider.enabled = alpha > 0.1f; // 当透明度大于0.1时启用碰撞体和触发器
            }
        }
        else
        {
            Debug.LogError("colliders 列表为空！");
        }
    }
}