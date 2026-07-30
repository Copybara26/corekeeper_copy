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
    public PlayerController player;

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

        // 시작 시 제작창을 닫으면서 플레이어 이동을 풀어줍니다!
        CloseCraftingUI();
    }

    private void Update()
    {
        if (craftingWindow.activeSelf)
        {
            if (!IsNearCraftingStation())
            {
                Debug.Log("작업대에서 멀어져 제작창이 닫힙니다.");
                CloseCraftingUI();
            }
        }
    }

    public bool IsNearCraftingStation()
    {
        if (player == null)
        {
            player = FindFirstObjectByType<PlayerController>();
            if (player == null) return false;
        }

        Vector2 center = player.transform.position;
        Vector2 size = player.areaSize;

        Collider2D[] hits = Physics2D.OverlapBoxAll(center, size, 0f);
        foreach (var hit in hits)
        {
            if (hit.GetComponent<CraftingStation>() != null || hit.name.Contains("CraftingStation"))
            {
                return true;
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

        if (player == null) player = FindFirstObjectByType<PlayerController>();
        if (player != null) player.canMove = false; // 제작창 열림 -> 이동 금지

        RefreshUI();
    }

    public void CloseCraftingUI()
    {
        craftingWindow.SetActive(false);

        if (player == null) player = FindFirstObjectByType<PlayerController>();
        if (player != null) player.canMove = true; // 제작창 닫힘 -> 이동 허용
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

        bool isNear = IsNearCraftingStation();
        bool canCraft = CraftingManager.Instance.CanCraft(recipe);

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

        // 제작 불가능(재료 부족 또는 작업대 x) 시 버튼 숨김
        bool shouldShowButton = canCraft && isNear;

        if (craftConfirmButton != null)
        {
            craftConfirmButton.gameObject.SetActive(shouldShowButton);
            craftConfirmButton.interactable = shouldShowButton;
        }

        if (!shouldShowButton)
        {
            Debug.Log($"[제작 버튼 숨김] 레시피: {recipe.resultItem.itemName} | 작업대 근처? {isNear} | 재료 충분? {canCraft}");
        }
    }

    private void ClearSelectedInfo()
    {
        selectedRecipe = null;
        if (selectedItemIcon != null) selectedItemIcon.sprite = null;
        if (selectedItemName != null) selectedItemName.text = "";
        if (selectedItemDescription != null) selectedItemDescription.text = "";
        if (ingredientsText != null) ingredientsText.text = "";

        if (craftConfirmButton != null)
        {
            craftConfirmButton.gameObject.SetActive(false);
        }
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