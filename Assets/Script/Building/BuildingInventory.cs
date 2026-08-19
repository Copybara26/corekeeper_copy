using System;
using System.Collections.Generic;
using UnityEngine;

public class BuildingInventory : MonoBehaviour
{
    public static BuildingInventory Instance { get; private set; }

    [SerializeField]
    private List<InventorySlotData> slots = new();

    public IReadOnlyList<InventorySlotData> Slots => slots;

    public event Action OnInventoryChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void AddItem(ItemData item, int amount)
    {
        if (item == null || amount <= 0)
            return;

        InventorySlotData existingSlot =
            slots.Find(slot => slot.item == item);

        if (existingSlot != null)
        {
            existingSlot.amount += amount;
        }
        else
        {
            slots.Add(
                new InventorySlotData(item, amount)
            );
        }

        Debug.Log(
            $"[건축 인벤토리] {item.itemName} {amount}개 추가"
        );

        OnInventoryChanged?.Invoke();
    }

    public bool RemoveItem(ItemData item, int amount)
    {
        if (item == null || amount <= 0)
            return false;

        InventorySlotData slot =
            slots.Find(slot => slot.item == item);

        if (slot == null || slot.amount < amount)
            return false;

        slot.amount -= amount;

        if (slot.amount <= 0)
        {
            slots.Remove(slot);
        }

        OnInventoryChanged?.Invoke();

        return true;
    }
}