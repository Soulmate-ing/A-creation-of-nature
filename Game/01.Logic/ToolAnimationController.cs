using MFarm.Inventory;
using System.Collections;
using UnityEngine;

public class ToolAnimationController : MonoBehaviour
{
    [SerializeField] private Transform holdItem; // ������еĹ���
    [SerializeField] private float swingAngle = 20f; // �Ӷ��Ƕ�
    [SerializeField] private float swingDuration = 0.2f; // ���λӶ�ʱ��
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

        // ʹ�ÿ����õ�����ֵ
        holdItem.localScale = isPlayerOnLeft ? normalScale : rightFlipScale;

        // ƽ������
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