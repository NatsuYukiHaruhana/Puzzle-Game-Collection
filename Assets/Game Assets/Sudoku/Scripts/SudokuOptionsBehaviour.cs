using UnityEngine;
using UnityEngine.SceneManagement;

public class SudokuOptionsBehaviour : MonoBehaviour
{
    public void NewGame(string gameType)
    {
        if (gameType == "6x6") {
            SudokuBoardVars.Width = SudokuBoardVars.Height = 6;
            SceneManager.LoadScene("Sudoku Game Scene");
        } else if (gameType == "9x9") {
            SudokuBoardVars.Width = SudokuBoardVars.Height = 9;
            SceneManager.LoadScene("Sudoku Game Scene");
        } else {
            Debug.LogError("SUDOKU ERROR: Board type is incorrect!");
        }
    }
}
