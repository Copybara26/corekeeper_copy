using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI; // LayoutRebuilder 사용
using TMPro;

public class CraftingSlotUI : MonoBehaviour
{
    [Header("UI 연결")]
    public Image itemIcon;
    public TextMeshProUGUI itemNameText;
    public Button craftButton;
    public CanvasGroup canvasGroup;
    public GameObject selectionFrame;

    [Header("프리팹 연결")]
    [SerializeField] private IngredientTooltip tooltipPrefab;
    [SerializeField] private GameObject slotSpacerPrefab;

    private ItemRecipe currentRecipe;

    private static IngredientTooltip activeTooltip;
    private static GameObject activeSpacer;
    private static CraftingSlotUI currentlySelectedSlot;

    public ItemRecipe CurrentRecipe => currentRecipe;

    public void Setup(ItemRecipe recipe, IngredientTooltip sharedTooltipPrefab = null, GameObject spacerPrefab = null)
    {
        currentRecipe = recipe;

        if (sharedTooltipPrefab != null) tooltipPrefab = sharedTooltipPrefab;
        if (spacerPrefab != null) slotSpacerPrefab = spacerPrefab;

        if (recipe != null && recipe.resultItem != null)
        {
            if (itemIcon != null) itemIcon.sprite = recipe.resultItem.icon;
            if (itemNameText != null) itemNameText.text = recipe.resultItem.itemName;
        }

        if (craftButton != null)
        {
            craftButton.onClick.RemoveAllListeners();
            craftButton.onClick.AddListener(OnClickSlot);
        }

        SetSelected(false);
        UpdateSlotState();
    }

    public void SetSelected(bool isSelected)
    {
        if (selectionFrame != null)
            selectionFrame.SetActive(isSelected);
    }

    public void UpdateSlotState()
    {
        if (currentRecipe == null) return;
        bool canCraft = CraftingManager.Instance != null && CraftingManager.Instance.CanCraft(currentRecipe);
        if (canvasGroup != null) canvasGroup.alpha = canCraft ? 1.0f : 0.4f;
    }

    private void OnClickSlot()
    {
        if (currentRecipe == null) return;

        if (CraftingUI.Instance != null)
        {
            CraftingUI.Instance.SelectRecipe(currentRecipe);
        }

        // 1. 동일 슬롯 클릭 시 닫기
        if (currentlySelectedSlot == this)
        {
            CloseCurrentTooltipAndSpacer();
            return;
        }

        // 2. 이전 툴팁 및 스페이서 제거
        CloseCurrentTooltipAndSpacer();

        currentlySelectedSlot = this;
        SetSelected(true);

        Transform contentParent = transform.parent;

        // ★ 핵심: 스페이서가 삭제되었으므로, 그리드 레이아웃 위치를 즉시 계산하도록 강제 갱신!
        // 이 코드가 있어야 클릭한 슬롯이 제자리로 돌아온 상태의 정확한 좌표를 가져옵니다.
        RectTransform contentRect = contentParent as RectTransform;
        if (contentRect != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(contentRect);
        }

        // 3. 현재 내 슬롯 인덱스 및 위치 계산
        int myIndex = transform.GetSiblingIndex();
        bool isFourthSlot = ((myIndex + 1) % 4 == 0);

        // 4. 1~3번 슬롯인 경우 스페이서 생성
        if (!isFourthSlot && slotSpacerPrefab != null)
        {
            activeSpacer = Instantiate(slotSpacerPrefab, contentParent);
            activeSpacer.name = "SpacerSlot_Temp";
            activeSpacer.transform.SetSiblingIndex(myIndex + 1);

            // 스페이서가 생성된 후의 레이아웃도 즉시 재계산
            if (contentRect != null)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(contentRect);
            }
        }

        // 5. 툴팁 생성 및 위치 설정
        if (tooltipPrefab != null)
        {
            Transform windowPanel = FindWindowPanel(transform);

            if (windowPanel == null)
            {
                Canvas rootCanvas = GetComponentInParent<Canvas>();
                if (rootCanvas != null) windowPanel = rootCanvas.transform;
            }

            activeTooltip = Instantiate(tooltipPrefab, windowPanel);

            string infoText = "";
            if (currentRecipe.ingredients != null)
            {
                foreach (var ing in currentRecipe.ingredients)
                {
                    if (ing.item != null)
                        infoText += $"{ing.item.itemName} x{ing.amount}\n";
                }
            }

            // 정확해진 내 슬롯 위치를 기준으로 오른쪽 배치
            activeTooltip.ShowAtSlotRight(GetComponent<RectTransform>(), infoText.TrimEnd(), windowPanel as RectTransform);
        }
    }

    private Transform FindWindowPanel(Transform current)
    {
        while (current != null)
        {
            if (current.name == "WindowPanel")
                return current;
            current = current.parent;
        }
        return null;
    }

    private static void CloseCurrentTooltipAndSpacer()
    {
        if (currentlySelectedSlot != null)
        {
            currentlySelectedSlot.SetSelected(false);
            currentlySelectedSlot = null;
        }

        if (activeSpacer != null)
        {
            DestroyImmediate(activeSpacer); // 즉시 삭제 처리
            activeSpacer = null;
        }

        if (activeTooltip != null)
        {
            DestroyImmediate(activeTooltip.gameObject);
            activeTooltip = null;
        }
    }
}