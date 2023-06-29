using System.Collections.Generic;
using UnityEngine;

public class BattleshipsCPUBehaviour : MonoBehaviour
{
    List<int> indexesForNextHits;

    private void Start()
    {
        indexesForNextHits = new List<int>();
    }

    public bool CPUAttack(int width, int height, List<BattleshipsTileBehaviour> p1Board, List<BattleshipsShipBehaviour> p1Ships, Sprite[] overlaySprites) {
        if (indexesForNextHits.Count == 0) { // we attack randomly, we cannot make any educated guesses
            int tries = 100;
            while (tries > 0) {
                int tileX = Random.Range(0, width),
                    tileY = Random.Range(0, height);
                int tileIndex = tileY * width + tileX;

                if (!p1Board[tileIndex].GetWasHit()) {
                    // add adjacent tiles for testing
                    if (tileX > 0 && !p1Board[tileY * width + tileX - 1].GetWasHit() && !indexesForNextHits.Contains(tileY * width + tileX - 1)) {
                        indexesForNextHits.Add(tileY * width + tileX - 1);
                    }
                    if (tileX < width - 1 && !p1Board[tileY * width + tileX + 1].GetWasHit() && !indexesForNextHits.Contains(tileY * width + tileX + 1)) {
                        indexesForNextHits.Add(tileY * width + tileX + 1);
                    }
                    if (tileY > 0 && !p1Board[(tileY - 1) * width + tileX].GetWasHit() && !indexesForNextHits.Contains((tileY - 1) * width + tileX)) {
                        indexesForNextHits.Add((tileY - 1) * width + tileX);
                    }
                    if (tileY < height - 1 && !p1Board[(tileY + 1) * width + tileX].GetWasHit() && !indexesForNextHits.Contains((tileY + 1) * width + tileX)) {
                        indexesForNextHits.Add((tileY + 1) * width + tileX);
                    }
                    p1Board[tileIndex].SetWasHit();

                    if (p1Board[tileIndex].GetContainsShip()) {
                        p1Board[tileIndex].SetOverlaySprite(overlaySprites[1]);
                        BattleshipsShipBehaviour shipHit = p1Board[tileIndex].GetShipContained();

                        if (shipHit.GetRotation() == 0) { // horizontal
                            shipHit.SetChunkHit(Mathf.Abs(tileX - shipHit.GetPosition().x));
                        } else { // vertical
                            shipHit.SetChunkHit(Mathf.Abs(shipHit.GetPosition().y - tileY));
                        }

                        if (shipHit.GetSunk() == true) {
                            foreach (BattleshipsShipBehaviour ship in p1Ships) {
                                if (ship.GetSunk() == false) {
                                    return false;
                                }
                            }

                            return true;
                        } else {
                            return false;
                        }
                    } else {
                        p1Board[tileIndex].SetOverlaySprite(overlaySprites[0]);
                        return false;
                    }
                }

                tries--;
            }

            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    int tileIndex = y * width + x;

                    if (!p1Board[tileIndex].GetWasHit()) {
                        // add adjacent tiles for testing
                        if (x > 0 && !p1Board[y * width + x - 1].GetWasHit() && !indexesForNextHits.Contains(y * width + x - 1)) {
                            indexesForNextHits.Add(y * width + x - 1);
                        }
                        if (x < width - 1 && !p1Board[y * width + x + 1].GetWasHit() && !indexesForNextHits.Contains(y * width + x + 1)) {
                            indexesForNextHits.Add(y * width + x + 1);
                        }
                        if (y > 0 && !p1Board[(y - 1) * width + x].GetWasHit() && !indexesForNextHits.Contains((y - 1) * width + x)) {
                            indexesForNextHits.Add((y - 1) * width + x);
                        }
                        if (y < height - 1 && !p1Board[(y + 1) * width + x].GetWasHit() && !indexesForNextHits.Contains((y + 1) * width + x)) {
                            indexesForNextHits.Add((y + 1) * width + x);
                        }

                        p1Board[tileIndex].SetWasHit();

                        if (p1Board[tileIndex].GetContainsShip()) {
                            p1Board[tileIndex].SetOverlaySprite(overlaySprites[1]);
                            BattleshipsShipBehaviour shipHit = p1Board[tileIndex].GetShipContained();

                            if (shipHit.GetRotation() == 0) { // horizontal
                                shipHit.SetChunkHit(Mathf.Abs(x - shipHit.GetPosition().x));
                            } else { // vertical
                                shipHit.SetChunkHit(Mathf.Abs(shipHit.GetPosition().y - y));
                            }

                            if (shipHit.GetSunk() == true) {
                                foreach (BattleshipsShipBehaviour ship in p1Ships) {
                                    if (ship.GetSunk() == false) {
                                        return false;
                                    }
                                }

                                return true;
                            } else {
                                return false;
                            }
                        } else {
                            p1Board[tileIndex].SetOverlaySprite(overlaySprites[0]);
                            return false;
                        }
                    }
                }
            }
        } else { // we can make some assumptions about where to hit next
            int tileIndex = indexesForNextHits[0]; indexesForNextHits.RemoveAt(0);
            int tileX = tileIndex % width,
                tileY = tileIndex / width;

            p1Board[tileIndex].SetWasHit();

            if (p1Board[tileIndex].GetContainsShip()) { // hit was successful
                // add adjacent tiles for testing
                if (tileX > 0 && !p1Board[tileY * width + tileX - 1].GetWasHit() && !indexesForNextHits.Contains(tileY * width + tileX - 1)) {
                    indexesForNextHits.Add(tileY * width + tileX - 1);
                }
                if (tileX < width - 1 && !p1Board[tileY * width + tileX + 1].GetWasHit() && !indexesForNextHits.Contains(tileY * width + tileX + 1)) {
                    indexesForNextHits.Add(tileY * width + tileX + 1);
                }
                if (tileY > 0 && !p1Board[(tileY - 1) * width + tileX].GetWasHit() && !indexesForNextHits.Contains((tileY - 1) * width + tileX)) {
                    indexesForNextHits.Add((tileY - 1) * width + tileX);
                }
                if (tileY < height - 1 && !p1Board[(tileY + 1) * width + tileX].GetWasHit() && !indexesForNextHits.Contains((tileY + 1) * width + tileX)) {
                    indexesForNextHits.Add((tileY + 1) * width + tileX);
                }

                p1Board[tileIndex].SetOverlaySprite(overlaySprites[1]);
                BattleshipsShipBehaviour shipHit = p1Board[tileIndex].GetShipContained();

                if (shipHit.GetRotation() == 0) { // horizontal
                    shipHit.SetChunkHit(Mathf.Abs(tileX - shipHit.GetPosition().x));
                } else { // vertical
                    shipHit.SetChunkHit(Mathf.Abs(shipHit.GetPosition().y - tileY));
                }

                if (shipHit.GetSunk() == true) {
                    foreach (BattleshipsShipBehaviour ship in p1Ships) {
                        if (ship.GetSunk() == false) {
                            return false;
                        }
                    }

                    return true;
                } else {
                    return false;
                }
            } else {
                p1Board[tileIndex].SetOverlaySprite(overlaySprites[0]);
                return false;
            }
        }

        return true; // we should never reach here, but if we do, that means that the entire board was hit
    }

    public void CPUPlaceShips(int width, int height, List<BattleshipsTileBehaviour> p2Board, GameObject p2BoardParent, List<BattleshipsShipBehaviour> p2Ships, Sprite[] shipSprites) {
        foreach(BattleshipsShipBehaviour ship in p2Ships) {
            int shipSize = ship.GetSize();
            ship.SetPositionAbsolute(new Position(0, 0), p2BoardParent.transform.position + new Vector3(((shipSize <= 3) ? 1f : 2f), 0f));

            int tries = 10;
            bool canPlace = true;
            do {
                // Try to place the ship somewhere random
                bool horizontalRotation = ((Random.Range(0f, 1f) < .5f) ? false : true);
                if (horizontalRotation) {
                    int xLimit = width - shipSize;

                    if (ship.GetRotation() != 0) {
                        ship.SetRotation(width, height, false);
                    }
                    ship.SetPositionAbsolute(new Position(Random.Range(0, xLimit + 1), Random.Range(0, height)), new Vector3());
                } else { 
                    int yLimit = height - shipSize;

                    if (ship.GetRotation() == 0) {
                        ship.SetRotation(width, height, false);
                    }
                    ship.SetPositionAbsolute(new Position(Random.Range(0, width), Random.Range(0, yLimit + 1)), new Vector3());
                }
                Debug.Log(horizontalRotation + " " + ship.GetPosition().x + " " + ship.GetPosition().y + " " + shipSize);

                // Check if we can place the ship here
                Position currPosition = ship.GetPosition(), movement;
                if (ship.GetRotation() == 0) {
                    movement = new Position(1, 0);
                } else {
                    movement = new Position(0, 1);
                }

                for (int i = 0; i < ship.GetSize(); i++) {
                    if (p2Board[currPosition.y * width + currPosition.x].GetContainsShip()) {
                        canPlace = false;
                        break;
                    }

                    currPosition += movement;
                }

                if (canPlace) {
                    currPosition = ship.GetPosition();

                    for (int i = 0; i < ship.GetSize(); i++) {
                        p2Board[currPosition.y * width + currPosition.x].SetContainsShip();
                        p2Board[currPosition.y * width + currPosition.x].SetShipContained(ship);

                        currPosition += movement;
                    }
                    Debug.Log(horizontalRotation + " " + ship.GetPosition().x + " " + ship.GetPosition().y + " " + shipSize);
                }
                tries--;
            } while (!canPlace && tries > 0);

            if (tries == 0 && canPlace == false) {
                int xLimit = width, yLimit = height; Position movement;
                if (ship.GetRotation() == 0) {
                    xLimit -= shipSize;
                    yLimit -= 1;
                    movement = new Position(1, 0);
                } else {
                    xLimit -= 1;
                    yLimit -= shipSize;
                    movement = new Position(0, 1);
                }

                for (int y = 0; y <= yLimit; y++) {
                    for (int x = 0; x <= xLimit; x++) {
                        Position currPosition = new Position(x, y);

                        canPlace = true;
                        for (int i = 0; i < shipSize; i++) {
                            if (p2Board[currPosition.y * width + currPosition.x].GetContainsShip()) {
                                canPlace = false;
                                break;
                            }

                            currPosition += movement;
                        }

                        if (canPlace) {
                            currPosition = new Position(x, y);
                            ship.SetPositionAbsolute(currPosition, new Vector3());

                            for (int i = 0; i < shipSize; i++) {
                                p2Board[currPosition.y * width + currPosition.x].SetContainsShip();
                                p2Board[currPosition.y * width + currPosition.x].SetShipContained(ship);

                                currPosition += movement;
                            }
                            Debug.Log(ship.GetRotation() + " " + ship.GetPosition().x + " " + ship.GetPosition().y + " " + shipSize + " ran out of tries!");
                            break;
                        }
                    }

                    if (canPlace) {
                        break;
                    }
                }
            }
        }
    }
}
