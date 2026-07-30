using UnityEngine;

public class CraftingStation : MonoBehaviour
{
    private void OnMouseDown()
    {
        if (CraftingUI.Instance == null) return;

        // 💡 1. 제작창이 이미 켜져 있다면 작업대 클릭 이벤트를 무시 (창 닫힘 방지)
        if (CraftingUI.Instance.craftingWindow != null && CraftingUI.Instance.craftingWindow.activeSelf)
        {
            return;
        }

        // 💡 2. 작업대 가까이 있을 때만 제작창 열기
        if (CraftingUI.Instance.IsNearCraftingStation())
        {
            CraftingUI.Instance.OpenCraftingUI(); // Toggle 대신 Open으로 명확히 지정
        }
        else
        {
            Debug.LogWarning("작업대와 거리가 너무 멀어 열 수 없습니다!");
        }
    }
}