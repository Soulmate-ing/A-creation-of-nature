using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MyGame.Inventory
{
    public class InventoryManager : Singleton<InventoryManager>
    {
        [Header("物品数据")]
        public ItemDataList_SO itemDataList_SO;

        [Header("建造蓝图")]
        public BluPrintDataList_SO bluePrintData;

        [Header("背包数据")]
        public InventoryBag_SO playerBag;

        private void OnEnable()
        {
            EventHandler.DropItemEvent += OnDropItemEvent;
            //建造
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

        //建造蓝图
        private void OnBuildFurnitureEvent(int ID, Vector3 mousePos)
        {
            // 检查物品是否在背包中
            if (!IsItemInBag(ID))
            {
                Debug.LogError("物品不在背包中，无法建造！");
                return;
            }

            // 检查蓝图是否存在
            BluePrintDetails bluePrint = bluePrintData.GetBluePrintDetails(ID);
            if (bluePrint == null)
            {
                Debug.LogError("蓝图不存在！");
                return;
            }

            // 检查蓝图的资源物品列表是否为空
            if (bluePrint.resourceItem.Count() == 0)
            {
                // 如果蓝图中没有资源物品，只移除一个该物品
                RemoveItem(ID, 1);
            }
            else
            {
                // 如果蓝图中有资源物品，移除这些资源物品
                foreach (var resourceItem in bluePrint.resourceItem)
                {
                    if (!IsItemInBag(resourceItem.itemID))
                    {
                        Debug.LogError("资源物品不在背包中，无法建造！");
                        return;
                    }
                    RemoveItem(resourceItem.itemID, resourceItem.itemAmount);
                }
            }

            // 隐藏建筑图片
            if (CursorManager.Instance != null)
            {
                CursorManager.Instance.buildImage.gameObject.SetActive(false);
            }

            // 更新UI
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }
        private void OnDropItemEvent(int ID, Vector3 pos)
        {
            RemoveItem(ID, 1);

            //如果物品数量为零，取消选中
            if (GetItemCount(ID) <= 0)
            {
                EventHandler.CallItemSelectedEvent(null, false);
            }
        }

        /// <summary>
        /// 通过ID返回物品信息
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ItemDetails GetItemDetails(int ID)
        {
            return itemDataList_SO.itemDetailsList.Find(i => i.itemID == ID);
        }

        /// <summary>
        /// 检查物品是否已经在背包中
        /// </summary>
        /// <param name="itemID">物品ID</param>
        /// <returns>是否在背包中</returns>
        public bool IsItemInBag(int ID)
        {
            return GetItemIndexInBag(ID) != -1;
        }

        /// <summary>
        /// 获取背包中指定物品的数量
        /// </summary>
        /// <param name="ID">物品ID</param>
        /// <returns>物品数量</returns>
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
        /// 获取物品的名字
        /// </summary>
        /// <param name="ID">物品ID</param>
        /// <returns>物品名字</returns>
        public string GetItemName(int ID)
        {
            ItemDetails itemDetails = GetItemDetails(ID);
            if (itemDetails != null)
            {
                return itemDetails.itemName;
            }
            return "未知物品";
        }

        /// <summary>
        /// 添加物品到Player背包里
        /// </summary>
        /// <param name="item"></param>
        /// <param name="toDestory">是否要销毁物品</param>
        public void AddItem(Item item, bool toDestory)
        {
            var index = GetItemIndexInBag(item.itemID);
            AddItemAtIndex(item.itemID, index, 1);
            if (toDestory)
                Destroy(item.gameObject);

            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }

        /// <summary>
        /// 检查背包是否有空位
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
        /// 通过物品ID找到背包已有物品位置
        /// </summary>
        /// <param name="ID">物品ID</param>
        /// <returns>-1则没有这个物品 否则返回序列号</returns>
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
        /// 在指定背包序号位置添加物品
        /// </summary>
        /// <param name="ID">物品ID</param>
        /// <param name="index">序号</param>
        /// <param name="amount">数量</param>
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
        /// Player背包范围内交换物品
        /// </summary>
        /// <param name="fromIndex">起始序号</param>
        /// <param name="targetIndex">目标数据序号</param>
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
        /// 移除指定数量的背包物品
        /// </summary>
        /// <param name="ID"> 物品ID</param>
        /// <param name="removeAmount"> 数量</param>
        public void RemoveItem(int ID, int removeAmount)
        {
            int index = GetItemIndexInBag(ID);
            if (index == -1)
            {
                Debug.LogWarning($"物品 ID={ID} 不存在于背包中");
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
                // 物品数量清零后，重置为空的 InventoryItem
                playerBag.itemList[index] = new InventoryItem();
            }

            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }

        /// <summary>
        /// 检查建造资源物品库存
        /// </summary>
        /// <param name="ID">图纸ID</param>
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