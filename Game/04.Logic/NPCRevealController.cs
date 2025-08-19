using UnityEngine;
using System.Collections;
using TMPro;
using MyGame.Inventory;
using System.Collections.Generic;

public class NPCRevealController : MonoBehaviour
{
    [Header("�������")]
    public int requiredItemID;  // ��Ҫ������ƷID
    public float fadeDuration = 1.5f; // ����ʱ��
    public float checkInterval = 0.2f; // �����

    [Header("�������")]
    public SpriteRenderer npcSpriteRenderer;
    public PlayerDetector playerDetector;

    private bool playerInRange;
    private bool hasRevealed;
    private float fadeProgress;
    private Coroutine checkCoroutine;

    public SoundName soundEffect;

    private List<Collider2D> colliders; // �洢������ײ��ʹ�����

    private void Awake()
    {
        // ��� npcSpriteRenderer �Ƿ�Ϊ null
        if (npcSpriteRenderer == null)
        {
            Debug.LogError("npcSpriteRenderer δ���ã�");
            return;
        }

        // ��ȡ������ײ��ʹ�����
        colliders = new List<Collider2D>();
        Collider2D[] allColliders = GetComponents<Collider2D>();
        foreach (Collider2D collider in allColliders)
        {
            colliders.Add(collider);
        }

        if (colliders.Count == 0)
        {
            Debug.LogError("NPC ��û����ײ��򴥷�����");
        }

        // ��ʼ��͸����
        SetAlpha(0f);

        // �����Ҷ����е�״̬
        Player player = FindObjectOfType<Player>();
        if (player != null && player.npcStates.ContainsKey(gameObject.name) && player.npcStates[gameObject.name])
        {
            SetAlpha(1f); // ֱ����ʾ NPC
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
            // ��ⱳ���Ƿ����ָ����Ʒ
            bool hasItem = InventoryManager.Instance.IsItemInBag(requiredItemID);

            if (hasItem)
            {
                StartCoroutine(FadeInNPC());
                yield break; // ֹͣ���ѭ��
            }

            yield return new WaitForSeconds(checkInterval);
        }
    }

    private IEnumerator FadeInNPC()
    {
        // ������Ч
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

        SetAlpha(1f); // ȷ��������ȫ��ʾ

        // ������Ҷ����е�NPC״̬
        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            player.npcStates[gameObject.name] = true;
        }
    }

    private void SetAlpha(float alpha)
    {
        // ����NPC����͸����
        if (npcSpriteRenderer == null)
        {
            Debug.LogError("npcSpriteRenderer Ϊ null��");
            return;
        }

        Color color = npcSpriteRenderer.color;
        color.a = alpha;
        npcSpriteRenderer.color = color;

        // ���������������͸����
        foreach (SpriteRenderer childRenderer in GetComponentsInChildren<SpriteRenderer>())
        {
            if (childRenderer != npcSpriteRenderer)
            {
                Color childColor = childRenderer.color;
                childColor.a = alpha;
                childRenderer.color = childColor;
            }
        }

        // ������ײ��ʹ����������úͽ���
        if (colliders != null)
        {
            foreach (Collider2D collider in colliders)
            {
                collider.enabled = alpha > 0.1f; // ��͸���ȴ���0.1ʱ������ײ��ʹ�����
            }
        }
        else
        {
            Debug.LogError("colliders �б�Ϊ�գ�");
        }
    }
}