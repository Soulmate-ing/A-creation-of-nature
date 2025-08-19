using MyGame.Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace MFarm.Inventory
{
    public class MineralHealth : MonoBehaviour
    {
        public List<Sprite> healthSprites; // �洢��ʯ��ͬHP״̬��ͼƬ
        public int currentHealth = 3; // ��ǰHP
        private SpriteRenderer spriteRenderer;
        private ItemManager itemManager;
        private CursorManager cursorManager;
        private ItemDetails currentTool;

        public List<int> itemIDs = new List<int>(); // �洢�����ɵ���ƷID�б�

        private bool playerInRange = false;
        private float displayTime = 2.0f; // HP ��ʾ��ʱ�䣨�룩
        private float timer;
        private bool isDisplayingHP = false;

        public SoundName pickAxe;
        public SoundName stock;

        private PoolManager poolManager;
        [Header("��Чλ��ƫ��")]
        public Vector3 effectOffset = new Vector3(0f, 0.5f, 0f);
        public static event System.Action<bool> OnMineralMine;

        private void Awake()
        {
            BoxCollider2D collider = gameObject.AddComponent<BoxCollider2D>();
            collider.isTrigger = false;
            collider.offset = new Vector2(-0.1f, 0.11f);
            collider.size = new Vector2(3.1f, 1.4f);
            collider.isTrigger = true;
            Debug.Log("�����");
        }
        private void Start()
        {
            poolManager = FindObjectOfType<PoolManager>();
            // ȷ�����ʵ��� HP �Ӷ���� SpriteRenderer
            spriteRenderer = transform.Find("HP").GetComponent<SpriteRenderer>();
            spriteRenderer.enabled = false; // Ĭ�ϲ���ʾ HP
            itemManager = FindObjectOfType<ItemManager>();
            cursorManager = FindObjectOfType<CursorManager>();

            UpdateSprite(); // ��ʼ����ʾ
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

        // ʹ�ô����������ҽ��뷶Χ
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                playerInRange = true;
            }
            transform.position = new Vector3(transform.position.x + 0.01f, transform.position.y, transform.position.z);
        }

        // ʹ�ô������������뿪��Χ
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                playerInRange = false;
            }
        }

        private void OnMouseDown()
        {
            // ��ȡ��ǰѡ��Ĺ���
            currentTool = cursorManager.currentItem;

            // ����Ƿ�ѡ�����ھ򹤾ߡ����״̬Ϊ�������������ײ��Χ��
            if (currentTool != null && currentTool.itemType == ItemType.MiningTool && playerInRange)
            {
                PlayerPositionDetector detector = GetComponentInChildren<PlayerPositionDetector>();
                if (detector != null)
                {
                    OnMineralMine?.Invoke(detector.IsPlayerOnLeft);
                }
                ShowHP(); // ��ʾ HP

                if (currentHealth > 0)
                {
                    currentHealth--;
                    if (pickAxe != SoundName.none)
                    {
                        var soundDetails = AudioManager.Instance.soundDetailsData.GetSoundDetails(pickAxe);
                        EventHandler.CallInitSoundEffect(soundDetails);
                    }
                    UpdateSprite();
                    // ����������Ч
                    if (poolManager != null)
                    {
                        // �޸ĺ��λ�ü��㣨ԭλ�� + ƫ������
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
            // ���������������Ʒ
            int itemCount = Random.Range(8, 15); // ����8-14����Ʒ
            for (int i = 0; i < itemCount; i++)
            {
                // ���б������ѡ��һ����ƷID
                int randomItemID;
                if (itemIDs.Count > 0)
                {
                    randomItemID = itemIDs[Random.Range(0, itemIDs.Count)];
                }
                else
                {
                    randomItemID = 1005; // Ĭ����ƷID
                }

                Vector3 spawnPos = transform.position + new Vector3(
                    Random.Range(-0.5f, 0.5f),
                    Random.Range(-0.5f, 0.5f),
                    0
                );
                itemManager.OnInstantiateItemScene(randomItemID, spawnPos);
            }

            Destroy(gameObject); // ���ٿ�ʯ
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