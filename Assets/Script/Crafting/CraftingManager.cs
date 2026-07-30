using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    public static CraftingManager Instance { get; private set; }

    [Header("레시피 데이터")]
    public System.Collections.Generic.List<ItemRecipe> availableRecipes = new System.Collections.Generic.List<ItemRecipe>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        LoadRecipes();
    }

    private void LoadRecipes()
    {
        ItemRecipe[] loadedRecipes = Resources.LoadAll<ItemRecipe>("Recipes/Items");
        availableRecipes.Clear();
        availableRecipes.AddRange(loadedRecipes);
        Debug.Log($"총 {availableRecipes.Count}개의 레시피를 불러왔습니다.");
    }

    /// <summary>
    /// 인벤토리의 실제 재료 수량을 확인하여 제작 가능 여부를 반환합니다.
    /// </summary>
    public bool CanCraft(ItemRecipe recipe)
    {
        if (recipe == null || recipe.resultItem == null)
            return false;

        if (Inventory.Instance == null)
        {
            Debug.LogWarning("[CraftingManager] Inventory.Instance가 null입니다! 씬에 Inventory 오브젝트가 있는지 확인하세요.");
            return false;
        }

        if (recipe.ingredients != null)
        {
            foreach (var ingredient in recipe.ingredients)
            {
                if (ingredient.item == null) continue;

                int currentAmount = Inventory.Instance.GetItemAmount(ingredient.item);

                // 💡 콘솔창에서 보유 수량과 필요 수량을 확인해보세요!
                // Debug.Log($"재료 검사 - {ingredient.item.itemName}: 보유량({currentAmount}) / 필요량({ingredient.amount})");

                if (currentAmount < ingredient.amount)
                {
                    return false; // 재료 부족
                }
            }
        }

        return true;
    }

    /// <summary>
    /// 실제 인벤토리에서 재료를 차감하고 결과물 아이템을 추가합니다.
    /// </summary>
    public bool CraftItem(ItemRecipe recipe)
    {
        if (recipe == null || recipe.resultItem == null) return false;

        // 1. 제작 가능 여부 최종 검사
        if (!CanCraft(recipe))
        {
            Debug.LogWarning($"<color=#FF0000>[제작 실패]</color> '{recipe.resultItem.itemName}' 제작 실패: 재료가 부족합니다!");
            return false;
        }

        // 2. 인벤토리에서 실제 재료 차감
        if (recipe.ingredients != null)
        {
            foreach (var ingredient in recipe.ingredients)
            {
                if (ingredient.item != null)
                {
                    Inventory.Instance.RemoveItem(ingredient.item, ingredient.amount);
                }
            }
        }

        // 3. 인벤토리에 결과 아이템 추가
        Inventory.Instance.AddItem(recipe.resultItem, recipe.resultAmount);

        // 4. 성공 디버그 로그 출력
        Debug.Log($"<color=#00FF00>[제작 완료]</color> '{recipe.resultItem.itemName}' {recipe.resultAmount}개 획득!");

        return true;
    }
}