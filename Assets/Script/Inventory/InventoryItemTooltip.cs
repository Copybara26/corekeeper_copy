using TMPro;
using UnityEngine;

public class InventoryItemTooltip : MonoBehaviour
{
    [Header("UI 연결")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text extraInfoText;

    [Header("슬롯 기준 위쪽 오프셋")]
    [SerializeField] private Vector2 offset = new Vector2(0f, 60f); // 슬롯 중심 기준 위로 얼마나 띄울지

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // 슬롯의 Transform과 아이템 데이터를 받아서 슬롯 위쪽에 고정
    public void SetPositionAndSetup(Transform slotTransform, ItemData item)
    {
        if (item == null) return;

        // 1. 슬롯의 월드 위치를 그대로 가져와서 오프셋만큼 위로 배치!
        if (rectTransform != null && slotTransform != null)
        {
            rectTransform.position = slotTransform.position + (Vector3)offset;

            // Z축 0 고정 및 프리팹 회전(Z:90) 유지
            rectTransform.anchoredPosition3D = new Vector3(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y, 0f);
        }

        // 2. 텍스트 정보 세팅
        if (nameText != null) nameText.text = item.itemName;
        if (descriptionText != null) descriptionText.text = item.description;

        if (extraInfoText != null)
        {
            if (item.isEdible)
            {
                extraInfoText.gameObject.SetActive(true);
                extraInfoText.text = $"체력 회복: +{item.healAmount}";
            }
            else
            {
                extraInfoText.gameObject.SetActive(false);
            }
        }
    }
}