using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MinesweeperBoard : MonoBehaviour
{
    private int width,
                height;

    private int nMines;

    [SerializeField]
    private TextMeshProUGUI timeSpent,
                            nMinesLeft;

    [SerializeField]
    private GameObject tilePrefab,
                       boardParent;
    private List<MinesweeperTile> tileArray;

    [SerializeField]
    private GameObject gameOver, gameScreenObject, highscoreObject;

    [SerializeField]
    private HighscoreTable highscoreTable;

    public Sprite[] numberTiles; // from sprite zero to sprite eight, based on the number of adjacent mines
    public Sprite neutral; // the default sprite
    public Sprite flagged; // an unrevealed, flagged tile sprite
    public Sprite questionMark; // if the user is unsure whether there is a mine or not there
    public Sprite mine; // the mine sprite
    public Sprite mineFlagged; // if a mine was correctly flagged, but the user lost
    public Sprite wrongFlag; // if the user flagged a tile that was not a mine

    private bool isFirstClick = true;
    private bool isGameOver = false;

    private int score;
    
    private void Start() {
        width = MinesweeperBoardVars.Width;
        height = MinesweeperBoardVars.Height;
        nMines = MinesweeperBoardVars.NMines;

        Debug.Log(width + " " + height + " " + nMines);

        tileArray = new List<MinesweeperTile>();

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                GameObject tile = Instantiate(tilePrefab, boardParent.transform);

                MinesweeperTile tileEntity = tile.AddComponent<MinesweeperTile>();
                tileEntity.Init(new Position(x, y), ("Tile " + y + " " + x));

                tileArray.Add(tileEntity);
            }
        }

        score = 0;
    }

    private void Update() {
        if (isGameOver == false) {
            if (Input.GetMouseButtonDown(0)) { // left click
                Vector3Int mousePos = GetMouseWorldPosition();

                if (mousePos.x >= 0 && mousePos.x < width && mousePos.y >= 0 && mousePos.y < height) {
                    if (isFirstClick) {
                        if (nMines != height * width - 1) { 
                            AddMines(new Position(mousePos.x, mousePos.y));
                        } 
                        else {
                            AddMines(new Position(-1, -1));
                        }
                        isFirstClick = false;
                    }
                    int tileIndex = mousePos.y * width + mousePos.x;

                    if (tileArray[tileIndex].GetState() == MinesweeperTile.State.Neutral) {
                        if (!tileArray[tileIndex].GetHasMine()) { 
                            UncoverEmptySpace(tileArray[tileIndex]);

                            if (IsGameWon()) {
                                gameOver.SetActive(true);
                                gameOver.GetComponentInChildren<TextMeshProUGUI>().SetText("You won!");
                                isGameOver = true;

                                score = MinesweeperBoardVars.NMines * 100 - Mathf.FloorToInt(Time.timeSinceLevelLoad);
                                AddHighscoreEntry();
                            }
                        } else {
                            UncoverWholeBoard();
                            gameOver.SetActive(true);
                            gameOver.GetComponentInChildren<TextMeshProUGUI>().SetText("You lost...");
                            isGameOver = true;
                        }
                    }
                }
            }

            if (Input.GetMouseButtonDown(1) && !isFirstClick) { // right click
                Vector3Int mousePos = GetMouseWorldPosition();
                int tileIndex = mousePos.y * width + mousePos.x;
                MinesweeperTile.State tileState = tileArray[tileIndex].GetState();

                if (tileState == MinesweeperTile.State.Neutral) {
                    tileArray[tileIndex].SetState(MinesweeperTile.State.Flagged);
                    tileArray[tileIndex].SetSprite(flagged);

                    nMines--;
                } else if (tileState == MinesweeperTile.State.Flagged) {
                    tileArray[tileIndex].SetState(MinesweeperTile.State.QuestionMarked);
                    tileArray[tileIndex].SetSprite(questionMark);

                    nMines++;
                } else if (tileState == MinesweeperTile.State.QuestionMarked) {
                    tileArray[tileIndex].SetState(MinesweeperTile.State.Neutral);
                    tileArray[tileIndex].SetSprite(neutral);
                }
            }

            nMinesLeft.SetText("Mines left: " + nMines);
            timeSpent.SetText("Time spent: " + Mathf.FloorToInt(Time.timeSinceLevelLoad));
        }
    }

    private Vector3Int GetMouseWorldPosition() {
        Camera cam = Camera.main;
        Vector3 mousePos = Input.mousePosition;

        Vector3 worldPos = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, cam.nearClipPlane));

        return new Vector3Int(Mathf.FloorToInt(worldPos.x), Mathf.FloorToInt(worldPos.y), Mathf.FloorToInt(worldPos.z));
    }

    private void AddMines(Position mousePos) {
        if (mousePos.x == -1 && mousePos.y == -1) {
            Position freePos = new Position(UnityEngine.Random.Range(0, width - 1), UnityEngine.Random.Range(0, height - 1));
            // ^ since only one position will be free of mines, we randomly choose one space free of any mines

            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    if (new Position(x, y) != freePos) {
                        tileArray[y * width + x].SetMine();
                    }
                }
            }

            tileArray[freePos.y * width + freePos.x].SetAdjacentMines(CountAdjacentMines(tileArray[freePos.y * width + freePos.x]));
            return;
        }

        for (int i = 0; i < nMines; i++) {
            Position minePos = new Position(UnityEngine.Random.Range(0, width - 1), UnityEngine.Random.Range(0, height - 1));
            int timesReset = 0;
            // in case a mine doesn't get placed somewhere for 10 times, then it is placed in the next available position starting from the lower left.
            // this was done for people who would select a very large number of mines, such as the maximum amount.

            while (tileArray[minePos.y * width + minePos.x].GetHasMine() || minePos == mousePos) {
                minePos = new Position(UnityEngine.Random.Range(0, width - 1), UnityEngine.Random.Range(0, height - 1));
                timesReset++;

                if (timesReset == 10) {
                    break;
                }
            }
            
            if (timesReset != 10) { // means we've found a valid spot, so we place it there 
                tileArray[minePos.y * width + minePos.x].SetMine();
            } else { // we could find a valid spot, so we place it on the next available one starting from the lower left
                bool minePlaced = false;

                for (int x = 0; x < width && !minePlaced; x++) {
                    for (int y = 0; y < height; y++) {
                        if (tileArray[y * width + x].GetHasMine() == false) {
                            tileArray[y * width + x].SetMine();
                            minePlaced = true;
                            break;
                        }
                    }
                }
            }
        }

        foreach (MinesweeperTile tile in tileArray) {
            tile.SetAdjacentMines(CountAdjacentMines(tile));
        }
    }

    private int CountAdjacentMines(MinesweeperTile tile) {
        if (tile.GetHasMine()) {
            return 0;
        }

        int nAdjacentMines = 0;
        Position tilePos = tile.GetGridPos();
        
        for (int y = tilePos.y + 1; y >= tilePos.y - 1; y--) {
            if (y < 0 || y >= height) {
                continue;
            }

            for (int x = tilePos.x - 1; x <= tilePos.x + 1; x++) {
                if ((x == tilePos.x && y == tilePos.y) || (x < 0) || (x >= width)) {
                    continue;
                }

                if (tileArray[y * width + x].GetHasMine()) {
                    nAdjacentMines++;
                }
            }
        }

        return nAdjacentMines;
    }

    private void UncoverEmptySpace(MinesweeperTile tile) {
        tile.SetSprite(numberTiles[tile.GetnAdjacentMines()]);
        tile.SetState(MinesweeperTile.State.Revealed);

        if (tile.GetnAdjacentMines() != 0) {
            return;
        }

        Position tilePos = tile.GetGridPos();

        for (int y = tilePos.y + 1; y >= tilePos.y - 1; y--) {
            if (y < 0 || y >= height) {
                continue;
            }

            for (int x = tilePos.x - 1; x <= tilePos.x + 1; x++) {
                if ((x == tilePos.x && y == tilePos.y) || (x < 0) || (x >= width)) {
                    continue;
                }
                
                if (tileArray[y * width + x].GetState() == MinesweeperTile.State.Neutral) {
                    UncoverEmptySpace(tileArray[y * width + x]);
                }
            }
        }
    }

    private void UncoverWholeBoard() {
        foreach(MinesweeperTile tile in tileArray) {
            MinesweeperTile.State tileState = tile.GetState();

            if (tileState == MinesweeperTile.State.Neutral || tileState == MinesweeperTile.State.QuestionMarked) { 
                if (tile.GetHasMine()) {
                    tile.SetSprite(mine);
                } else {
                    tile.SetSprite(numberTiles[tile.GetnAdjacentMines()]);
                }
            }   
            else if (tileState == MinesweeperTile.State.Flagged) {
                if (tile.GetHasMine()) {
                    tile.SetSprite(mineFlagged);
                } else {
                    tile.SetSprite(wrongFlag);
                }
            }
        }
    }

    private bool IsGameWon() {
        foreach (MinesweeperTile tile in tileArray) {
            if (!tile.GetHasMine() && tile.GetState() != MinesweeperTile.State.Revealed) {
                return false;
            }
        }

        return true;
    }

    public void ReturnToMainMenu() {
        SceneManager.LoadScene("Minesweeper Main Menu Scene");
    }

    public void ShowHighscores() {
        gameScreenObject.SetActive(false);
        highscoreObject.SetActive(true);
    }

    private void AddHighscoreEntry() {
        highscoreTable.AddHighscoreEntry(score, CurrentUser.CurrentUserName, "Minesweeper");
    }

    //Getters
    public int GetWidth() {
        return width;
    }

    public int GetHeight() {
        return height;
    }
}