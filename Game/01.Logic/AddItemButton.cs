using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MyGame.Inventory
{
    public class AddItemButton : MonoBehaviour
    {
        private Item item;
        private Button button;
        public TextMeshProUGUI text;

        private void Awake()
        {
            item = GameObject.Find("GetItem").GetComponent<Item>();
            button = GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(AddItemToBag);
            }
            // 默认不显示文本
            if (text != null)
            {
                text.text = "";
            }
        }

        private void AddItemToBag()
        {
            // 检查物品是否已经在背包中
            if (InventoryManager.Instance.IsItemInBag(item.itemID))
            {
                // 如果物品已在背包中，弹出提示
                SetText("背包中已有该物品！", 2f);
                return;
            }
            // 调用InventoryManager的AddItem方法，直接添加物品到背包
            InventoryManager.Instance.AddItem(item, false);
            SetText("物品已添加到背包！", 2f);
        }

        private void SetText(string message, float duration)
        {
            if (text != null)
            {
                text.text = message;
                // 使用协程来实现文本的渐变显示和消失
                StartCoroutine(FadeText(duration));
            }
        }

        private System.Collections.IEnumerator FadeText(float duration)
        {
            // 等待一段时间后清空文本
            yield return new WaitForSeconds(duration);
            text.text = "";
        }
    }
}