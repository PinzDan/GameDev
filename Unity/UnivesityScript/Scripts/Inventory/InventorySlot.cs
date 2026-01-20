[System.Serializable]
public class InventorySlot
{
    public ItemSO item;
    public int quantity;

    public InventorySlot(ItemSO newItem, int newQuantity)
    {
        item = newItem;
        quantity = newQuantity;
    }
}
