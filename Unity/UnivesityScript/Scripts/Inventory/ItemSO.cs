using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemSO : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public ItemType itemType;
    public bool isStackable = true;
    public int maxStack = 99;

    public GameObject weaponPrefab;

    [Header("Consumable Settings")]
    public float healAmount = 0f;
    public float staminaAmount = 0f;
    public float staminaDuration = 0f;
    public float damageIncrease = 0f;
    public bool isAntidote = false;
    public bool isWater = false;
}