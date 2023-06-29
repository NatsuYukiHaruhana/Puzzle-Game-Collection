using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SudokuBoard : MonoBehaviour
{
    public enum BoardType
    {
        Small, // 6x6 board
        Big // 9x9 board
    }

    private int width,
                height;

    [SerializeField]
    private GameObject tilePrefab,
                       boardParent,
                       gameScreenObject,
                       highscoreObject,
                       gameOverObject;
    [SerializeField]
    private HighscoreTable highscoreTable;
    [SerializeField]
    private TextMeshProUGUI timeSpent;

    [SerializeField]
    private Sprite[] numberTiles;
    [SerializeField]
    private Sprite emptyTile;
    
    private List<SudokuTile> tileArray;
    private SudokuTile selectedTile;

    private bool solved;

    int score;

    private void Start()
    {
        width = SudokuBoardVars.Width;
        height = SudokuBoardVars.Height;

        tileArray = new List<SudokuTile>();
        selectedTile = null;
        solved = false;

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                GameObject tile = Instantiate(tilePrefab, boardParent.transform);

                SudokuTile tileEntity = tile.AddComponent<SudokuTile>();
                tileEntity.Init(new Position(x, y), ("Tile " + y + " " + x));
                tileEntity.SetTileType(SudokuTile.Type.NonModifiable);

                tileArray.Add(tileEntity);
            }
        }

        GenerateBoard();
    }

    private void Update()
    {
        if (!solved) {
            timeSpent.SetText("Time spent: " + Mathf.FloorToInt(Time.timeSinceLevelLoad).ToString());

            if (Input.GetMouseButtonDown(0)) { // left click
                Vector3Int mousePos = GetMouseWorldPosition();
                if (mousePos.x >= 0 && mousePos.x < width && mousePos.y >= 0 && mousePos.y < height) {
                    int tileIndex = mousePos.y * width + mousePos.x;
                    selectedTile = tileArray[tileIndex];
                }
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha1)) {
                ChangeTileNumber(1);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2)) {
                ChangeTileNumber(2);
            }

            if (Input.GetKeyDown(KeyCode.Alpha3)) {
                ChangeTileNumber(3);
            }

            if (Input.GetKeyDown(KeyCode.Alpha4)) {
                ChangeTileNumber(4);
            }

            if (Input.GetKeyDown(KeyCode.Alpha5)) {
                ChangeTileNumber(5);
            }

            if (Input.GetKeyDown(KeyCode.Alpha6)) {
                ChangeTileNumber(6);
            }

            if (Input.GetKeyDown(KeyCode.Alpha7) && width == 9) {
                ChangeTileNumber(7);
            }

            if (Input.GetKeyDown(KeyCode.Alpha8) && width == 9) {
                ChangeTileNumber(8);
            }

            if (Input.GetKeyDown(KeyCode.Alpha9) && width == 9) {
                ChangeTileNumber(9);
            }
        }
    }

    private void ChangeTileNumber(int number) { // this changes the tile's number, but also checks if the board is solved
        if (selectedTile == null) {
            return;
        }

        if (selectedTile.GetTileType() == SudokuTile.Type.Modifiable) {
            if (selectedTile.GetNumber() == number) {
                selectedTile.SetNumber(0);
                selectedTile.SetSprite(emptyTile);
            } else {
                selectedTile.SetNumber(number);
                selectedTile.SetSprite(numberTiles[number - 1]);

                IsSolved();

                if (solved) {
                    gameOverObject.SetActive(true);
                    gameOverObject.GetComponentInChildren<TextMeshProUGUI>().SetText("You won!");

                    score = SudokuBoardVars.Width * 10000 - Mathf.FloorToInt(Time.timeSinceLevelLoad);
                    AddHighscoreEntry();
                }
            }
        }
    }

    private bool IsValid(int x, int y, int num) {
        for (int checkY = 0; checkY < height; checkY++) {
            if (checkY == y) {
                continue;
            }
            if (tileArray[checkY * width + x].GetCorrectNumber() == num) {
                return false;
            }
        }

        for (int checkX = 0; checkX < width; checkX++) {
            if (checkX == x) {
                continue;
            }

            if (tileArray[y * width + checkX].GetCorrectNumber() == num) {
                return false;
            }
        }

        int rectX = (x - x % 3),
            rectY = (y - y % (height / 3));

        for (int checkX = 0; checkX < 3; checkX++) {
            for (int checkY = 0; checkY < (height / 3); checkY++) {
                if (checkX + rectX == x && checkY + rectY == y) {
                    continue;
                }

                Debug.Log((rectY + checkY) * width + rectX + checkX);
                if (tileArray[(rectY + checkY) * width + rectX + checkX].GetCorrectNumber() == num) {
                    return false;
                }
            }
        }

        return true;
    }

    private bool SolveSudoku() {
        int x = -1, y = -1;
        bool isEmpty = true;
        for (int checkX = 0; checkX < width; checkX++) {
            for (int checkY = 0; checkY < height; checkY++) {
                if (tileArray[checkY * width + checkX].GetCorrectNumber() == 0) {
                    x = checkX;
                    y = checkY;

                    isEmpty = false;
                    break;
                }
            }

            if (!isEmpty) {
                break;
            }
        }

        if (isEmpty) {
            return true;
        }

        List<int> nums = new List<int>();
        for (int num = 1; num <= width; num++) {
            nums.Add(num);
        }

        while(nums.Count != 0) { 
            int num = nums[UnityEngine.Random.Range(0, nums.Count)];
            nums.Remove(num);
            if (IsValid(x, y, num)) {
                tileArray[y * width + x].SetCorrectNumber(num);
                if (SolveSudoku()) {
                    return true;
                } else {
                    tileArray[y * width + x].SetCorrectNumber(0);
                }
            }
        }

        return false;
    }

    private void GenerateBoard() {
        List<int> positions = new List<int>();
        List<int> visitedPositions = new List<int>();
        bool forcedNextPosition = false;

        List<List<int>> availableNums = new List<List<int>>();

        for (int i = 0; i < width * height; i++) {
            positions.Add(i);
            availableNums.Add(new List<int>());

            for (int num = 1; num <= width; num++) {
                availableNums[i].Add(num);
            }
        }

        int position = 0;
        do {
            if (!forcedNextPosition) { 
                position = positions[UnityEngine.Random.Range(0, positions.Count)];
            } else {
                int nextPos = UnityEngine.Random.Range(0, visitedPositions.Count);

                availableNums[nextPos].Remove(tileArray[visitedPositions[nextPos]].GetCorrectNumber());
                tileArray[visitedPositions[nextPos]].SetCorrectNumber(0);

                positions.Add(visitedPositions[nextPos]);

                position = visitedPositions[nextPos];
                visitedPositions.RemoveAt(nextPos);
            }

            int x = position % width, y = position / width;

            for (int checkX = 0; checkX < width; checkX++) {
                if (checkX == x) {
                    continue;
                }

                if (availableNums[position].Contains(tileArray[y * width + checkX].GetCorrectNumber())) {
                    availableNums[position].Remove(tileArray[y * width + checkX].GetCorrectNumber());
                }
            }
            for (int checkY = 0; checkY < height; checkY++) {
                if (checkY == y) {
                    continue;
                }

                if (availableNums[position].Contains(tileArray[checkY * width + x].GetCorrectNumber())) {
                    availableNums[position].Remove(tileArray[checkY * width + x].GetCorrectNumber());
                }
            }

            int rectX = (x - x % 3), rectY = (y - y % (height / 3));

            for (int checkX = 0; checkX < 3; checkX++) {
                for (int checkY = 0; checkY < (height / 3); checkY++) {
                    if ((rectY + checkY) * width + rectX + checkX == position) {
                        continue;
                    }

                    if (availableNums[position].Contains(tileArray[(rectY + checkY) * width + rectX + checkX].GetCorrectNumber())) {
                        availableNums[position].Remove(tileArray[(rectY + checkY) * width + rectX + checkX].GetCorrectNumber());
                    }
                }
            }

            if (availableNums[position].Count == 0) {
                forcedNextPosition = true;

                for (int num = 1; num <= width; num++) {
                    availableNums[position].Add(num);
                }

                continue;
            } else {
                forcedNextPosition = false;
            }

            bool isValid;
            do { 
                tileArray[position].SetCorrectNumber(availableNums[position][UnityEngine.Random.Range(0, availableNums[position].Count)]);
                isValid = SolveSudoku();

                if (!isValid) {
                    availableNums[position].Remove(tileArray[position].GetCorrectNumber());

                    if (availableNums[position].Count == 0) {
                        forcedNextPosition = true;

                        for (int num = 1; num <= width; num++) {
                            availableNums[position].Add(num);
                        }

                        isValid = true;
                    }
                }
            } while (!isValid);

            if (isValid) {
                visitedPositions.Add(position);
                tileArray[position].SetSprite(numberTiles[tileArray[position].GetCorrectNumber() - 1]);
                positions.Remove(position);
            }
        } while (positions.Count != 0);

        int hideTilesNum = width == 3 ? UnityEngine.Random.Range(42, 63) : UnityEngine.Random.Range(15, 23);

        for (int i = 0; i < width * height; i++) {
            positions.Add(i);
        }

        List<int> removedNumsList = new List<int>();
        for (int i = 0; i < width; i++) {
            removedNumsList.Add(0);
        }

        for (int i = 0; i < hideTilesNum; i++) {
            do {
                position = positions[UnityEngine.Random.Range(0, positions.Count)];
            } while (removedNumsList[tileArray[position].GetCorrectNumber() - 1] == width - 2);

            tileArray[position].SetTileType(SudokuTile.Type.Modifiable);
            tileArray[position].SetSprite(emptyTile);
            positions.Remove(position);
            removedNumsList[tileArray[position].GetCorrectNumber() - 1]++;
        }

        foreach(int pos in positions) {
            tileArray[pos].SetNumber(tileArray[pos].GetCorrectNumber());
        }
    }

    private void IsSolved() {
        foreach(SudokuTile tile in tileArray) {
            if (tile.GetNumber() != tile.GetCorrectNumber()) {
                return;
            }
        }

        solved = true;
    }

    private Vector3Int GetMouseWorldPosition() {
        Camera cam = Camera.main;
        Vector3 mousePos = Input.mousePosition;

        Vector3 worldPos = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, cam.nearClipPlane));

        return new Vector3Int(Mathf.FloorToInt(worldPos.x), Mathf.FloorToInt(worldPos.y), Mathf.FloorToInt(worldPos.z));
    }

    public void BackToGameMenu() {
        SceneManager.LoadScene("Sudoku Main Menu Scene");
    }

    public void ShowHighscores() {
        gameScreenObject.SetActive(false);
        highscoreObject.SetActive(true);
    }

    private void AddHighscoreEntry() {
        highscoreTable.AddHighscoreEntry(score, CurrentUser.CurrentUserName, "Sudoku");
    }

    //Getters
    public int GetHeight() {
        return height;
    }

    public int GetWidth() {
        return width;
    }
}
