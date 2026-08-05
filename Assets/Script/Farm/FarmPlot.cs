using System.Collections;
using UnityEngine;

public class FarmPlot : MonoBehaviour
{
    [Header("토마토")]
    [SerializeField] private ItemData tomatoSeed;
    [SerializeField] private GameObject tomatoCropPrefab;

    [Header("밀")]
    [SerializeField] private ItemData wheatSeed;
    [SerializeField] private GameObject wheatCropPrefab;

    private bool hasCrop;

    private GameObject currentCrop;

    //private void OnMouseDown()
    //{
    //    TryPlant();
    //}

    public void TryPlant()
    {
        if (hasCrop)
        {
            Debug.Log("이미 작물이 심어져 있습니다.");
            return;
        }

        if (Inventory.Instance == null)
        {
            return;
        }

        ItemData selectedItem = Inventory.Instance.SelectedItem;

        GameObject selectedCropPrefab = null;

        if (selectedItem == tomatoSeed)
        {
            selectedCropPrefab = tomatoCropPrefab;
        }
        else if (selectedItem == wheatSeed)
        {
            selectedCropPrefab = wheatCropPrefab;
        }
        else
        {
            Debug.Log("씨앗이 선택되어 있지 않습니다.");
            return;
        }

        if (selectedCropPrefab == null)
        {
            Debug.LogError("선택한 작물 프리팹이 연결되지 않았습니다.");
            return;
        }

        if (!Inventory.Instance.RemoveItem(selectedItem, 1))
        {
            Debug.Log("선택한 씨앗이 부족합니다.");
            return;
        }

        currentCrop = Instantiate(
        selectedCropPrefab,
        transform.position,
        Quaternion.identity
        );

        Crop cropScript = currentCrop.GetComponent<Crop>();

        if (cropScript != null)
        {
            cropScript.SetFarmPlot(this);
        }

        hasCrop = true;

        Debug.Log($"{selectedItem.itemName} 심기 완료");
    }

    public void CropHarvested()
    {
        currentCrop = null;
        StartCoroutine(UnlockPlotNextFrame());
    }

    private IEnumerator UnlockPlotNextFrame()
    {
        yield return null;
        hasCrop = false;
    }

    public void RemoveCrop()
    {
        if (currentCrop != null)
        {
            Destroy(currentCrop);
            currentCrop = null;
        }

        hasCrop = false;
    }
}