using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.XR;


public class AudioController : MonoBehaviour
{


    [SerializeField] private AudioClip[] footStepsClips; int index = 0;
    [SerializeField] AudioClip[] mainTheme;

    [Header("Clip combat")]
    [SerializeField] AudioClip[] combatClip;
    [SerializeField] private AudioSource CurrentActiveAudio;
    [SerializeField] private AudioSource movementAudioSource;
    public StateManager stateManager;
    public PlayerStateManager playerStateManager;
    private PlayerState oldState;


    public float snapshotChangeDuration = 1f;

    /* Snaphot : 
     * - playingSnapshot: snapshot per il gioco -> (Abbassaleggermente il volume del gruppo menu)
     * - menuSnapshot: snapshot per il menu -> (Alza il volume del gruppo menu e abbassa il volume del gruppo battle)
     * - battleSnapshot: snapshot per la battaglia  -> (Alza il volume del gruppo battle e abbassa il volume del gruppo menu)
     * - totalSnapshot: snapshot per il cambio totale di stato -> (abbassa il volume di tutti e due i group)
     */
    [SerializeField] AudioMixerSnapshot menuSnapshot;
    [SerializeField] AudioMixerSnapshot battleSnapshot;
    [SerializeField] AudioMixerSnapshot totalSnapshot;
    [SerializeField] AudioMixerSnapshot playingSnapshot;




    void Awake()
    {
        CurrentActiveAudio = GameObject.Find("MenuAudio").GetComponent<AudioSource>();
        mainTheme = Resources.LoadAll<AudioClip>("Audio/mainTheme");
        footStepsClips = Resources.LoadAll<AudioClip>("Audio/footsteps");

        SceneManager.sceneLoaded += OnSceneLoaded;
    }


    // ========== Gestione Main Theme in base al livello  ========== //
    private AudioClip GetClipByName(string clipName)
    {
        foreach (AudioClip clip in mainTheme)
        {
            if (clip.name == clipName)
                return clip;
        }
        return null; // Se non trovato, ritorna null
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "MappaTutorial":

                ChangeMusic(GetClipByName("MappaTutorial")); // esempio
                break;
            case "Livello1":
                ChangeMusic(GetClipByName("Livello1"));
                break;
            case "Livello2":
                ChangeMusic(GetClipByName("Livello2"));
                break;
            case "Livello3":
                ChangeMusic(GetClipByName("Livello3"));
                break;
        }
    }

    private void ChangeMusic(AudioClip clip)
    {
        if (CurrentActiveAudio.clip != clip)
        {
            CurrentActiveAudio.Stop();
            CurrentActiveAudio.clip = clip;
            CurrentActiveAudio.Play();
        }
    }
    void OnEnable()
    {
        if (stateManager != null)
            stateManager.OnStateChanged += HandleStateManagerChanged;
        if (playerStateManager != null)
            playerStateManager.OnPlayerStateChanged += HandlePlayerStateManagerChanged;
    }

    void OnDisable()
    {
        if (stateManager != null)
            stateManager.OnStateChanged -= HandleStateManagerChanged;
        if (playerStateManager != null)
            playerStateManager.OnPlayerStateChanged -= HandlePlayerStateManagerChanged;
    }

    private void HandleStateManagerChanged(GameState newState)
    {
        switch (newState)
        {
            case GameState.menu:
                CurrentActiveAudio = GameObject.Find("MenuAudio").GetComponent<AudioSource>();
                CurrentActiveAudio.Play();
                break;
            case GameState.playing:
                snapshotChange(playingSnapshot);
                Debug.Log("Stato cambiato a playing");
                // Gestisci la logica per il cambio di stato a playing
                break;
            case GameState.paused:
                Debug.Log("Stato cambiato a paused");
                // Gestisci la logica per il cambio di stato a paused
                break;
            case GameState.dialogue:
                StartCoroutine(waitEnter());
                break;
        }
    }

    IEnumerator waitEnter()
    {
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));
        CurrentActiveAudio = GameObject.Find("Invio").GetComponent<AudioSource>();
        CurrentActiveAudio.Play();

    }

    private void HandlePlayerStateManagerChanged(PlayerState newPlayerState)
    {
        Debug.Log("PlayerStateManager, ci sono : " + newPlayerState);

        if ((newPlayerState & PlayerState.Combat) != 0)
        {

            Debug.Log("PlayerStateManager: Combat");
            CurrentActiveAudio = GameObject.Find("normalBattle").GetComponent<AudioSource>();
            var activeEnemy = GameObject.FindGameObjectWithTag("activeEnemy").name;


            if (activeEnemy == "Golem")
                CurrentActiveAudio.clip = combatClip[1];
            else if (activeEnemy == "Turtle")
                CurrentActiveAudio.clip = combatClip[2];
            else if (activeEnemy == "Dragon")
                CurrentActiveAudio.clip = combatClip[3];
            else
                CurrentActiveAudio.clip = combatClip[0];

            Debug.Log("CurrentActiveAudio: " + CurrentActiveAudio.name);
            snapshotChange(battleSnapshot);
            oldState = PlayerState.Combat;


        }
        else if ((newPlayerState & PlayerState.Normal) != 0)
        {
            CurrentActiveAudio = GameObject.Find("MenuAudio").GetComponent<AudioSource>();
            snapshotChange(playingSnapshot); // Cambia lo snapshot per il menu
            oldState = newPlayerState;

        }

    }

    public void playFootstep()
    {
        if (footStepsClips[index] != null)
        {
            Debug.Log("clip: " + index + " " + footStepsClips[index].name);
            movementAudioSource.PlayOneShot(footStepsClips[index]);
            index = (index + 1) % footStepsClips.Length; //per rendere cicliclo l'indice

        }


    }


    void snapshotChange(AudioMixerSnapshot snapshot = null)
    {
        if (snapshot == null)
        {
            snapshot = totalSnapshot;
        }

        // Riproduci l'audio attivo corrente se non è già in riproduzione
        if (!CurrentActiveAudio.isPlaying)
            CurrentActiveAudio.Play();

        snapshot.TransitionTo(snapshotChangeDuration);
    }
}
