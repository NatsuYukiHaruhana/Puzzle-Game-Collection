using System.Collections.Generic;
using UnityEngine;

public class KlondikeSolitairePileBehaviour : MonoBehaviour
{
    public enum RuleType {
        Rule_DifferentSuits,
        Rule_SameSuit,
        Rule_NoRules,
        NULL
    }

    private RuleType type;
    private List<KlondikeSolitaireCardBehaviour> cards;    

    private const float startingZ = -0.01f;
    private const float yOffset = -0.3f, zOffset = -0.03f;

    public void Init(Vector3 position, RuleType type) {
        this.transform.position = position;

        this.type = type;

        cards = new List<KlondikeSolitaireCardBehaviour>();
    }

    private void AddCards(KlondikeSolitaireCardBehaviour card, ref int score, bool doCheck) {
        // score
        if (doCheck) {
            if (type == RuleType.Rule_SameSuit) { // we moved a card to the foundation
                score += 10;
            } else if (type == RuleType.Rule_DifferentSuits) { // we moved a card to the tableau
                if (card.GetPile().GetRuleType() == RuleType.Rule_SameSuit) { // if moved from foundation
                    if (score >= 15) {
                        score -= 15;
                    } else {
                        score = 0;
                    }
                } else if (card.GetPile().GetRuleType() == RuleType.Rule_NoRules) { // if moved from waste
                    score += 5;
                }
            }
        }

        // moving cards
        if (card.GetPile() == null) {
            cards.Add(card);
            MoveCard(card);
            card.SetPile(this);
        } else if (card.GetPile().GetTopCard() == card) {
            if (card.GetPile().GetCount() > 1 && type != RuleType.Rule_NoRules && !card.GetPile().GetCardAt(card.GetPile().GetCount() - 2).GetIsVisible()) {
                card.GetPile().GetCardAt(card.GetPile().GetCount() - 2).ChangeVisible();
                score += 5; // turning over a card yields 5 points
            }
            if (type == RuleType.Rule_NoRules) {
                card.ChangeVisible();
            }

            cards.Add(card);
            MoveCard(card);
            card.GetPile().RemoveCard(card);
            card.SetPile(this);
        } else {
            List<KlondikeSolitaireCardBehaviour> newCards = card.GetPile().GetCardsFromGivenCard(card);

            if (newCards.Count != card.GetPile().GetCount() && !card.GetPile().GetCardAt(card.GetPile().GetCount() - newCards.Count - 1).GetIsVisible()) {
                card.GetPile().GetCardAt(card.GetPile().GetCount() - newCards.Count - 1).ChangeVisible();
                score += 5; // turning over a card yields 5 points
            }

            foreach(KlondikeSolitaireCardBehaviour newCard in newCards) {
                cards.Add(newCard);
                MoveCard(newCard);
                newCard.GetPile().RemoveCard(newCard);
                newCard.SetPile(this);
            }
        }
    }

    private void MoveCard(KlondikeSolitaireCardBehaviour card) {
        if (type == RuleType.Rule_DifferentSuits) {
            card.transform.position = this.transform.position + new Vector3(0f, yOffset * (cards.Count - 1), startingZ + zOffset * (cards.Count - 1));
        } else {
            card.transform.position = this.transform.position + new Vector3(0f, 0f, zOffset * (cards.Count - 1));
        }
    }

    private void RemoveCard(KlondikeSolitaireCardBehaviour card) {
        cards.Remove(card);
    }

    private List<KlondikeSolitaireCardBehaviour> GetCardsFromGivenCard(KlondikeSolitaireCardBehaviour card) {
        List<KlondikeSolitaireCardBehaviour> pile = null;

        for (int i = 0; i < cards.Count; i++) {
            if (cards[i] == card) {
                pile = new List<KlondikeSolitaireCardBehaviour>();
                while(i < cards.Count) {
                    pile.Add(cards[i++]);
                }
                break;
            }
        }

        return pile;
    }

