using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class ShopUI : MonoBehaviour
{
    public UIDocument uiDocument;
    public List<ItemSO> shopItems;
    public List<int> itemPrices;

    private VisualElement _shopRoot;
    private Button _closeButton;
    private VisualElement _shopSlotsContainer;
    private Label _statusMessageLabel;
    
    private int _selectedIndex = 0;
    
    private void Awake()
    {
        uiDocument = GetComponent<UIDocument>();
        if (uiDocument != null)
        {
            var root = uiDocument.rootVisualElement;
            _shopRoot = root.Q<VisualElement>("shop-root");
            _shopSlotsContainer = root.Q<VisualElement>("shop-slots-container");
            _statusMessageLabel = root.Q<Label>("status-message");
            
            _closeButton = root.Q<Button>("close-button");
            if (_closeButton != null)
            {
                _closeButton.clicked += CloseShop;
            }
        }
    }
    
    private void OnEnable()

    {
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
        RefreshShopUI();
        _selectedIndex = 0;
        
        UpdateSelection();
    }
    
    private void OnDisable()
    {
        for (int i = 0; i < _shopSlotsContainer.childCount; i++)
        {
            var slot = _shopSlotsContainer.Q<VisualElement>($"shop-slot-{i}");
            slot?.RemoveFromClassList("selected");
        }
    }
    
    private void Update()
    {
        // Gestione della selezione con la rotellina del mouse
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        float verticalInput =  Input.GetAxis("Vertical");
        if (scrollInput > 0f || verticalInput > 0.1f) // scroll su o W/Up
        {
            _selectedIndex = (_selectedIndex - 1 + shopItems.Count) % shopItems.Count;
            UpdateSelection();
        }
        else if (scrollInput < 0f || verticalInput < -0.1f) // scroll giù o S/Down
        {
            _selectedIndex = (_selectedIndex + 1) % shopItems.Count;
            UpdateSelection();
        }
        
        // Acquisto con INVIO (Enter)
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("Return clicked");
            BuySelectedItem();
        }
    }

    private void RefreshShopUI()
    {
        if (_shopSlotsContainer != null)
        {
            for (int i = 0; i < shopItems.Count; i++)
            {
                var itemSlot = _shopSlotsContainer.Q<VisualElement>($"shop-slot-{i}");
                if (itemSlot != null)
                {
                    var itemIcon = itemSlot.Q<VisualElement>("icon");
                    var itemPrice = itemSlot.Q<Label>($"price-label-{i}");

                    if (i < shopItems.Count && shopItems[i] != null && shopItems[i].icon != null)
                    {
                        itemIcon.style.backgroundImage = new StyleBackground(shopItems[i].icon);
                    }
                    else
                    {
                        itemIcon.style.backgroundImage = null;
                    }
                    
                    if (i < itemPrices.Count)
                    {
                        itemPrice.text = itemPrices[i].ToString();
                    }
                    else
                    {
                        itemPrice.text = "N/A";
                    }
                }
            }
        }
    }

    private void UpdateSelection()
    {
        for (int i = 0; i < shopItems.Count; i++)
        {
            var slot = _shopSlotsContainer.Q<VisualElement>($"shop-slot-{i}");
            if (slot != null)
            {
                if (i == _selectedIndex)
                {
                    slot.AddToClassList("selected");
                }
                else
                {
                    slot.RemoveFromClassList("selected");
                }
            }
        }
    }

    private void BuySelectedItem()
    {
        Debug.Log($"index: {_selectedIndex}");
        if (_selectedIndex >= 0 && _selectedIndex < shopItems.Count && shopItems[_selectedIndex] != null)
        {
            ItemSO itemToBuy = shopItems[_selectedIndex];
            int price = itemPrices[_selectedIndex];

            if(GameManager.Instance.RemoveCoins(price))
            {
                // CurrencyUI.Instance.UpdateUI(CurrencyManager.Coins);
                CurrencyUI.Instance.UpdateUI(GameManager.Instance.getCoins());
            
                InventoryManager.Instance.AddItem(itemToBuy, 1);
                ShowStatusMessage($"Acquistato: {itemToBuy.itemName}!", Color.green);
            }
            else
            {
                ShowStatusMessage("Non hai abbastanza monete!", Color.red);
            }
        }
    }
    
    private void CloseShop()
    {
        NPCInteraction npc = FindObjectOfType<NPCInteraction>();
        if (npc != null)
        {
            StartCoroutine(npc.CloseDialogue());
        }
    }

    private void ShowStatusMessage(string message, Color color)
    {
        if (_statusMessageLabel != null)
        {
            _statusMessageLabel.text = message;
            _statusMessageLabel.style.color = new StyleColor(color);
            StartCoroutine(ClearStatusMessageAfterDelay(2f));
        }
    }

    private IEnumerator ClearStatusMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (_statusMessageLabel != null)
        {
            _statusMessageLabel.text = "";
        }
    }
}