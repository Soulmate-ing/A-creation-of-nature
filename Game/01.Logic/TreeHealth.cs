using MyGame.Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFarm.Inventory
{
    public class TreeHealth : MonoBehaviour
    {
        public List<Sprite> healthSprites; // 存储树木不同HP状态的图片
        public int currentHealth = 5; // 当前HP
        private SpriteRenderer spriteRenderer;
        private ItemManager itemManager;
        private CursorManager cursorManager;
        public ItemDetails currentTool;

        public int instantiateItemID = 1003;

        public bool playerInRange = false;
        private float displayTime = 2.0f; // HP 显示的时间（秒）
        private float timer;
        private bool isDisplayingHP = false;

        public SoundName soundEffect1;
        public SoundName soundEffect2;

        private PoolManager poolManager;
        public static event System.Action<bool> OnTreeChop;
        private void Awake()
        {
            //StartCoroutine("UpdateCollision");
            BoxCollider2D collider = gameObject.AddComponent<BoxCollider2D>();
            collider.isTrigger = false;
            collider.offset = new Vector2(-0.17f, 0.4f);
            collider.size = new Vector2(4f, 5.8f);
            collider.isTrigger = true;
            Debug.Log("已添加");
        }
        private void Start()
        {
            poolManager = FindObjectOfType<PoolManager>();
            // 确保访问的是 HP 子对象的 SpriteRenderer
            spriteRenderer = transform.Find("HP").GetComponent<SpriteRenderer>();
            spriteRenderer.enabled = false; // 默认不显示 HP

            itemManager = FindObjectOfType<ItemManager>();
            cursorManager = FindObjectOfType<CursorManager>();
            UpdateSprite(); // 初始化显示
        }

        private void Update()
        {
            if (isDisplayingHP)
            {
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    HideHP();
                }
            }
        }

        // 使用触发器检测玩家进入范围
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
                playerInRange = true;
            transform.position = new Vector3(transform.position.x + 0.01f, transform.position.y, transform.position.z);
        }

        // 使用触发器检测玩家离开范围
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
                playerInRange = false;
        }

        private void OnMouseDown()
        {
            // 获取当前选择的工具
            currentTool = cursorManager.currentItem;

            // 检查是否选择了砍伐工具、鼠标状态为可用且玩家在碰撞范围内
            if (currentTool != null && currentTool.itemType == ItemType.ChopTool && playerInRange)
            {
                // 获取玩家位置检测器
                PlayerPositionDetector detector = GetComponentInChildren<PlayerPositionDetector>();
                if (detector != null)
                {
                    // 触发事件
                    OnTreeChop?.Invoke(detector.IsPlayerOnLeft);
                }
                ShowHP(); // 显示 HP

                if (currentHealth > 0)
                {
                    currentHealth--;
                    //播放声音
                    if (soundEffect1 != SoundName.none)
                    {
                        var soundDetails = AudioManager.Instance.soundDetailsData.GetSoundDetails(soundEffect1);
                        EventHandler.CallInitSoundEffect(soundDetails);
                    }
                    UpdateSprite();

                    // 生成落叶粒子特效
                    if (poolManager != null)
                    {
                        poolManager.OnParticleEffectEvent(ParticleEffectType.LeavesFalling, transform.position);
                    }

                    if (currentHealth <= 0)
                    {
                        if (soundEffect2 != SoundName.none)
                        {
                            var soundDetails = AudioManager.Instance.soundDetailsData.GetSoundDetails(soundEffect2);
                            EventHandler.CallInitSoundEffect(soundDetails);
                        }
                        DestroyTree();
                    }
                }
            }
        }

        private void UpdateSprite()
        {
            //Debug.Log("Updating sprite for health: " + currentHealth);
            if (healthSprites.Count > currentHealth && healthSprites.Count > 0)
            {
                spriteRenderer.sprite = healthSprites[currentHealth];
            }
            else
            {
                Debug.LogError("Not enough sprites in healthSprites list for health: " + currentHealth);
            }
        }

        private void DestroyTree()
        {
            // 生成随机数量的物品
            int itemCount = Random.Range(4, 11); // 生成2-5个物品
            for (int i = 0; i < itemCount; i++)
            {
                Vector3 spawnPos = transform.position + new Vector3(
                    Random.Range(-0.5f, 0.5f),
                    Random.Range(-0.5f, 0.5f),
                    0
                );
                itemManager.OnInstantiateItemScene(instantiateItemID, spawnPos);
            }

            Destroy(gameObject); // 销毁树木
        }

        private void ShowHP()
        {
            spriteRenderer.enabled = true;
            isDisplayingHP = true;
            timer = displayTime;
        }

        private void HideHP()
        {
            spriteRenderer.enabled = false;
            isDisplayingHP = false;
        }
    }
}