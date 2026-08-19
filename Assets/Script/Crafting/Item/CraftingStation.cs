using UnityEngine;
using UnityEngine.EventSystems; // 필수 추가

public class CraftingStation : MonoBehaviour, IPointerClickHandler
{
    // OnMouseDown 대신 OnPointerClick 사용
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("[CraftingStation] 클릭 감지됨!");

        if (BuildingManager.Instance != null &&
            BuildingManager.Instance.isBuildMode)
        {
            return;
        }

        if (CookingUI.Instance != null && CookingUI.Instance.cookingWindow != null && CookingUI.Instance.cookingWindow.activeSelf)
            return;

        if (CraftingUI.Instance != null && CraftingUI.Instance.craftingWindow != null && CraftingUI.Instance.craftingWindow.activeSelf)
            return;

        if (CraftingUI.Instance != null)
        {
            CraftingUI.Instance.ToggleCraftingUI();
        }
    }
}