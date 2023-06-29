using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class KlondikeSolitaireGameBehaviour : MonoBehaviour
{
    [SerializeField]
    private GameObject pilePrefab, cardPrefab, gameScreenObject, highscoreObject;

    [SerializeField]
    private HighscoreTable highscoreTable;

    [SerializeField]
    private Sprite[] cardSprites;

    [SerializeField]
    private Sprite cardBack, emptyDrawPile;

    [SerializeField]
    private TextMeshProUGUI timeSpent, scoreText, gameOverText;

    [SerializeField]
    private GameObject gameOverObject, solveButton;

    private KlondikeSolitairePileBehaviour drawPile, wastePile;
    private List<KlondikeSolitairePileBehaviour> winPile, bottomPile;
    private const float startingX = -4.6f, startingY = 0.3f;
    private const float xOffset = 1.5f, yOffset = 0.3f;

    private List<KlondikeSolitaireCardBehaviour> deck;
    KlondikeSolitaireCardBehaviour selectedCard;

    private bool gameOver = false;
    private int score = 0;

    private void Start()
    {
        deck = new List<KlondikeSolitaireCardBehaviour>();
        selectedCard = null;

        for (int i = 0; i < 52; i++) {
            GameObject newCard = Instantiate<GameObject>(cardPrefab, gameScreenObject.transform);

            deck.Add(newCard.AddComponent<KlondikeSolitaireCardBehaviour>());

            deck[i].Init((KlondikeSolitaireCardBehaviour.SuitType)(i / 13), (KlondikeSolitaireCardBehaviour.Number)(i % 13), cardSprites[i], cardBack);
        }

        Shuffle(deck);

        bottomPile = new List<KlondikeSolitairePileBehaviour>();
        float xPos = startingX;
        for (int i = 0; i < 7; i++, xPos += xOffset) {
            GameObject newPile = Instantiate<GameObject>(pilePrefab, gameScreenObject.transform);

            bottomPile.Add(newPile.AddComponent<KlondikeSolitairePileBehaviour>());
            bottomPile[i].Init(new Vector3(xPos, startingY, 2f), KlondikeSolitairePileBehaviour.RuleType.Rule_DifferentSuits);

            for (int j = 0; j <= i; j++) {
                bottomPile[i].AddCard(deck[j], ref score, false);
                deck.RemoveAt(j);
            }

            bottomPile[i].GetTopCard().ChangeVisible();
        }

        winPile = new List<KlondikeSolitairePileBehaviour>();
        xPos = startingX + 3 * xOffset;
        for (int i = 0; i < 4; i++, xPos += xOffset) {
            GameObject newPile = Instantiate<GameObject>(pilePrefab, gameScreenObject.transform);

            winPile.Add(newPile.AddComponent<KlondikeSolitairePileBehaviour>());
            winPile[i].Init(new Vector3(xPos, startingY + 1f, 1f), KlondikeSolitairePileBehaviour.RuleType.Rule_SameSuit);
        }

        {
            GameObject newPile = Instantiate<GameObject>(pilePrefab, gameScreenObject.transform);

            wastePile = (newPile.AddComponent<KlondikeSolitairePileBehaviour>());
            wastePile.Init(new Vector3(startingX + xOffset, startingY + 1f, 1f), KlondikeSolitairePileBehaviour.RuleType.Rule_NoRules);
        }

        {
            GameObject newPile = Instantiate<GameObject>(pilePrefab, gameScreenObject.transform);

            drawPile = (newPile.AddComponent<KlondikeSolitairePileBehaviour>());
            drawPile.Init(new Vector3(startingX, startingY + 1f, 1f), KlondikeSolitairePileBehaviour.RuleType.Rule_NoRules);

            while (deck.Count > 0) {
                drawPile.AddCard(deck[0], ref score, false);
                deck.RemoveAt(0);
            }
        }
    }

    private void Update()
    {
        if (!gameOver) {
            timeSpent.SetText("Time Spent: " + Mathf.FloorToInt(Time.timeSinceLevelLoad).ToString());
            scoreText.SetText("Score: " + score.ToString());
            if (Input.GetMouseButtonDown(0)) {
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -10));
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hit) {
                    if (hit.collider.CompareTag("Card")) {
                        KlondikeSolitaireCardBehaviour cardHit = hit.collider.gameObject.GetComponent<KlondikeSolitaireCardBehaviour>();
                        if (selectedCard == null) {
                            if (cardHit.GetPile() == drawPile) {
                                if (drawPile.GetHasCards()) { 
                                    wastePile.AddCard(cardHit, ref score);
                                }
                            } else if (cardHit.GetIsVisible()) {
                                selectedCard = cardHit;
                                selectedCard.ChangeColor();
                            }
                        } else {                        
                            if (cardHit.GetIsVisible() && cardHit.GetPile() != wastePile) {
                                cardHit.GetPile().AddCard(selectedCard, ref score);

                                if (CanBeSolved()) {
                                    solveButton.SetActive(true);
                                } else {
                                    solveButton.SetActive(false);
                                }
                                CheckHasWon();
                            }
                            selectedCard.ChangeColor();
                            selectedCard = null;
                        }
                    } else if (hit.collider.CompareTag("Pile")) {
                        if (selectedCard != null) {
                            KlondikeSolitairePileBehaviour pileHit = hit.collider.gameObject.GetComponent<KlondikeSolitairePileBehaviour>();
                            if (pileHit != drawPile && pileHit != wastePile) {
                                pileHit.AddCard(selectedCard, ref score);
                            }
                            selectedCard.ChangeColor();
                            selectedCard = null;
                        } else {
                            if (hit.collider.gameObject.GetComponent<KlondikeSolitairePileBehaviour>() == drawPile) {
                                if (drawPile.GetHasCards()) {
                                    if (CanBeSolved()) {
                                        solveButton.SetActive(true);
                                    } else {
                                        solveButton.SetActive(false);
                                    }
                                    wastePile.AddCard(drawPile.GetTopCard(), ref score);
                                } else {
                                    if (wastePile.GetHasCards() && score >= 100) {
                                        score -= 100;
                                    } else {
                                        score = 0;
                                    }

                                    while (wastePile.GetHasCards()) {
                                        drawPile.AddCard(wastePile.GetTopCard(), ref score);
                                    }
                                }
                            }
                        }
                    } else {
                        if (selectedCard != null) { 
                            selectedCard.ChangeColor();
                            selectedCard = null;
                        }
                    }
                } else {
                    if (selectedCard != null) {
                        selectedCard.ChangeColor();
                        selectedCard = null;
                    }
                }
            }
        }
    }

    private bool CanBeSolved() {
        if (drawPile.GetHasCards()) {
            return false;
        }

        for (int i = 0; i < 7; i++) {
            if (bottomPile[i].GetHasUnrevealedCards()) {
                return false;
            }
        }

        return true;
    }

    public void SolveTable() {
        score += wastePile.RemoveAllCards() * 10;
        for (int i = 0; i < 7; i++) {
            score += bottomPile[i].RemoveAllCards() * 10;
        }

        for (int i = 0; i < 4; i++) {
            winPile[i].RemoveAllCards();
        }

        for (int i = 0; i < 52; i++) {
            GameObject newCard = Instantiate<GameObject>(cardPrefab, gameScreenObject.transform);
            KlondikeSolitaireCardBehaviour newCardBehaviour = newCard.AddComponent<KlondikeSolitaireCardBehaviour>();
            newCardBehaviour.Init((KlondikeSolitaireCardBehaviour.SuitType)(i / 13), (KlondikeSolitaireCardBehaviour.Number)(i % 13), cardSprites[i], cardBack);
            newCardBehaviour.ChangeVisible();

            winPile[i / 13].AddCard(newCardBehaviour, ref score, false);
        }

        solveButton.SetActive(false);
        CheckHasWon();
    }

    private void CheckHasWon() {
        foreach(KlondikeSolitairePileBehaviour pile in winPile) {
            if (!pile.GetHasMaxNumOfCards()) {
                return;
            }
        }

        gameOver = true;
        gameOverObject.SetActive(true);
        gameOverText.SetText("You won!");

        AddHighscoreEntry();
    }

    private void Shuffle(List<KlondikeSolitaireCardBehaviour> deck) {
        const int goThroughDeckTimes = 5;
        for (int i = 0; i < deck.Count * goThroughDeckTimes; i++) {
            int switchWith = UnityEngine.Random.Range(0, deck.Count);

            KlondikeSolitaireCardBehaviour aux = deck[i % deck.Count];
            deck[i % deck.Count] = deck[switchWith];
            deck[switchWith] = aux;
        }
    }

    public void BackToGameMenu() {
        SceneManager.LoadScene("Klondike Solitaire Main Menu Scene");
    }

    public void ShowHighscores() {
        gameScreenObject.SetActive(false);
        highscoreObject.SetActive(true);
    }

    private void AddHighscoreEntry() {
        highscoreTable.AddHighscoreEntry(score, CurrentUser.CurrentUserName, "Klondike Solitaire");
    }
}
