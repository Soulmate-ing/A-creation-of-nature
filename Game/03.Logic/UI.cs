using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public static UI instance;
    int playCount;
    public bool isGameStarted = false; // 游戏是否开始的标志
    public static bool ChuangameComplete;
    public Image bar;
    public GameObject BAR;
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public float shipweight = 45;
    public float fuli = 50;
    public Text zaizhong;
    public GameObject win;
    public GameObject False;
    public int time = 40;
    public Text timetext;
    public GameObject rule;
    public GameObject speedupword;
    public bool isFlashing;
    float flashSpeed=2f;//闪烁速度
    public float maxalpha=0.55f;//最大透明度
    public Image flashImage;
    public float panelDelay1=1.3f;//沉船动画的时间
    public float panelDelay2=1.5f;//船胜利动画的时间
    public float panelDelay3=0.5f;//进度条延时隐藏的时间
    private Coroutine stopCor1;
    public boatcontrol boatcontrol;
   

    void Start()
    {
        isGameStarted = false; // 初始化游戏状态为未开始
        updateText();
        timetext.text = $"倒计时{time}";
        bar =GameObject.Find("bar (1)").GetComponent<Image>();
        bar.fillAmount = 0.14f;//初始化血条
        boatcontrol=GameObject.Find("ship").GetComponent<boatcontrol>();
    }
    void Update()
    {
        // 游戏逻辑
        if (isGameStarted)
        {
            if (time < 0.1 && shipweight <= fuli)
            {
                boatcontrol.OnGameSuccess();
                StartCoroutine(gamewin());
                BAR.SetActive(false);

            }
            if (shipweight > fuli)
            {
                StartCoroutine(BARfalse());
                StartCoroutine(gameover());//播完动画再弹出面板
                StopCoroutine(stopCor1);
                

            }
            
        }

        //屏幕闪烁条件
        if (shipweight == fuli)
        {   flashImage.gameObject.SetActive(true);
            StartCoroutine(FlashEffect());
        }
        if(time<0.1f||shipweight>fuli)
        {
            isFlashing = false;
            StopCoroutine(FlashEffect());
            flashImage.gameObject.SetActive(false);
        }
    }

    public void updateText()
    {
        zaizhong.text = $"总重量/浮力:{shipweight}/{fuli}";
    }

    IEnumerator countdown()
    {
        while (time > 0)
        {
            yield return new WaitForSeconds(1);
            time--;
            timetext.text = $"倒计时{time}";
          
        }
    }

    public void getwin()
    {
        isGameStarted = false;
        win.SetActive(true);
        UI.ChuangameComplete = true;
        Talkable.TotalcompletedNPCs++;
        TaskManager.Instance.UpdateTaskState(3);
    }

    public void getfalse()
    {
        isGameStarted = false;
        False.SetActive(true);
    }

    public void replay()
    {
        playCount = 1;
        PlayerPrefs.SetInt("Count", playCount);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void playgame()
    {
        rule.SetActive(false);
        isGameStarted = true; // 设置游戏状态为已开始
        stopCor1 = StartCoroutine("countdown");
        updateText();
        timetext.text = $"倒计时{time}";
    }

    public void Initialization()
    {
        isGameStarted = true;
        shipweight = 45;
        fuli = 50;
        time = 40;
        StartCoroutine("countdown");
        updateText();
        timetext.text = $"倒计时{time}";
        win.SetActive(false);
        False.SetActive(false);
    }
    //屏幕平滑闪烁
    IEnumerator FlashEffect()
    {
        isFlashing = true;
        while (true)
        {
            float alpha = Mathf.PingPong(Time.time * flashSpeed, maxalpha);
            flashImage.color=new Color(1,0, 0, alpha);
            yield return null;
        }
    }
    //让船下沉动画播放完后
    IEnumerator gameover()
    {
        yield return new WaitForSeconds(panelDelay1);
        getfalse();
        
    }
    IEnumerator gamewin()
    {
        yield return new WaitForSeconds(panelDelay1);
       getwin();

    }
    IEnumerator BARfalse()
    {
        yield return new WaitForSeconds(panelDelay3);
        BAR.SetActive(false);
    }

}