using System.Collections.Generic;
using UnityEngine;

public class FarmManager : MonoBehaviour
{
    [Header("격자")]
    [SerializeField] private Grid grid;

    [Header("갈린 흙")]
    [SerializeField] private GameObject farmSoilPrefab;

    [Header("플레이어")]
    [SerializeField] private Transform player;
    [SerializeField] private float farmRange = 2f;

    [Header("땅과 장애물 검사")]
    [SerializeField] private LayerMask farmableGroundLayer;
    [SerializeField] private LayerMask ResourceLayer;
    [SerializeField] private float ResourceCheckRadius = 0.35f;

    [SerializeField] private LayerMask farmPlotLayer;
    [SerializeField] private LayerMask interactableLayer;

    private Camera mainCamera;

    // 이미 갈아놓은 위치 저장
    private readonly Dictionary<Vector3Int, GameObject> tilledCells = new();

    public static FarmManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (blockFarmClick)
            {
                Debug.Log("자원 채취 클릭이라 농사 입력 무시");
                return;
            }

            HandleFarmClick();
        }
    }

    private void TryTillSoil()
    {
        if (mainCamera == null ||
            grid == null ||
            player == null ||
            farmSoilPrefab == null)
        {
            Debug.LogWarning("FarmManager 연결 항목을 확인하세요.");
            return;
        }

        Vector3 mouseWorldPosition =
            mainCamera.ScreenToWorldPoint(Input.mousePosition);

        mouseWorldPosition.z = 0f;

        // 마우스 위치를 Grid 칸으로 변환
        Vector3Int cellPosition =
            grid.WorldToCell(mouseWorldPosition);

        Vector3 cellCenter =
            grid.GetCellCenterWorld(cellPosition);

        // 플레이어 근처인지 검사
        if (Vector2.Distance(player.position, cellCenter) > farmRange)
        {
            Debug.Log("너무 멀어서 땅을 갈 수 없습니다.");
            return;
        }

        // 농사 가능한 섬 내부인지 검사
        Collider2D ground = Physics2D.OverlapPoint(
            cellCenter,
            farmableGroundLayer
        );

        if (ground == null)
        {
            Debug.Log("농사 가능한 땅이 아닙니다.");
            return;
        }

        // 이미 갈아놓은 칸인지 검사
        if (tilledCells.ContainsKey(cellPosition))
        {
            Debug.Log("이미 갈아놓은 땅입니다.");
            return;
        }

        // 나무, 돌, 건물 등이 있는지 검사
        Collider2D Resource = Physics2D.OverlapCircle(
            cellCenter,
            ResourceCheckRadius,
            ResourceLayer
        );

        if (Resource != null && !Resource.CompareTag("Player"))
        {
            Debug.Log($"장애물 때문에 경작 불가: {Resource.name}");
            return;
        }

        // 갈린 흙 생성
        GameObject soil = Instantiate(
        farmSoilPrefab, 
        cellCenter,
        Quaternion.identity
        );

        soil.name = $"FarmSoil_{cellPosition.x}_{cellPosition.y}";

        tilledCells.Add(cellPosition, soil);

        Debug.Log($"땅 갈기 완료: {cellPosition}");
    }

    public bool IsTilled(Vector3Int cellPosition)
    {
        return tilledCells.ContainsKey(cellPosition);
    }

    public void RemoveTilledSoilAtWorldPosition(Vector3 worldPosition)
    {
        if (grid == null)
        {
            return;
        }

        Vector3Int cellPosition =
            grid.WorldToCell(worldPosition);

        if (!tilledCells.TryGetValue(
                cellPosition,
                out GameObject soilObject))
        {
            return;
        }

        if (soilObject != null)
        {
            FarmPlot plot = soilObject.GetComponent<FarmPlot>();

            if (plot != null) 
            {
                plot.RemoveCrop();
            }

            Destroy(soilObject);
        }

        tilledCells.Remove(cellPosition);

        Debug.Log($"자원 리젠으로 갈린 땅 제거: {cellPosition}");
    }

    private void HandleFarmClick()
    {
        if (BuildingManager.Instance != null &&
            BuildingManager.Instance.isBuildMode)
        {
            return;
        }

        // 제작 UI가 열려 있으면 농사 클릭 막기
        if (CraftingUI.Instance != null &&
            CraftingUI.Instance.craftingWindow != null &&
            CraftingUI.Instance.craftingWindow.activeSelf)
        {
            return;
        }

        // 요리 UI가 열려 있으면 농사 클릭 막기
        if (CookingUI.Instance != null &&
            CookingUI.Instance.cookingWindow != null &&
            CookingUI.Instance.cookingWindow.activeSelf)
        {
            return;
        }

        Vector3 mousePosition =
            mainCamera.ScreenToWorldPoint(Input.mousePosition);

        mousePosition.z = 0f;

        // ★ 요리대, 제작대 등 상호작용 오브젝트면 농사 처리 안 함
        Collider2D interactable = Physics2D.OverlapPoint(
            mousePosition,
            interactableLayer
        );

        if (interactable != null)
        {
            Debug.Log($"상호작용 오브젝트 클릭: {interactable.name}");
            return;
        }

        Collider2D[] hits =
            Physics2D.OverlapPointAll(mousePosition);

        // 1순위: 작물 수확
        foreach (Collider2D hit in hits)
        {
            Crop crop = hit.GetComponent<Crop>();

            if (crop == null)
            {
                crop = hit.GetComponentInParent<Crop>();
            }

            if (crop != null)
            {
                crop.TryHarvest();
                return;
            }
        }

        // 2순위: 베리 덤불 수확
        foreach (Collider2D hit in hits)
        {
            BerryBush berryBush =
                hit.GetComponent<BerryBush>();

            if (berryBush == null)
            {
                berryBush =
                    hit.GetComponentInParent<BerryBush>();
            }

            if (berryBush != null)
            {
                Debug.Log($"베리 덤불 클릭: {berryBush.name}");

                berryBush.TryHarvest();
                return;
            }
        }

        // 3순위: 갈린 땅에 씨앗 심기
        foreach (Collider2D hit in hits)
        {
            FarmPlot plot =
                hit.GetComponent<FarmPlot>();

            if (plot == null)
            {
                plot =
                    hit.GetComponentInParent<FarmPlot>();
            }

            if (plot != null)
            {
                plot.TryPlant();
                return;
            }
        }

        // 4순위: 새 땅 갈기
        TryTillSoil();
    }

    private bool blockFarmClick;

    public void BlockFarmClickThisFrame()
    {
        blockFarmClick = true;
    }

    private void LateUpdate()
    {
        blockFarmClick = false;
    }
}