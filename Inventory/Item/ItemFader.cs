using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFader : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// �𽥻ָ���ɫ
    /// </summary>
    public void FadeIn()
    {
        if (spriteRenderer != null)
        {
            Color targetColor = new Color(1, 1, 1, 1);
            spriteRenderer.DOColor(targetColor, Settings.itemfadeDuration);
        }
    }

    /// <summary>
    /// �𽥰�͸��
    /// </summary>
    public void FadeOut()
    {
        if (spriteRenderer != null)
        {
            Color targetColor = new Color(1, 1, 1, Settings.targetAlpha);
            spriteRenderer.DOColor(targetColor, Settings.itemfadeDuration);
        }
    }

    private void OnDestroy()
    {
        // ȷ��������ʱֹͣ���ж���
        DOTween.KillAll();
    }
}