using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems; // 필수 추가

public class CookingPot : MonoBehaviour, IPointerClickHandler
{
    [Header("현재 요리 상태")]
    public bool isCooking = false;

    [Header("완성품 표시용 (요리솥 자식 오브젝트의 SpriteRenderer)")]
    [SerializeField] private SpriteRenderer resultSpriteRenderer;

    private CookingRecipe currentRecipe;
    private bool hasFinishedFood = false;

    private void Start()
    {
        if (resultSpriteRenderer != null)
            resultSpriteRenderer.gameObject.SetActive(false);
    }

    // OnMouseDown 대신 OnPointerClick 사용
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("[CookingPot] 클릭 감지됨!");

        if (BuildingManager.Instance != null &&
            BuildingManager.Instance.isBuildMode)
        {
            return;
        }

        if (CraftingUI.Instance != null && CraftingUI.Instance.craftingWindow != null && CraftingUI.Instance.craftingWindow.activeSelf)
            return;

        if (CookingUI.Instance != null && CookingUI.Instance.cookingWindow != null && CookingUI.Instance.cookingWindow.activeSelf)
            return;

        if (isCooking)
        {
            Debug.Log("요리가 진행 중입니다...");
            return;
        }

        if (CookingUI.Instance != null)
        {
            bool isNear = CookingUI.Instance.IsNearCookingPot();

            if (hasFinishedFood)
            {
                if (isNear) CollectFood();
                return;
            }

            if (isNear)
            {
                CookingUI.Instance.OpenCookingUI(this);
            }
        }
    }

    public void StartCooking(CookingRecipe recipe)
    {
        if (recipe == null || recipe.resultItem == null) return;
        StartCoroutine(CookCoroutine(recipe));
    }

    private IEnumerator CookCoroutine(CookingRecipe recipe)
    {
        isCooking = true;
        currentRecipe = recipe;

        if (CookingUI.Instance != null)
            CookingUI.Instance.CloseCookingUI();

        float duration = recipe.cookTime > 0 ? recipe.cookTime : 3f;
        yield return new WaitForSeconds(duration);

        isCooking = false;
        hasFinishedFood = true;

        if (currentRecipe != null && currentRecipe.resultItem != null && resultSpriteRenderer != null)
        {
            resultSpriteRenderer.sprite = currentRecipe.resultItem.icon;
            resultSpriteRenderer.gameObject.SetActive(true);
        }
    }

    private void CollectFood()
    {
        if (currentRecipe == null || currentRecipe.resultItem == null)
            return;

        if (Inventory.Instance == null)
            return;

        bool added = Inventory.Instance.AddItem(
            currentRecipe.resultItem,
            currentRecipe.resultAmount
        );

        // 인벤토리가 꽉 찼으면 음식 유지
        if (!added)
        {
            Debug.Log("인벤토리가 가득 차서 요리를 가져갈 수 없습니다.");
            return;
        }

        // 여기까지 왔다는 건 정상적으로 인벤토리에 들어간 것
        hasFinishedFood = false;
        currentRecipe = null;

        if (resultSpriteRenderer != null)
        {
            resultSpriteRenderer.gameObject.SetActive(false);
        }
    }
}