using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMP_Text amountText;
    [SerializeField] private GameObject selectionFrame;

    [Header("툴팁 프리팹 연결")]
    [SerializeField] private InventoryItemTooltip tooltipPrefab; // 👈 툴팁 프리팹 연결

    private int slotIndex;
    private InventorySlotData currentSlotData;

    // 현재 슬롯에서 생성된 툴팁 인스턴스 보관용 static (다른 슬롯에 마우스 가면 이전 것 삭제용)
    private static InventoryItemTooltip activeTooltip;

    public void SetIndex(int index)
    {
        slotIndex = index;
    }

    public void SetSlot(InventorySlotData slotData)
    {
        currentSlotData = slotData;

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
        currentSlotData = null;

        itemIcon.enabled = false;
        itemIcon.sprite = null;
        amountText.text = string.Empty;

        // 슬롯 비워질 때 툴팁도 제거
        DestroyActiveTooltip();
    }

    public void SetSelected(bool selected)
    {
        if (selectionFrame != null)
        {
            selectionFrame.SetActive(selected);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (currentSlotData != null && currentSlotData.item != null)
            {
                Inventory.Instance.UseItem(currentSlotData.item);

                // 아이템을 먹어서 슬롯이 비워졌을 수 있으므로 툴팁 파괴
                if (currentSlotData == null || currentSlotData.item == null)
                {
                    DestroyActiveTooltip();
                }
            }
            return;
        }

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (Inventory.Instance != null)
            {
                Inventory.Instance.SelectSlot(slotIndex);
            }

            if (currentSlotData == null || currentSlotData.item == null) return;

            if (CookingUI.Instance != null &&
                CookingUI.Instance.cookingWindow != null &&
                CookingUI.Instance.cookingWindow.activeSelf)
            {
                CookingUI.Instance.TryAddIngredient(currentSlotData.item);
            }
        }
    }

    // 마우스 진입 시 툴팁 생성
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("마우스 올림!"); // 👈 콘솔에 뜨는지 확인용

        if (currentSlotData != null && currentSlotData.item != null && tooltipPrefab != null)
        {
            DestroyActiveTooltip();

            Canvas parentCanvas = GetComponentInParent<Canvas>();
            Transform canvasTransform = parentCanvas != null ? parentCanvas.transform : transform.parent;

            // 1. 툴팁 생성
            activeTooltip = Instantiate(tooltipPrefab, canvasTransform, false);
            activeTooltip.transform.SetAsLastSibling();

            // 2. 슬롯 위치(transform)와 아이템 데이터 전달
            activeTooltip.SetPositionAndSetup(transform, currentSlotData.item);
        }
    }

    // 마우스 이탈 시 툴팁 파괴
    public void OnPointerExit(PointerEventData eventData)
    {
        DestroyActiveTooltip();
    }

    private void DestroyActiveTooltip()
    {
        if (activeTooltip != null)
        {
            Destroy(activeTooltip.gameObject);
            activeTooltip = null;
        }
    }
}