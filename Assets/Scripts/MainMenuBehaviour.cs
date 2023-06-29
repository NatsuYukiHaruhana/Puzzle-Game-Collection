using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuBehaviour : MonoBehaviour
{
    [SerializeField]
    private string currentGame;

    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject gameSettingsMenu = null;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject audioMenu;
    [SerializeField] private GameObject displayMenu;

    public void EnterSettings() {
        mainMenu.SetActive(false);
        audioMenu.SetActive(false);
        displayMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void EnterAudioMenu() {
        audioMenu.SetActive(true);
        settingsMenu.SetActive(false);
    }

    public void EnterDisplayMenu() {
        displayMenu.SetActive(true);
        settingsMenu.SetActive(false);
    }

    public void EnterMainMenu() {
        mainMenu.SetActive(true);
        settingsMenu.SetActive(false);
        if (gameSettingsMenu != null) { 
            gameSettingsMenu.SetActive(false);
        }
    }

    public void EnterGameSettings() {
        if (gameSettingsMenu != null) { 
            mainMenu.SetActive(false);
            gameSettingsMenu.SetActive(true);
        }
    }

    public void LoadGameMenu(string game) {
        SceneManager.LoadScene(game + " Main Menu Scene");
    }

    public void LoadGameMenu() {
        SceneManager.LoadScene(currentGame + " Main Menu Scene");
    }

    public void LoadMainMenu() {
        SceneManager.LoadScene("Main Menu");
    }

    public void QuitGame() {
        Application.Quit();
    }
}
