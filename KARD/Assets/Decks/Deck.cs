using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Race
{
    Human,
    Demon,
    Undead,
    Elf
}

public class Deck : MonoBehaviour
{
    public static int MAX_CARDS = 30;
    public static string choosen_deck_name;

    public Race race;
    public string deck_name;
    List<Card> menu_cards;

    CardCollection collection;

    void Start() {}

    void Update() {}

    public void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void StartTheGame()
    {
        if (menu_cards.Count != MAX_CARDS)
        {
            throw new NotEnoughCardException();
        }
        collection = new CardCollection(menu_cards);
        collection.StartTheGame();
    }

    public void AddCard(Card card)
    {
        if (menu_cards.Count == MAX_CARDS)
        {
            return;
        }
        else
        {
            menu_cards.Add(card);
        }
    }

    public void DeleteCard(int idx)
    {
        if (idx < 0 || idx > menu_cards.Count)
        {
            return;
        }
        else
        {
            menu_cards.RemoveAt(idx);
        }
    }

    Card getNextCard()
    {
        return collection.getNextCard();
    }
}

class DeckNotFoundException : System.Exception
{}

class NotEnoughCardException : System.Exception
{}