using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    public bool IsPaused { get; private set; } = false;
    public bool GameStarted { get; private set; } = false;

    public static event Action OnGameStarted;
    public static event Action OnGameWon;
    public static event Action OnGamePaused, OnGameResumed;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // Garante que apenas uma instância exista
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BootGame();
    }

    public void BootGame()
    {
        GameStarted = false;
        GlobalVolume.Instance.DisableDOF();
        Utils.GetPlayer().GetComponent<PlayerInput>().actions.FindActionMap("Player").Disable();
        UIController.Instance.HUDScreen.SetActive(false);
        UIController.Instance.OpenMenu(UIController.Instance.mainMenu);
    }

    public void StartGame()
    {
        GameStarted = true;
        Utils.GetPlayer().GetComponent<PlayerInput>().actions.FindActionMap("Player").Enable();
        UIController.Instance.HUDScreen.SetActive(true);
        OnGameStarted?.Invoke();
    }

    public void RestartGame(bool toMainMenu)
    {
        if(IsPaused) ResumeGame();
        GameStarted = false;
        StartCoroutine(ReloadPrincipalScene(toMainMenu));
    }

    IEnumerator ReloadPrincipalScene(bool toMainMenu)
    {
        yield return SceneManager.UnloadSceneAsync("CenaProtótipo");

        yield return SceneManager.LoadSceneAsync("CenaProtótipo", LoadSceneMode.Additive);

        //UIController.Instance.CloseAllMenus();

        if(toMainMenu) BootGame();
        else StartGame();
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        IsPaused = true;
        UIController.Instance.HUDScreen.SetActive(false);
        OnGamePaused?.Invoke();
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        IsPaused = false;
        UIController.Instance.HUDScreen.SetActive(true);
        OnGameResumed?.Invoke();
    }
}
