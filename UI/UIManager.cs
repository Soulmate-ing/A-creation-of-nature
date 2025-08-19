using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.UI;
using MyGame.Transition;

public class UIManager : Singleton<UIManager>
{
    public GameObject MainCanvas;
    public GameObject StartMenu;
    public GameObject VideoMenu;

    public bool canPlayVideo;
    VideoPlayer videoPlayer;

    private Player player;
    private SpriteRenderer playerSprite;
    [Header("��������")]
    public InventoryBag_SO playerBag;

    [Header("SettingUI")]
    public GameObject SettingCanvas;
    public GameObject SettingUI;
    public bool isVisable = true;
    public List<string> allowedScenes = new List<string> { "01.Field" };
    public Slider volumeSlider;

    private void Start()
    {
        // ȷ��Slider�����ȷ����
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.AddListener(AudioManager.Instance.SetMasterVolume);
        }
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        MainCanvas.SetActive(false); // ��ʼ����������
        StartMenu.SetActive(true);   // ��ʾ��ʼ�˵�
        videoPlayer = VideoMenu.GetComponent<VideoPlayer>();

        SettingUI.SetActive(false);
        SettingCanvas.SetActive(false); // ��ʼ����SettingCanvas
    }
    private void OnEnable()
    {
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadEvent;
    }
    private void OnDisable()
    {
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadEvent;
    }

    private void OnAfterSceneLoadEvent()
    {
        // ��鵱ǰ�����Ƿ��������б���
        if (allowedScenes.Contains(SceneManager.GetActiveScene().name))
        {
            isVisable = true;
        }
        else
        {
            isVisable = false;
        }
    }
    private void Update()
    {
        if (videoPlayer.frame == (long)(videoPlayer.frameCount - 1))
        {
            VideoMenu.SetActive(false);
            print("��Ƶ�������");
            // ������Ƶ���Ž����¼�
            VideoPlaybackFinished();
            canPlayVideo = false;
        }

        // ��VideoMenu����ʱ��ǿ������SettingCanvas
        if (VideoMenu.activeSelf)
        {
            SettingCanvas.SetActive(false);
        }
        else
        {
            // �������isVisable������ʾ״̬
            if (!isVisable)
            {
                SettingCanvas.SetActive(false);
            }
            else
            {
                SettingCanvas.SetActive(true);
            }
        }
    }
    // ��ť����¼�
    public void OnStartButtonClick()
    {
        canPlayVideo = true; // ������Ƶ����
        StartMenu.SetActive(false);
        VideoMenu.SetActive(true);
        // ��������ƶ�
        SetPlayerMovement(false);
        // �������UI״̬
        CheckOtherUIStatus();
    }

    public void SetPlayerMovement(bool canMove)
    {
        if (player != null)
        {
            player.AllowPlayerMovement(canMove);
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    // ��Ƶ�����󼤻�������
    public void EnableMainCanvas()
    {
        MainCanvas.SetActive(true);
        EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        // �������UI״̬
        CheckOtherUIStatus();
        SettingCanvas.SetActive(true); // ��Ƶ���������ʾSettingCanvas
    }

    // ��Ƶ���Ž����¼�
    public void VideoPlaybackFinished()
    {
        //StartCoroutine(TransitionManager.Instance.LoadSceneSetActice("01.Field"));
        // ��������ƶ�
        SetPlayerMovement(true);
        // ����������
        EnableMainCanvas();
        // �����¼���֪ͨ���������Ƶ���Ž���
        EventHandler.CallVideoPlaybackFinishedEvent();


    }

    public void OpenSetting()
    {
        SettingUI.SetActive(true);
        Time.timeScale = 0;
        SettingCanvas.SetActive(true); // ������ʱ��ʾSettingCanvas
    }

    public void CloseSetting()
    {
        SettingUI.SetActive(false);
        Time.timeScale = 1;
        SettingCanvas.SetActive(false); // �ر�����ʱ����SettingCanvas
    }

    // �������UI״̬��ȷ��SettingUI����������UIͬʱ��ʾ
    private void CheckOtherUIStatus()
    {
        // �������UI�����ر�SettingUI
        if (VideoMenu.activeSelf)
        {
            CloseSetting();
            SettingCanvas.SetActive(false);
        }
    }
}