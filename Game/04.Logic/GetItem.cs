using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace MyGame.Inventory

{
    public class GetItem : MonoBehaviour
    {
        public Item item;
        private Button button;
        public TextMeshProUGUI text;

        private void Awake()
        {
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
            InventoryManager.Instance.AddItem(item, false);
            SetText("物品已添加到背包！", 2f);

            // 禁用按钮，防止再次点击
            if (button != null)
            {
                button.interactable = false;
            }
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

