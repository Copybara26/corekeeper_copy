using UnityEngine;
using UnityEngine.UI;

public class CookingUI : MonoBehaviour
{
    public static CookingUI Instance { get; private set; }

    [Header("UI 윈도우")]
    public GameObject cookingWindow;

    [Header("요리 슬롯 UI Image (Icon1, Icon2 할당)")]
    [SerializeField] private Image slot1Icon;
    [SerializeField] private Image slot2Icon;

    [Header("현재 등록된 재료 데이터")]
    [SerializeField] private ItemData ingredient1;
    [SerializeField] private ItemData ingredient2;

    private CookingPot currentPot;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // 인벤토리에서 재료 선택 시 요리 슬롯으로 이동 (인벤토리 -1)
    public void TryAddIngredient(ItemData item)
    {
        if (item == null) return;

        // Slot 1이 비어있는 경우
        if (ingredient1 == null)
        {
            if (Inventory.Instance != null && Inventory.Instance.RemoveItem(item, 1))
            {
                ingredient1 = item;
                UpdateSlotUI();
            }
            return;
        }

        // Slot 2가 비어있는 경우
        if (ingredient2 == null)
        {
            if (Inventory.Instance != null && Inventory.Instance.RemoveItem(item, 1))
            {
                ingredient2 = item;
                UpdateSlotUI();
            }
            return;
        }

        Debug.Log("요리 슬롯이 가득 찼습니다.");
    }

    // 💡 [신규] 요리 슬롯 1 클릭 시 인벤토리로 반환
    public void RemoveIngredient1()
    {
        if (ingredient1 == null) return;

        if (Inventory.Instance != null)
        {
            Inventory.Instance.AddItem(ingredient1, 1); // 인벤토리에 다시 1개 추가
        }

        ingredient1 = null; // 슬롯 데이터 비우기
        UpdateSlotUI(); // UI 갱신
    }

    // 💡 [신규] 요리 슬롯 2 클릭 시 인벤토리로 반환
    public void RemoveIngredient2()
    {
        if (ingredient2 == null) return;

        if (Inventory.Instance != null)
        {
            Inventory.Instance.AddItem(ingredient2, 1); // 인벤토리에 다시 1개 추가
        }

        ingredient2 = null; // 슬롯 데이터 비우기
        UpdateSlotUI(); // UI 갱신
    }

    // 화면의 UI 아이콘 갱신 함수
    private void UpdateSlotUI()
    {
        // Slot 1 갱신
        if (ingredient1 != null && ingredient1.icon != null)
        {
            slot1Icon.sprite = ingredient1.icon;
            slot1Icon.color = Color.white;
            slot1Icon.enabled = true;
        }
        else
        {
            slot1Icon.sprite = null;
            slot1Icon.color = new Color(1f, 1f, 1f, 0f);
        }

        // Slot 2 갱신
        if (ingredient2 != null && ingredient2.icon != null)
        {
            slot2Icon.sprite = ingredient2.icon;
            slot2Icon.color = Color.white;
            slot2Icon.enabled = true;
        }
        else
        {
            slot2Icon.sprite = null;
            slot2Icon.color = new Color(1f, 1f, 1f, 0f);
        }
    }

    // Cook 버튼 클릭 시
    public void OnClickCookButton()
    {
        if (currentPot == null) return;

        CookingRecipe matchedRecipe = CookingManager.Instance != null
            ? CookingManager.Instance.GetRecipe(ingredient1, ingredient2)
            : null;

        if (matchedRecipe == null || matchedRecipe.resultItem == null)
        {
            Debug.LogWarning("올바른 요리 조합이 아니거나 결과물이 없습니다.");
            return;
        }

        // 요리 시작 시 슬롯 데이터 초기화 (이미 인벤토리에서 차감된 상태이므로 반환 없이 비움만 수행)
        ingredient1 = null;
        ingredient2 = null;
        UpdateSlotUI();

        currentPot.StartCooking(matchedRecipe);
    }

    // Close 버튼 클릭 시 (창을 닫을 때 등록해둔 재료 모두 반환)
    public void OnClickCloseButton()
    {
        RemoveIngredient1();
        RemoveIngredient2();
        CloseCookingUI();
    }

    public void OpenCookingUI(CookingPot pot)
    {
        currentPot = pot;

        if (cookingWindow != null)
            cookingWindow.SetActive(true);

        ClearSlots();
    }

    public void CloseCookingUI()
    {
        if (cookingWindow != null)
            cookingWindow.SetActive(false);
    }

    public void ClearSlots()
    {
        ingredient1 = null;
        ingredient2 = null;
        UpdateSlotUI();
    }

    public bool IsNearCookingPot() => true;
}