using UnityEngine;
using UnityEngine.UI;

public class MineralGameManager : MonoBehaviour
{
    public RectTransform targetrange; // 目标范围
    public RectTransform target; // 目标点
    public RectTransform zhizhen; // 指针
    public float speed = 50; // 指针移动速度
    public int maxFailures = 5; // 最大失败次数

    private float boundaryMax; // 边界最大值
    private float boundaryMin; // 边界最小值
    private float zhizhenspeed; // 指针实际速度
    private bool ismoveingRight = true; // 指针移动方向
    private bool isActive = true; // 游戏是否进行中
    private int failureCount = 0; // 当前失败次数
    private MineralUIManager uiManager; // 引用UI管理脚本

    public SoundName success;
    public SoundName failure;

    public GameObject Fire;
    private Vector3 fireInitialScale;  // 火焰初始大小
    [SerializeField] private float maxFireScaleMultiplier; // 最大缩放倍数
    [SerializeField] private float scaleSmoothness = 5f;         // 缩放平滑系数

    private void Start()
    {
        uiManager = FindObjectOfType<MineralUIManager>();
        zhizhenspeed = speed * Time.fixedDeltaTime * 10f;
        // 确保边界值与 targetrange 的宽度完全匹配
        boundaryMax = targetrange.sizeDelta.x / 2;
        boundaryMin = -targetrange.sizeDelta.x / 2;
        TargetMoveRange();

        // 初始化失败次数显示
        uiManager.UpdateFailureCount(0, maxFailures);

        if (Fire != null)
            fireInitialScale = Fire.transform.localScale;
        else
            Debug.LogError("Fire 未赋值！");
    }

    private void Update()
    {
        // 仅在没有任何面板打开时检测输入
        if (!uiManager.IsAnyPanelActive() && Input.GetKeyDown(KeyCode.Space))
        {
            CheckAlignment();
        }
    }

    private void FixedUpdate()
    {
        // 仅在没有任何面板打开时移动指针
        if (isActive && !uiManager.IsAnyPanelActive())
        {
            Zhizhenmove();
        }
    }

    private void TargetMoveRange()
    {
        // 确保得分区域在 targetrange 的范围内
        float moverange = Random.Range(boundaryMin, boundaryMax);
        target.anchoredPosition = new Vector2(moverange, target.anchoredPosition.y);
    }

    private void Zhizhenmove()
    {
        float move = zhizhenspeed * (ismoveingRight ? 1 : -1);
        zhizhen.anchoredPosition += new Vector2(move, 0);

        UpdateFireScale();

        // 确保指针不会超出 targetrange 的范围
        if (zhizhen.anchoredPosition.x >= boundaryMax)
            ismoveingRight = false;
        if (zhizhen.anchoredPosition.x <= boundaryMin)
            ismoveingRight = true;

    }
    private void UpdateFireScale()
    {
        if (Fire == null) return;
        float positionRatio = Mathf.InverseLerp(
            boundaryMax,  // 右边界
            boundaryMin,  // 左边界
            zhizhen.anchoredPosition.x
        );

        // 根据位置比例计算目标缩放倍数
        float targetMultiplier = Mathf.Lerp(
            1f,
            maxFireScaleMultiplier,
            positionRatio
        );

        // 计算目标尺寸并平滑过渡
        Vector3 targetScale = fireInitialScale * targetMultiplier;
        Fire.transform.localScale = Vector3.Lerp(
            Fire.transform.localScale,
            targetScale,
            Time.deltaTime * scaleSmoothness
        );
    }

    private void CheckAlignment()
    {
        isActive = false;
        float zhizhenPos = zhizhen.anchoredPosition.x;
        float zoneCenter = target.anchoredPosition.x;
        float zoneStart = zoneCenter - target.sizeDelta.x / 2;
        float zoneEnd = zoneCenter + target.sizeDelta.x / 2;

        if (zhizhenPos >= zoneStart && zhizhenPos <= zoneEnd)
        {
            if (success != SoundName.none)
            {
                var soundDetails = AudioManager.Instance.soundDetailsData.GetSoundDetails(success);
                EventHandler.CallInitSoundEffect(soundDetails);
            }
            HandleSuccess();
        }
        else
        {
            if (failure != SoundName.none)
            {
                var soundDetails = AudioManager.Instance.soundDetailsData.GetSoundDetails(failure);
                EventHandler.CallInitSoundEffect(soundDetails);
            }
            HandleFailure();
        }
    }

    private void HandleSuccess()
    {
        Debug.Log("校正成功");
        uiManager.success.SetActive(true);
        CancelInvoke("CloseSuccessTip");
        Invoke("CloseSuccessTip", 0.5f);

        float newProgress = Mathf.Min(uiManager.jindutiao.value + 0.25f, 1f);
        uiManager.UpdateProgress(newProgress);

        if (newProgress >= 1f)
        {
            uiManager.ShowWinBoard();
        }
        else
        {
            TargetMoveRange();
            isActive = true;
        }
    }

    private void HandleFailure()
    {
        Debug.Log("校正失败");
        uiManager.falsegame.SetActive(true);
        CancelInvoke("CloseFalseTip");
        Invoke("CloseFalseTip", 0.5f);

        failureCount++;
        // 更新失败次数显示
        uiManager.UpdateFailureCount(failureCount, maxFailures);

        if (failureCount >= maxFailures)
        {
            uiManager.ShowFalseBoard();
        }
        else
        {
            isActive = true;
        }
    }

    public void CloseSuccessTip() => uiManager.success.SetActive(false);
    public void CloseFalseTip() => uiManager.falsegame.SetActive(false);

    public void ResetGame()
    {
        isActive = true;
        failureCount = 0;
        uiManager.ResetUI(maxFailures); // 修改调用方式
        zhizhen.anchoredPosition = Vector2.zero;
        TargetMoveRange();
    }
    public void Replay() => ResetGame();
}