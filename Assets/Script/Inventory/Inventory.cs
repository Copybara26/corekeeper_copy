using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }

    [Header("테스트용 아이템")]
    [SerializeField] private ItemData wood;
    [SerializeField] private ItemData stone;
    [SerializeField] private ItemData iron;

    [SerializeField] private ItemData TomatoSeed;
    [SerializeField] private ItemData WheatSeed;

    [Header("현재 인벤토리")]
    [SerializeField] private List<InventorySlotData> slots = new();

    public IReadOnlyList<InventorySlotData> Slots => slots;

    private const int MAX_SLOT = 9;

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

    private void Start()
    {
        SelectedSlotIndex = 0;

        AddItem(TomatoSeed, 5);
        AddItem(WheatSeed, 5);

        OnInventoryChanged?.Invoke();
    }

    public int SelectedSlotIndex { get; private set; } = 0;

    public ItemData SelectedItem
    {
        get
        {
            if (SelectedSlotIndex < 0 ||
                SelectedSlotIndex >= slots.Count)
            {
                return null;
            }

            return slots[SelectedSlotIndex].item;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SelectSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SelectSlot(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SelectSlot(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SelectSlot(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) SelectSlot(4);
        if (Input.GetKeyDown(KeyCode.Alpha6)) SelectSlot(5);
        if (Input.GetKeyDown(KeyCode.Alpha7)) SelectSlot(6);
        if (Input.GetKeyDown(KeyCode.Alpha8)) SelectSlot(7);
        if (Input.GetKeyDown(KeyCode.Alpha9)) SelectSlot(8);
    }

    public void SelectSlot(int index)
    {
        if (index < 0 || index >= MAX_SLOT)
        {
            return;
        }

        SelectedSlotIndex = index;

        if (SelectedItem != null)
        {
            Debug.Log($"{index + 1}번 슬롯 선택: {SelectedItem.itemName}");
        }
        else
        {
            Debug.Log($"{index + 1}번 빈 슬롯 선택");
        }

        // 선택 테두리도 새로 갱신
        OnInventoryChanged?.Invoke();
    }

    public void AddItem(ItemData item, int amount)
    {
        if (item == null || amount <= 0)
        {
            return;
        }

        InventorySlotData existingSlot =
            slots.Find(slot => slot.item == item);

        if (existingSlot == null && slots.Count >= MAX_SLOT)
        {
            Debug.Log("인벤토리가 가득 찼습니다.");
            return;
        }

        if (existingSlot != null)
        {
            existingSlot.amount += amount;
        }
        else
        {
            slots.Add(new InventorySlotData(item, amount));
        }

        OnInventoryChanged?.Invoke();
    }

    public int GetItemAmount(ItemData item)
    {
        InventorySlotData slot =
            slots.Find(currentSlot => currentSlot.item == item);

        return slot == null ? 0 : slot.amount;
    }

    public bool RemoveItem(ItemData item, int amount)
    {
        InventorySlotData slot =
            slots.Find(currentSlot => currentSlot.item == item);

        if (slot == null || slot.amount < amount)
        {
            return false;
        }

        slot.amount -= amount;

        if (slot.amount <= 0)
        {
            slots.Remove(slot);
        }

        OnInventoryChanged?.Invoke();
        return true;
    }
}