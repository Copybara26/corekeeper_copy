using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HealthUI : MonoBehaviour
{
    public static HealthUI Instance;

    [Header("Health Settings")]
    public int maxHealth = 10;       // 총 체력 (하트 5개 기준)
    public int currentHealth = 6;   // 현재 체력

    [Header("UI References")]
    public Transform heartContainer; // 하트 Image들이 들어갈 UI 부모(Grid/Horizontal Layout)
    public GameObject heartPrefab;   // 하트 Prefab (Image 컴포넌트 포함)

    [Header("Heart Sprites")]
    public Sprite fullHeart;  // 채워진 하트
    public Sprite halfHeart;  // 반 칸 하트
    public Sprite emptyHeart; // 빈 하트

    private List<Image> heartImages = new List<Image>();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        InitHearts();
        UpdateUI();
    }

    // 하트 개수만큼 UI 생성
    void InitHearts()
    {
        foreach (Transform child in heartContainer)
            Destroy(child.gameObject);

        heartImages.Clear();

        int totalHeartCount = maxHealth / 2;
        for (int i = 0; i < totalHeartCount; i++)
        {
            GameObject newHeart = Instantiate(heartPrefab, heartContainer);
            heartImages.Add(newHeart.GetComponent<Image>());
        }
    }

    // 체력 변경 시 UI 갱신
    public void UpdateUI()
    {
        for (int i = 0; i < heartImages.Count; i++)
        {
            int heartHealth = currentHealth - (i * 2);

            if (heartHealth >= 2)
                heartImages[i].sprite = fullHeart;
            else if (heartHealth == 1)
                heartImages[i].sprite = halfHeart;
            else
                heartImages[i].sprite = emptyHeart;
        }
    }

    // 체력 회복 함수
    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        UpdateUI();
    }
}