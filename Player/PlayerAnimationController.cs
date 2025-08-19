using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [Header("Animation Parameters")]
    [SerializeField] private string isMovingParam = "isMoving";
    [SerializeField] private string moveXParam = "moveX";
    [SerializeField] private string moveYParam = "moveY";

    private Animator animator;
    private Vector2 movementInput;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        UpdateAnimationParameters();
    }

    private void UpdateAnimationParameters()
    {
        // 获取移动输入
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");

        // 更新移动状态
        bool isMoving = inputX != 0 || inputY != 0;
        animator.SetBool(isMovingParam, isMoving);

        if (isMoving)
        {
            // 归一化输入向量
            Vector2 normalizedInput = new Vector2(inputX, inputY).normalized;
            movementInput = normalizedInput;

            // 设置动画参数
            animator.SetFloat(moveXParam, normalizedInput.x);
            animator.SetFloat(moveYParam, normalizedInput.y);

            // 设置优先级参数
            if (Mathf.Abs(inputY) > Mathf.Abs(inputX))
            {
                animator.SetBool("prioritizeVertical", true);
            }
            else
            {
                animator.SetBool("prioritizeVertical", false);
            }
        }
        else
        {
            animator.SetBool("prioritizeVertical", false);
        }
    }
    public void ForceSetAnimationDirection(Vector2 direction)
    {
        if (direction != Vector2.zero)
        {
            Vector2 normalizedDirection = direction.normalized;
            animator.SetFloat(moveXParam, normalizedDirection.x);
            animator.SetFloat(moveYParam, normalizedDirection.y);
        }
    }
}