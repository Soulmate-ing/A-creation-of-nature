using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GridMapManager : Singleton<GridMapManager>
{
    [Header("��ͼ��Ϣ")]
    public List<MapData_SO> mapDataList;

    //��������+����Ͷ�Ӧ����Ƭ��Ϣ
    private Dictionary<string, TileDetails> tileDetailsDict = new Dictionary<string, TileDetails>();

    private Grid currentGrid;

    private void OnEnable()
    {
        EventHandler.ExecuteActionAfterAnimation += OnExecuteActionAfterAnimation;
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
    }
    private void OnDisable()
    {
        EventHandler.ExecuteActionAfterAnimation -= OnExecuteActionAfterAnimation;
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;

    }

    private void Start()
    {
        foreach (var mapData in mapDataList)
        {
            InitTileDetailsDict(mapData);
        }
    }
    private void OnAfterSceneLoadedEvent()
    {
        currentGrid = FindObjectOfType<Grid>();
    }

    /// <summary>
    /// ���ݵ�ͼ��Ϣ�����ֵ�
    /// </summary>
    /// <param name="mapData">��ͼ��Ϣ</param>
    private void InitTileDetailsDict(MapData_SO mapData)
    {
        foreach (TileProperty tileProperty in mapData.tileProperties)
        {
            TileDetails tileDetails = new TileDetails
            {
                gridX = tileProperty.tileCoordinate.x,
                gridY = tileProperty.tileCoordinate.y
            };
            //�ֵ��Key
            string key = tileDetails.gridX + "x" + tileDetails.gridY + "y" + mapData.sceneName;

            if (GetTileDetails(key) != null)
            {
                tileDetails = GetTileDetails(key);
            }
            switch (tileProperty.gridType)
            {
                case GridType.Diggable:
                    tileDetails.canDig = tileProperty.boolTypeValue;
                    break;
                case GridType.DropItem:
                    tileDetails.canDropItem = tileProperty.boolTypeValue;
                    break;
                case GridType.PlaceFurniture:
                    tileDetails.canPlaceFurniture = tileProperty.boolTypeValue;
                    break;
                case GridType.NPCObstacle:
                    tileDetails.isNPCObstacle = tileProperty.boolTypeValue;
                    break;
            }
            if (GetTileDetails(key) != null)
                tileDetailsDict[key] = tileDetails;
            else
                tileDetailsDict.Add(key, tileDetails);
        }
    }

    /// <summary>
    /// ����key������Ƭ��Ϣ
    /// </summary>
    /// <param name="key">x+y+��ͼ����</param>
    /// <returns></returns>
    private TileDetails GetTileDetails(string key)
    {
        if (tileDetailsDict.ContainsKey(key))
        {
            return tileDetailsDict[key];
        }
        return null;
    }

    /// <summary>
    /// ��������������귵����Ƭ��Ϣ
    /// </summary>
    /// <param name="mouseGirdPos">�����������</param>
    /// <returns></returns>
    public TileDetails GetTileDetailsOnMousePosition(Vector3Int mouseGirdPos)
    {
        string key = mouseGirdPos.x + "x" + mouseGirdPos.y + "y" + SceneManager.GetActiveScene().name;
        return GetTileDetails(key);
    }

    /// <summary>
    /// ִ��ʵ�ʹ��߻���Ʒ����
    /// </summary>
    /// <param name="mouseWorldPos">�������</param>
    /// <param name="itemDetails">��Ʒ��Ϣ</param>
    private void OnExecuteActionAfterAnimation(Vector3 mouseWorldPos, ItemDetails itemDetails)
    {
        var mouseGridPos = currentGrid.WorldToCell(mouseWorldPos);
        var currentTilec = GetTileDetailsOnMousePosition(mouseGridPos);

        if (currentTilec != null)
        {
            //��Ʒʹ�õ�ʵ�ʹ���
            switch (itemDetails.itemType)
            {
                case ItemType.Seed:
                    break;
                case ItemType.Commodity:
                    EventHandler.CallDropItemEvent(itemDetails.itemID, mouseWorldPos);
                    break;
                case ItemType.Furniture:
                    //�ڵ�ͼ��������Ʒ ItemManager
                    //�Ƴ���ǰ��Ʒ��ͼֽ��InventoryManager
                    //�Ƴ���Դ��Ʒ
                    EventHandler.CallBuildFurnitureEvent(itemDetails.itemID, mouseWorldPos);
                    break;
                case ItemType.MiningTool:
                    break;
                case ItemType.ChopTool:
                    break;
                case ItemType.BreakTool:
                    break;
                case ItemType.WaterTool:
                    break;
                case ItemType.CollectTool:
                    break;
                case ItemType.ReapablesScenery:
                    break;
                default:
                    break;
            }
        }
    }
}