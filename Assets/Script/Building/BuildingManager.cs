using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance;

    [Header("설치할 건물")]
    [SerializeField] private GameObject chickenHousePrefab;
    [SerializeField] private LayerMask ResourceLayer;

    [Header("설치 가능 영역 검사")]
    [SerializeField] private Vector2 placementCheckOffset = new Vector2(0f, -0.5f);
    [SerializeField] private Vector2 placementCheckSize = new Vector2(2f, 1f);

    private GameObject previewObject;
    private SpriteRenderer previewRenderer;

    [Header("건축 모드 UI")]
    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private GameObject buildingInventoryUI;

    [SerializeField] private BuildingInventoryUI buildingInventoryUIController;

    public bool isBuildMode;
    private bool canPlace = true;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        inventoryUI.SetActive(true);
        buildingInventoryUI.SetActive(false);

        if (buildingInventoryUIController != null)
        {
            buildingInventoryUIController.OnSelectionChanged += RefreshPreview;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (isBuildMode)
            {
                ExitBuildMode();
            }
            else
            {
                EnterBuildMode();
            }
        }

        if (!isBuildMode || previewObject == null)
            return;

        FollowMouse();
        CheckCanPlace();
        UpdatePreviewColor();

        if (Input.GetMouseButtonDown(0) && canPlace)
        {
            PlaceBuilding();
            RefreshPreview();
        }
    }

    private void ToggleBuildMode()
    {
        if (isBuildMode)
        {
            ExitBuildMode();
            return;
        }

        isBuildMode = true;

        previewObject = Instantiate(chickenHousePrefab);
        previewRenderer = previewObject.GetComponent<SpriteRenderer>();

        Collider2D previewCollider =
            previewObject.GetComponent<Collider2D>();

        if (previewCollider != null)
        {
            previewCollider.enabled = false;
        }
    }

    private void FollowMouse()
    {
        Vector3 mousePos =
            Camera.main.ScreenToWorldPoint(Input.mousePosition);

        mousePos.z = 0f;
        previewObject.transform.position = mousePos;
    }

    private void UpdatePreviewColor()
    {
        if (previewRenderer == null)
            return;

        previewRenderer.color = canPlace
            ? new Color(0f, 1f, 0f, 0.5f)
            : new Color(1f, 0f, 0f, 0.5f);
    }

    private void PlaceBuilding()
    {
        ItemData selectedItem =
            buildingInventoryUIController.GetSelectedItem();

        if (selectedItem == null ||
            selectedItem.buildingPrefab == null)
        {
            return;
        }

        GameObject placedBuilding = Instantiate(
            selectedItem.buildingPrefab,
            previewObject.transform.position,
            Quaternion.identity
        );

        SpriteRenderer placedRenderer =
            placedBuilding.GetComponent<SpriteRenderer>();

        if (placedRenderer != null)
        {
            placedRenderer.color = Color.white;
        }

        BuildingInventory.Instance.RemoveItem(
            selectedItem,
            1
        );
    }

    private void EnterBuildMode()
    {
        isBuildMode = true;

        inventoryUI.SetActive(false);
        buildingInventoryUI.SetActive(true);

        RefreshPreview();
    }

    private void ExitBuildMode()
    {
        isBuildMode = false;

        inventoryUI.SetActive(true);
        buildingInventoryUI.SetActive(false);

        if (previewObject != null)
        {
            Destroy(previewObject);
            previewObject = null;
        }

        previewRenderer = null;
    }

    private void CheckCanPlace()
    {
        Vector2 checkPosition =
            (Vector2)previewObject.transform.position + placementCheckOffset;

        Collider2D hit = Physics2D.OverlapBox(
            checkPosition,
            placementCheckSize,
            0f,
            ResourceLayer
        );

        canPlace = hit == null;
    }

    private void RefreshPreview()
    {
        if (!isBuildMode)
            return;

        if (previewObject != null)
        {
            Destroy(previewObject);
            previewObject = null;
        }

        ItemData selectedItem =
            buildingInventoryUIController.GetSelectedItem();

        if (selectedItem == null ||
            selectedItem.buildingPrefab == null)
        {
            previewRenderer = null;
            return;
        }

        previewObject = Instantiate(selectedItem.buildingPrefab);

        previewRenderer =
            previewObject.GetComponent<SpriteRenderer>();

        Collider2D previewCollider =
            previewObject.GetComponent<Collider2D>();

        if (previewCollider != null)
        {
            previewCollider.enabled = false;
        }

        FollowMouse();
    }
}