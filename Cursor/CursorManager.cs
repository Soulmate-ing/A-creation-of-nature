using MyGame.Inventory;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CursorManager : Singleton<CursorManager>
{
    public Sprite normal, tool, furniture, item;
    private Sprite currentSprite; //存储当前鼠标图片
    private Image cursorImage;
    private RectTransform cursorCanvas;
    //建造图标跟随
    [HideInInspector]public Image buildImage;

    //鼠标检测
    private Camera mainCamera;
    private Grid currentGrid;

    private Vector3 mouseWorldPos;
    private Vector3Int mouseGridPos;

    private bool cursorEnable;
    [HideInInspector] public bool cursorPositionVaild;

    [HideInInspector]public ItemDetails currentItem;


    private Transform PlayerTransform => FindObjectOfType<Player>().transform;
    private void OnEnable()
    {
        EventHandler.ItemSelectedEvent += OnItemSelectedEvent;
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
        EventHandler.BuildFurnitureEvent += OnBuildFurnitureEvent;
    }
    private void OnDisable()
    {
        EventHandler.ItemSelectedEvent -= OnItemSelectedEvent;
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
        EventHandler.BuildFurnitureEvent -= OnBuildFurnitureEvent;
    }
    private void Start()
    {

        cursorCanvas = GameObject.FindGameObjectWithTag("CursorCanvas").GetComponent<RectTransform>();
        cursorImage = cursorCanvas.GetChild(0).GetComponent<Image>();

        buildImage = cursorCanvas.GetChild(1).GetComponent<Image>();
        buildImage.gameObject.SetActive(false);

        currentSprite = normal;
        SetCursorImage(normal);

        mainCamera = Camera.main;
    }
    private void Update()
    {
        Cursor.visible = false;
        if (cursorCanvas == null)
            return;
        cursorImage.transform.position = Input.mousePosition;

        if (!InteractWithUI() && cursorEnable)
        {
            SetCursorImage(currentSprite);
            CheckCursorValid();
            CheckPlayerInput();
        }
        else
        {
            SetCursorImage(normal);
            //buildImage.gameObject.SetActive(false);
        }
    }
    private void CheckPlayerInput()
    {
        if (Input.GetMouseButtonDown(0) && cursorPositionVaild)
        {
            //执行方法
            EventHandler.CallMouseClickEvent(mouseWorldPos, currentItem);
        }
    }

    private void OnBeforeSceneUnloadEvent()
    {
        cursorEnable = false;
    }
    private void OnAfterSceneLoadedEvent()
    {
        currentGrid = FindObjectOfType<Grid>();
    }
    private void OnBuildFurnitureEvent(int ID, Vector3 mousePos)
    {
        // 建造完成后重置鼠标指针
        ResetCursorToNormal();
    }
    private void ResetCursorToNormal()
    {
        currentSprite = normal;
        SetCursorImage(normal);
        cursorEnable = false;
        buildImage.gameObject.SetActive(false);
        currentItem = null;
    }
    #region 设置鼠标样式
    /// <summary>
    /// 设置鼠标图片
    /// </summary>
    /// <param name="sprite"></param>
    private void SetCursorImage(Sprite sprite)
    {
        cursorImage.sprite = sprite;
        cursorImage.color = new Color(1, 1, 1, 1);
    }
    /// <summary>
    /// 设置鼠标可用状态
    /// </summary>
    private void SetCursorVaild()
    {
        cursorPositionVaild = true;
        cursorImage.color = new Color(1, 1, 1, 1);
        buildImage.color = new Color(1, 1, 1, 0.5f);
    }
    /// <summary>
    /// 设置鼠标不可用状态
    /// </summary>
    private void SetCursorInVaild()
    {
        cursorPositionVaild = false;
        cursorImage.color = new Color(1, 0, 0, 0.4f);
        buildImage.color = new Color(1, 0, 0, 0.5f);
    }
    #endregion

    private void OnItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    {
        if (!isSelected)
        {
            currentItem = null;
            cursorEnable = false;
            currentSprite = normal;
            buildImage.gameObject.SetActive(false);
        }
        else
        {
            currentItem = itemDetails;
            //WORKFLOW:添加对应类型的图片
            currentSprite = itemDetails.itemType switch
            {
                ItemType.ChopTool => tool,
                ItemType.MiningTool => tool,
                ItemType.WaterTool => tool,
                ItemType.BreakTool => tool,
                ItemType.CollectTool => tool,
                ItemType.Commodity => item,
                ItemType.Furniture => furniture,
                _ => normal
            };
            cursorEnable = true;

            //显示建造物品图片
            if(itemDetails.itemType == ItemType.Furniture)
            {
                buildImage.gameObject.SetActive(true);
                buildImage.sprite = itemDetails.itemOnWorldSprite;
                buildImage.SetNativeSize();
            }
        }
    }
    private void CheckCursorValid()
    {

        mouseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z));
        mouseGridPos = currentGrid.WorldToCell(mouseWorldPos);

        //Debug.Log("WorldPos:" + mouseWorldPos + "  GridPos:" + mouseGridPos);

        var playerGridPos = currentGrid.WorldToCell(PlayerTransform.position);

        //建造图片跟随
        buildImage.rectTransform.position = Input.mousePosition;

        //判断在使用范围内
        if (Mathf.Abs(mouseGridPos.x - playerGridPos.x) > currentItem.itemUseRadius || Mathf.Abs(mouseGridPos.y - playerGridPos.y) > currentItem.itemUseRadius)
        {
            SetCursorInVaild();
            return;
        }

        TileDetails currentTile = GridMapManager.Instance.GetTileDetailsOnMousePosition(mouseGridPos);
        if (currentTile != null)
        {
            switch (currentItem.itemType)
            {
                case ItemType.Commodity:
                    if (currentTile.canDropItem && currentItem.canDropped) SetCursorVaild();
                    else SetCursorInVaild();
                    break;
                case ItemType.Furniture:
                    if (currentTile.canPlaceFurniture && InventoryManager.Instance.CheckStock(currentItem.itemID)) SetCursorVaild();
                    else SetCursorInVaild();
                    break;
                case ItemType.ChopTool:
                    SetCursorVaild();
                    break;
                case ItemType.MiningTool:
                    SetCursorVaild();
                    break;
            }
        }
        else
        {
            SetCursorInVaild();
        }
    }
    /// <summary>
    ///  是否与UI互动
    /// </summary>
    /// <returns></returns>
    private bool InteractWithUI()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return true;
        }
        return false;
    }
   
}