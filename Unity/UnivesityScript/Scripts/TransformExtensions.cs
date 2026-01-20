using UnityEngine;

public static class TransformExtensions
{
    // Funzione di estensione per trovare un oggetto figlio in tutta la gerarchia
    public static Transform FindDeepChild(this Transform parent, string name)
    {
        Transform result = parent.Find(name);
        if (result != null)
            return result;

        foreach (Transform child in parent)
        {
            result = child.FindDeepChild(name);
            if (result != null)
                return result;
        }

        return null;
    }
}