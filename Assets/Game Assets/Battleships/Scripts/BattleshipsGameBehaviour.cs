using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class BattleshipsGameBehaviour : MonoBehaviour
{
    private const int width = 10, height = 10;
    private const int p1BoardMinX = -11, p1BoardMaxX = -2, p2BoardMinX = 0, p2BoardMaxX = 9, boardMinY = -9, boardMaxY = 0;

    [SerializeField]
    private TextMeshProUGUI player2Text, playerTurnText;

    [SerializeField]
    private GameObject gameOverObject;

    [SerializeField]
    private Sprite[] shipSprites;

    [SerializeField]
    private Sprite[] overlaySprites;

    [SerializeField]
    private GameObject shipParentPrefab, shipChunkPrefab, tilePrefab, boardPrefab;

    private List<BattleshipsShipBehaviour> p1Ships, p2Ships;
    private List<BattleshipsTileBehaviour> p1Board, p2Board;
    private GameObject p1BoardParent, p2BoardParent;
    
    private BattleshipsCPUBehaviour CPU;

    private BattleshipsShipBehaviour selectedShip = null;
    private bool p1Turn = true;
    private bool attackPhase = false;
    private bool gameOver = false;
    private string winner = null;

    // Start is called before the first frame update
    void Start()
    {
        if (BattleshipsGameVars.Type == BattleshipsGameVars.GameType.AgainstCPU) {
            player2Text.SetText("CPU");
            CPU = GameObject.Find("CPU").GetComponent<BattleshipsCPUBehaviour>();
        } else {
            player2Text.SetText("Player 2");
            GameObject.Find("CPU").SetActive(false);
        }
        playerTurnText.SetText("Player 1's turn!");

        p1Ships = new List<BattleshipsShipBehaviour>();
        p2Ships = new List<BattleshipsShipBehaviour>();

        int[] shipSizes = {2, 3, 3, 4, 5}; float startPosX = -width + 1;
        for (int i = 0; i < 5; i++) {
            GameObject p1Ship = Instantiate<GameObject>(shipParentPrefab, this.transform),
                        p2Ship = Instantiate<GameObject>(shipParentPrefab, this.transform);

            p1Ships.Add(p1Ship.AddComponent<BattleshipsShipBehaviour>());
            p1Ships[i].Init(shipSizes[i], shipChunkPrefab, shipSprites, new Vector3(startPosX, -height));

            p2Ships.Add(p2Ship.AddComponent<BattleshipsShipBehaviour>());
            p2Ships[i].Init(shipSizes[i], shipChunkPrefab, shipSprites, new Vector3(startPosX, -height), false);

            startPosX += shipSizes[i] + 0.25f;
        }

        p1Board = new List<BattleshipsTileBehaviour>();
        p2Board = new List<BattleshipsTileBehaviour>();

        p1BoardParent = Instantiate<GameObject>(boardPrefab, new Vector3(-width - .5f, 0.5f), new Quaternion(0, 0, 0, 0));
        p1BoardParent.name = "Player 1 Board";
        p2BoardParent = Instantiate<GameObject>(boardPrefab, new Vector3(.5f, 0.5f), new Quaternion(0, 0, 0, 0));
        p2BoardParent.name = "Player 2 Board";

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                GameObject p1Tile = Instantiate<GameObject>(tilePrefab, p1BoardParent.transform),
                            p2Tile = Instantiate<GameObject>(tilePrefab, p2BoardParent.transform);

                p1Board.Add(p1Tile.AddComponent<BattleshipsTileBehaviour>());
                p2Board.Add(p2Tile.AddComponent<BattleshipsTileBehaviour>());

                p1Board[y * width + x].Init(new Position(x, y), "Tile " + y + " " + x);
                p2Board[y * width + x].Init(new Position(x, y), "Tile " + y + " " + x);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!attackPhase) {
            if (Input.GetKeyDown(KeyCode.Alpha1)) { // 1st ship
                if (selectedShip != null) {
                    if (selectedShip.GetPlaced() == false) {
                        if (selectedShip.GetRotation() != 0) {
                            selectedShip.SetRotation();
                        }
                        selectedShip.SetPositionOutsideOfBoard();
                    }
                }

                if (p1Turn == true) {
                    selectedShip = p1Ships[0];

                    if (selectedShip.GetPlaced() == false) { 
                        selectedShip.SetPositionAbsolute(new Position(0, 0), p1BoardParent.transform.position + new Vector3(1f, 0f));
                    } else {
                        Position currPosition = selectedShip.GetPosition(), movement;
                        if (selectedShip.GetRotation() == 0) {
                            movement = new Position(1, 0);
                        } else {
                            movement = new Position(0, -1);
                        }

                        for (int i = 0; i < selectedShip.GetSize(); i++) {
                            p1Board[currPosition.y * width + currPosition.x].SetContainsShip();
                            p1Board[currPosition.y * width + currPosition.x].SetShipContained(null);

                            currPosition += movement;
                        }
                    }
                } else {
                    selectedShip = p2Ships[0];

                    if (selectedShip.GetPlaced() == false) {
                        selectedShip.SetPositionAbsolute(new Position(0, 0), p2BoardParent.transform.position + new Vector3(1f, 0f));
                    } else {
                        Position currPosition = selectedShip.GetPosition(), movement;
                        if (selectedShip.GetRotation() == 0) {
                            movement = new Position(1, 0);
                        } else {
                            movement = new Position(0, -1);
                        }

                        for (int i = 0; i < selectedShip.GetSize(); i++) {
                            p2Board[currPosition.y * width + currPosition.x].SetContainsShip();
                            p2Board[currPosition.y * width + currPosition.x].SetShipContained(null);

                            currPosition += movement;
                        }
                    }
                }
            } else if (Input.GetKeyDown(KeyCode.Alpha2)) { // 2nd ship
                if (selectedShip != null) {
                    if (selectedShip.GetPlaced() == false) {
                        if (selectedShip.GetRotation() != 0) {
                            selectedShip.SetRotation();
                        }
                        selectedShip.SetPositionOutsideOfBoard();
                    }
                }

                if (p1Turn == true) {
                    selectedShip = p1Ships[1];

                    if (selectedShip.GetPlaced() == false) {
                        selectedShip.SetPositionAbsolute(new Position(0, 0), p1BoardParent.transform.position + new Vector3(1f, 0f));
                    } else {
                        Position currPosition = selectedShip.GetPosition(), movement;
                        if (selectedShip.GetRotation() == 0) {
                            movement = new Position(1, 0);
                        } else {
                            movement = new Position(0, -1);
                        }

                        for (int i = 0; i < selectedShip.GetSize(); i++) {
                            p1Board[currPosition.y * width + currPosition.x].SetContainsShip();
                            p1Board[currPosition.y * width + currPosition.x].SetShipContained(null);

                            currPosition += movement;
                        }
                    }
                } else {
                    selectedShip = p2Ships[1];

                    if (selectedShip.GetPlaced() == false) {
                        selectedShip.SetPositionAbsolute(new Position(0, 0), p2BoardParent.transform.position + new Vector3(1f, 0f));
                    } else {
                        Position currPosition = selectedShip.GetPosition(), movement;
                        if (selectedShip.GetRotation() == 0) {
                            movement = new Position(1, 0);
                        } else {
                            movement = new Position(0, -1);
                        }

                        for (int i = 0; i < selectedShip.GetSize(); i++) {
                            p2Board[currPosition.y * width + currPosition.x].SetContainsShip();
                            p2Board[currPosition.y * width + currPosition.x].SetShipContained(null);

                            currPosition += movement;
                        }
                    }
                }
            } else if (Input.GetKeyDown(KeyCode.Alpha3)) { // 3rd ship
                if (selectedShip != null) {
                    if (selectedShip.GetPlaced() == false) {
                        if (selectedShip.GetRotation() != 0) {
                            selectedShip.SetRotation();
                        }
                        selectedShip.SetPositionOutsideOfBoard();
                    }
                }

                if (p1Turn == true) {
                    selectedShip = p1Ships[2];

                    if (selectedShip.GetPlaced() == false) {
                        selectedShip.SetPositionAbsolute(new Position(0, 0), p1BoardParent.transform.position + new Vector3(1f, 0f));
                    } else {
                        Position currPosition = selectedShip.GetPosition(), movement;
                        if (selectedShip.GetRotation() == 0) {
                            movement = new Position(1, 0);
                        } else {
                            movement = new Position(0, -1);
                        }

                        for (int i = 0; i < selectedShip.GetSize(); i++) {
                            p1Board[currPosition.y * width + currPosition.x].SetContainsShip();
                            p1Board[currPosition.y * width + currPosition.x].SetShipContained(null);

                            currPosition += movement;
                        }
                    }
                } else {
                    selectedShip = p2Ships[2];

                    if (selectedShip.GetPlaced() == false) {
                        selectedShip.SetPositionAbsolute(new Position(0, 0), p2BoardParent.transform.position + new Vector3(1f, 0f));
                    } else {
                        Position currPosition = selectedShip.GetPosition(), movement;
                        if (selectedShip.GetRotation() == 0) {
                            movement = new Position(1, 0);
                        } else {
                            movement = new Position(0, -1);
                        }

                        for (int i = 0; i < selectedShip.GetSize(); i++) {
                            p2Board[currPosition.y * width + currPosition.x].SetContainsShip();
                            p2Board[currPosition.y * width + currPosition.x].SetShipContained(null);

                            currPosition += movement;
                        }
                    }
                }
            } else if (Input.GetKeyDown(KeyCode.Alpha4)) { // 4th ship
                if (selectedShip != null) {
                    if (selectedShip.GetPlaced() == false) {
                        if (selectedShip.GetRotation() != 0) {
                            selectedShip.SetRotation();
                        }
                        selectedShip.SetPositionOutsideOfBoard();
                    }
                }

                if (p1Turn == true) {
                    selectedShip = p1Ships[3];

                    if (selectedShip.GetPlaced() == false) {
                        selectedShip.SetPositionAbsolute(new Position(0, 0), p1BoardParent.transform.position + new Vector3(2f, 0f));
                    } else {
                        Position currPosition = selectedShip.GetPosition(), movement;
                        if (selectedShip.GetRotation() == 0) {
                            movement = new Position(1, 0);
                        } else {
                            movement = new Position(0, -1);
                        }

                        for (int i = 0; i < selectedShip.GetSize(); i++) {
                            p1Board[currPosition.y * width + currPosition.x].SetContainsShip();
                            p1Board[currPosition.y * width + currPosition.x].SetShipContained(null);

                            currPosition += movement;
                        }
                    }
                } else {
                    selectedShip = p2Ships[3];

                    if (selectedShip.GetPlaced() == false) {
                        selectedShip.SetPositionAbsolute(new Position(0, 0), p2BoardParent.transform.position + new Vector3(2f, 0f));
                    } else {
                        Position currPosition = selectedShip.GetPosition(), movement;
                        if (selectedShip.GetRotation() == 0) {
                            movement = new Position(1, 0);
                        } else {
                            movement = new Position(0, -1);
                        }

                        for (int i = 0; i < selectedShip.GetSize(); i++) {
                            p2Board[currPosition.y * width + currPosition.x].SetContainsShip();
                            p2Board[currPosition.y * width + currPosition.x].SetShipContained(null);

                            currPosition += movement;
                        }
                    }
                }
            } else if (Input.GetKeyDown(KeyCode.Alpha5)) { // 5th ship
                if (selectedShip != null) {
                    if (selectedShip.GetPlaced() == false) {
                        if (selectedShip.GetRotation() != 0) {
                            selectedShip.SetRotation();
                        }
                        selectedShip.SetPositionOutsideOfBoard();
                    }
                }

                if (p1Turn == true) {
                    selectedShip = p1Ships[4];

                    if (selectedShip.GetPlaced() == false) {
                        selectedShip.SetPositionAbsolute(new Position(0, 0), p1BoardParent.transform.position + new Vector3(2f, 0f));
                    } else {
                        Position currPosition = selectedShip.GetPosition(), movement;
                        if (selectedShip.GetRotation() == 0) {
                            movement = new Position(1, 0);
                        } else {
                            movement = new Position(0, -1);
                        }

                        for (int i = 0; i < selectedShip.GetSize(); i++) {
                            p1Board[currPosition.y * width + currPosition.x].SetContainsShip();
                            p1Board[currPosition.y * width + currPosition.x].SetShipContained(null);

                            currPosition += movement;
                        }
                    }
                } else {
                    selectedShip = p2Ships[4];

                    if (selectedShip.GetPlaced() == false) {
                        selectedShip.SetPositionAbsolute(new Position(0, 0), p2BoardParent.transform.position + new Vector3(2f, 0f));
                    } else {
                        Position currPosition = selectedShip.GetPosition(), movement;
                        if (selectedShip.GetRotation() == 0) {
                            movement = new Position(1, 0);
                        } else {
                            movement = new Position(0, -1);
                        }

                        for (int i = 0; i < selectedShip.GetSize(); i++) {
                            p2Board[currPosition.y * width + currPosition.x].SetContainsShip();
                            p2Board[currPosition.y * width + currPosition.x].SetShipContained(null);

                            currPosition += movement;
                        }
                    }
                }
            } else if (Input.GetKeyDown(KeyCode.Return)) { // submitted positions
                bool canSubmit = true;
                if (p1Turn == true) {
                    foreach(BattleshipsShipBehaviour ship in p1Ships) {
                        if (ship.GetPlaced() == false) {
                            canSubmit = false;
                            break;
                        }
                    }
                } else {
                    foreach (BattleshipsShipBehaviour ship in p2Ships) {
                        if (ship.GetPlaced() == false) {
                            canSubmit = false;
                            break;
                        }
                    }
                }

                if (canSubmit) {
                    if (p1Turn == true && BattleshipsGameVars.Type == BattleshipsGameVars.GameType.AgainstCPU) {
                        CPU.CPUPlaceShips(width, height, p2Board, p2BoardParent, p2Ships, shipSprites);
                        attackPhase = true;
                    } else if (p1Turn == true && BattleshipsGameVars.Type == BattleshipsGameVars.GameType.AgainstHumanLocal) {
                        p1Turn = false;
                        foreach (BattleshipsShipBehaviour ship in p1Ships) {
                            ship.SetVisibility(false);
                        }
                        foreach (BattleshipsShipBehaviour ship in p2Ships) {
                            ship.SetVisibility(true);
                        }
                        playerTurnText.SetText(player2Text.GetParsedText() + "'s turn!");
                    } else if (p1Turn == false && BattleshipsGameVars.Type == BattleshipsGameVars.GameType.AgainstHumanLocal) {
                        p1Turn = true;
                        attackPhase = true;
                        foreach (BattleshipsShipBehaviour ship in p2Ships) {
                            ship.SetVisibility(false);
                        }
                        foreach (BattleshipsShipBehaviour ship in p1Ships) {
                            ship.SetVisibility(true);
                        }
                        playerTurnText.SetText("Player 1's turn!");
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) {
                if (selectedShip != null) {
                    if (selectedShip.GetPosition().y > 0) {
                        selectedShip.SetPosition(selectedShip.GetPosition() + new Position(0, -1));
                    }
                }
            } else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) {
                if (selectedShip != null) {
                    if (selectedShip.GetRotation() == 0) {
                        if (selectedShip.GetPosition().y < height - 1) {
                            selectedShip.SetPosition(selectedShip.GetPosition() + new Position(0, 1));
                        } 
                    } else {
                        if (selectedShip.GetPosition().y + selectedShip.GetSize() - 1 < height - 1) {
                            selectedShip.SetPosition(selectedShip.GetPosition() + new Position(0, 1));
                        }
                    }
                }
            } else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) {
                if (selectedShip != null) {
                    if (selectedShip.GetPosition().x > 0) { 
                        selectedShip.SetPosition(selectedShip.GetPosition() + new Position(-1, 0));
                    }
                }
            } else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) {
                if (selectedShip != null) {
                    if (selectedShip.GetRotation() != 0) { 
                        if (selectedShip.GetPosition().x < width - 1) { 
                            selectedShip.SetPosition(selectedShip.GetPosition() + new Position(1, 0));
                        }
                    } else {
                        if (selectedShip.GetPosition().x + selectedShip.GetSize() - 1 < width - 1) {
                            selectedShip.SetPosition(selectedShip.GetPosition() + new Position(1, 0));
                        }
                    }
                }
            } else if (Input.GetKeyDown(KeyCode.R)) { // rotate
                if (selectedShip != null) {
                    selectedShip.SetRotation(width, height);
                }
            } else if (Input.GetKeyDown(KeyCode.P)) { // place
                if (selectedShip != null) {
                    bool canPlace = true;
                    if (p1Turn) {
                        Position currPosition = selectedShip.GetPosition(), movement;
                        if (selectedShip.GetRotation() == 0) {
                            movement = new Position(1, 0);
                        } else {
                            movement = new Position(0, 1);
                        }

                        for (int i = 0; i < selectedShip.GetSize(); i++) {
                            if (p1Board[currPosition.y * width + currPosition.x].GetContainsShip()) {
                                canPlace = false;
                            }

                            currPosition += movement;
                        }

                        if (canPlace) {
                            currPosition = selectedShip.GetPosition();

                            for (int i = 0; i < selectedShip.GetSize(); i++) {
                                p1Board[currPosition.y * width + currPosition.x].SetContainsShip();
                                p1Board[currPosition.y * width + currPosition.x].SetShipContained(selectedShip);

                                currPosition += movement;
                            }
                        }
                    } else {
                        Position currPosition = selectedShip.GetPosition(), movement;
                        if (selectedShip.GetRotation() == 0) {
                            movement = new Position(1, 0);
                        } else {
                            movement = new Position(0, -1);
                        }

                        for (int i = 0; i < selectedShip.GetSize(); i++) {
                            if (p2Board[currPosition.y * width + currPosition.x].GetContainsShip()) {
                                canPlace = false;
                            }

                            currPosition += movement;
                        }

                        if (canPlace) {
                            currPosition = selectedShip.GetPosition();

                            for (int i = 0; i < selectedShip.GetSize(); i++) {
                                p2Board[currPosition.y * width + currPosition.x].SetContainsShip();
                                p2Board[currPosition.y * width + currPosition.x].SetShipContained(selectedShip);

                                currPosition += movement;
                            }
                        }
                    }

                    if (canPlace) {
                        selectedShip.SetPlaced(true);
                        selectedShip = null;
                    }
                }
            }
        } else if (!gameOver) { // battle phase
            if (Input.GetMouseButtonDown(0)) { // left click
                Vector3Int mousePosWorld = GetMouseWorldPosition();

                Debug.Log(mousePosWorld.x + " " + mousePosWorld.y);

                if (p1Turn) {
                    if (mousePosWorld.x >= p2BoardMinX && mousePosWorld.x <= p2BoardMaxX && mousePosWorld.y >= boardMinY && mousePosWorld.y <= boardMaxY) {
                        Position mousePos = new Position(mousePosWorld.x, Mathf.Abs(mousePosWorld.y));
                        int tileIndex = mousePos.y * width + mousePos.x;

                        if (!p2Board[tileIndex].GetWasHit()) {
                            p2Board[tileIndex].SetWasHit();

                            if (p2Board[tileIndex].GetContainsShip()) {
                                p2Board[tileIndex].SetOverlaySprite(overlaySprites[1]);
                                BattleshipsShipBehaviour shipHit = p2Board[tileIndex].GetShipContained();

                                if (shipHit.GetRotation() == 0) { // horizontal
                                    shipHit.SetChunkHit(Mathf.Abs(mousePos.x - shipHit.GetPosition().x));
                                } else { // vertical
                                    shipHit.SetChunkHit(Mathf.Abs(shipHit.GetPosition().y - mousePos.y));
                                }

                                if (shipHit.GetSunk() == true) {
                                    gameOver = true;
                                    foreach (BattleshipsShipBehaviour ship in p2Ships) {
                                        if (ship.GetSunk() == false) {
                                            gameOver = false;
                                            break;
                                        }
                                    }

                                    if (gameOver == true) {
                                        winner = "Player 1";
                                        playerTurnText.SetText("");
                                    } else {
                                        p1Turn = !p1Turn;
                                        playerTurnText.SetText(player2Text.GetParsedText() + "'s turn!");
                                    }
                                } else {
                                    p1Turn = !p1Turn;
                                    playerTurnText.SetText(player2Text.GetParsedText() + "'s turn!");
                                }
                            } else {
                                p2Board[tileIndex].SetOverlaySprite(overlaySprites[0]);
                                p1Turn = !p1Turn;
                                playerTurnText.SetText(player2Text.GetParsedText() + "'s turn!");
                            }

                            if (BattleshipsGameVars.Type == BattleshipsGameVars.GameType.AgainstHumanLocal) {
                                foreach (BattleshipsShipBehaviour ship in p1Ships) {
                                    ship.SetVisibility(false);
                                }
                                foreach (BattleshipsShipBehaviour ship in p2Ships) {
                                    ship.SetVisibility(true);
                                }
                            }
                        }
                    }
                } else if (BattleshipsGameVars.Type != BattleshipsGameVars.GameType.AgainstCPU) {
                    if (mousePosWorld.x >= p1BoardMinX && mousePosWorld.x <= p1BoardMaxX && mousePosWorld.y >= boardMinY && mousePosWorld.y <= boardMaxY) {
                        Position mousePos = new Position(mousePosWorld.x - p1BoardMinX, Mathf.Abs(mousePosWorld.y));
                        int tileIndex = mousePos.y * width + mousePos.x;

                        if (!p1Board[tileIndex].GetWasHit()) {
                            p1Board[tileIndex].SetWasHit();

                            if (p1Board[tileIndex].GetContainsShip()) {
                                p1Board[tileIndex].SetOverlaySprite(overlaySprites[1]);
                                BattleshipsShipBehaviour shipHit = p1Board[tileIndex].GetShipContained();

                                if (shipHit.GetRotation() == 0) { // horizontal
                                    shipHit.SetChunkHit(Mathf.Abs(mousePos.x - shipHit.GetPosition().x));
                                } else { // vertical
                                    shipHit.SetChunkHit(Mathf.Abs(shipHit.GetPosition().y - mousePos.y));
                                }

                                if (shipHit.GetSunk() == true) { 
                                    gameOver = true;
                                    foreach(BattleshipsShipBehaviour ship in p1Ships) {
                                        if (ship.GetSunk() == false) {
                                            gameOver = false;
                                            break;
                                        }
                                    }

                                    if (gameOver == true) {
                                        winner = "Player 2";
                                        playerTurnText.SetText("");
                                    } else {
                                        p1Turn = !p1Turn;
                                        playerTurnText.SetText("Player 1's turn!");
                                    }
                                } else {
                                    p1Turn = !p1Turn;
                                    playerTurnText.SetText("Player 1's turn!");
                                }
                            } else {
                                p1Board[tileIndex].SetOverlaySprite(overlaySprites[0]);
                                p1Turn = !p1Turn;
                                playerTurnText.SetText("Player 1's turn!");
                            }

                            foreach (BattleshipsShipBehaviour ship in p2Ships) {
                                ship.SetVisibility(false);
                            }
                            foreach (BattleshipsShipBehaviour ship in p1Ships) {
                                ship.SetVisibility(true);
                            }
                        }
                    }
                }
            } else if (BattleshipsGameVars.Type == BattleshipsGameVars.GameType.AgainstCPU) {
                if (!p1Turn) {
                    if (CPU.CPUAttack(width, height, p1Board, p1Ships, overlaySprites)) { // returns true if game over
                        gameOver = true;
                        winner = "CPU";
                        playerTurnText.SetText("");
                    } else {
                        p1Turn = !p1Turn;
                        playerTurnText.SetText("Player 1's turn!");
                    }
                }
            }
        } else { // game over
            if (winner != "Player 1") {
                foreach (BattleshipsShipBehaviour ship in p1Ships) {
                    ship.SetVisibility(true);
                }
            } else {
                foreach (BattleshipsShipBehaviour ship in p2Ships) {
                    ship.SetVisibility(true);
                }
            }

            gameOverObject.SetActive(true);
            gameOverObject.GetComponentInChildren<TextMeshProUGUI>().SetText(winner + " won!");
        }
    }

    private Vector3Int GetMouseWorldPosition() {
        Camera cam = Camera.main;
        Vector3 mousePos = Input.mousePosition;

        Vector3 worldPos = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, cam.nearClipPlane));

        return new Vector3Int(Mathf.FloorToInt(worldPos.x), Mathf.FloorToInt(worldPos.y), Mathf.FloorToInt(worldPos.z));
    }

    public void ReturnToGameMenu() {
        SceneManager.LoadScene("Battleships Main Menu Scene");
    }
}
