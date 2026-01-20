using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Collections;


public class HotbarUI : MonoBehaviour
{
    public UIDocument uiDocument;
    private VisualElement[] slots;
    private Label[] descriptionLabels;
    private Label[] quantityLabels;
    public static HotbarUI Instance;
    private int selectedIndex = 0;
    private VisualElement root;

    private void Awake()
    {
        root = uiDocument.rootVisualElement;
        setLabel();
        Instance = this;
        RefreshAllSlots();
    }
    private void OnEnable()
    {
        root = uiDocument.rootVisualElement;
        setLabel();

        caricaItems(true);
        InventoryManager.Instance.OnHotbarUpdated += UpdateHotbarSlot;
    }

    private void OnDisable()
    {
        // if (InventoryManager.Instance != null)
        // {
        //     InventoryManager.Instance.OnHotbarUpdated -= UpdateHotbarSlot;
        // }
    }

    private void Start()
    {
        //     if (uiDocument == null)
        //     {
        //         Debug.LogError("[HotbarUI] uiDocument non assegnato nel Inspector!");
        //         return;
        //     }

        //     var root = uiDocument.rootVisualElement;
        //     if (root == null)
        //     {
        //         Debug.LogError("[HotbarUI] rootVisualElement Ã¨ null!");
        //         return;
        //     }





        Debug.Log("HotbarUI: Inizializzazione degli slot UI completata.");

        //UpdateSelection();
    }

    private void setLabel()
    {

        slots = new VisualElement[9];
        descriptionLabels = new Label[9];
        quantityLabels = new Label[9];
        Debug.Log("HotbarUI: Inizializzazione degli slot UI...");
        for (int i = 0; i < 9; i++)
        {
            string slotName = $"slot-{i}";
            Debug.Log($"HotbarUI: Cercando {slotName}..., {slots[i]}");
            slots[i] = root.Q<VisualElement>(slotName);
            Debug.Log($"HotbarUI: Trovato {slotName}: {slots[i]}");

            if (slots[i] != null)
            {
                descriptionLabels[i] = slots[i].Q<Label>("descriptionLabel");
                quantityLabels[i] = slots[i].Q<Label>("quantityLabel");
            }
            else
            {
                Debug.LogError($"[HotbarUI] Impossibile trovare: {slotName}");
            }
        }
        UpdateSelection();
    }

    private void Update()
    {
        for (int i = 0; i < 9; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                selectedIndex = i;
                UpdateSelection();
            }
        }

        float scroll = Input.mouseScrollDelta.y;
        if (scroll != 0)
        {
            selectedIndex = (scroll > 0)
                ? (selectedIndex - 1 + slots.Length) % slots.Length
                : (selectedIndex + 1) % slots.Length;
            UpdateSelection();
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            UseSelectedItem();
        }
    }

    private void UseSelectedItem()
    {
        InventoryManager.Instance.UseItem(selectedIndex);
    }

    private void Reset()
    {
        for (int i = 0; i < InventoryManager.Instance.hotbar.Count; i++)
        {
            InventoryManager.Instance.hotbar[i] = null;
        }
    }


    public void caricaItems(bool reset = false)
    {
        if (reset)
            Reset();

        List<InventorySlot> inventario = GameManager.Instance.dati.inventario;
        int i = 0;
        foreach (InventorySlot item in inventario)
        {

            if (item.item == null)
                continue;

            InventoryManager.Instance.AddItem(item.item, item.quantity);

        }
        // invece di RefreshAllSlots() diretto
        StartCoroutine(RefreshNextFrame());
    }

    private IEnumerator RefreshNextFrame()
    {
        // aspetta fine frame
        yield return null;
        RefreshAllSlots();
    }

    /// <summary>
    /// Forza l' aggiornamento di tutti gli slot
    /// </summary>
    public void RefreshAllSlots()
    {
        Debug.Log("[HotbarUI] RefreshAllSlots chiamato");

        if (InventoryManager.Instance == null || slots == null)
        {
            Debug.LogWarning("[HotbarUI] Impossibile aggiornare gli slot. Riferimenti mancanti.");
            return;
        }

        for (int i = 0; i < InventoryManager.Instance.hotbar.Count; i++)
        {
            UpdateHotbarSlot(i);
        }

        UpdateSelection();
    }

    public void UpdateHotbarSlot(int index)
    {
        ItemSO item = InventoryManager.Instance.hotbar[index]?.item;
        Debug.Log($"[HotbarUI] UpdateHotbarSlot chiamato per index {index}, item: {item}");
        if (index < 0 || index >= slots.Length || slots[index] == null) return;

        Debug.Log("Ci semu");
        var slotData = InventoryManager.Instance.hotbar[index];
        var iconElement = slots[index].Q<VisualElement>("icon");
        Debug.Log($"Icona slot {index}: {iconElement}");
        var quantityLabel = quantityLabels[index];
        Debug.Log("Ci semu2");

        if (iconElement != null)
        {
            Debug.Log("Ci semu 3");
            if (slotData != null && slotData.item != null && slotData.item.icon != null)
            {
                iconElement.style.backgroundImage = new StyleBackground(slotData.item.icon);

                Debug.Log($"[HotbarUI] Slot {index}: impostata icona {slotData.item}, {slotData.item.icon}");
            }
            else
            {
                iconElement.style.backgroundImage = null;
                Debug.Log($"[HotbarUI] Slot {index}: vuoto");
            }
        }

        if (quantityLabel != null)
        {
            if (slotData != null && slotData.quantity > 1)
            {
                quantityLabel.text = $"{slotData.quantity}";
                quantityLabel.AddToClassList("show");
            }
            else
            {
                quantityLabel.text = "";
                quantityLabel.RemoveFromClassList("show");
            }
        }
    }

    public void UpdateSelection()
    {
        Debug.Log($"[HotbarUI] UpdateSelection chiamato. Slot selezionato: {selectedIndex}");

        if (slots == null) return;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null)
            {
                slots[i].RemoveFromClassList("selected");
                if (descriptionLabels[i] != null)
                {
                    descriptionLabels[i].RemoveFromClassList("show");
                }
            }
        }

        if (selectedIndex >= 0 && selectedIndex < slots.Length && slots[selectedIndex] != null)
        {
            slots[selectedIndex].AddToClassList("selected");

            var slotData = InventoryManager.Instance.hotbar[selectedIndex];
            if (slotData != null && slotData.item != null && descriptionLabels[selectedIndex] != null)
            {
                if (slotData.item.itemType == ItemType.Consumable)
                {
                    string descriptionText = "";

                    if (slotData.item.healAmount > 0)
                    {
                        descriptionText = $"+{slotData.item.healAmount} HP";
                    }
                    else if (slotData.item.staminaAmount > 0 && slotData.item.staminaDuration > 0)
                    {
                        descriptionText = $"Stamina per {slotData.item.staminaDuration}s";
                    }
                    else if (slotData.item.damageIncrease > 0)
                    {
                        descriptionText = $"Aumenta il danno permanentemente";
                    }
                    else if (slotData.item.staminaAmount>0) 
                    {
                        descriptionText = $"Aumenta la stamina permanentemente di 10";
                    }
                    else if (slotData.item.isAntidote)
                    {
                        descriptionText = $"Cura veleno";
                    }

                    if (!string.IsNullOrEmpty(descriptionText))
                    {
                        descriptionLabels[selectedIndex].text = descriptionText;
                        descriptionLabels[selectedIndex].AddToClassList("show");
                    }
                }
            }
        }
    }
}