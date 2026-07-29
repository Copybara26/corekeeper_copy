using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookingManager : MonoBehaviour
{
    public static CookingManager Instance { get; private set; }

    [Header("모든 요리 조합 데이터 목록")]
    public List<CookingRecipe> cookingRecipes = new List<CookingRecipe>();

    [Header("조합 실패 시 나올 기본 아이템 (예: 탄 요리)")]
    public ItemData failedFoodItem;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        // Resources/Recipes/Cooking 폴더의 모든 요리 레시피 자동 로드
        CookingRecipe[] loaded = Resources.LoadAll<CookingRecipe>("Recipes/Cooking");
        cookingRecipes.AddRange(loaded);
    }

    // 재료 2개를 넣어서 요리하기
    public ItemData Cook(ItemData item1, ItemData item2)
    {
        if (item1 == null || item2 == null) return null;

        // 1. InventoryManager -> Inventory 로 수정
        Inventory.Instance.RemoveItem(item1, 1);
        Inventory.Instance.RemoveItem(item2, 1);

        foreach (var recipe in cookingRecipes)
        {
            if (recipe.IsMatch(item1, item2))
            {
                Debug.Log($"요리 성공! 결과물: {recipe.resultFood.itemName}");
                // 2. InventoryManager -> Inventory 로 수정
                Inventory.Instance.AddItem(recipe.resultFood, recipe.resultAmount);
                return recipe.resultFood;
            }
        }

        Debug.Log("알 수 없는 조합입니다... 요리에 실패했습니다!");
        // 3. InventoryManager -> Inventory 로 수정
        Inventory.Instance.AddItem(failedFoodItem, 1);
        return failedFoodItem;
    }
}