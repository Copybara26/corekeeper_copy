using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance;

    [Header("설치할 건물")]
    [SerializeField] private GameObject ChikenHousePrefab;
    private SpriteRenderer previewRenderer;

    private GameObject previewObject;

    private bool isBuildMode;
    bool canPlace = true;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            ToggleBuildMode();
        }
        if (isBuildMode && previewObject != null)
        {
            Vector3 mousePos =
                Camera.main.ScreenToWorldPoint(Input.mousePosition);

            mousePos.z = 0;

            previewObject.transform.position = mousePos;
        }
    }

    void ToggleBuildMode()
    {
        isBuildMode = !isBuildMode;

        if (isBuildMode)
        {
            previewObject = Instantiate(ChikenHousePrefab);
            previewRenderer = previewObject.GetComponent<SpriteRenderer>();

            if (canPlace)
            {
                previewRenderer.color = Color.green;
            }
            else
            {
                previewRenderer.color = Color.red;
            }

            if (Input.GetMouseButtonDown(0) && canPlace)
            {
                Instantiate(ChikenHousePrefab,
                            previewObject.transform.position,
                            Quaternion.identity);
            }
        }
        else
        {
            Destroy(previewObject);
        }
    }
}