using MyGame.Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFarm.Inventory
{
    public class TreeHealth : MonoBehaviour
    {
        public List<Sprite> healthSprites; // �洢��ľ��ͬHP״̬��ͼƬ
        public int currentHealth = 5; // ��ǰHP
        private SpriteRenderer spriteRenderer;
        private ItemManager itemManager;
        private CursorManager cursorManager;
        public ItemDetails currentTool;

        public int instantiateItemID = 1003;

        public bool playerInRange = false;
        private float displayTime = 2.0f; // HP ��ʾ��ʱ�䣨�룩
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
                playerInRange = true;
            transform.position = new Vector3(transform.position.x + 0.01f, transform.position.y, transform.position.z);
        }

        // ʹ�ô������������뿪��Χ
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
                playerInRange = false;
        }

        private void OnMouseDown()
        {
            // ��ȡ��ǰѡ��Ĺ���
            currentTool = cursorManager.currentItem;

            // ����Ƿ�ѡ���˿������ߡ����״̬Ϊ�������������ײ��Χ��
            if (currentTool != null && currentTool.itemType == ItemType.ChopTool && playerInRange)
            {
                // ��ȡ���λ�ü����
                PlayerPositionDetector detector = GetComponentInChildren<PlayerPositionDetector>();
                if (detector != null)
                {
                    // �����¼�
                    OnTreeChop?.Invoke(detector.IsPlayerOnLeft);
                }
                ShowHP(); // ��ʾ HP

                if (currentHealth > 0)
                {
                    currentHealth--;
                    //��������
                    if (soundEffect1 != SoundName.none)
                    {
                        var soundDetails = AudioManager.Instance.soundDetailsData.GetSoundDetails(soundEffect1);
                        EventHandler.CallInitSoundEffect(soundDetails);
                    }
                    UpdateSprite();

                    // ������Ҷ������Ч
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
            // ���������������Ʒ
            int itemCount = Random.Range(4, 11); // ����2-5����Ʒ
            for (int i = 0; i < itemCount; i++)
            {
                Vector3 spawnPos = transform.position + new Vector3(
                    Random.Range(-0.5f, 0.5f),
                    Random.Range(-0.5f, 0.5f),
                    0
                );
                itemManager.OnInstantiateItemScene(instantiateItemID, spawnPos);
            }

            Destroy(gameObject); // ������ľ
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