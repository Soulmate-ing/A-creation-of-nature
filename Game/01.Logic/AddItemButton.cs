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
            // Ĭ�ϲ���ʾ�ı�
            if (text != null)
            {
                text.text = "";
            }
        }

        private void AddItemToBag()
        {
            // �����Ʒ�Ƿ��Ѿ��ڱ�����
            if (InventoryManager.Instance.IsItemInBag(item.itemID))
            {
                // �����Ʒ���ڱ����У�������ʾ
                SetText("���������и���Ʒ��", 2f);
                return;
            }
            // ����InventoryManager��AddItem������ֱ�������Ʒ������
            InventoryManager.Instance.AddItem(item, false);
            SetText("��Ʒ����ӵ�������", 2f);
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