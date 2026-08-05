using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Crop : MonoBehaviour
{
    [Header("성장")]
    [SerializeField] private Sprite[] growthSprites;
    [SerializeField] private float timePerStage = 3f;

    [Header("수확 아이템")]
    [SerializeField] private ItemData cropItem;
    [SerializeField] private ItemData seedItem;

    [Header("수확 드랍")]
    [SerializeField] private List<DropItem> harvestDrops = new();

    private SpriteRenderer spriteRenderer;
    private FarmPlot farmPlot;

    private int currentStage;
    private bool isFullyGrown;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        StartCoroutine(GrowCoroutine());
    }

    public void SetFarmPlot(FarmPlot plot)
    {
        farmPlot = plot;
    }

    private IEnumerator GrowCoroutine()
    {
        if (growthSprites == null || growthSprites.Length == 0)
        {
            Debug.LogError("성장 스프라이트가 없습니다.");
            yield break;
        }

        currentStage = 0;
        spriteRenderer.sprite = growthSprites[currentStage];

        while (currentStage < growthSprites.Length - 1)
        {
            yield return new WaitForSeconds(timePerStage);

            currentStage++;
            spriteRenderer.sprite = growthSprites[currentStage];

            Debug.Log($"작물 성장 단계: {currentStage}");
        }

        isFullyGrown = true;
        Debug.Log("작물이 다 자랐습니다.");
    }

    //private void OnMouseDown()
    //{
    //    Debug.Log($"[1] 작물 클릭됨: {gameObject.name}");
    //    Debug.Log($"완전 성장: {isFullyGrown}, 수확 완료: {isHarvested}");

    //    TryHarvest();
    //}

    private bool isHarvested = false;

    public void TryHarvest()
    {
        Debug.Log($"[2] TryHarvest 실행: {gameObject.name}");

        if (isHarvested)
        {
            Debug.Log("[중단] 이미 수확 처리된 작물");
            return;
        }

        if (!isFullyGrown)
        {
            Debug.Log("[중단] 아직 다 자라지 않음");
            return;
        }

        if (Inventory.Instance == null)
        {
            Debug.LogError("[중단] Inventory가 없음");
            return;
        }

        isHarvested = true;

        Debug.Log("[3] 드랍 처리 시작");

        DropHarvestItems();

        Debug.Log("[4] 드랍 처리 끝");

        if (farmPlot != null)
        {
            farmPlot.CropHarvested();
            Debug.Log("[5] FarmPlot 수확 처리 완료");
        }
        else
        {
            Debug.LogWarning("[5] FarmPlot 연결 없음");
        }

        Debug.Log("[6] 작물 제거");

        Destroy(gameObject);
    }

    private void DropHarvestItems()
    {
        Debug.Log($"드랍 목록 개수: {harvestDrops.Count}");

        foreach (DropItem drop in harvestDrops)
        {
            if (drop == null)
            {
                Debug.LogWarning("DropItem이 비어 있음");
                continue;
            }

            if (drop.item == null)
            {
                Debug.LogWarning("드랍 아이템이 연결되지 않음");
                continue;
            }

            int randomValue = Random.Range(0, 100);

            Debug.Log(
                $"드랍 검사: {drop.item.itemName}, " +
                $"확률값 {randomValue}, 설정 확률 {drop.chance}"
            );

            if (randomValue >= drop.chance)
            {
                continue;
            }

            int minimum = Mathf.Max(1, drop.minAmount);
            int maximum = Mathf.Max(minimum, drop.maxAmount);
            int amount = Random.Range(minimum, maximum + 1);

            Debug.Log(
                $"AddItem 호출 직전: {drop.item.itemName} {amount}개"
            );

            Inventory.Instance.AddItem(drop.item, amount);

            Debug.Log(
                $"AddItem 호출 완료: {drop.item.itemName} {amount}개"
            );
        }
    }
}