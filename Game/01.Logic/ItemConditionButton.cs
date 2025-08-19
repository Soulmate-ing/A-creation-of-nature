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
        public List<RequiredItem> requiredItems = new List<RequiredItem>(); // ��Ҫ������Ʒ�б�
        public Button targetButton;  // Ŀ�갴ť
        public TextMeshProUGUI tipText; // ��ʾ�ı�
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
                tipText.text = "���ϲ���";
                isTipVisible = true;
                Invoke("ClearTipText", 5f); // 5��������ʾ�ı�
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
                // ִ�а�ť�߼�������������Ʒ
                foreach (var requiredItem in requiredItems)
                {
                    InventoryManager.Instance.RemoveItem(requiredItem.itemID, requiredItem.amount);
                }
                Debug.Log("��ť�ѵ������Ʒ�����ģ�");

                // ������Ʒ��UI
                EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, InventoryManager.Instance.playerBag.itemList);

                // ǿ�Ƹ��°�ť״̬
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