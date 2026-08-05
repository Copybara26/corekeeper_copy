using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMP_Text amountText;
    [SerializeField] private GameObject selectionFrame;

    public void SetSlot(InventorySlotData slotData)
    {
        if (slotData == null || slotData.item == null)
        {
            ClearSlot();
            return;
        }

        itemIcon.enabled = true;
        itemIcon.sprite = slotData.item.icon;

        amountText.text = slotData.amount.ToString();
    }

    public void ClearSlot()
    {
        itemIcon.enabled = false;
        itemIcon.sprite = null;

        amountText.text = string.Empty;
    }

    public void SetSelected(bool selected)
    {
        if (selectionFrame != null)
        {
            selectionFrame.SetActive(selected);
        }
    }
}