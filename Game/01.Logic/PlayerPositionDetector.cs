using MFarm.Inventory;
using UnityEngine;

public class PlayerPositionDetector : MonoBehaviour
{
    [Header("������")]
    [SerializeField] private float triggerRadius = 1f; // �������뾶
    [SerializeField] private Vector2 leftTriggerOffset = new Vector2(-2f, 0f); // ��ߴ�����ƫ��
    [SerializeField] private Vector2 rightTriggerOffset = new Vector2(2f, 0f); // �ұߴ�����ƫ��

    private CircleCollider2D leftTrigger;
    private CircleCollider2D rightTrigger;
    private TreeHealth treeHealth; // �����������ű�

    public event System.Action<bool> OnPlayerPositionChanged; // �¼���֪ͨ���λ�ñ仯
    public bool IsPlayerOnLeft { get; private set; }

    private void Awake()
    {
        treeHealth = GetComponentInParent<TreeHealth>(); 
        InitializeTriggers();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // �����ж��߼�
            Vector3 treePos = transform.parent.position;
            Vector3 playerPos = collision.transform.position;

            // �����X���������X����ʱ���Ҳ�
            IsPlayerOnLeft = playerPos.x < treePos.x;

            //Debug.Log($"�������ࣺ{IsPlayerOnLeft}"); 
            OnPlayerPositionChanged?.Invoke(IsPlayerOnLeft);
        }
    }

    private void InitializeTriggers()
    {
        // �����ߴ�����
        leftTrigger = gameObject.AddComponent<CircleCollider2D>();
        leftTrigger.isTrigger = true;
        leftTrigger.offset = leftTriggerOffset;
        leftTrigger.radius = triggerRadius;

        // ����ұߴ�����
        rightTrigger = gameObject.AddComponent<CircleCollider2D>();
        rightTrigger.isTrigger = true;
        rightTrigger.offset = rightTriggerOffset;
        rightTrigger.radius = triggerRadius;
    }

    private void OnDrawGizmosSelected()
    {
        // �Ը����壨����Ϊ��׼����
        Vector3 treePos = transform.parent.position;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(
            treePos + new Vector3(leftTriggerOffset.x, leftTriggerOffset.y, 0f),
            triggerRadius
        );

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(
            treePos + new Vector3(rightTriggerOffset.x, rightTriggerOffset.y, 0f),
            triggerRadius
        );
    }
}