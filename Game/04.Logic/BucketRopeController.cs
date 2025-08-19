using UnityEngine;
using UnityEngine.SceneManagement;

public class BucketRopeController : MonoBehaviour
{
    [Header("绳索设置")]
    public float ropeSpeed = 10f;
    public SpriteRenderer ropeSprite;
    public GameObject bucket;
    public float maxRopeLength = 12f;

    [Header("物品设置")]
    public Sprite emptyBucket;
    public Sprite watermelon1Bucket;
    public Sprite watermelon2Bucket;
    public Sprite goldBucket;
    public GameObject continuePanel1;
    public GameObject continuePanel2;
    public GameObject completePanel;
    public GameObject StartPanel;
    public static bool PulleygameComplete = false;

    private bool hasWatermelon1;
    private bool hasWatermelon2;
    private bool hasGold;
    private bool isGameActive = true;

    [Header("音效")]
    public SoundName refloatation; // 水

    void Update()
    {
        if (!isGameActive) return;

        HandleRopeMovement();
        CheckBucketState();
    }

    void HandleRopeMovement()
    {
        // 处理绳索伸缩
        if (Input.GetKey(KeyCode.E) && ropeSprite.size.x > 0)
        {
            bucket.transform.Translate(-Vector3.down * ropeSpeed * Time.deltaTime);
            ropeSprite.size -= Vector2.right * ropeSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.Q) && ropeSprite.size.x < maxRopeLength)
        {
            bucket.transform.Translate(Vector3.down * ropeSpeed * Time.deltaTime);
            ropeSprite.size += Vector2.right * ropeSpeed * Time.deltaTime;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Watermelon1") &&
            bucket.GetComponent<SpriteRenderer>().sprite != goldBucket &&
            bucket.GetComponent<SpriteRenderer>().sprite != watermelon2Bucket)
        {
            HandleCollectItem(other.gameObject, watermelon1Bucket, ref hasWatermelon1);
        }
        else if (other.CompareTag("Watermelon2") &&
                 bucket.GetComponent<SpriteRenderer>().sprite != goldBucket &&
                 bucket.GetComponent<SpriteRenderer>().sprite != watermelon1Bucket)
        {
            HandleCollectItem(other.gameObject, watermelon2Bucket, ref hasWatermelon2);
        }
        else if (other.CompareTag("Gold") &&
                 bucket.GetComponent<SpriteRenderer>().sprite != watermelon1Bucket &&
                 bucket.GetComponent<SpriteRenderer>().sprite != watermelon2Bucket)
        {
            HandleCollectItem(other.gameObject, goldBucket, ref hasGold);
        }
    }

    void HandleCollectItem(GameObject item, Sprite bucketSprite, ref bool flag)
    {
        bucket.GetComponent<SpriteRenderer>().sprite = bucketSprite;
        Destroy(item);
        flag = true;

        // 播放音效
        if (refloatation != SoundName.none)
        {
            var soundDetails = AudioManager.Instance.soundDetailsData.GetSoundDetails(refloatation);
            EventHandler.CallInitSoundEffect(soundDetails);
        }
    }

    void CheckBucketState()
    {
        if (hasWatermelon1 && ropeSprite.size.x < 0)
        {
            ShowContinuePanel1();
        }
        else if (hasWatermelon2 && ropeSprite.size.x < 0)
        {
            ShowContinuePanel2();
        }
        else if (hasGold && ropeSprite.size.x < 0)
        {
            ShowCompletePanel();
            BucketRopeController.PulleygameComplete = true;
            TaskManager.Instance.UpdateTaskState(5);
        }
    }

    void ShowContinuePanel1()
    {
        isGameActive = false;
        continuePanel1.SetActive(true);
    }

    void ShowContinuePanel2()
    {
        isGameActive = false;
        continuePanel2.SetActive(true);
    }

    void ShowCompletePanel()
    {
        isGameActive = false;
        completePanel.SetActive(true);
    }

    public void OnContinueClicked()
    {
        ResetBucket();
        continuePanel1.SetActive(false);
        isGameActive = true;
    }

    public void OnContinueClickedWatermelon2()
    {
        ResetBucket();
        continuePanel2.SetActive(false);
        isGameActive = true;
    }

    public void CloseStartPanel()
    {
        StartPanel.SetActive(false);
    }

    void ResetBucket()
    {
        bucket.GetComponent<SpriteRenderer>().sprite = emptyBucket;
        hasWatermelon1 = false;
        hasWatermelon2 = false;
        hasGold = false;
        ropeSprite.size = new Vector2(0, ropeSprite.size.y);
    }
}