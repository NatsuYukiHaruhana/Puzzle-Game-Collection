using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class _2048BoardBehaviour : MonoBehaviour
{
    private enum Direction {
        Up,
        Down,
        Left,
        Right,
        NULL
    }

    private enum TileType {
        Free,
        Occupied,
        CanConvert,
        NULL
    }

    private const int size = 4; // both the width and height are 4
    private bool endless;
    private int limit;
    private bool isGameOver;
    private int moves;
    private bool firstMove;
    private int score;
    private Dictionary<Direction, Vector2Int> directions;
    private List<int> possibleFirstNums;

    [SerializeField]
    private TextMeshProUGUI timeSpent, movesDone;

    [SerializeField]
    private GameObject tilePrefab, boardParent, gameOver, gameScreenObject, highscoreObject;

    [SerializeField]
    private HighscoreTable highscoreTable;

    [SerializeField]
    private List<Sprite> colorTiles;

    private List<_2048TileBehaviour> tileArray;
    private List<Position> freePositions;

    private void Start()
    {
        endless = _2048BoardVars.Endless;
        limit = _2048BoardVars.Limit;
        isGameOver = false;
        moves = 0;
        score = 0;
        firstMove = true;
        movesDone.SetText("Moves done: " + moves.ToString());

        directions = new Dictionary<Direction, Vector2Int>();
        directions.Add(Direction.Up, new Vector2Int(0, -1));
        directions.Add(Direction.Down, new Vector2Int(0, 1));
        directions.Add(Direction.Left, new Vector2Int(-1, 0));
        directions.Add(Direction.Right, new Vector2Int(1, 0));

        possibleFirstNums = new List<int>();
        possibleFirstNums.Add(2);
        possibleFirstNums.Add(4);

        tileArray = new List<_2048TileBehaviour>();
        for (int i = 0; i < size * size; i++) {
            tileArray.Add(null);
        }

        freePositions = new List<Position>();

        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                //Debug.Log("Created tile " + x + " " + y);
                freePositions.Add(new Position(x, y));
            }
        }

        CreateNewTile();
    }

    private void Update()
    {
        if (!isGameOver) {
            timeSpent.SetText("Time spent: " + Mathf.FloorToInt(Time.timeSinceLevelLoad));
            foreach (_2048TileBehaviour tile in tileArray) {
                if (tile == null) {
                    continue;
                }

                tile.SetWasMoved(false);
                tile.SetWasModified(false);
            }

            Direction currDirection = Direction.NULL;

            if (Input.GetKeyDown(KeyCode.UpArrow)) {
                currDirection = Direction.Up;
            } else if (Input.GetKeyDown(KeyCode.DownArrow)) {
                currDirection = Direction.Down;
            } else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
                currDirection = Direction.Left;
            } else if (Input.GetKeyDown(KeyCode.RightArrow)) {
                currDirection = Direction.Right;
            }

            MoveTiles(currDirection);

            if (currDirection != Direction.NULL) {
                moves++;
                movesDone.SetText("Moves done: " + moves.ToString());

                if (freePositions.Count != 0) { 
                    CreateNewTile();
                }

                if (IsGameOver()) {
                    isGameOver = true;
                    gameOver.GetComponentInChildren<TextMeshProUGUI>().SetText("You lost...");
                    gameOver.SetActive(true);

                    int maxScore = tileArray[0].GetNumber();
                    for (int i = 1; i < tileArray.Count; i++) {
                        if (maxScore < tileArray[i].GetNumber()) {
                            maxScore = tileArray[i].GetNumber();
                        }
                    }

                    score = maxScore * 100 - moves - Mathf.FloorToInt(Time.timeSinceLevelLoad);
                    AddHighscoreEntry();
                }

                if (!endless) {
                    if (IsWon()) {
                        isGameOver = true;
                        gameOver.GetComponentInChildren<TextMeshProUGUI>().SetText("You won!");
                        gameOver.SetActive(true);

                        score = _2048BoardVars.Limit * 100 - moves - Mathf.FloorToInt(Time.timeSinceLevelLoad);
                        AddHighscoreEntry();
                    }
                }
            }
        }
    }

    private void CreateNewTile() {
        GameObject newTile = Instantiate(tilePrefab, boardParent.transform);
        _2048TileBehaviour tileEntity = newTile.AddComponent<_2048TileBehaviour>();

        int takenPosition = UnityEngine.Random.Range(0, freePositions.Count), firstNum;
        if (firstMove) { 
            firstNum = 2;
            firstMove = false;
        } else {
            firstNum = possibleFirstNums[UnityEngine.Random.Range(0, possibleFirstNums.Count)];
        }

        tileEntity.Init(freePositions[takenPosition], firstNum, 
            "Tile " + freePositions[takenPosition].x + ", " + freePositions[takenPosition].y, colorTiles[PowerOf2(firstNum) % colorTiles.Count - 1]);

        tileArray[freePositions[takenPosition].y * size + freePositions[takenPosition].x] = tileEntity;
        //Debug.Log("Taken position " + takenPosition + " x = " + freePositions[takenPosition].x + " y = " + freePositions[takenPosition].y);
        freePositions.RemoveAt(takenPosition);
    }

    private bool IsGameOver() {
        if (freePositions.Count == 0) {
            foreach (_2048TileBehaviour tile in tileArray) {
                foreach (KeyValuePair<Direction, Vector2Int> dir in directions) {
                    TileType tileType = NextPosition(dir.Key, tile).Key;
                    if (tileType == TileType.Free || tileType == TileType.CanConvert) {
                        return false;
                    }
                }
            }

            return true;
        }

        return false;
    }

    private bool IsWon() {
        foreach(_2048TileBehaviour tile in tileArray) {
            if (tile == null) {
                continue;
            }

            if (tile.GetNumber() == limit) {
                return true;
            }
        }

        return false;
    }

    private KeyValuePair<TileType, Position> NextPosition(Direction direction, _2048TileBehaviour tile) {
        Position newPosition = tile.GetPosition() + directions[direction];

        if (newPosition.x < 0 || newPosition.x >= size || newPosition.y < 0 || newPosition.y >= size) {
            return new KeyValuePair<TileType, Position>(TileType.NULL, newPosition);
        }

        if (tileArray[newPosition.y * size + newPosition.x] == null) {
            newPosition = newPosition + directions[direction];

            while (newPosition.x >= 0 && newPosition.x < size && newPosition.y >= 0 && newPosition.y < size) {
                if (tileArray[newPosition.y * size + newPosition.x] != null) {
                    if (tileArray[newPosition.y * size + newPosition.x].GetNumber() == tile.GetNumber() && 
                        tileArray[newPosition.y * size + newPosition.x].GetWasModified() == false) {
                        return new KeyValuePair<TileType, Position>(TileType.CanConvert, newPosition);
                    } else {
                        return new KeyValuePair<TileType, Position>(TileType.Free, newPosition - directions[direction]);
                    }
                }

                newPosition = newPosition + directions[direction];
            }

            return new KeyValuePair<TileType, Position>(TileType.Free, newPosition - directions[direction]);
        }

        if (tileArray[newPosition.y * size + newPosition.x].GetNumber() == tile.GetNumber() &&
            tileArray[newPosition.y * size + newPosition.x].GetWasModified() == false) {
            return new KeyValuePair<TileType, Position>(TileType.CanConvert, newPosition);
        }

        return new KeyValuePair<TileType, Position>(TileType.Occupied, newPosition);
    }

    private void MoveTiles(Direction direction) {
        switch(direction) {
            case Direction.Up: {
                for (int y = 1; y < size; y++) {
                    for (int x = 0; x < size; x++) {
                        if (tileArray[y * size + x] == null) {
                            continue;
                        }

                        MoveTile(direction, tileArray[y * size + x]);
                    }
                }

                break;
            }

            case Direction.Down: {
                for (int y = size - 2; y >= 0; y--) {
                    for (int x = 0; x < size; x++) {
                        if (tileArray[y * size + x] == null) {
                            continue;
                        }

                        MoveTile(direction, tileArray[y * size + x]);
                    }
                }

                break;
            }

            case Direction.Left: {
                for (int x = 1; x < size; x++) {
                    for (int y = 0; y < size; y++) {
                        if (tileArray[y * size + x] == null) {
                            continue;
                        }

                        MoveTile(direction, tileArray[y * size + x]);
                    }
                }

                break;
            }

            case Direction.Right: {
                for (int x = size - 2; x >= 0; x--) {
                    for (int y = 0; y < size; y++) {
                        if (tileArray[y * size + x] == null) {
                            continue;
                        }

                        MoveTile(direction, tileArray[y * size + x]);
                    }
                }

                break;
            }

            case Direction.NULL: { // no direction given this frame
                break;
            }
        }
    }

    private void MoveTile(Direction direction, _2048TileBehaviour tile) {
        KeyValuePair<TileType, Position> nextPosition = NextPosition(direction, tile);

        if (nextPosition.Key == TileType.Free) {
            //Debug.Log("Moved");
            int x = tile.GetPosition().x, y = tile.GetPosition().y;
            
            tile.SetPosition(nextPosition.Value);
            tileArray[nextPosition.Value.y * size + nextPosition.Value.x] = tile;

            tileArray[y * size + x] = null;
            freePositions.Add(new Position(x, y));
            freePositions.Remove(nextPosition.Value);
        } else if (nextPosition.Key == TileType.CanConvert) {
            //Debug.Log("Converted");
            int x = tile.GetPosition().x, y = tile.GetPosition().y;
            
            freePositions.Add(new Position(x, y));
            Destroy(tileArray[y * size + x].gameObject);
            tileArray[y * size + x] = null;

            Sprite newSprite = colorTiles[(PowerOf2(tileArray[nextPosition.Value.y * size + nextPosition.Value.x].GetNumber()) + 1) % colorTiles.Count - 1];
            tileArray[nextPosition.Value.y * size + nextPosition.Value.x].ChangeNumber(newSprite);
        }
    }

    private int PowerOf2(int value) {
        if (!Mathf.IsPowerOfTwo(value)) {
            return -1;
        }

        int power = 0;
        while (value != 1) {
            value /= 2;
            power++;
        }

        return power;
    }

    public void ReturnToMainMenu() {
        SceneManager.LoadScene("2048 Main Menu Scene");
    }

    public void ShowHighscores() {
        gameScreenObject.SetActive(false);
        highscoreObject.SetActive(true);
    }

    private void AddHighscoreEntry() {
        highscoreTable.AddHighscoreEntry(score, CurrentUser.CurrentUserName, "2048");
    }
}
