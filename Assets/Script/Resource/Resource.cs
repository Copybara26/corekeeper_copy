using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    [Header("자원 체력")]
    [SerializeField] private int maxHp = 3;
    private int currentHp;

    [Header("드랍 아이템")]
    [SerializeField] private List<DropItem> drops = new();

    [Header("리젠")]
    [SerializeField] private float respawnTime = 8f;

    [Header("땅 판정 위치")]
    [SerializeField] private Transform groundPoint;

    private ResourceEffect resourceEffect;
    private SpriteRenderer spriteRenderer;
    private Collider2D resourceCollider;

    private bool isBroken;

    private void Awake()
    {
        currentHp = maxHp;

        resourceEffect = GetComponent<ResourceEffect>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        resourceCollider = GetComponent<Collider2D>();

        if (spriteRenderer == null)
        {
            Debug.LogError(
                $"{gameObject.name}: SpriteRenderer를 찾지 못했습니다."
            );
        }

        if (resourceCollider == null)
        {
            Debug.LogError(
                $"{gameObject.name}: Collider2D를 찾지 못했습니다."
            );
        }
    }

    public void TakeDamage(int damage)
    {
        if (isBroken || damage <= 0)
        {
            return;
        }

        // 자원 타격에 사용된 클릭으로 땅까지 갈리지 않게 함
        if (FarmManager.Instance != null)
        {
            FarmManager.Instance.BlockFarmClickThisFrame();
        }

        currentHp -= damage;

        Debug.Log(
            $"{gameObject.name} 타격! " +
            $"남은 HP: {currentHp}/{maxHp}"
        );

        resourceEffect?.PlayHitEffect();

        if (currentHp <= 0)
        {
            DestroyResource();
        }
    }

    private void DestroyResource()
    {
        if (isBroken)
        {
            return;
        }

        isBroken = true;

        // 파괴된 자원을 더 이상 공격하지 못하게 함
        if (resourceCollider != null)
        {
            resourceCollider.enabled = false;
        }

        DropItems();

        resourceEffect?.PlayDestroyEffect();

        StartCoroutine(RespawnCoroutine());
    }

    private void DropItems()
    {
        if (Inventory.Instance == null)
        {
            Debug.LogWarning(
                "Inventory.Instance를 찾을 수 없습니다."
            );
            return;
        }

        foreach (DropItem drop in drops)
        {
            if (drop == null || drop.item == null)
            {
                continue;
            }

            int randomValue = Random.Range(0, 100);

            if (randomValue >= drop.chance)
            {
                continue;
            }

            int minimum = Mathf.Max(1, drop.minAmount);
            int maximum = Mathf.Max(minimum, drop.maxAmount);

            int amount = Random.Range(
                minimum,
                maximum + 1
            );

            Inventory.Instance.AddItem(
                drop.item,
                amount
            );

            Debug.Log(
                $"{drop.item.itemName} {amount}개 획득"
            );
        }
    }

    private IEnumerator RespawnCoroutine()
    {
        // 파괴 축소 효과가 끝날 때까지 기다림
        if (resourceEffect != null)
        {
            yield return new WaitForSeconds(
                resourceEffect.DestroyDuration
            );
        }

        // 자원을 완전히 숨김
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }

        yield return new WaitForSeconds(respawnTime);

        currentHp = maxHp;

        resourceEffect?.ResetEffect();

        // 나무가 다시 생길 위치의 갈린 땅 제거
        if (FarmManager.Instance != null)
        {
            Vector3 groundPosition;

            if (groundPoint != null)
            {
                groundPosition = groundPoint.position;
            }
            else if (resourceCollider != null)
            {
                groundPosition = resourceCollider.bounds.center;
            }
            else
            {
                groundPosition = transform.position;
            }

            FarmManager.Instance.RemoveTilledSoilAtWorldPosition(
                groundPosition
            );
        }

        // 자원을 다시 표시
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
        }

        // 다시 공격 및 장애물 감지가 가능하게 함
        if (resourceCollider != null)
        {
            resourceCollider.enabled = true;
        }

        isBroken = false;

        Debug.Log($"{gameObject.name} 리젠 완료");
    }
}