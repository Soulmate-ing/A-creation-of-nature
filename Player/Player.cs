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
    private bool gameStarted = false; // ���һ����־����ʾ��Ϸ�Ƿ��Ѿ���ʼ

    // ������ʾͷ����Ʒ�� SpriteRenderer
    public SpriteRenderer holdItemRenderer;

    // ��������ƶ��ĳ��������б�
    public List<string> allowedScenes = new List<string> { "01.Field" }; // Ĭ�������� "MainScene" �������ƶ�

    public SoundName footStep;
    private float timeSinceLastSound = 0f;
    public float soundInterval = 0.5f; // ��Ч���ż��ʱ�䣨�룩

    private SpriteRenderer playerSprite;
    private BoxCollider2D playerCollider; // ��� BoxCollider2D ����

    public Dictionary<string, bool> npcStates = new Dictionary<string, bool>();

    // ���������� ToolAnimationController
    private ToolAnimationController toolAnimationController;

    private void Awake()
    {
        playerSprite = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<BoxCollider2D>(); // ��ȡ BoxCollider2D ����
        inputDisable = true; // Ĭ�Ͻ�������
        gameStarted = false; // ��Ϸδ��ʼ

        playerSprite.enabled = false;
        playerCollider.enabled = false; // Ĭ�Ͻ�����ײ��
        toolAnimationController = GetComponent<ToolAnimationController>(); // ��ȡ ToolAnimationController ����
    }

    private void OnEnable()
    {
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadEvent;
        EventHandler.MoveToPosition += OnMoveToPosition;
        EventHandler.ItemSelectedEvent += OnItemSelectedEvent; // �����Ʒѡ���¼�
        EventHandler.MouseClickedEvent += OnMouseClickedEvent;
        // ������Ƶ���Ž����¼�
        EventHandler.VideoPlaybackFinishedEvent += OnVideoPlaybackFinished;
    }

    private void OnDisable()
    {
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadEvent;
        EventHandler.MoveToPosition -= OnMoveToPosition;
        EventHandler.ItemSelectedEvent -= OnItemSelectedEvent; // �����Ʒѡ���¼�
        EventHandler.MouseClickedEvent -= OnMouseClickedEvent;
        // ȡ��������Ƶ���Ž����¼�
        EventHandler.VideoPlaybackFinishedEvent -= OnVideoPlaybackFinished;
    }

    private void OnMoveToPosition(Vector3 targetPosition)
    {
        transform.position = targetPosition;
    }

    private void OnAfterSceneLoadEvent()
    {
        // ��鵱ǰ�����Ƿ��������б���
        if (allowedScenes.Contains(SceneManager.GetActiveScene().name))
        {
            inputDisable = !gameStarted;
            // ������Ϸ�Ƿ�ʼ���ƽ�ɫ��ʾ
            playerSprite.enabled = gameStarted;
            playerCollider.enabled = gameStarted; // ͬ����ײ��״̬
        }
        else
        {
            inputDisable = true;
            playerSprite.enabled = false; // ��������ǿ������
            playerCollider.enabled = false; // ������ײ��
            if (holdItemRenderer != null)
            {
                holdItemRenderer.enabled = false;
            }
        }
    }

    private void OnBeforeSceneUnloadEvent()
    {
        inputDisable = true;
        // ����ͷ����Ʒ
        if (holdItemRenderer != null)
        {
            holdItemRenderer.enabled = false;
        }
        // ������Һͽ�����ײ��
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

        // �������Ƿ����ƶ�
        if (movementInput != Vector2.zero)
        {
            // ���¼�ʱ��
            timeSinceLastSound += Time.deltaTime;

            // �����ʱ��������Ч���ʱ�䣬������Ч
            if (timeSinceLastSound >= soundInterval)
            {
                if (footStep != SoundName.none)
                {
                    var soundDetails = AudioManager.Instance.soundDetailsData.GetSoundDetails(footStep);
                    EventHandler.CallInitSoundEffect(soundDetails);
                }
                timeSinceLastSound = 0f; // ���ü�ʱ��
            }
        }
        else
        {
            // ���ֹͣ�ƶ������ü�ʱ��
            timeSinceLastSound = 0f;
        }
    }

    private void Movement()
    {
        rb.MovePosition(rb.position + movementInput * speed * Time.deltaTime);
    }

    // ������Ʒѡ���¼�
    private void OnItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    {
        // ����Ƿ��������ƶ��ĳ�����
        if (!allowedScenes.Contains(SceneManager.GetActiveScene().name))
        {
            return;
        }

        // �����Ʒ�Ƿ���Ա�Я��
        if (itemDetails != null && itemDetails.canCarried && isSelected)
        {
            // �����Ʒ���Ա�Я�����ұ�ѡ�У���ʾ��ͷ��
            if (holdItemRenderer != null)
            {
                holdItemRenderer.sprite = itemDetails.itemOnWorldSprite;
                holdItemRenderer.enabled = true;
            }

            // �������߶���
            PlayToolAnimation();
        }
        else
        {
            // �����Ʒ���ɱ�Я������δ��ѡ�У�����ͷ����Ʒ
            if (holdItemRenderer != null)
            {
                holdItemRenderer.enabled = false;
            }
        }
    }

    private void OnMouseClickedEvent(Vector3 pos, ItemDetails itemDetails)
    {
        // ִ�ж���
        EventHandler.CallExecuteActionAfterAnimation(pos, itemDetails);
    }

    // ������Ƶ���Ž����¼�
    private void OnVideoPlaybackFinished()
    {
        gameStarted = true;
        playerSprite.enabled = true;
        playerCollider.enabled = true; // ������ײ��
        // ��������ƶ�
        AllowPlayerMovement(true);
    }

    // ��������ƶ��ķ���
    public void AllowPlayerMovement(bool allow)
    {
        gameStarted = allow;
        inputDisable = !allow;

        if (allow)
        {
            Debug.Log("����ƶ�������");
            playerSprite.enabled = true;
            playerCollider.enabled = true; // ������ײ��
        }
        else
        {
            Debug.Log("����ƶ��ѽ���");
            playerSprite.enabled = false;
            playerCollider.enabled = false; // ������ײ��
        }
    }

    // ���Ź��߶���
    private void PlayToolAnimation()
    {
        if (toolAnimationController != null)
        {
            // ������������
            bool isPlayerOnLeft = true;
            toolAnimationController.StartCoroutine(toolAnimationController.SwingTool(isPlayerOnLeft));
        }
    }
}