using UnityEngine;
using System.Collections.Generic;
using System;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public PlayerStateManager stateManager;
    public SaveData dati = new SaveData();

    public GameObject player;


    public event Action OnDataLoaded;
    public bool isDataLoaded = false;
    public bool isFirstLaunch = true;
    public bool gameOver = false;

    public bool checkPointEnabled = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // Qui puoi decidere se caricare automaticamente dal primo slot
            //CaricaPrimoSaveDisponibile();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        EnemiesStats.addScore += AggiungiPunteggio;
    }


    //=======Gestione Salvataggi========//

    // Salva nello slot specificato (1-3)
    // Salva nello slot specificato (1-3)
    public void SalvaGioco(int slot)
    {
        if (slot < 1 || slot > 3)
        {
            Debug.LogError("Slot non valido! Deve essere 1, 2 o 3");
            return;
        }


        dati.inventario.Clear();
        foreach (var slotInv in InventoryManager.Instance.hotbar)
        {
            if (slotInv != null && slotInv.item != null)
            {
                dati.inventario.Add(new InventorySlot(slotInv.item, slotInv.quantity));
            }
        }

        dati.slot = slot;
        SaveSystem.Salva(dati, slot);
        Debug.Log($"Gioco salvato nello slot {slot}");
    }


    // Carica da uno slot specifico
    public void CaricaGioco(int slot)
    {
        Debug.Log($"Tentativo di caricare il gioco dallo slot {slot}");
        if (slot < 1 || slot > 3)
        {
            Debug.LogError("Slot non valido! Deve essere 1, 2 o 3");
            return;
        }

        var loadedData = SaveSystem.Carica(slot);
        if (loadedData != null)
        {
            dati = loadedData;
            Debug.Log($"Gioco caricato dallo slot {slot}");

            isDataLoaded = true;
            OnDataLoaded?.Invoke();
        }
        else
        {
            Debug.LogWarning($"Nessun salvataggio trovato nello slot {slot}");
        }
    }

    // Carica il primo slot disponibile, se presente
    public void CaricaPrimoSaveDisponibile()
    {
        for (int i = 1; i <= 3; i++)
        {
            if (SaveSystem.SlotEsiste(i))
            {
                CaricaGioco(i);
                return;
            }
        }
        Debug.Log("Nessun salvataggio trovato. Nuovo gioco iniziale.");
        dati = new SaveData(); // Reset dei dati se non ci sono save
        dati.slot = 1; // Imposta lo slot di default a 1
    }


    public void SalvaAutomatico()
    {
        for (int i = 1; i <= 3; i++)
        {
            if (!SaveSystem.SlotEsiste(i))
            {
                SalvaGioco(i);
                return;
            }
        }

        // Se tutti e 3 gli slot sono occupati, sovrascrivi il primo
        SalvaGioco(1);
        Debug.Log("Tutti gli slot pieni. Sovrascritto lo slot 1.");
    }

    // ======================================//
    // Gestione punteggio

    public void SetMaxHealth(float health) => dati.maxHealth = health;
    public void SetMaxStamina(float stamina) => dati.maxStamina = stamina;
    public void AggiungiPunteggio(int valore) => dati.punteggioTotale += valore;
    public void CompletaLivello() => dati.livelliCompletatiCount++;
    public void setTutorial(bool stato) => dati.tutorialCompletato = stato;
    public void SetPlayerName(string name) => dati.playerName = name;


    public float getMaxHealth() { return dati.maxHealth; }
    public float getMaxStamina() { return dati.maxStamina; }

    public float getDannoBase() { return dati.dannoBase; }
    public void setDannoBase(float danno) { dati.dannoBase = danno; }
    public int getCoins() { return dati.coins; }
    public int getLivelliCompletati() { return dati.livelliCompletatiCount; }

    public int getSlot() { return dati.slot; }

    public bool GetTutorialStatus() { return dati.tutorialCompletato; }

    public void AddCoins(int coin) => dati.coins += coin;
    public bool RemoveCoins(int coin)
    {
        if (dati.coins >= coin)
        {
            dati.coins -= coin;
            return true;
        }
        return false;
    }


    public void AddItem(InventorySlot slot)
    {
        {

            InventorySlot existingSlot = dati.inventario.Find(s => s.item == slot.item);

            if (existingSlot != null)
                existingSlot.quantity += slot.quantity;
            else
                dati.inventario.Add(new InventorySlot(slot.item, slot.quantity));

        }

    }

    public void RemoveItem(int index)
    {
        if (dati.inventario[index].quantity > 1)
        {
            dati.inventario[index].quantity--;
        }
        else
        {
            dati.inventario.RemoveAt(index);
        }
    }

    public List<InventorySlot> GetInventory()
    {
        return dati.inventario;
    }

    // Gestione livelli completati
    public void CompletaLivello(string livello)
    {
        dati.livelliCompletatiCount++;
        Debug.Log($"Livello {livello} completato! Totale livelli completati: {dati.livelliCompletatiCount}");
    }

    // Tutorial completato
    public void ImpostaTutorialCompletato() => dati.tutorialCompletato = true;

    public void resetStato()
    {
        stateManager.reset();
    }


}
