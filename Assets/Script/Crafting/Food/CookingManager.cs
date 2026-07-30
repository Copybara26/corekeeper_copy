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
            // 💡 ingredients 대신 ingredientA, ingredientB 체크로 변경!
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

    public CookingRecipe GetRecipe(string item1, string item2)
    {
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