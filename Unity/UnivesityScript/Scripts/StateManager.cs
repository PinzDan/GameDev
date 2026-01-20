using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;


public enum GameState
{
    menu,
    playing,
    paused,

    dialogue
}
public class StateManager : MonoBehaviour
{

    [Header("Elementi UI")]
    public GameObject menuUI;
    public GameObject gameUI;
    public GameObject pauseUI;

    [Header("UI Tutorial")]
    public GameObject tutorialUI;


    [Header("Elementi tutorial")]
    public GameObject slimeTutorial;
    public bool tutorialState = false;


    [Header("Riferimenti")]
    public GameState state;
    public PlayerStateManager playerState;
    public GameObject commands;




    public event Action<GameState> OnStateChanged;

    public GameState getState()
    {
        return state;
    }

    public bool itsPaused()
    {
        return state == GameState.paused;
    }


    public void ApplyGameData()
    {
        tutorialState = GameManager.Instance.GetTutorialStatus();

        if (tutorialState)
        {
            commands.SetActive(true);
            Destroy(slimeTutorial);
        }

    }



    void Start()
    {
        GameManager.Instance.OnDataLoaded += ApplyGameData;
        state = GameState.menu;
        Time.timeScale = 1;
        if (!tutorialState)
            commands.SetActive(false);
        UpdateUI();
    }


    public void Pause()
    {
        state = GameState.paused;
        UpdateUI();
        Time.timeScale = 0;
        Debug.Log("Gioco in pausa");
    }

    public void Resume()
    {
        state = GameState.playing;
        UpdateUI();
        Time.timeScale = 1;
        Debug.Log("Gioco attivo ");
    }

    public void Dialogo()
    {
        state = GameState.dialogue;
        UpdateUI();
    }



    public void UpdateUI()
    {
        switch (state)
        {
            case GameState.menu:
                menuUI.SetActive(true);
                gameUI.SetActive(false);
                pauseUI.SetActive(false);
                break;
            case GameState.playing:
                if (tutorialUI != null)
                    tutorialUI.SetActive(!tutorialState);

                menuUI.SetActive(false);
                gameUI.SetActive(true);
                pauseUI.SetActive(false);

                break;
            case GameState.paused:
                menuUI.SetActive(false);
                gameUI.SetActive(false);
                pauseUI.SetActive(true);
                break;
                //case GameState.dialogue:
        }
        OnStateChanged?.Invoke(state);
    }
}
