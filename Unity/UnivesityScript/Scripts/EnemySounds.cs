using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySounds : MonoBehaviour
{
    [SerializeField] private AudioSource enemySource;

    private Dictionary<string, AudioClip> enemyClips;

    private AudioClip[] footSteps;

    private int index;

    void OnEnable()
    {
        index = 0;

        AudioSource source = gameObject.AddComponent<AudioSource>();

        enemySource = source;

        enemySource.spatialBlend = 1f;
        enemySource.rolloffMode = AudioRolloffMode.Logarithmic;
        enemySource.minDistance = 40f;
        enemySource.maxDistance = 200f;
        enemySource.dopplerLevel = 0f;

        enemyClips = new Dictionary<string, AudioClip>();

        string path = "Audio/" + gameObject.name;

        string stepsPath = "Audio/" + gameObject.name + "/Step";

        AudioClip[] clips = Resources.LoadAll<AudioClip>(path);

        footSteps = Resources.LoadAll<AudioClip>(stepsPath); /* importo le clip dei footstep */

        fillDictionary(clips, enemyClips);
    }

    private void fillDictionary(Array clips, Dictionary<string, AudioClip> keyValuePairs)
    {
        if (clips == null) return;

        foreach (AudioClip clip in clips)
        {
            Debug.Log($"clip aggiutna: {clip}, {clip.name}");
            keyValuePairs.Add(clip.name, clip);
        }
    }

    public void riproduci(string name)
    {
        Debug.Log("Riproduco");
        if (enemyClips.TryGetValue(name, out AudioClip clip))
        {
            Debug.Log("clip: " + clip);
            enemySource.PlayOneShot(clip);
        }
    }

    public void riproduciSteps()
    {
        if (footSteps[index] == null) return;

        enemySource.PlayOneShot(footSteps[index]);
        index = (index + 1) % footSteps.Length;
    }
}
