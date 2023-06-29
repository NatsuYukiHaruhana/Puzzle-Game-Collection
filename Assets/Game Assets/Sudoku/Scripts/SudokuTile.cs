using UnityEngine;

public class SudokuTile : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    
    private Position gridPos;
    private Type tileType;
    private ModifiedState modifiedState;

    private int number, correctNumber;
    /* Values | Meaning
    *    0   |  Empty
    *    1   | tile has a 1 on it
    *    .
    *    .
    *    .
    *    9   | tile has a 9 on it
    */
    private bool valid;

    public void Init(Position gridPos, string name = "Tile") {
        spriteRenderer = this.GetComponent<SpriteRenderer>();

        this.gridPos = gridPos;
        this.transform.position = new Vector3(gridPos.x + 0.5f, gridPos.y + 0.5f, 0f);
        this.name = name;

        tileType = Type.NULL;
        modifiedState = ModifiedState.NULL;

        number = 0;
        correctNumber = 0;
        valid = true;
    }

    public enum Type {
        Modifiable, // tile can be modified by player
        NonModifiable, // tile cannot be modified by player
        NULL // error type
    }

    public enum ModifiedState {
        Pencil, // tile is marked with the pencil tool (won't be taken into consideration for a correct solution)
        Pen, // tile is marked with the pen tool
        NULL // error state or is unmodifiable
    }

    // Getters
    public Position GetPosition() {
        return gridPos;
    }

    public Type GetTileType() {
        return tileType;
    }

    public ModifiedState GetState() {
        return modifiedState;
    }

    public int GetNumber() {
        return number;
    }

    public int GetCorrectNumber() {
        return correctNumber;
    }

    public bool GetValid() {
        return valid;
    }

    // Setters
    public void SetTileType(Type newType) {
        tileType = newType;
    }

    public void SetState(ModifiedState newState) {
        modifiedState = newState;
    }

    public void SetNumber(int newNum) {
        number = newNum;
    }

    public void SetCorrectNumber(int newNum) {
        correctNumber = newNum;
    }

    public void SetSprite(Sprite sprite) {
        spriteRenderer.sprite = sprite;
    }

    public void ChangeValidity() {
        valid = !valid;
    }
}

