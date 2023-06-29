using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class _2048TileBehaviour : MonoBehaviour
{
    private int number;
    private bool wasMoved;
    private bool wasModified;
    private Position pos;
    private TextMeshProUGUI numberText;
    private SpriteRenderer spriteRend;

    private void Awake() {
        numberText = GetComponentInChildren<TextMeshProUGUI>();
        spriteRend = GetComponentInChildren<SpriteRenderer>();
    }

    public void Init(Position newPos, int newNumber, string name, Sprite sprite) {
        this.transform.position = new Vector3(newPos.x * 1f - 1.5f, -newPos.y * 1f + 1.5f, 0f);
        this.name = name;

        number = newNumber;
        wasMoved = false;
        wasModified = false;
        pos = newPos;
        numberText.SetText(newNumber.ToString());
        spriteRend.sprite = sprite;
    }

    public void ChangeNumber(Sprite sprite) { 
        number *= 2;
        numberText.SetText(number.ToString());
        spriteRend.sprite = sprite;
        wasModified = true;
    }

    // Getters
    public int GetNumber() {
        return number;
    }

    public Position GetPosition() {
        return pos;
    }

    public bool GetWasMoved() {
        return wasMoved;
    }

    public bool GetWasModified() {
        return wasModified;
    }

    // Setters
    public void SetPosition(Position newPosition) {
        pos = newPosition;
        wasMoved = true;

        this.transform.position = new Vector3(pos.x * 1f - 1.5f, -pos.y * 1f + 1.5f);
        this.name = "Tile " + pos.x + ", " + pos.y;
    }

    public void SetWasMoved(bool newValue) {
        wasMoved = newValue;
    }

    public void SetWasModified(bool newValue) {
        wasModified = newValue;
    }
}
