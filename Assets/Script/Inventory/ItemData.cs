using UnityEngine;

[CreateAssetMenu(
    fileName = "NewItem",
    menuName = "Inventory/Item Data"
)]
public class ItemData : ScriptableObject
{
    [Header("아이템 정보")]
    public string itemName;

    [TextArea]
    public string description;

    public Sprite icon;

    [Header("음식/회복 설정")]
    public bool isEdible = false; // 먹을 수 있는 아이템인지 체크 (식재료, 요리 등)
    public int healAmount = 1;    // 회복량 (1 = 하트 반 칸, 2 = 하트 한 칸)
}