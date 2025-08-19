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
            // Ĭ�ϲ���ʾ�ı�
            if (text != null)
            {
                text.text = "";
            }
        }

        private void AddItemToBag()
        {
            InventoryManager.Instance.AddItem(item, false);
            SetText("��Ʒ����ӵ�������", 2f);

            // ���ð�ť����ֹ�ٴε��
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
                // ʹ��Э����ʵ���ı��Ľ�����ʾ����ʧ
                StartCoroutine(FadeText(duration));
            }
        }

        private System.Collections.IEnumerator FadeText(float duration)
        {
            // �ȴ�һ��ʱ�������ı�
            yield return new WaitForSeconds(duration);
            text.text = "";
        }
    }
}

