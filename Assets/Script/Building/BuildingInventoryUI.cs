using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;

public class BuildingInventoryUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMP_Text amountText;

    public event Action OnSelectionChanged;

    private int selectedIndex = 0;

    private void Start()
    {
        if (BuildingInventory.Instance != null)
        {
            BuildingInventory.Instance.OnInventoryChanged += RefreshUI;
        }

        RefreshUI();
    }

    private void OnDestroy()
    {
        if (BuildingInventory.Instance != null)
        {
            BuildingInventory.Instance.OnInventoryChanged -= RefreshUI;
        }
    }

    public void PreviousItem()
    {
        if (BuildingInventory.Instance == null)
            return;

        int count = BuildingInventory.Instance.Slots.Count;

        if (count == 0)
            return;

        selectedIndex--;

        if (selectedIndex < 0)
        {
            selectedIndex = count - 1;
        }

        RefreshUI();
        OnSelectionChanged?.Invoke();
    }

    public void NextItem()
    {
        if (BuildingInventory.Instance == null)
            return;

        int count = BuildingInventory.Instance.Slots.Count;

        if (count == 0)
            return;

        selectedIndex++;

        if (selectedIndex >= count)
        {
            selectedIndex = 0;
        }

        RefreshUI();
        OnSelectionChanged?.Invoke();
    }

    private void RefreshUI()
    {
        if (BuildingInventory.Instance == null)
            return;

        var slots = BuildingInventory.Instance.Slots;

        if (slots.Count == 0)
        {
            itemIcon.enabled = false;
            amountText.text = "";
            selectedIndex = 0;
            return;
        }

        if (selectedIndex >= slots.Count)
        {
            selectedIndex = slots.Count - 1;
        }

        InventorySlotData slot = slots[selectedIndex];

        itemIcon.enabled = true;
        itemIcon.sprite = slot.item.icon;

        amountText.text = $"x{slot.amount}";
    }

    public ItemData GetSelectedItem()
    {
        if (BuildingInventory.Instance == null)
            return null;

        var slots = BuildingInventory.Instance.Slots;

        if (slots.Count == 0)
            return null;

        if (selectedIndex < 0 || selectedIndex >= slots.Count)
            return null;

        return slots[selectedIndex].item;
    }
}