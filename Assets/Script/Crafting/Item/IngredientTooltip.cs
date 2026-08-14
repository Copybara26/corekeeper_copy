using UnityEngine;
using TMPro;

public class IngredientTooltip : MonoBehaviour
{
    [Header("UI 연결")]
    [SerializeField] private TMP_Text ingredientText;
    [SerializeField] private RectTransform rectTransform;

    [Header("위치 미세 조절 (Offset)")]
    [Tooltip("오른쪽/왼쪽 거리 조절 (+값이면 오른쪽, -값이면 왼쪽)")]
    public float offsetX = 10f;

    [Tooltip("위/아래 거리 조절 (+값이면 위쪽, -값이면 아래쪽)")]
    public float offsetY = 0f;

    private void Awake()
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();
    }

    public void ShowAtSlotRight(RectTransform slotRect, string ingredientInfo, RectTransform parentPanel)
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();

        if (ingredientText != null)
        {
            ingredientText.text = string.IsNullOrEmpty(ingredientInfo) ? "재료 정보 없음" : ingredientInfo;
        }

        gameObject.SetActive(true);

        // WindowPanel 상에서 최상단 레이어로 정렬
        rectTransform.SetAsLastSibling();

        if (parentPanel == null) return;

        // 선택된 슬롯의 World 모서리 좌표 구하기
        Vector3[] slotCorners = new Vector3[4];
        slotRect.GetWorldCorners(slotCorners);

        // slotCorners[2] = 우상단, slotCorners[3] = 우하단 -> 오른쪽 중앙 지점
        Vector3 slotRightWorld = (slotCorners[2] + slotCorners[3]) * 0.5f;

        Canvas parentCanvas = GetComponentInParent<Canvas>();
        Camera uiCamera = (parentCanvas != null && parentCanvas.renderMode == RenderMode.ScreenSpaceCamera) ? parentCanvas.worldCamera : null;

        // WindowPanel의 로컬 좌표계 기준으로 슬롯 우측 위치 변환
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parentPanel, RectTransformUtility.WorldToScreenPoint(uiCamera, slotRightWorld), uiCamera, out Vector2 localPoint))
        {
            float tooltipHalfWidth = rectTransform.rect.width * 0.5f;

            // X축: (툴팁 너비 절반) + offsetX 만큼 오른쪽으로 이동
            localPoint.x += (tooltipHalfWidth + offsetX);

            // Y축: offsetY 만큼 위/아래 이동
            localPoint.y += offsetY;

            rectTransform.anchoredPosition = localPoint;
        }
    }
}