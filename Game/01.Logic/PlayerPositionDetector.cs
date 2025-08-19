using MFarm.Inventory;
using UnityEngine;

public class PlayerPositionDetector : MonoBehaviour
{
    [Header("检测参数")]
    [SerializeField] private float triggerRadius = 1f; // 触发器半径
    [SerializeField] private Vector2 leftTriggerOffset = new Vector2(-2f, 0f); // 左边触发器偏移
    [SerializeField] private Vector2 rightTriggerOffset = new Vector2(2f, 0f); // 右边触发器偏移

    private CircleCollider2D leftTrigger;
    private CircleCollider2D rightTrigger;
    private TreeHealth treeHealth; // 引用树的主脚本

    public event System.Action<bool> OnPlayerPositionChanged; // 事件：通知玩家位置变化
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
            // 坐标判断逻辑
            Vector3 treePos = transform.parent.position;
            Vector3 playerPos = collision.transform.position;

            // 当玩家X坐标大于树X坐标时在右侧
            IsPlayerOnLeft = playerPos.x < treePos.x;

            //Debug.Log($"玩家在左侧：{IsPlayerOnLeft}"); 
            OnPlayerPositionChanged?.Invoke(IsPlayerOnLeft);
        }
    }

    private void InitializeTriggers()
    {
        // 添加左边触发器
        leftTrigger = gameObject.AddComponent<CircleCollider2D>();
        leftTrigger.isTrigger = true;
        leftTrigger.offset = leftTriggerOffset;
        leftTrigger.radius = triggerRadius;

        // 添加右边触发器
        rightTrigger = gameObject.AddComponent<CircleCollider2D>();
        rightTrigger.isTrigger = true;
        rightTrigger.offset = rightTriggerOffset;
        rightTrigger.radius = triggerRadius;
    }

    private void OnDrawGizmosSelected()
    {
        // 以父物体（树）为基准绘制
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