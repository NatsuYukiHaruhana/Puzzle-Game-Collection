using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleshipsShipBehaviour : MonoBehaviour
{
    private List<GameObject> shipChunks;
    private List<bool> chunkHit;

    private Vector3 initialPosition, boardInitialPosition, rotationOffset;
    private bool setBoardInitialPosition = false;
    private Position pos; // taken from leftmost/highest tile on the board
    private int rotation; // can be either 0 or 270 degrees, for horizontal, respectively vertical
    private int size; // from 2 to 5
    private bool placed = false;
    private bool sunk = false;

    public void Init(int shipSize, GameObject shipChunk, Sprite[] shipSprites, Vector3 startingPos, bool display = true) {
        this.transform.position = initialPosition = startingPos + new Vector3(shipSize / 2, 0f);
        rotationOffset = new Vector3(0f, 0f, 0f);
        rotation = 0; // start off horizontal
        size = shipSize;

        shipChunks = new List<GameObject>();
        chunkHit = new List<bool>();

        for (int i = 0; i < size; i++) {
            GameObject newShipChunk = Instantiate<GameObject>(shipChunk, this.transform);
            newShipChunk.transform.position = startingPos + new Vector3(1f * i, 0);

            SpriteRenderer chunkRend = newShipChunk.AddComponent<SpriteRenderer>();
            chunkRend.sortingOrder = 1;

            if (i == 0) {
                chunkRend.sprite = shipSprites[1];
                newShipChunk.transform.eulerAngles = new Vector3(0, 0, 90);
            } else if (i == size - 1) {
                chunkRend.sprite = shipSprites[1];
                newShipChunk.transform.eulerAngles = new Vector3(0, 0, 270);
            } else {
                chunkRend.sprite = shipSprites[0];
                newShipChunk.transform.eulerAngles = new Vector3(0, 0, 90);
            }

            if (!display) {
                chunkRend.enabled = false;
            }

            shipChunks.Add(newShipChunk);
            chunkHit.Add(false);
        }
    }

    // Setters
    public void SetVisibility(bool visible) {
        for (int i = 0; i < size; i++) {
            SpriteRenderer chunkRend = shipChunks[i].GetComponent<SpriteRenderer>();

            chunkRend.enabled = visible;
        }
    }

    public void SetPositionAbsolute(Position pos, Vector3 startingPos) {
        if (setBoardInitialPosition == false) {
            boardInitialPosition = startingPos + new Vector3(pos.x, -pos.y);
            setBoardInitialPosition = true;
        }

        this.pos = pos;
        this.transform.position = boardInitialPosition + new Vector3(pos.x, -pos.y) + rotationOffset; Debug.Log(pos.x + " " + pos.y);
    }

    public void SetPosition(Position pos) {
        Position movement = pos - this.pos;
        this.pos = pos;
        this.transform.position += new Vector3(movement.x, -movement.y);
        Debug.Log(pos.x + " " + pos.y);
    }

    public void SetPositionOutsideOfBoard() {
        this.transform.position = initialPosition;
    }

    public void SetRotation(int width = 0, int height = 0, bool changePos = true) {
        if (changePos) {
            Position initPos = pos;
            if (rotation == 0) {
                if (size <= 3) {
                    pos += new Position(1, -1);
                } else {
                    pos += new Position(2, -2);
                }

                rotation = 270;
            } else {
                if (size <= 3) {
                    pos += new Position(-1, 1);
                } else {
                    pos += new Position(-2, 2);
                }

                rotation = 0;
            }

            if (width != 0) {
                if (pos.x < 0 || pos.y < 0) {
                    pos = initPos;

                    rotation = (rotation == 0) ? 270 : 0;
                } else if (rotation == 0) { // we flipped, now the ship is horizontal
                    if (pos.x + size - 1 >= width) {
                        pos = initPos;

                        rotation = 270;
                    }
                } else {
                    if (pos.y + size - 1 >= height) {
                        pos = initPos;

                        rotation = 0;
                    }
                }
            }
            this.transform.eulerAngles = new Vector3(0, 0, rotation);
            Debug.Log(pos.x + " " + pos.y);
        } else {
            rotation = (rotation == 0) ? 270 : 0;

            this.transform.eulerAngles = new Vector3(0, 0, rotation);
            if (rotation == 0) {
                rotationOffset = new Vector3(0f, 0f, 0f);
            } else {
                float offset = (size <= 3) ? -1f : -2f;
                rotationOffset = new Vector3(offset, offset);
            }
        }
    }

    public void SetPlaced(bool value) {
        placed = value;
    }

    public void SetChunkHit(int chunk) {
        chunkHit[chunk] = true;

        sunk = true;
        foreach(bool chunkH in chunkHit) {
            if (chunkH == false) {
                sunk = false;
                return;
            }
        }
    }

    // Getters
    public Position GetPosition() {
        return pos;
    }

    public bool GetPlaced() {
        return placed;
    }

    public int GetSize() {
        return size;
    }

    public int GetRotation() {
        return rotation;
    }

    public bool GetSunk() {
        return sunk;
    }
}
