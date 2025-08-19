using UnityEngine;
using UnityEngine.UI;

public class MineralGameManager : MonoBehaviour
{
    public RectTransform targetrange; // Ŀ�귶Χ
    public RectTransform target; // Ŀ���
    public RectTransform zhizhen; // ָ��
    public float speed = 50; // ָ���ƶ��ٶ�
    public int maxFailures = 5; // ���ʧ�ܴ���

    private float boundaryMax; // �߽����ֵ
    private float boundaryMin; // �߽���Сֵ
    private float zhizhenspeed; // ָ��ʵ���ٶ�
    private bool ismoveingRight = true; // ָ���ƶ�����
    private bool isActive = true; // ��Ϸ�Ƿ������
    private int failureCount = 0; // ��ǰʧ�ܴ���
    private MineralUIManager uiManager; // ����UI����ű�

    public SoundName success;
    public SoundName failure;

    public GameObject Fire;
    private Vector3 fireInitialScale;  // �����ʼ��С
    [SerializeField] private float maxFireScaleMultiplier; // ������ű���
    [SerializeField] private float scaleSmoothness = 5f;         // ����ƽ��ϵ��

    private void Start()
    {
        uiManager = FindObjectOfType<MineralUIManager>();
        zhizhenspeed = speed * Time.fixedDeltaTime * 10f;
        // ȷ���߽�ֵ�� targetrange �Ŀ����ȫƥ��
        boundaryMax = targetrange.sizeDelta.x / 2;
        boundaryMin = -targetrange.sizeDelta.x / 2;
        TargetMoveRange();

        // ��ʼ��ʧ�ܴ�����ʾ
        uiManager.UpdateFailureCount(0, maxFailures);

        if (Fire != null)
            fireInitialScale = Fire.transform.localScale;
        else
            Debug.LogError("Fire δ��ֵ��");
    }

    private void Update()
    {
        // ����û���κ�����ʱ�������
        if (!uiManager.IsAnyPanelActive() && Input.GetKeyDown(KeyCode.Space))
        {
            CheckAlignment();
        }
    }

    private void FixedUpdate()
    {
        // ����û���κ�����ʱ�ƶ�ָ��
        if (isActive && !uiManager.IsAnyPanelActive())
        {
            Zhizhenmove();
        }
    }

    private void TargetMoveRange()
    {
        // ȷ���÷������� targetrange �ķ�Χ��
        float moverange = Random.Range(boundaryMin, boundaryMax);
        target.anchoredPosition = new Vector2(moverange, target.anchoredPosition.y);
    }

    private void Zhizhenmove()
    {
        float move = zhizhenspeed * (ismoveingRight ? 1 : -1);
        zhizhen.anchoredPosition += new Vector2(move, 0);

        UpdateFireScale();

        // ȷ��ָ�벻�ᳬ�� targetrange �ķ�Χ
        if (zhizhen.anchoredPosition.x >= boundaryMax)
            ismoveingRight = false;
        if (zhizhen.anchoredPosition.x <= boundaryMin)
            ismoveingRight = true;

    }
    private void UpdateFireScale()
    {
        if (Fire == null) return;
        float positionRatio = Mathf.InverseLerp(
            boundaryMax,  // �ұ߽�
            boundaryMin,  // ��߽�
            zhizhen.anchoredPosition.x
        );

        // ����λ�ñ�������Ŀ�����ű���
        float targetMultiplier = Mathf.Lerp(
            1f,
            maxFireScaleMultiplier,
            positionRatio
        );

        // ����Ŀ��ߴ粢ƽ������
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
        Debug.Log("У���ɹ�");
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
        Debug.Log("У��ʧ��");
        uiManager.falsegame.SetActive(true);
        CancelInvoke("CloseFalseTip");
        Invoke("CloseFalseTip", 0.5f);

        failureCount++;
        // ����ʧ�ܴ�����ʾ
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
        uiManager.ResetUI(maxFailures); // �޸ĵ��÷�ʽ
        zhizhen.anchoredPosition = Vector2.zero;
        TargetMoveRange();
    }
    public void Replay() => ResetGame();
}