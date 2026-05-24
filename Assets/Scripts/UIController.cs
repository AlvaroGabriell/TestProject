using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIController : MonoBehaviour
{
    public static UIController Instance { get; private set; }

    public GameObject HUDScreen, mainMenu, pauseMenu, winScreen;

    private Stack<GameObject> menuStack = new();

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

    // Use this function whenever you need to open a menu
    // It ensures that only one menu is active at a time, closing the current menu before opening the new one
    // Maintaining a stack of menus to manage navigation between them.
    public void OpenMenu(GameObject menu)
    {
        if (menuStack.Count > 0)
        {
            menuStack.Peek().SetActive(false);
        }

        menu.SetActive(true);
        GlobalVolume.Instance.EnableDOF();
        menuStack.Push(menu);
    }

    // Use this function to close the current menu and reactivate the previous one, if any.
    public void CloseCurrentMenu()
    {
        if (menuStack.Count > 0)
        {
            menuStack.Pop().SetActive(false);
        }
        if (menuStack.Count > 0)
        {
            menuStack.Peek().SetActive(true);
        }
        else GlobalVolume.Instance.DisableDOF();
    }

    public void CloseAllMenus()
    {
        while(menuStack.Count > 0)
        {
            menuStack.Pop().SetActive(false);
        }
        if(menuStack.Count <= 0) GlobalVolume.Instance.DisableDOF();
    }

    public void OnPlay()
    {
        CloseCurrentMenu();
        GameController.Instance.StartGame();
    }
    public void OnPause()
    {
        OpenMenu(pauseMenu);
        GameController.Instance.PauseGame();
        Utils.GetPlayer().GetComponent<PlayerInput>().actions.FindActionMap("Player").Disable();
    }
    public void OnResume()
    {
        CloseCurrentMenu();
        GameController.Instance.ResumeGame();
        Utils.GetPlayer().GetComponent<PlayerInput>().actions.FindActionMap("Player").Enable();
    }
    public void OnQuit()
    {
        Application.Quit();
    }
    public void OnRestart()
    {
        GameController.Instance.RestartGame(false);
    }
    public void OnMainMenu()
    {
        GameController.Instance.ResumeGame();
        GameController.Instance.RestartGame(true);
    }
    public void OnBack()
    {
        CloseCurrentMenu();
    }
}
