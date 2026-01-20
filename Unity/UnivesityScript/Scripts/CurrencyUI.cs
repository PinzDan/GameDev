using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Collections;

public class CurrencyUI : MonoBehaviour
{
    public static CurrencyUI Instance;
    public Sprite coinSprite;

    private UIDocument _document;
    private Label _coinCounter;
    private VisualElement _coinIcon;

    private void Awake()
    {
        Instance = this;
        _document = GetComponent<UIDocument>();
        if (_document == null) return;
    }

    private void OnEnable()
    {
        GameManager.Instance.OnDataLoaded += ApplyGameData;
        var root = _document.rootVisualElement;
        _coinCounter = root.Q<Label>("coin-counter");
        _coinIcon = root.Q<VisualElement>("coin-icon");

        if (_coinIcon != null && coinSprite != null)
        {
            _coinIcon.style.backgroundImage = new StyleBackground(coinSprite);
        }


        UpdateUI(GameManager.Instance.getCoins());
    }

    public void ApplyGameData()
    {
        UpdateUI(GameManager.Instance.getCoins());
    }



    public void UpdateUI(int newCoinValue)
    {
        if (_coinCounter != null)
        {
            _coinCounter.text = newCoinValue.ToString();
        }
    }
}