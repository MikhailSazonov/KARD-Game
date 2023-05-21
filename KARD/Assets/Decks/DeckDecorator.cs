using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeckDecorator : MonoBehaviour
{
    public GameObject button;
    public GameObject del_button;
    [HideInInspector] public Deck deck;
    [HideInInspector] public int idx;

    // Start is called before the first frame update
    void Start()
    {
        button.SetActive(false);
        del_button.SetActive(false);
        deck = null;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void DeepCopyToThis(Deck from)
    {
        deck = new Deck();
        for (int i = 0; i < from.Size(); ++i)
        {
            CreatureCard new_card = new CreatureCard();
            new_card.creatureName = ((CreatureCard)from.menu_cards[i]).creatureName;
            new_card.cardName = from.menu_cards[i].cardName;
            new_card.imagePath = from.menu_cards[i].imagePath;
            new_card.manacost = from.menu_cards[i].manacost;
            deck.AddCard(new_card);
        }
        deck.race = from.race;
        deck.deck_name = from.deck_name;
    }

    void DeepCopyToPres(Deck from)
    {
        DeckMenuManager.preserved_decks[idx] = new Deck();
        for (int i = 0; i < from.Size(); ++i)
        {
            CreatureCard new_card = new CreatureCard();
            new_card.creatureName = ((CreatureCard)from.menu_cards[i]).creatureName;
            new_card.cardName = from.menu_cards[i].cardName;
            new_card.imagePath = from.menu_cards[i].imagePath;
            new_card.manacost = from.menu_cards[i].manacost;
            DeckMenuManager.preserved_decks[idx].AddCard(new_card);
        }
        DeckMenuManager.preserved_decks[idx].race = from.race;
        DeckMenuManager.preserved_decks[idx].deck_name = from.deck_name;
    }

    public void SetDeck(Deck new_deck)
    {
        button.SetActive(true);
        del_button.SetActive(true);
        DeepCopyToThis(new_deck);
        DeepCopyToPres(new_deck);
        button.GetComponentInChildren<TMP_Text>().text = deck.deck_name;
        DeckSerializer.Serialize();
    }

    public void RemoveDeck()
    {
        button.SetActive(false);
        del_button.SetActive(false);
        deck = null;
        DeckMenuManager.preserved_decks[idx] = null;
        DeckSerializer.Serialize();
    }

    public bool isFree()
    {
        return deck == null;
    }

    public string getName()
    {
        return deck.deck_name;
    }

    public bool isFull()
    {
        return deck.isFull();
    }
}
