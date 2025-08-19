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
    /// 逐渐恢复颜色
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
    /// 逐渐半透明
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
        // 确保在销毁时停止所有动画
        DOTween.KillAll();
    }
}