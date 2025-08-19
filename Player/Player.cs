using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    public float speed;
    private float inputX;
    private float inputY;
    private Vector2 movementInput;

    private bool inputDisable;
    private bool gameStarted = false; // 添加一个标志，表示游戏是否已经开始

    // 用于显示头顶物品的 SpriteRenderer
    public SpriteRenderer holdItemRenderer;

    // 允许玩家移动的场景名称列表
    public List<string> allowedScenes = new List<string> { "01.Field" }; // 默认允许在 "MainScene" 场景中移动

    public SoundName footStep;
    private float timeSinceLastSound = 0f;
    public float soundInterval = 0.5f; // 音效播放间隔时间（秒）

    private SpriteRenderer playerSprite;
    private BoxCollider2D playerCollider; // 添加 BoxCollider2D 引用

    public Dictionary<string, bool> npcStates = new Dictionary<string, bool>();

    // 新增：引用 ToolAnimationController
    private ToolAnimationController toolAnimationController;

    private void Awake()
    {
        playerSprite = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<BoxCollider2D>(); // 获取 BoxCollider2D 引用
        inputDisable = true; // 默认禁用输入
        gameStarted = false; // 游戏未开始

        playerSprite.enabled = false;
        playerCollider.enabled = false; // 默认禁用碰撞器
        toolAnimationController = GetComponent<ToolAnimationController>(); // 获取 ToolAnimationController 引用
    }

    private void OnEnable()
    {
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadEvent;
        EventHandler.MoveToPosition += OnMoveToPosition;
        EventHandler.ItemSelectedEvent += OnItemSelectedEvent; // 添加物品选中事件
        EventHandler.MouseClickedEvent += OnMouseClickedEvent;
        // 订阅视频播放结束事件
        EventHandler.VideoPlaybackFinishedEvent += OnVideoPlaybackFinished;
    }

    private void OnDisable()
    {
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadEvent;
        EventHandler.MoveToPosition -= OnMoveToPosition;
        EventHandler.ItemSelectedEvent -= OnItemSelectedEvent; // 添加物品选中事件
        EventHandler.MouseClickedEvent -= OnMouseClickedEvent;
        // 取消订阅视频播放结束事件
        EventHandler.VideoPlaybackFinishedEvent -= OnVideoPlaybackFinished;
    }

    private void OnMoveToPosition(Vector3 targetPosition)
    {
        transform.position = targetPosition;
    }

    private void OnAfterSceneLoadEvent()
    {
        // 检查当前场景是否在允许列表中
        if (allowedScenes.Contains(SceneManager.GetActiveScene().name))
        {
            inputDisable = !gameStarted;
            // 根据游戏是否开始控制角色显示
            playerSprite.enabled = gameStarted;
            playerCollider.enabled = gameStarted; // 同步碰撞器状态
        }
        else
        {
            inputDisable = true;
            playerSprite.enabled = false; // 非主场景强制隐藏
            playerCollider.enabled = false; // 禁用碰撞器
            if (holdItemRenderer != null)
            {
                holdItemRenderer.enabled = false;
            }
        }
    }

    private void OnBeforeSceneUnloadEvent()
    {
        inputDisable = true;
        // 隐藏头顶物品
        if (holdItemRenderer != null)
        {
            holdItemRenderer.enabled = false;
        }
        // 隐藏玩家和禁用碰撞器
        playerSprite.enabled = false;
        playerCollider.enabled = false;
    }

    private void Update()
    {
        if (!inputDisable)
        {
            PlayerInput();
        }
    }

    private void FixedUpdate()
    {
        Movement();
    }

    private void PlayerInput()
    {
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");
        movementInput = new Vector2(inputX, inputY);

        // 检查玩家是否在移动
        if (movementInput != Vector2.zero)
        {
            // 更新计时器
            timeSinceLastSound += Time.deltaTime;

            // 如果计时器超过音效间隔时间，播放音效
            if (timeSinceLastSound >= soundInterval)
            {
                if (footStep != SoundName.none)
                {
                    var soundDetails = AudioManager.Instance.soundDetailsData.GetSoundDetails(footStep);
                    EventHandler.CallInitSoundEffect(soundDetails);
                }
                timeSinceLastSound = 0f; // 重置计时器
            }
        }
        else
        {
            // 玩家停止移动，重置计时器
            timeSinceLastSound = 0f;
        }
    }

    private void Movement()
    {
        rb.MovePosition(rb.position + movementInput * speed * Time.deltaTime);
    }

    // 处理物品选中事件
    private void OnItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    {
        // 检查是否在允许移动的场景中
        if (!allowedScenes.Contains(SceneManager.GetActiveScene().name))
        {
            return;
        }

        // 检查物品是否可以被携带
        if (itemDetails != null && itemDetails.canCarried && isSelected)
        {
            // 如果物品可以被携带并且被选中，显示在头顶
            if (holdItemRenderer != null)
            {
                holdItemRenderer.sprite = itemDetails.itemOnWorldSprite;
                holdItemRenderer.enabled = true;
            }

            // 触发工具动画
            PlayToolAnimation();
        }
        else
        {
            // 如果物品不可被携带或者未被选中，隐藏头顶物品
            if (holdItemRenderer != null)
            {
                holdItemRenderer.enabled = false;
            }
        }
    }

    private void OnMouseClickedEvent(Vector3 pos, ItemDetails itemDetails)
    {
        // 执行动画
        EventHandler.CallExecuteActionAfterAnimation(pos, itemDetails);
    }

    // 处理视频播放结束事件
    private void OnVideoPlaybackFinished()
    {
        gameStarted = true;
        playerSprite.enabled = true;
        playerCollider.enabled = true; // 启用碰撞器
        // 启用玩家移动
        AllowPlayerMovement(true);
    }

    // 允许玩家移动的方法
    public void AllowPlayerMovement(bool allow)
    {
        gameStarted = allow;
        inputDisable = !allow;

        if (allow)
        {
            Debug.Log("玩家移动已启用");
            playerSprite.enabled = true;
            playerCollider.enabled = true; // 启用碰撞器
        }
        else
        {
            Debug.Log("玩家移动已禁用");
            playerSprite.enabled = false;
            playerCollider.enabled = false; // 禁用碰撞器
        }
    }

    // 播放工具动画
    private void PlayToolAnimation()
    {
        if (toolAnimationController != null)
        {
            // 假设玩家在左侧
            bool isPlayerOnLeft = true;
            toolAnimationController.StartCoroutine(toolAnimationController.SwingTool(isPlayerOnLeft));
        }
    }
}