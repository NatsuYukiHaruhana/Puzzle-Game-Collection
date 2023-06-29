using UnityEngine;
using TMPro;

public class Settings_Display : MonoBehaviour
{
    [SerializeField]
    private TMP_Dropdown displayModeDropdown, displayResDropdown;
    
    private void Start()
    {
        displayModeDropdown.value = PlayerPrefs.GetInt("DisplayMode", 0);
        ChangeDisplayMode(PlayerPrefs.GetInt("DisplayMode"));        
        int screenRes = PlayerPrefs.GetInt("ScreenResolution", -1);

        if (screenRes == -1) { 
            ChangeResolution(Screen.currentResolution);
            switch (Screen.currentResolution.width) {
                case 800:
                    displayResDropdown.value = 10;
                    break;
                case 1024:
                    displayResDropdown.value = 9;
                    break;
                case 1280:
                    switch (Screen.currentResolution.height) {
                        case 720:
                            displayResDropdown.value = 8;
                            break;
                        case 800:
                            displayResDropdown.value = 7;
                            break;
                        case 1024:
                            displayResDropdown.value = 6;
                            break;
                    }

                    break;
                case 1360:
                    displayResDropdown.value = 5;
                    break;
                case 1366:
                    displayResDropdown.value = 4;
                    break;
                case 1440:
                    displayResDropdown.value = 3;
                    break;
                case 1600:
                    displayResDropdown.value = 2;
                    break;
                case 1680:
                    displayResDropdown.value = 1;
                    break;
                case 1920:
                    displayResDropdown.value = 0;
                    break;
                default:
                    displayResDropdown.value = 0;
                    break;
            }
        } else {
            ChangeResolution(screenRes);
            displayResDropdown.value = screenRes;
        }
    }

    public void ChangeDisplayMode(int displayMode) // 0 - fullscreen, 1 - fullscreen borderless, 2 - windowed
    {
        switch(displayMode)
        {
            case 0:
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                PlayerPrefs.SetInt("DisplayMode", displayMode);
                break;

            case 1:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                PlayerPrefs.SetInt("DisplayMode", displayMode);
                break;

            case 2:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                PlayerPrefs.SetInt("DisplayMode", displayMode);
                break;

            default:
                Debug.LogError("Wrong display mode selected!");
                break;
        }
    }

    public void ChangeResolution(int resolutionChosen)
    {
        switch(resolutionChosen)
        {
            case 0:
                Screen.SetResolution(1920, 1080, Screen.fullScreenMode);
                PlayerPrefs.SetInt("ScreenResolution", resolutionChosen);
                break;

            case 1:
                Screen.SetResolution(1680, 1050, Screen.fullScreenMode);
                PlayerPrefs.SetInt("ScreenResolution", resolutionChosen);
                break;

            case 2:
                Screen.SetResolution(1600, 900, Screen.fullScreenMode);
                PlayerPrefs.SetInt("ScreenResolution", resolutionChosen);
                break;

            case 3:
                Screen.SetResolution(1440, 900, Screen.fullScreenMode);
                PlayerPrefs.SetInt("ScreenResolution", resolutionChosen);
                break;

            case 4:
                Screen.SetResolution(1366, 768, Screen.fullScreenMode);
                PlayerPrefs.SetInt("ScreenResolution", resolutionChosen);
                break;

            case 5:
                Screen.SetResolution(1360, 768, Screen.fullScreenMode);
                PlayerPrefs.SetInt("ScreenResolution", resolutionChosen);
                break;

            case 6:
                Screen.SetResolution(1280, 1024, Screen.fullScreenMode);
                PlayerPrefs.SetInt("ScreenResolution", resolutionChosen);
                break;
                
            case 7:
                Screen.SetResolution(1280, 800, Screen.fullScreenMode);
                PlayerPrefs.SetInt("ScreenResolution", resolutionChosen);
                break;

            case 8:
                Screen.SetResolution(1280, 720, Screen.fullScreenMode);
                PlayerPrefs.SetInt("ScreenResolution", resolutionChosen);
                break;

            case 9:
                Screen.SetResolution(1024, 768, Screen.fullScreenMode);
                PlayerPrefs.SetInt("ScreenResolution", resolutionChosen);
                break;

            case 10:
                Screen.SetResolution(800, 600, Screen.fullScreenMode);
                PlayerPrefs.SetInt("ScreenResolution", resolutionChosen);
                break;

            default:
                Debug.LogError("Wrong resolution chosen!");
                break;
        }
    }

    private void ChangeResolution(Resolution resolutionChosen) {
        Screen.SetResolution(resolutionChosen.width, resolutionChosen.height, Screen.fullScreenMode, resolutionChosen.refreshRate);
    }
}
