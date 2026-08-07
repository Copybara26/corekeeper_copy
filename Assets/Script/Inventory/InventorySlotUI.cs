using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlotUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMP_Text amountText;
    [SerializeField] private GameObject selectionFrame;

    // 이 UI가 몇 번째 슬롯인지 저장
    private int slotIndex;

    // [요리/상호작용 기능 추가] 현재 슬롯의 데이터 기억용 변수
    private InventorySlotData currentSlotData;

    public void SetIndex(int index)
    {
        slotIndex = index;
    }

    public void SetSlot(InventorySlotData slotData)
    {
        currentSlotData = slotData; // 👈 [요리/상호작용 기능 추가]

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
        currentSlotData = null; // 👈 [요리/상호작용 기능 추가]

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

    // [요리/상호작용 기능 추가] 슬롯 클릭 시 요리창으로 아이템 전달
    public void OnPointerClick(PointerEventData eventData)
    {
        // 마우스 왼쪽 클릭만 처리
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }

        // 클릭한 인벤토리 슬롯 선택
        if (Inventory.Instance != null)
        {
            Inventory.Instance.SelectSlot(slotIndex);
        }

        if (currentSlotData == null || currentSlotData.item == null)
        {
            return;
        }

        if (CookingUI.Instance != null &&
            CookingUI.Instance.cookingWindow != null &&
            CookingUI.Instance.cookingWindow.activeSelf)
        {
            CookingUI.Instance.TryAddIngredient(
                currentSlotData.item
            );
        }
    }
}