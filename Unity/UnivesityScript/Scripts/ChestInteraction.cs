using UnityEngine;

public class ChestInteraction : MonoBehaviour
{
    public GameObject interactText;
    public Animator chestAnimator;

    // NUOVA CLASSE per gestire gli oggetti e le loro quantità
    [System.Serializable]
    public class LootItem
    {
        public ItemSO item;
        public int quantity = 1;
    }

    // Riferimenti per gli oggetti da dare (una lista)
    public LootItem[] itemsToGive;

    // Riferimento per le monete da dare
    public int coinsToGive = 25;

    private bool isPlayerInRange = false;
    private bool chestOpened = false;

    private void Update()
    {
        if (isPlayerInRange && !chestOpened && Input.GetKeyDown(KeyCode.E))
        {
            OpenChest();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !chestOpened)
        {
            isPlayerInRange = true;
            if (interactText != null) interactText.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (interactText != null) interactText.SetActive(false);
        }
    }

    private void OpenChest()
    {
        if (chestOpened) return;

        chestAnimator.SetTrigger("Open");

        // La cassa dà le monete
        if (GameManager.Instance != null && CurrencyUI.Instance != null)
        {
            GameManager.Instance.AddCoins(coinsToGive);
            CurrencyUI.Instance.UpdateUI(GameManager.Instance.getCoins());
            Debug.Log($"[ChestInteraction] Aggiunte {coinsToGive} monete.");
        }

        // E dà tutti gli oggetti nella lista
        if (InventoryManager.Instance != null)
        {
            foreach (LootItem lootItem in itemsToGive)
            {
                if (lootItem.item != null)
                {
                    bool added = InventoryManager.Instance.AddItem(lootItem.item, lootItem.quantity);

                    if (added)
                    {
                        Debug.Log($"[ChestInteraction] {lootItem.quantity}x {lootItem.item.itemName} aggiunta all'inventario.");
                    }
                    else
                    {
                        Debug.Log($"[ChestInteraction] Impossibile aggiungere {lootItem.quantity}x {lootItem.item.itemName} all'inventario.");
                    }
                }
            }
        }

        chestOpened = true;
        if (interactText != null) interactText.SetActive(false);

        Collider chestCollider = GetComponent<Collider>();
        if (chestCollider != null)
        {
            chestCollider.enabled = false;
        }
    }
}