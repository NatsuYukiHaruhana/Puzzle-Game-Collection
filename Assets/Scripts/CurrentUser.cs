using UnityEngine;
using TMPro;

public class CurrentUser : MonoBehaviour
{
    static private string currentUserName;

    public static string CurrentUserName {
        get {
            return currentUserName;
        }

        set {
            currentUserName = value;
        }
    }

    [SerializeField]
    private TMP_InputField userNameInputField;

    private void Awake() {
        if (!PlayerPrefs.HasKey("Username")) {
            currentUserName = "Guest";

            PlayerPrefs.SetString("Username", currentUserName);
            PlayerPrefs.Save();
        } else {
            currentUserName = PlayerPrefs.GetString("Username");
        }

        userNameInputField.SetTextWithoutNotify(currentUserName);
    }

    public void SetCurrentUserName(string newValue) {
        currentUserName = newValue;

        PlayerPrefs.SetString("Username", currentUserName);
        PlayerPrefs.Save();
    }
}
