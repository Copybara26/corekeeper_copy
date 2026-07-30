using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftingSlotUI : MonoBehaviour
{
    [Header("UI 연결")]
    public Image itemIcon;                 // 슬롯 내 아이템 아이콘 이미지
    public TextMeshProUGUI itemNameText;  // 슬롯 내 아이템 이름 텍스트 (선택 사항)
    public Button craftButton;            // 슬롯 클릭 이벤트를 받을 버튼
    public CanvasGroup canvasGroup;       // 제작 가능 여부에 따른 투명도 조절용
    public GameObject selectionFrame;     // 선택 강조 테두리/배경 오브젝트

    private ItemRecipe currentRecipe;     // 현재 슬롯이 들고 있는 레시피 정보

    // 외부(CraftingUI 등)에서 현재 슬롯의 레시피를 읽을 수 있는 프로퍼티
    public ItemRecipe CurrentRecipe => currentRecipe;

    /// <summary>
    /// 슬롯 생성 시 레시피 데이터를 받아 UI를 초기화합니다.
    /// </summary>
    public void Setup(ItemRecipe recipe)
    {
        currentRecipe = recipe;

        // 레시피 및 결과물 정보 연결
        if (recipe != null && recipe.resultItem != null)
        {
            if (itemIcon != null) itemIcon.sprite = recipe.resultItem.icon;
            if (itemNameText != null) itemNameText.text = recipe.resultItem.itemName;
        }

        // 버튼 클릭 이벤트 리스너 초기화 및 등록
        if (craftButton != null)
        {
            craftButton.onClick.RemoveAllListeners();
            craftButton.onClick.AddListener(OnClickSlot);
        }

        // 초기 상태 설정
        SetSelected(false);
        UpdateSlotState();
    }

    /// <summary>
    /// 슬롯의 선택 상태(테두리 활성화 등)를 제어합니다.
    /// </summary>
    public void SetSelected(bool isSelected)
    {
        if (selectionFrame != null)
        {
            selectionFrame.SetActive(isSelected);
        }
    }

    /// <summary>
    /// 인벤토리의 재료 수량을 확인하여 슬롯의 비주얼(투명도) 상태를 갱신합니다.
    /// </summary>
    public void UpdateSlotState()
    {
        if (currentRecipe == null) return;

        // 제작 가능 여부 체크
        bool canCraft = CraftingManager.Instance.CanCraft(currentRecipe);

        // 재료가 부족하더라도 레시피를 선택해 상세정보는 볼 수 있어야 하므로,
        // 버튼을 끄기보다는 투명도(Alpha)를 조절하여 비활성화 느낌을 줍니다.
        if (canvasGroup != null)
        {
            canvasGroup.alpha = canCraft ? 1.0f : 0.4f; // 제작 불가 시 40% 투명
        }
    }

    /// <summary>
    /// 슬롯 클릭 시 호출되는 함수입니다.
    /// </summary>
    private void OnClickSlot()
    {
        if (currentRecipe != null)
        {
            // 메인 UI에 자신(선택된 레시피)의 정보를 표시하도록 요청
            CraftingUI.Instance.SelectRecipe(currentRecipe);
        }
    }
}