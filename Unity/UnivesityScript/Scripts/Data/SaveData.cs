using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class SaveData
{

    public string playerName;

    public float maxHealth;
    public float maxStamina;

    public float dannoBase;
    public int punteggioTotale;
    public List<InventorySlot> inventario = new List<InventorySlot>();
    public int livelliCompletatiCount;
    public int coins;

    public bool sideQuestCompleted;
    public bool tutorialCompletato;

    public int slot;

    public SaveData()
    {

        playerName = "";
        maxHealth = 100f;
        maxStamina = 100f;
        dannoBase = 20f;
        punteggioTotale = 0;
        inventario = new List<InventorySlot>();
        livelliCompletatiCount = 0;
        coins = 0;
        sideQuestCompleted = false;
        tutorialCompletato = false;
        slot = -1; // Valore di default
    }
}





