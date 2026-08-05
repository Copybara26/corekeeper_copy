using System.Collections;
using UnityEngine;

public class CookingPot : MonoBehaviour
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

    private void OnMouseDown()
    {
        if (CraftingUI.Instance != null && CraftingUI.Instance.craftingWindow != null && CraftingUI.Instance.craftingWindow.activeSelf)
            return;

        if (CookingUI.Instance != null && CookingUI.Instance.cookingWindow != null && CookingUI.Instance.cookingWindow.activeSelf)
            return;

        if (isCooking)
        {
            Debug.Log("요리가 진행 중입니다...");
            return;
        }

        if (hasFinishedFood)
        {
            if (CookingUI.Instance != null && CookingUI.Instance.IsNearCookingPot())
            {
                CollectFood();
            }
            return;
        }

        if (CookingUI.Instance != null && CookingUI.Instance.IsNearCookingPot())
        {
            CookingUI.Instance.OpenCookingUI(this);
        }
    }

    public void StartCooking(CookingRecipe recipe)
    {
        // 💡 recipe가 null인 경우 사전 차단
        if (recipe == null || recipe.resultItem == null)
        {
            Debug.LogError("시작하려는 레시피 또는 완성 아이템(resultItem) 정보가 null입니다.");
            return;
        }

        StartCoroutine(CookCoroutine(recipe));
    }

    private IEnumerator CookCoroutine(CookingRecipe recipe)
    {
        isCooking = true;
        currentRecipe = recipe;

        if (CookingUI.Instance != null)
            CookingUI.Instance.CloseCookingUI();

        Debug.Log("요리를 시작합니다...");

        float duration = recipe.cookTime > 0 ? recipe.cookTime : 3f;
        yield return new WaitForSeconds(duration);

        isCooking = false;
        hasFinishedFood = true;

        // 💡 요리 완료 시 위에 아이콘 표시 (Null 체크 강화)
        if (currentRecipe != null && currentRecipe.resultItem != null && resultSpriteRenderer != null)
        {
            resultSpriteRenderer.sprite = currentRecipe.resultItem.icon;
            resultSpriteRenderer.gameObject.SetActive(true);
        }

        Debug.Log($"요리 완료! 요리솥 위에 {recipe.resultItem.itemName} 생성됨.");
    }

    private void CollectFood()
    {
        if (currentRecipe != null && currentRecipe.resultItem != null)
        {
            if (Inventory.Instance != null)
            {
                Inventory.Instance.AddItem(currentRecipe.resultItem, currentRecipe.resultAmount);
                Debug.Log($"[획득] {currentRecipe.resultItem.itemName} x{currentRecipe.resultAmount}");
            }
        }

        hasFinishedFood = false;
        currentRecipe = null;

        if (resultSpriteRenderer != null)
        {
            resultSpriteRenderer.gameObject.SetActive(false);
        }
    }
}