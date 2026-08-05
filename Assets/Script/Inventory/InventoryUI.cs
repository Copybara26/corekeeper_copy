using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Transform slotContainer;
    [SerializeField] private InventorySlotUI slotPrefab;

    private readonly List<InventorySlotUI> createdSlots = new();

    private void Start()
    {
        for (int i = 0; i < 9; i++)
        {
            InventorySlotUI slot =
                Instantiate(slotPrefab, slotContainer);

            slot.ClearSlot();

            createdSlots.Add(slot);
        }

        Inventory.Instance.OnInventoryChanged += Refresh;
        Refresh();
    }

    private void OnDestroy()
    {
        if (Inventory.Instance != null)
        {
            Inventory.Instance.OnInventoryChanged -= Refresh;
        }
    }

    private void Refresh()
    {
        IReadOnlyList<InventorySlotData> inventorySlots =
            Inventory.Instance.Slots;

        for (int i = 0; i < createdSlots.Count; i++)
        {
            if (i < inventorySlots.Count)
            {
                createdSlots[i].SetSlot(inventorySlots[i]);
            }
            else
            {
                createdSlots[i].ClearSlot();
            }

            bool isSelected =
                i == Inventory.Instance.SelectedSlotIndex;

            createdSlots[i].SetSelected(isSelected);
        }
    }
}