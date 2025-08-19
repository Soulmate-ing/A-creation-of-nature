using MyGame.Inventory;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyGame.Inventory
{
    public class ItemManager : MonoBehaviour
    {
        public Item itemPerfab;
        public Item bounceItemPerfab;
        private Transform itemParent;

        private Transform playerTransform => FindObjectOfType<Player>().transform;

        //记录场景Item
        private Dictionary<string,List<SceneItem>> sceneItemDict = new Dictionary<string, List<SceneItem>>();
        //记录场景家具
        private Dictionary<string, List<SceneFurniture>> sceneFurnitureDict = new Dictionary<string, List<SceneFurniture>>();

        private void OnEnable()
        {
            EventHandler.InstantiateItemScene += OnInstantiateItemScene;
            EventHandler.DropItemEvent += OnDropItemEvent;
            EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
            EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
            //建造
            EventHandler.BuildFurnitureEvent += OnBuildFurnitureEvent;
        }
        private void OnDisable()
        {
            EventHandler.InstantiateItemScene -= OnInstantiateItemScene;
            EventHandler.DropItemEvent -= OnDropItemEvent;
            EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
            EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
            EventHandler.BuildFurnitureEvent -= OnBuildFurnitureEvent;
        }

        private void OnBuildFurnitureEvent(int ID,Vector3 mousePos)
        {
            BluePrintDetails bluePrint = InventoryManager.Instance.bluePrintData.GetBluePrintDetails(ID);
            var buildItem = Instantiate(bluePrint.buildPrefab, mousePos,Quaternion.identity,itemParent);
        }

        private void OnBeforeSceneUnloadEvent()
        {
            GetAllSceneItems();
            GetAllSceneFurniture();
        }

        private void OnAfterSceneLoadedEvent()
        {
            itemParent = GameObject.FindWithTag("ItemParent").transform;
            RecreateAllItems();
            RebuildFurniture();
        }
        /// <summary>
        /// 在指定坐标位置生成物品
        /// </summary>
        /// <param name="ID">物品ID</param>
        /// <param name="pos">世界坐标</param>
        public void OnInstantiateItemScene(int ID, Vector3 pos)
        {
            var item = Instantiate(itemPerfab, pos, Quaternion.identity,itemParent);
            item.itemID = ID;
        }

        private void OnDropItemEvent(int ID, Vector3 mousePos)
        {
            //扔东西的效果
            var item = Instantiate(bounceItemPerfab, playerTransform.position, Quaternion.identity, itemParent);
            item.itemID = ID;
            var dir = (mousePos - playerTransform.position).normalized;
            item.GetComponent<ItemBounce>().InitBounceItem(mousePos, dir);
        }

        /// <summary>
        /// 获取当前场景所有Item
        /// </summary>
        private void GetAllSceneItems()
        {
            List<SceneItem> currentSceneItems = new List<SceneItem>();
            foreach (var item in FindObjectsOfType<Item>())
            {
                SceneItem sceneItem = new SceneItem
                {
                    itemID = item.itemID,
                    position = new SerializableVector3(item.transform.position)
                };
                currentSceneItems.Add(sceneItem);
            }
            if (sceneItemDict.ContainsKey(SceneManager.GetActiveScene().name))
            {
                //找到数据就更新item数据列表
                sceneItemDict[SceneManager.GetActiveScene().name] = currentSceneItems;
            }
            else//如果是新场景
            {
                sceneItemDict.Add(SceneManager.GetActiveScene().name, currentSceneItems);
            }
        }

        /// <summary>
        /// 刷新重建当前场景的物品
        /// </summary>
        private void RecreateAllItems()
        {
            List<SceneItem> currentSceneItems = new List<SceneItem>();
            if (sceneItemDict.TryGetValue(SceneManager.GetActiveScene().name, out currentSceneItems))
            {
                if (currentSceneItems != null)
                {
                    // 清场
                    foreach (var item in FindObjectsOfType<Item>())
                    {
                        // 确保不销毁 GetItem 对象
                        if (item.name != "GetItem")
                        {
                            Destroy(item.gameObject);
                        }
                    }
                    foreach (var item in currentSceneItems)
                    {
                        Item newItem = Instantiate(itemPerfab, item.position.ToVector3(), Quaternion.identity, itemParent);
                        newItem.Init(item.itemID);
                    }
                }
            }
        }

        /// <summary>
        /// 获得场景所有家具
        /// </summary>
        private void GetAllSceneFurniture()
        {
            List<SceneFurniture> currentSceneFurniture = new List<SceneFurniture>();

            foreach (var item in FindObjectsOfType<Furniture>())
            {
                SceneFurniture sceneFurniture = new SceneFurniture
                {
                    itemID = item.itemID,
                    position = new SerializableVector3(item.transform.position)
                };
                //if (item.GetComponent<Box>())
                //    sceneFurniture.boxIndex = item.GetComponent<Box>().index;

                currentSceneFurniture.Add(sceneFurniture);
            }

            if (sceneFurnitureDict.ContainsKey(SceneManager.GetActiveScene().name))
            {
                //找到数据就更新item数据列表
                sceneFurnitureDict[SceneManager.GetActiveScene().name] = currentSceneFurniture;
            }
            else    //如果是新场景
            {
                sceneFurnitureDict.Add(SceneManager.GetActiveScene().name, currentSceneFurniture);
            }
        }

        /// <summary>
        /// 重建当前场景家具
        /// </summary>
        private void RebuildFurniture()
        {
            List<SceneFurniture> currentSceneFurniture = new List<SceneFurniture>();

            if (sceneFurnitureDict.TryGetValue(SceneManager.GetActiveScene().name, out currentSceneFurniture))
            {
                if (currentSceneFurniture != null)
                {
                    foreach (SceneFurniture sceneFurniture in currentSceneFurniture)
                    {
                        OnBuildFurnitureEvent(sceneFurniture.itemID, sceneFurniture.position.ToVector3());
                    }
                }
            }
        }
    }
}


