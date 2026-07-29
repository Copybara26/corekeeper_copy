using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftingUI : MonoBehaviour
{
    public static CraftingUI Instance { get; private set; }

    [Header("UI 창 및 컨테이너 연결")]
    public GameObject craftingWindow;
    public Transform slotContainer;
    public GameObject slotPrefab;

    [Header("선택된 레시피 정보 창")]
    public Image selectedItemIcon;
    public TextMeshProUGUI selectedItemName;
    public TextMeshProUGUI selectedItemDescription;
    public TextMeshProUGUI ingredientsText;
    public Button craftConfirmButton;

    [Header("닫기 버튼")]
    public Button closeButton;

    [Header("작업대 감지 설정")]
    public PlayerController player; // 플레이어 참조

    private List<CraftingSlotUI> spawnedSlots = new List<CraftingSlotUI>();
    private ItemRecipe selectedRecipe;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (craftConfirmButton != null)
            craftConfirmButton.onClick.AddListener(OnCraftButtonClicked);

        if (closeButton != null)
            closeButton.onClick.AddListener(CloseCraftingUI);

        CloseCraftingUI();
    }

    private void Update()
    {
        // 'C' 키를 누르면 제작창 토글
        if (Input.GetKeyDown(KeyCode.C))
        {
            ToggleCraftingUI();
        }

        // 제작창이 열려있는 동안 작업대 범위를 벗어나면 자동으로 닫음
        if (craftingWindow.activeSelf)
        {
            if (!IsNearCraftingStation())
            {
                Debug.Log("작업대에서 멀어져 제작창이 닫힙니다.");
                CloseCraftingUI();
            }
        }
    }

    /// <summary>
    /// 플레이어 주변 (areaSize 범위 내)에 작업대(CraftingStation)가 있는지 2D로 검사
    /// </summary>
    public bool IsNearCraftingStation()
    {
        // 플레이어가 없으면 자동 검색
        if (player == null)
        {
            player = FindFirstObjectByType<PlayerController>();
            if (player == null) return false;
        }

        // PlayerController의 위치와 areaSize 영역을 가져와 검사
        Bounds checkBounds = new Bounds(player.transform.position, player.areaSize);

        // 범위 내의 모든 2D Collider 탐색
        Collider2D[] hits = Physics2D.OverlapBoxAll(checkBounds.center, checkBounds.size, 0f);
        foreach (var hit in hits)
        {
            if (hit.GetComponent<CraftingStation>() != null)
            {
                return true; // 범위 안에 작업대가 있음!
            }
        }

        return false;
    }

    public void ToggleCraftingUI()
    {
        if (craftingWindow.activeSelf)
        {
            CloseCraftingUI();
        }
        else
        {
            if (IsNearCraftingStation())
            {
                OpenCraftingUI();
            }
            else
            {
                Debug.LogWarning("주변에 작업대가 없어 제작창을 열 수 없습니다!");
            }
        }
    }

    public void OpenCraftingUI()
    {
        craftingWindow.SetActive(true);
        RefreshUI();
    }

    public void CloseCraftingUI()
    {
        craftingWindow.SetActive(false);
    }

    public void RefreshUI()
    {
        foreach (var slot in spawnedSlots)
        {
            if (slot != null) Destroy(slot.gameObject);
        }
        spawnedSlots.Clear();

        foreach (Transform child in slotContainer)
        {
            Destroy(child.gameObject);
        }

        var recipes = CraftingManager.Instance.availableRecipes;
        foreach (var recipe in recipes)
        {
            GameObject slotObj = Instantiate(slotPrefab, slotContainer);
            CraftingSlotUI slotUI = slotObj.GetComponent<CraftingSlotUI>();
            if (slotUI != null)
            {
                slotUI.Setup(recipe);
                spawnedSlots.Add(slotUI);
            }
        }

        if (recipes.Count > 0) SelectRecipe(recipes[0]);
        else ClearSelectedInfo();
    }

    public void SelectRecipe(ItemRecipe recipe)
    {
        selectedRecipe = recipe;

        foreach (var slot in spawnedSlots)
        {
            bool isSelected = (slot.CurrentRecipe == recipe);
            slot.SetSelected(isSelected);
        }

        if (recipe == null || recipe.resultItem == null)
        {
            ClearSelectedInfo();
            return;
        }

        if (selectedItemIcon != null) selectedItemIcon.sprite = recipe.resultItem.icon;
        if (selectedItemName != null) selectedItemName.text = recipe.resultItem.itemName;
        if (selectedItemDescription != null) selectedItemDescription.text = recipe.resultItem.description;

        if (ingredientsText != null)
        {
            string ingText = "필요 재료:\n";
            foreach (var ing in recipe.ingredients)
            {
                int currentCount = Inventory.Instance != null ? Inventory.Instance.GetItemAmount(ing.item) : 0;
                string colorHex = (currentCount >= ing.amount) ? "#00FF00" : "#FF0000";
                ingText += $"<color={colorHex}>- {ing.item.itemName}: {currentCount}/{ing.amount}</color>\n";
            }
            ingredientsText.text = ingText;
        }

        if (craftConfirmButton != null)
        {
            // 재료 충분 + 작업대 근처 조건 만족 시 버튼 활성화
            craftConfirmButton.interactable = CraftingManager.Instance.CanCraft(recipe) && IsNearCraftingStation();
        }
    }

    private void ClearSelectedInfo()
    {
        selectedRecipe = null;
        if (selectedItemIcon != null) selectedItemIcon.sprite = null;
        if (selectedItemName != null) selectedItemName.text = "";
        if (selectedItemDescription != null) selectedItemDescription.text = "";
        if (ingredientsText != null) ingredientsText.text = "";
        if (craftConfirmButton != null) craftConfirmButton.interactable = false;
    }

    private void OnCraftButtonClicked()
    {
        if (selectedRecipe == null) return;

        if (!IsNearCraftingStation())
        {
            Debug.LogWarning("작업대에서 너무 멀어졌습니다!");
            CloseCraftingUI();
            return;
        }

        bool success = CraftingManager.Instance.CraftItem(selectedRecipe);
        if (success)
        {
            foreach (var slot in spawnedSlots)
            {
                slot.UpdateSlotState();
            }
            SelectRecipe(selectedRecipe);
        }
    }
}