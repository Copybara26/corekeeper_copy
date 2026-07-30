using UnityEngine;

[CreateAssetMenu(fileName = "New Cooking Recipe", menuName = "Crafting/Cooking Recipe")]
public class CookingRecipe : ScriptableObject
{
    [Header("필요 재료 (2개 조합)")]
    public ItemData ingredientA; // 첫 번째 재료
    public ItemData ingredientB; // 두 번째 재료

    [Header("결과물 설정")]
    public ItemData resultItem;  // 완성될 요리 아이템
    public int resultAmount = 1;  // 완성 개수

    [Header("요리 설정")]
    public float cookTime = 3f;  // 요리 소요 시간
}