using MFarm.Inventory;
using System.Collections;
using UnityEngine;

public class ToolAnimationController : MonoBehaviour
{
    [SerializeField] private Transform holdItem; // 玩家手中的工具
    [SerializeField] private float swingAngle = 20f; // 挥动角度
    [SerializeField] private float swingDuration = 0.2f; // 单次挥动时间
    [SerializeField] private Vector3 rightFlipScale = new Vector3(-1, 1, 1);
    [SerializeField] private Vector3 normalScale = Vector3.one;
    private bool isSwinging = false;

    private void OnEnable()
    {
        TreeHealth.OnTreeChop += HandleTreeChop;
        MineralHealth.OnMineralMine += HandleTreeChop;
    }

    private void OnDisable()
    {
        TreeHealth.OnTreeChop -= HandleTreeChop;
        MineralHealth.OnMineralMine -= HandleTreeChop;
    }

    private void HandleTreeChop(bool isPlayerOnLeft)
    {
        if (!isSwinging)
            StartCoroutine(SwingTool(isPlayerOnLeft));
    }

    public IEnumerator SwingTool(bool isPlayerOnLeft)
    {
        isSwinging = true;

        // 使用可配置的缩放值
        holdItem.localScale = isPlayerOnLeft ? normalScale : rightFlipScale;

        // 平滑动画
        float elapsed = 0f;
        Vector3 startRot = holdItem.localEulerAngles;
        Vector3 targetRot = startRot + new Vector3(0, 0, swingAngle);

        while (elapsed < swingDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Sin(elapsed / swingDuration * Mathf.PI);
            holdItem.localEulerAngles = Vector3.Lerp(startRot, targetRot, t);
            yield return null;
        }

        holdItem.localEulerAngles = startRot;
        isSwinging = false;
    }
}