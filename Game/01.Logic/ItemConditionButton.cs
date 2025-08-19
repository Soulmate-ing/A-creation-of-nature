using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MyGame.Inventory
{
    [System.Serializable]
    public class RequiredItem
    {
        public int itemID;
        public int amount;
    }

    public class ItemConditionButton : MonoBehaviour
    {
        public List<RequiredItem> requiredItems = new List<RequiredItem>(); // 需要检测的物品列表
        public Button targetButton;  // 目标按钮
        public TextMeshProUGUI tipText; // 提示文本
        public GameObject startUI;

        private bool isTipVisible = false;

        private void Start()
        {
            UpdateButtonState();
        }

        private void Update()
        {
            UpdateButtonState();
        }

        private void UpdateButtonState()
        {
            bool hasEnoughItems = true;

            foreach (var requiredItem in requiredItems)
            {
                int itemCount = InventoryManager.Instance.GetItemCount(requiredItem.itemID);
                if (itemCount < requiredItem.amount)
                {
                    hasEnoughItems = false;
                    break;
                }
            }

            targetButton.interactable = hasEnoughItems;

            if (!hasEnoughItems && !isTipVisible)
            {
                tipText.text = "材料不足";
                isTipVisible = true;
                Invoke("ClearTipText", 5f); // 5秒后清空提示文本
            }
            else if (hasEnoughItems && isTipVisible)
            {
                tipText.text = "";
                isTipVisible = false;
            }
        }

        private void ClearTipText()
        {
            tipText.text = "";
            isTipVisible = false;
        }

        public void OnButtonClicked()
        {
            if (targetButton.interactable)
            {
                // 执行按钮逻辑，例如消耗物品
                foreach (var requiredItem in requiredItems)
                {
                    InventoryManager.Instance.RemoveItem(requiredItem.itemID, requiredItem.amount);
                }
                Debug.Log("按钮已点击，物品已消耗！");

                // 更新物品栏UI
                EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, InventoryManager.Instance.playerBag.itemList);

                // 强制更新按钮状态
                UpdateButtonState();
            }
        }
        public void StartUITrue()
        {
            startUI.SetActive(true);
        }
        public void StartUIFalse()
        {
            startUI.SetActive(false);
            DialogueControl.Instance.leftnameImage.gameObject.SetActive(false);
            DialogueControl.Instance.leftnameText.gameObject.SetActive(false);
            DialogueControl.Instance.rightnameImage.gameObject.SetActive(false);
            DialogueControl.Instance.rightnameText.gameObject.SetActive(false);
            DialogueControl.Instance.dialogueBox.SetActive(false);
            DialogueControl.Instance.EscDialogueButton.gameObject.SetActive(false);
            DialogueControl.Instance.ShuiCheButton.gameObject.SetActive(false);
            DialogueControl.Instance.KuangShiButton.gameObject.SetActive(false);
            DialogueControl.Instance.ChuanButton.gameObject.SetActive(false);
            DialogueControl.Instance.PulleyButton.gameObject.SetActive(false);
            DialogueControl.Instance.SuiPianButton.SetActive(false);
        }
    }
}