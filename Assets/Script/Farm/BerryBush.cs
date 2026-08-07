using System.Collections;
using UnityEngine;

public class BerryBush : MonoBehaviour
{
    [Header("덤불 스프라이트")]
    [SerializeField] private Sprite emptyBushSprite;
    [SerializeField] private Sprite berryBushSprite;

    [Header("베리 아이템")]
    [SerializeField] private ItemData berryItem;

    [Header("수확 설정")]
    [SerializeField] private int minBerryAmount = 1;
    [SerializeField] private int maxBerryAmount = 3;

    [Header("다시 자라는 시간")]
    [SerializeField] private float regrowTime = 8f;

    [Header("게임 시작 상태")]
    [SerializeField] private bool startWithBerries = true;

    private SpriteRenderer spriteRenderer;
    private bool hasBerries;
    private bool isRegrowing;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogError(
                $"{gameObject.name}: SpriteRenderer를 찾지 못했습니다."
            );
        }
    }

    private void Start()
    {
        hasBerries = startWithBerries;
        UpdateSprite();

        // 빈 덤불로 시작하면 바로 성장 시작
        if (!hasBerries)
        {
            StartCoroutine(RegrowCoroutine());
        }
    }

    //private void OnMouseDown()
    //{
    //    TryHarvest();
    //}

    public void TryHarvest()
    {
        if (!hasBerries || isRegrowing)
        {
            Debug.Log("아직 베리가 자라지 않았습니다.");
            return;
        }

        FarmManager.Instance?.BlockFarmClickThisFrame();

        if (Inventory.Instance == null)
        {
            Debug.LogError("Inventory를 찾지 못했습니다.");
            return;
        }

        if (berryItem == null)
        {
            Debug.LogError(
                $"{gameObject.name}: Berry Item이 연결되지 않았습니다."
            );
            return;
        }

        int minimum = Mathf.Max(1, minBerryAmount);
        int maximum = Mathf.Max(minimum, maxBerryAmount);

        int berryAmount = Random.Range(
            minimum,
            maximum + 1
        );

        Inventory.Instance.AddItem(
            berryItem,
            berryAmount
        );

        Debug.Log(
            $"{berryItem.itemName} {berryAmount}개 획득"
        );

        hasBerries = false;
        UpdateSprite();

        StartCoroutine(RegrowCoroutine());
    }

    private IEnumerator RegrowCoroutine()
    {
        if (isRegrowing)
        {
            yield break;
        }

        isRegrowing = true;

        yield return new WaitForSeconds(regrowTime);

        hasBerries = true;
        isRegrowing = false;

        UpdateSprite();

        Debug.Log($"{gameObject.name}: 베리가 다시 자랐습니다.");
    }

    private void UpdateSprite()
    {
        if (spriteRenderer == null)
        {
            return;
        }

        spriteRenderer.sprite =
            hasBerries
                ? berryBushSprite
                : emptyBushSprite;
    }
}