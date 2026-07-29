using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 재료 정보를 담는 구조체
[System.Serializable]
public struct Ingredient
{
    public ItemData item; // 필요한 재료 아이템
    public int amount;    // 필요한 개수
}

[CreateAssetMenu(fileName = "New Recipe", menuName = "Crafting/Recipe")]
public class ItemRecipe : ScriptableObject
{
    [Header("결과물 설정")]
    public ItemData resultItem; // 제작될 아이템
    public int resultAmount = 1; // 제작될 개수

    [Header("필요 재료 목록")]
    public List<Ingredient> ingredients = new List<Ingredient>();
}