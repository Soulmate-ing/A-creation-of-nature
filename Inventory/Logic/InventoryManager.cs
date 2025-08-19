using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MyGame.Inventory
{
    public class InventoryManager : Singleton<InventoryManager>
    {
        [Header("��Ʒ����")]
        public ItemDataList_SO itemDataList_SO;

        [Header("������ͼ")]
        public BluPrintDataList_SO bluePrintData;

        [Header("��������")]
        public InventoryBag_SO playerBag;

        private void OnEnable()
        {
            EventHandler.DropItemEvent += OnDropItemEvent;
            //����
            EventHandler.BuildFurnitureEvent += OnBuildFurnitureEvent;
        }

        private void OnDisable()
        {
            EventHandler.DropItemEvent -= OnDropItemEvent;
            EventHandler.BuildFurnitureEvent -= OnBuildFurnitureEvent;
        }

        private void Start()
        {
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }

        //������ͼ
        private void OnBuildFurnitureEvent(int ID, Vector3 mousePos)
        {
            // �����Ʒ�Ƿ��ڱ�����
            if (!IsItemInBag(ID))
            {
                Debug.LogError("��Ʒ���ڱ����У��޷����죡");
                return;
            }

            // �����ͼ�Ƿ����
            BluePrintDetails bluePrint = bluePrintData.GetBluePrintDetails(ID);
            if (bluePrint == null)
            {
                Debug.LogError("��ͼ�����ڣ�");
                return;
            }

            // �����ͼ����Դ��Ʒ�б��Ƿ�Ϊ��
            if (bluePrint.resourceItem.Count() == 0)
            {
                // �����ͼ��û����Դ��Ʒ��ֻ�Ƴ�һ������Ʒ
                RemoveItem(ID, 1);
            }
            else
            {
                // �����ͼ������Դ��Ʒ���Ƴ���Щ��Դ��Ʒ
                foreach (var resourceItem in bluePrint.resourceItem)
                {
                    if (!IsItemInBag(resourceItem.itemID))
                    {
                        Debug.LogError("��Դ��Ʒ���ڱ����У��޷����죡");
                        return;
                    }
                    RemoveItem(resourceItem.itemID, resourceItem.itemAmount);
                }
            }

            // ���ؽ���ͼƬ
            if (CursorManager.Instance != null)
            {
                CursorManager.Instance.buildImage.gameObject.SetActive(false);
            }

            // ����UI
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }
        private void OnDropItemEvent(int ID, Vector3 pos)
        {
            RemoveItem(ID, 1);

            //�����Ʒ����Ϊ�㣬ȡ��ѡ��
            if (GetItemCount(ID) <= 0)
            {
                EventHandler.CallItemSelectedEvent(null, false);
            }
        }

        /// <summary>
        /// ͨ��ID������Ʒ��Ϣ
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ItemDetails GetItemDetails(int ID)
        {
            return itemDataList_SO.itemDetailsList.Find(i => i.itemID == ID);
        }

        /// <summary>
        /// �����Ʒ�Ƿ��Ѿ��ڱ�����
        /// </summary>
        /// <param name="itemID">��ƷID</param>
        /// <returns>�Ƿ��ڱ�����</returns>
        public bool IsItemInBag(int ID)
        {
            return GetItemIndexInBag(ID) != -1;
        }

        /// <summary>
        /// ��ȡ������ָ����Ʒ������
        /// </summary>
        /// <param name="ID">��ƷID</param>
        /// <returns>��Ʒ����</returns>
        public int GetItemCount(int ID)
        {
            int index = GetItemIndexInBag(ID);
            if (index != -1)
            {
                return playerBag.itemList[index].itemAmount;
            }
            return 0;
        }

        /// <summary>
        /// ��ȡ��Ʒ������
        /// </summary>
        /// <param name="ID">��ƷID</param>
        /// <returns>��Ʒ����</returns>
        public string GetItemName(int ID)
        {
            ItemDetails itemDetails = GetItemDetails(ID);
            if (itemDetails != null)
            {
                return itemDetails.itemName;
            }
            return "δ֪��Ʒ";
        }

        /// <summary>
        /// �����Ʒ��Player������
        /// </summary>
        /// <param name="item"></param>
        /// <param name="toDestory">�Ƿ�Ҫ������Ʒ</param>
        public void AddItem(Item item, bool toDestory)
        {
            var index = GetItemIndexInBag(item.itemID);
            AddItemAtIndex(item.itemID, index, 1);
            if (toDestory)
                Destroy(item.gameObject);

            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }

        /// <summary>
        /// ��鱳���Ƿ��п�λ
        /// </summary>
        /// <returns></returns>
        private bool CheckBagCapacity()
        {
            for (int i = 0; i < playerBag.itemList.Count; i++)
            {
                if (playerBag.itemList[i].itemID == 0)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// ͨ����ƷID�ҵ�����������Ʒλ��
        /// </summary>
        /// <param name="ID">��ƷID</param>
        /// <returns>-1��û�������Ʒ ���򷵻����к�</returns>
        private int GetItemIndexInBag(int ID)
        {
            for (int i = 0; i < playerBag.itemList.Count; i++)
            {
                if (playerBag.itemList[i].itemID == ID)
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// ��ָ���������λ�������Ʒ
        /// </summary>
        /// <param name="ID">��ƷID</param>
        /// <param name="index">���</param>
        /// <param name="amount">����</param>
        private void AddItemAtIndex(int ID, int index, int amount)
        {
            if (index == -1 && CheckBagCapacity())
            {
                var item = new InventoryItem { itemID = ID, itemAmount = amount };
                for (int i = 0; i < playerBag.itemList.Count; i++)
                {
                    if (playerBag.itemList[i].itemID == 0)
                    {
                        playerBag.itemList[i] = item;
                        break;
                    }
                }
            }
            else
            {
                int currentAmount = playerBag.itemList[index].itemAmount + amount;
                var item = new InventoryItem { itemID = ID, itemAmount = currentAmount };
                playerBag.itemList[index] = item;
            }
        }

        /// <summary>
        /// Player������Χ�ڽ�����Ʒ
        /// </summary>
        /// <param name="fromIndex">��ʼ���</param>
        /// <param name="targetIndex">Ŀ���������</param>
        public void SwapItem(int fromIndex, int targetIndex)
        {
            InventoryItem currentItem = playerBag.itemList[fromIndex];
            InventoryItem targetItem = playerBag.itemList[targetIndex];

            if (targetItem.itemID != 0)
            {
                playerBag.itemList[fromIndex] = targetItem;
                playerBag.itemList[targetIndex] = currentItem;
            }
            else
            {
                playerBag.itemList[targetIndex] = currentItem;
                playerBag.itemList[fromIndex] = new InventoryItem();
            }
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }

        /// <summary>
        /// �Ƴ�ָ�������ı�����Ʒ
        /// </summary>
        /// <param name="ID"> ��ƷID</param>
        /// <param name="removeAmount"> ����</param>
        public void RemoveItem(int ID, int removeAmount)
        {
            int index = GetItemIndexInBag(ID);
            if (index == -1)
            {
                Debug.LogWarning($"��Ʒ ID={ID} �������ڱ�����");
                return;
            }

            InventoryItem currentItem = playerBag.itemList[index];
            if (currentItem.itemAmount > removeAmount)
            {
                currentItem.itemAmount -= removeAmount;
                playerBag.itemList[index] = currentItem;
            }
            else
            {
                // ��Ʒ�������������Ϊ�յ� InventoryItem
                playerBag.itemList[index] = new InventoryItem();
            }

            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }

        /// <summary>
        /// ��齨����Դ��Ʒ���
        /// </summary>
        /// <param name="ID">ͼֽID</param>
        /// <returns></returns>
        public bool CheckStock(int ID)
        {
            var bluePrintDetails = bluePrintData.GetBluePrintDetails(ID);
            foreach (var resourceItem in bluePrintDetails.resourceItem)
            {
                var itemStock = playerBag.GetInventoryItem(resourceItem.itemID);
                if (itemStock.itemAmount >= resourceItem.itemAmount)
                {
                    continue;
                }
                else return false;
            }
            return true;
        }
    }
}