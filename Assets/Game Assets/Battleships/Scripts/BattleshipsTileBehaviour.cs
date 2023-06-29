using UnityEngine;

public class BattleshipsTileBehaviour : MonoBehaviour
{
    private SpriteRenderer tileSpriteRend, overlaySpriteRend;
    
    private Position pos;
    private bool containsShip;
    private BattleshipsShipBehaviour shipContained;
    private bool wasHit;

    public void Init(Position pos, string name) {
        SpriteRenderer[] spriteRends = GetComponentsInChildren<SpriteRenderer>();
        tileSpriteRend = spriteRends[0];
        overlaySpriteRend = spriteRends[1];

        this.transform.position += new Vector3(pos.x, -pos.y, 0);

        this.pos = pos;
        shipContained = null;
        containsShip = false;
        wasHit = false;

        this.name = name;
    }

    // Getters
    public bool GetContainsShip() {
        return containsShip;
    }

    public BattleshipsShipBehaviour GetShipContained() {
        return shipContained;
    }

    public bool GetWasHit() {
        return wasHit;
    }

    // Setters
    public void SetContainsShip() { // this alternates between true and false
        containsShip = !containsShip;
    }

    public void SetShipContained(BattleshipsShipBehaviour ship) {
        shipContained = ship;
    }

    public void SetWasHit() { // this sets the value to true
        wasHit = true;
    }

    public void SetOverlaySprite(Sprite sprite) {
        overlaySpriteRend.sprite = sprite;
    }
}
