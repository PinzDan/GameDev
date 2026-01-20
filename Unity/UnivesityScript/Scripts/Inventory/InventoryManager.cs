using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public Action<int> OnHotbarUpdated;

    public int hotbarSize = 9;
    public List<InventorySlot> hotbar = new List<InventorySlot>();
    public InventorySlot equippedWeaponSlot;

    public int coins = 0;

    private PlayerStats playerStats;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        if (hotbar.Count == 0)
        {
            for (int i = 0; i < hotbarSize; i++)
            {
                hotbar.Add(null);
            }
        }
    }

    public bool AddItem(ItemSO item, int quantity)
    {
        for (int i = 0; i < hotbar.Count; i++)
        {
            var slot = hotbar[i];

            if (slot != null && slot.item == item && item.isStackable)
            {
                slot.quantity += quantity;
                OnHotbarUpdated?.Invoke(i);
                return true;
            }

            if (slot == null || slot.item == null)
            {
                hotbar[i] = new InventorySlot(item, quantity);
                OnHotbarUpdated?.Invoke(i);
                return true;
            }
        }

        Debug.Log("Hotbar piena!");
        return false;
    }

    public void UseItem(int index)
    {
        if (index < 0 || index >= hotbar.Count) return;
        var slot = hotbar[index];
        if (slot == null || slot.item == null) return;

        if (playerStats == null)
        {
            playerStats = FindObjectOfType<PlayerStats>();
            if (playerStats == null)
            {
                Debug.LogError("PlayerStats non trovato in UseItem()!");
                return;
            }
        }

        if (slot.item.itemType == ItemType.Consumable)
        {
            if (slot.item.healAmount > 0) playerStats.Heal(slot.item.healAmount);
            if (slot.item.staminaAmount > 0 && slot.item.staminaDuration > 0)
                playerStats.RestoreStamina(slot.item.staminaAmount, slot.item.staminaDuration);
            if (slot.item.damageIncrease > 0)
            {
                playerStats.baseDamage += slot.item.damageIncrease;
                GameManager.Instance.setDannoBase(playerStats.baseDamage);
            }
            if (slot.item.itemName == "Stamina Gem") playerStats.BoostMaxStamina(slot.item.staminaAmount);
            if (slot.item.isAntidote) playerStats.CurePoisonEffect();
            if (slot.item.isWater) playerStats.CureBurningEffect();

            slot.quantity--;
            if (slot.quantity <= 0) hotbar[index] = null;

            OnHotbarUpdated?.Invoke(index);
        }
        else if (slot.item.itemType == ItemType.Weapon)
        {
            EquipWeapon(slot.item);
            hotbar[index] = null;
            OnHotbarUpdated?.Invoke(index);
        }

        // 🔥 salva sempre dopo l'uso
        SaveInventoryToGameManager();
    }

    private void EquipWeapon(ItemSO weapon)
    {
        equippedWeaponSlot = new InventorySlot(weapon, 1);
        Debug.Log("Arma equipaggiata: " + weapon.itemName);
    }

    // 🔥 Metodo per salvare inventario nel GameManager
    public void SaveInventoryToGameManager()
    {
        if (GameManager.Instance != null && GameManager.Instance.dati != null)
        {
            GameManager.Instance.dati.inventario.Clear();
            foreach (var slot in hotbar)
            {
                if (slot != null && slot.item != null)
                {
                    GameManager.Instance.dati.inventario.Add(new InventorySlot(slot.item, slot.quantity));
                }
            }
            Debug.Log("[InventoryManager] Inventario salvato nel GameManager");
        }
    }
}
