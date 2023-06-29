using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleshipsOptionsBehaviour : MonoBehaviour
{
    public void PlayGame(string type) {
        if (type == "CPU") {
            BattleshipsGameVars.Type = BattleshipsGameVars.GameType.AgainstCPU;
        } else if (type == "HumanLocal") {
            BattleshipsGameVars.Type = BattleshipsGameVars.GameType.AgainstHumanLocal;
        } else if (type == "HumanOnline") {
            BattleshipsGameVars.Type = BattleshipsGameVars.GameType.AgainstHumanOnline;
        } else {
            BattleshipsGameVars.Type = BattleshipsGameVars.GameType.NULL;
        }

        SceneManager.LoadScene("Battleships Game Scene");
    }
}
