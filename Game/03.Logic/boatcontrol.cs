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

    public float boundaryOffset = 0.5f; // �߽�ƫ����

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
        // ȷ�����ĳ�ʼ�ٶ�Ϊ0
        boat.velocity = Vector2.zero;
        // ȷ������������Ӱ��
        boat.gravityScale = 0;
        boom.gameObject.SetActive(false);
        myrender = GetComponent<Renderer>();
        animator = GetComponent<Animator>();
        col = GetComponent<PolygonCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // ֻ������Ϸ��ʼʱ�Ŵ������ƶ�
        if (UI.instance.isGameStarted)
        {
            Y = Input.GetAxis("Vertical");
            boat.velocity = new Vector2(boat.velocity.x, Y * speed);
            bround();
        }
    
        //��Ϸ�����������ƶ�
         if (UI.instance.time<0.1f||UI.instance.shipweight>UI.instance.fuli)
        {
            boat.velocity = Vector3.zero;
            col.enabled = false;
            //UI.instance.BAR.SetActive(false);
        }

        //��Ϸʧ�ܣ��л���������
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
            //��ը����
            transform.GetChild(0).gameObject.SetActive(true);
           

            //�������仯
            UI.instance.bar.fillAmount +=0.125f;
            Debug.Log(UI.instance.shipweight);
            //��˸
            shipblink(numBlink, second);
           
        }
    }
    public void falseboom()
    {
        boom.gameObject.SetActive(false);
    }
    //��˸Ч��
    public void shipblink(int numBlink, float second)
    {
        StartCoroutine(DoBlink(numBlink, second));
    }
    IEnumerator DoBlink(int numBlink, float second)//numBlink ��˸������second��˸��ʱ��
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
        // ���ô��ı߽磨����ƫ������
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
    //��Ϸ�ɹ��󣬴����������ƶ�
    IEnumerator MoveRight()
    {
        // ����Ŀ��λ��Ϊ��Ļ�Ҳ��⣨�򵥰棺ֱ�������ƶ�10����λ��
        Vector3 targetPos = transform.position + Vector3.right * 10f;

        // �����ƶ�ֱ������Ŀ��
        while (transform.position.x < targetPos.x)
        {
            transform.Translate(Vector3.right * 0.5f * Time.deltaTime);
            yield return null; // ÿִ֡��һ��
        }
    }
    //��Ϸ�ɹ��󣬴����������ƶ��ĺ���
    public void OnGameSuccess()
    {
        StartCoroutine(MoveRight());
    }

}