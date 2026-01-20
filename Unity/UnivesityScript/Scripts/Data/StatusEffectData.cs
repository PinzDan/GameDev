using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/StatusEffect")]
public class StatusEffectData : ScriptableObject
{
    public string effectName;
    public float durata;
    public float intervallo; // per danni nel tempo
    public float DannoPerIntervallo; // danno per intervallo

    public ParticleSystem vfxChargingPrefab; // particelle o effetti visivi
    public ParticleSystem vfxExplodePrefab;


    // public AudioClip soundEffect; // suono dell'effetto
}