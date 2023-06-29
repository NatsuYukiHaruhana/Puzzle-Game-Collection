using UnityEngine;

public class MinesweeperTile : MonoBehaviour
{
    public void Init(Position gridPos, string name = "Tile") {
        spriteRenderer = this.GetComponent<SpriteRenderer>();

        this.gridPos = gridPos;
        this.transform.position = new Vector3(gridPos.x + 0.5f, gridPos.y + 0.5f, 0f);
        this.name = name;

        tileState = State.Neutral;
        hasMine = false;
        nAdjacentMines = 0;
    }

    public enum State
    {
        Neutral, // not revealed nor flagged
        Revealed, // is revealed
        Flagged, // is flagged
        QuestionMarked, // is a question mark tile, user is not sure whether it is a mine or not
        NULL // error state
    }

    private SpriteRenderer spriteRenderer;

    private Position gridPos;
    private State tileState;
    private bool hasMine;
    private int nAdjacentMines;

    //Getters
    public State GetState() {
        return tileState;
    }

    public Position GetGridPos() {
        return gridPos;
    }

    public bool GetHasMine() {
        return hasMine;
    }

    public int GetnAdjacentMines() {
        return nAdjacentMines;
    }

    //Setters
    public void SetMine() {
        hasMine = true;
    }

    public void SetState(State state) {
        tileState = state;
    }

    public void SetSprite(Sprite sprite) {
        spriteRenderer.sprite = sprite;
    }

    public void SetAdjacentMines(int nMines) {
        nAdjacentMines = nMines;
    }
}