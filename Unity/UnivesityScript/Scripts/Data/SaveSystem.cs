using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static string GetSlotPath(int slot) => Application.persistentDataPath + $"/save_slot{slot}.json";

    public static void Salva(SaveData data, int slot)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(GetSlotPath(slot), json);
    }

    public static SaveData Carica(int slot)
    {
        string path = GetSlotPath(slot);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<SaveData>(json);
        }
        return null; // Nessun salvataggio per questo slot
    }

    public static bool SlotEsiste(int slot) => File.Exists(GetSlotPath(slot));

    public static void CancellaSlot(int slot)
    {
        string path = GetSlotPath(slot);
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
    public static void NuovoGioco(int slot)
    {
        SaveData datiIniziali = new SaveData();
        datiIniziali.slot = slot;

        // Trova il primo slot libero (da 1 a 3)
        if (!SlotEsiste(slot))
        {
            Debug.Log($"Creazione nuovo gioco nello slot {slot}");
            Salva(datiIniziali, slot);

            return;

        }
        Debug.Log($"Slot {slot} già esistente");
    }

}

