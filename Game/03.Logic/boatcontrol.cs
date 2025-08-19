using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boatcontrol : MonoBehaviour
{
    Rigidbody2D boat;
    float Y;
    public float speed;
    Transform boatpos;
    public float topboundary;
    public float buttomboundary;
    public float leftboundary;
    public float rightboundary;

    public float boundaryOffset = 0.5f; // 边界偏移量

    public SoundName strike;
    public GameObject boom;
    private Renderer myrender;
    public int numBlink;
    public float second;
    public Animator animator;
    public PolygonCollider2D col;
    
    // Start is called before the first frame update
    void Start()
    {
        boat = GetComponent<Rigidbody2D>();
        boatpos = GetComponent<Transform>();
        // 确保船的初始速度为0
        boat.velocity = Vector2.zero;
        // 确保船不受重力影响
        boat.gravityScale = 0;
        boom.gameObject.SetActive(false);
        myrender = GetComponent<Renderer>();
        animator = GetComponent<Animator>();
        col = GetComponent<PolygonCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // 只有在游戏开始时才处理船的移动
        if (UI.instance.isGameStarted)
        {
            Y = Input.GetAxis("Vertical");
            boat.velocity = new Vector2(boat.velocity.x, Y * speed);
            bround();
        }
    
        //游戏结束船不可移动
         if (UI.instance.time<0.1f||UI.instance.shipweight>UI.instance.fuli)
        {
            boat.velocity = Vector3.zero;
            col.enabled = false;
            //UI.instance.BAR.SetActive(false);
        }

        //游戏失败，切换沉船动画
        if(UI.instance.shipweight > UI.instance.fuli)
        {
            shipdown();
        }
        
       


    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "stone")
        {
            if (strike != SoundName.none)
            {
                var soundDetails = AudioManager.Instance.soundDetailsData.GetSoundDetails(strike);
                EventHandler.CallInitSoundEffect(soundDetails);
            }
            UI.instance.shipweight += 1f;
            UI.instance.updateText();
            Destroy(collision.gameObject);
            //爆炸动画
            transform.GetChild(0).gameObject.SetActive(true);
           

            //进度条变化
            UI.instance.bar.fillAmount +=0.125f;
            Debug.Log(UI.instance.shipweight);
            //闪烁
            shipblink(numBlink, second);
           
        }
    }
    public void falseboom()
    {
        boom.gameObject.SetActive(false);
    }
    //闪烁效果
    public void shipblink(int numBlink, float second)
    {
        StartCoroutine(DoBlink(numBlink, second));
    }
    IEnumerator DoBlink(int numBlink, float second)//numBlink 闪烁次数，second闪烁的时间
    {
        for (int i = 0; i < numBlink*2; i++)
        {   myrender.enabled=!myrender.enabled;
            yield return new WaitForSeconds(second);
        }
        myrender.enabled = true;
    }

    void shipdown()
    {
        animator.SetBool("falsegame", true);
    }
    void shipwin()
    {
        animator.SetBool("shipWin", true);
    }
    void bround()
    {
        // 设置船的边界（加上偏移量）
            if (boat.position.y < buttomboundary + boundaryOffset)
            {
                boatpos.position = new Vector3(boatpos.position.x, buttomboundary + boundaryOffset, boatpos.position.z);
            }
            if (boat.position.y > topboundary - boundaryOffset)
            {
                boatpos.position = new Vector3(boatpos.position.x, topboundary - boundaryOffset, boatpos.position.z);
            }
            if (boat.position.x < leftboundary + boundaryOffset)
            {
                boatpos.position = new Vector3(leftboundary + boundaryOffset, boatpos.position.y, boatpos.position.z);
            }
            /*if (boat.position.x > rightboundary - boundaryOffset)
            {
                boatpos.position = new Vector3(rightboundary - boundaryOffset, boatpos.position.y, boatpos.position.z);
            }*/
    }
    //游戏成功后，船从左向右移动
    IEnumerator MoveRight()
    {
        // 设置目标位置为屏幕右侧外（简单版：直接向右移动10个单位）
        Vector3 targetPos = transform.position + Vector3.right * 10f;

        // 持续移动直到到达目标
        while (transform.position.x < targetPos.x)
        {
            transform.Translate(Vector3.right * 0.5f * Time.deltaTime);
            yield return null; // 每帧执行一次
        }
    }
    //游戏成功后，船从左向右移动的函数
    public void OnGameSuccess()
    {
        StartCoroutine(MoveRight());
    }

}