    //Setters
    public void AddCard(KlondikeSolitaireCardBehaviour card, ref int score, bool doCheck = true) {
        if (!doCheck || type == RuleType.Rule_NoRules) {
            AddCards(card, ref score, doCheck);
            return;
        }

        if (cards.Count > 0) { 
            KlondikeSolitaireCardBehaviour lastCard = cards[cards.Count - 1];

            if (type == RuleType.Rule_DifferentSuits) {
                if (!card.GetIsVisible()) {
                    AddCards(card, ref score, doCheck);
                } else if (lastCard.GetSuitType() == KlondikeSolitaireCardBehaviour.SuitType.Hearts ||
                        lastCard.GetSuitType() == KlondikeSolitaireCardBehaviour.SuitType.Diamond) {

                    if (card.GetSuitType() == KlondikeSolitaireCardBehaviour.SuitType.Clubs ||
                            card.GetSuitType() == KlondikeSolitaireCardBehaviour.SuitType.Spades) {

                        if (card.GetNumber() == lastCard.GetNumber() - 1) {
                            AddCards(card, ref score, doCheck);
                        }
                    }
                } else if (lastCard.GetSuitType() == KlondikeSolitaireCardBehaviour.SuitType.Clubs ||
                             lastCard.GetSuitType() == KlondikeSolitaireCardBehaviour.SuitType.Spades) {

                    if (card.GetSuitType() == KlondikeSolitaireCardBehaviour.SuitType.Hearts ||
                            card.GetSuitType() == KlondikeSolitaireCardBehaviour.SuitType.Diamond) {

                        if (card.GetNumber() == lastCard.GetNumber() - 1) {
                            AddCards(card, ref score, doCheck);
                        }
                    }
                }
            } else if (type == RuleType.Rule_SameSuit) {
                if (lastCard.GetSuitType() == card.GetSuitType() && lastCard.GetNumber() + 1 == card.GetNumber()) {
                    AddCards(card, ref score, doCheck);
                } 
            }
        } else {
            if (type == RuleType.Rule_DifferentSuits) {
                if (card.GetNumber() == KlondikeSolitaireCardBehaviour.Number.K) {
                    AddCards(card, ref score, doCheck);
                }
            } else if (type == RuleType.Rule_SameSuit) {
                if (card.GetNumber() == KlondikeSolitaireCardBehaviour.Number.A) {
                    AddCards(card, ref score, doCheck);
                }
            }
        }
    }

    public int RemoveAllCards() {
        int cardCount = cards.Count;
        while (cards.Count > 0) {
            KlondikeSolitaireCardBehaviour topCard = GetTopCard();
            RemoveCard(topCard);

            Destroy(topCard.gameObject);
        }

        return cardCount;
    }

    //Getters
    public bool GetHasMaxNumOfCards() {
        return cards.Count == (int)KlondikeSolitaireCardBehaviour.Number.NULL;
    }

    public bool GetHasCards() {
        return cards.Count > 0;
    }

    public bool GetHasUnrevealedCards() {
        foreach(KlondikeSolitaireCardBehaviour card in cards) {
            if (!card.GetIsVisible()) {
                return true;
            }
        }

        return false;
    }

    public KlondikeSolitaireCardBehaviour GetTopCard() {
        return GetCardAt(cards.Count - 1);
    }

    public KlondikeSolitaireCardBehaviour GetCardAt(int pos) {
        if (pos >= cards.Count) {
            return null;
        }

        return cards[pos];
    }

    public KlondikeSolitaireCardBehaviour GetCard(KlondikeSolitaireCardBehaviour other) {
        foreach(KlondikeSolitaireCardBehaviour card in cards) {
            if (card == other) {
                return card;
            }
        }

        return null;
    }

    private int GetCount() {
        return cards.Count;
    }

    private RuleType GetRuleType() {
        return type;
    }
}
