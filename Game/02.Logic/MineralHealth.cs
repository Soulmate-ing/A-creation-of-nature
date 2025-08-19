using MyGame.Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace MFarm.Inventory
{
    public class MineralHealth : MonoBehaviour
    {
        public List<Sprite> healthSprites; // 存储矿石不同HP状态的图片
        public int currentHealth = 3; // 当前HP
        private SpriteRenderer spriteRenderer;
        private ItemManager itemManager;
        private CursorManager cursorManager;
        private ItemDetails currentTool;

        public List<int> itemIDs = new List<int>(); // 存储可生成的物品ID列表

        private bool playerInRange = false;
        private float displayTime = 2.0f; // HP 显示的时间（秒）
        private float timer;
        private bool isDisplayingHP = false;

        public SoundName pickAxe;
        public SoundName stock;

        private PoolManager poolManager;
        [Header("特效位置偏移")]
        public Vector3 effectOffset = new Vector3(0f, 0.5f, 0f);
        public static event System.Action<bool> OnMineralMine;

        private void Awake()
        {
            BoxCollider2D collider = gameObject.AddComponent<BoxCollider2D>();
            collider.isTrigger = false;
            collider.offset = new Vector2(-0.1f, 0.11f);
            collider.size = new Vector2(3.1f, 1.4f);
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
            {
                playerInRange = true;
            }
            transform.position = new Vector3(transform.position.x + 0.01f, transform.position.y, transform.position.z);
        }

        // 使用触发器检测玩家离开范围
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                playerInRange = false;
            }
        }

        private void OnMouseDown()
        {
            // 获取当前选择的工具
            currentTool = cursorManager.currentItem;

            // 检查是否选择了挖掘工具、鼠标状态为可用且玩家在碰撞范围内
            if (currentTool != null && currentTool.itemType == ItemType.MiningTool && playerInRange)
            {
                PlayerPositionDetector detector = GetComponentInChildren<PlayerPositionDetector>();
                if (detector != null)
                {
                    OnMineralMine?.Invoke(detector.IsPlayerOnLeft);
                }
                ShowHP(); // 显示 HP

                if (currentHealth > 0)
                {
                    currentHealth--;
                    if (pickAxe != SoundName.none)
                    {
                        var soundDetails = AudioManager.Instance.soundDetailsData.GetSoundDetails(pickAxe);
                        EventHandler.CallInitSoundEffect(soundDetails);
                    }
                    UpdateSprite();
                    // 生成粒子特效
                    if (poolManager != null)
                    {
                        // 修改后的位置计算（原位置 + 偏移量）
                        Vector3 spawnPos = transform.position + effectOffset;
                        poolManager.OnParticleEffectEvent(ParticleEffectType.Rock, spawnPos);
                    }
                    if (currentHealth <= 0)
                    {
                        if (stock != SoundName.none)
                        {
                            var soundDetails = AudioManager.Instance.soundDetailsData.GetSoundDetails(stock);
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
            int itemCount = Random.Range(8, 15); // 生成8-14个物品
            for (int i = 0; i < itemCount; i++)
            {
                // 从列表中随机选择一个物品ID
                int randomItemID;
                if (itemIDs.Count > 0)
                {
                    randomItemID = itemIDs[Random.Range(0, itemIDs.Count)];
                }
                else
                {
                    randomItemID = 1005; // 默认物品ID
                }

                Vector3 spawnPos = transform.position + new Vector3(
                    Random.Range(-0.5f, 0.5f),
                    Random.Range(-0.5f, 0.5f),
                    0
                );
                itemManager.OnInstantiateItemScene(randomItemID, spawnPos);
            }

            Destroy(gameObject); // 销毁矿石
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