using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemToolTip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    public void SetuoTooltip(ItemDetails itemDetails, SlotType slotType)
    {
        nameText.text = itemDetails.itemName;
        descriptionText.text = itemDetails.itemDescription;
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    } 
}
