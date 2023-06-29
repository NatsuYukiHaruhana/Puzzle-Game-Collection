using UnityEngine;
using UnityEngine.SceneManagement;

public class KlondikeSolitaireNewGameBehaviour : MonoBehaviour
{
    public void StartGame(string type) {
        if (type == "Draw 1") {
            KlondikeSolitaireVars.Type = KlondikeSolitaireVars.GameType.Draw1;
        } else if (type == "Draw 3") {
            KlondikeSolitaireVars.Type = KlondikeSolitaireVars.GameType.Draw3;
        } else {
            KlondikeSolitaireVars.Type = KlondikeSolitaireVars.GameType.NULL;
        }

        SceneManager.LoadScene("Klondike Solitaire Game Scene");
    }
}
