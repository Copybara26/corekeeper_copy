using UnityEngine;

[CreateAssetMenu(fileName = "New Cooking Recipe", menuName = "Crafting/Cooking Recipe")]
public class CookingRecipe : ScriptableObject
{
    [Header("조합할 재료 2개")]
    public ItemData ingredientA;
    public ItemData ingredientB;

    [Header("조합 결과물")]
    public ItemData resultFood;
    public int resultAmount = 1;

    // A+B 순서가 바뀌어도 같은 조합인지 체크해주는 함수
    public bool IsMatch(ItemData item1, ItemData item2)
    {
        bool matchNormal = (ingredientA == item1 && ingredientB == item2);
        bool matchReverse = (ingredientA == item2 && ingredientB == item1);
        return matchNormal || matchReverse;
    }
}