using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KlondikeSolitaireCardBehaviour : MonoBehaviour
{
    private SpriteRenderer spriteRend;
    private Sprite cardBack, cardSprite;

    public enum SuitType {
        Hearts,
        Diamond,
        Clubs,
        Spades,
        NULL
    }

    public enum Number {
        A,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        J,
        Q,
        K,
        NULL
    }

    private SuitType suit;
    private Number num;

    private KlondikeSolitairePileBehaviour currentPile = null;
    private bool isVisible = false;

    private void Awake() {
        spriteRend = GetComponent<SpriteRenderer>();
    }

    public void Init(SuitType suit, Number num, Sprite cardSprite, Sprite cardBack) {
        this.cardSprite = cardSprite;
        this.cardBack = cardBack;

        spriteRend.sprite = cardBack;
        
        this.suit = suit;
        this.num = num;
        this.isVisible = false;
    }

    public void ChangeVisible() {
        isVisible = !isVisible;

        if (isVisible) {
            spriteRend.sprite = cardSprite;
        } else {
            spriteRend.sprite = cardBack;
        }
    }

    public void ChangeColor() {
        if (spriteRend.color.b != 0) {
            spriteRend.color = new Color(255, 255, 0);
        } else {
            spriteRend.color = new Color(255, 255, 255);
        }
    }

    //Setters
    public void SetPosition(Vector3 position) {
        this.transform.position = position;
    }

    public void SetPile(KlondikeSolitairePileBehaviour newPile) {
        currentPile = newPile;
    }

    //Getters
    public SuitType GetSuitType() {
        return suit;
    }

    public Number GetNumber() {
        return num;
    }

    public bool GetIsVisible() {
        return isVisible;
    }

    public KlondikeSolitairePileBehaviour GetPile() {
        return currentPile;
    }
}
