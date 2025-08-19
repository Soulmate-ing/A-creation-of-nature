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
    [Header("背包数据")]
    public InventoryBag_SO playerBag;

    [Header("SettingUI")]
    public GameObject SettingCanvas;
    public GameObject SettingUI;
    public bool isVisable = true;
    public List<string> allowedScenes = new List<string> { "01.Field" };
    public Slider volumeSlider;

    private void Start()
    {
        // 确保Slider组件正确挂载
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.AddListener(AudioManager.Instance.SetMasterVolume);
        }
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        MainCanvas.SetActive(false); // 初始隐藏主界面
        StartMenu.SetActive(true);   // 显示开始菜单
        videoPlayer = VideoMenu.GetComponent<VideoPlayer>();

        SettingUI.SetActive(false);
        SettingCanvas.SetActive(false); // 初始隐藏SettingCanvas
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
        // 检查当前场景是否在允许列表中
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
            print("视频播放完毕");
            // 触发视频播放结束事件
            VideoPlaybackFinished();
            canPlayVideo = false;
        }

        // 当VideoMenu激活时，强制隐藏SettingCanvas
        if (VideoMenu.activeSelf)
        {
            SettingCanvas.SetActive(false);
        }
        else
        {
            // 否则根据isVisable决定显示状态
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
    // 按钮点击事件
    public void OnStartButtonClick()
    {
        canPlayVideo = true; // 触发视频播放
        StartMenu.SetActive(false);
        VideoMenu.SetActive(true);
        // 禁用玩家移动
        SetPlayerMovement(false);
        // 检查其他UI状态
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

    // 视频结束后激活主界面
    public void EnableMainCanvas()
    {
        MainCanvas.SetActive(true);
        EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        // 检查其他UI状态
        CheckOtherUIStatus();
        SettingCanvas.SetActive(true); // 视频播放完后显示SettingCanvas
    }

    // 视频播放结束事件
    public void VideoPlaybackFinished()
    {
        //StartCoroutine(TransitionManager.Instance.LoadSceneSetActice("01.Field"));
        // 启用玩家移动
        SetPlayerMovement(true);
        // 激活主界面
        EnableMainCanvas();
        // 触发事件，通知其他组件视频播放结束
        EventHandler.CallVideoPlaybackFinishedEvent();


    }

    public void OpenSetting()
    {
        SettingUI.SetActive(true);
        Time.timeScale = 0;
        SettingCanvas.SetActive(true); // 打开设置时显示SettingCanvas
    }

    public void CloseSetting()
    {
        SettingUI.SetActive(false);
        Time.timeScale = 1;
        SettingCanvas.SetActive(false); // 关闭设置时隐藏SettingCanvas
    }

    // 检查其他UI状态，确保SettingUI不会与其他UI同时显示
    private void CheckOtherUIStatus()
    {
        // 如果其他UI激活，则关闭SettingUI
        if (VideoMenu.activeSelf)
        {
            CloseSetting();
            SettingCanvas.SetActive(false);
        }
    }
}