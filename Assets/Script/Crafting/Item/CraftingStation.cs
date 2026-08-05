using UnityEngine;

public class CraftingStation : MonoBehaviour
{
    private void OnMouseDown()
    {
        // 1. 요리창이 켜져 있으면 작업대 클릭 금지
        if (CookingUI.Instance != null && CookingUI.Instance.cookingWindow != null && CookingUI.Instance.cookingWindow.activeSelf)
        {
            return;
        }

        // 2. 이미 제작창이 켜져 있으면 작업대 클릭 금지
        if (CraftingUI.Instance != null && CraftingUI.Instance.craftingWindow != null && CraftingUI.Instance.craftingWindow.activeSelf)
        {
            return;
        }

        // 3. 제작창 토글
        if (CraftingUI.Instance != null)
        {
            CraftingUI.Instance.ToggleCraftingUI();
        }
    }
}