using System.Collections.Generic;
using UnityEngine;

public class CookingManager : MonoBehaviour
{
    public static CookingManager Instance { get; private set; }

    private Dictionary<string, CookingRecipe> recipeDictionary = new Dictionary<string, CookingRecipe>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        LoadAllFoodRecipes();
    }

    private void LoadAllFoodRecipes()
    {
        recipeDictionary.Clear();

        // Resources/Recipes/Foods 폴더 안의 모든 CookingRecipe 에셋 로드
        CookingRecipe[] loadedRecipes = Resources.LoadAll<CookingRecipe>("Recipes/Foods");

        if (loadedRecipes.Length == 0)
        {
            Debug.LogError("Resources/Recipes/Foods 폴더에서 요리 레시피를 찾지 못했습니다!");
            return;
        }

        foreach (var recipe in loadedRecipes)
        {
            if (recipe.ingredientA == null || recipe.ingredientB == null)
            {
                Debug.LogWarning($"[{recipe.name}] 레시피의 재료 아이템이 비어있어서 로드에서 제외되었습니다.");
                continue;
            }

            string item1 = recipe.ingredientA.itemName;
            string item2 = recipe.ingredientB.itemName;

            string key = GetRecipeKey(item1, item2);

            if (!recipeDictionary.ContainsKey(key))
            {
                recipeDictionary.Add(key, recipe);
            }
        }

        Debug.Log($"총 {recipeDictionary.Count}개의 요리 레시피가 성공적으로 로드되었습니다.");
    }

    // 💡 [추가] ItemData 오브젝트 자체를 넘겨받는 GetRecipe 함수
    public CookingRecipe GetRecipe(ItemData item1, ItemData item2)
    {
        if (item1 == null || item2 == null) return null;

        // ItemData 내부의 itemName을 가져와 기존 GetRecipe(string, string) 메서드로 전달
        return GetRecipe(item1.itemName, item2.itemName);
    }

    // 기존 문자열 기반 GetRecipe 함수
    public CookingRecipe GetRecipe(string item1, string item2)
    {
        if (string.IsNullOrEmpty(item1) || string.IsNullOrEmpty(item2)) return null;

        string key = GetRecipeKey(item1, item2);

        if (recipeDictionary.TryGetValue(key, out CookingRecipe recipe))
        {
            return recipe;
        }

        return null;
    }

    private string GetRecipeKey(string item1, string item2)
    {
        return string.Compare(item1, item2) < 0 ? $"{item1}_{item2}" : $"{item2}_{item1}";
    }
}