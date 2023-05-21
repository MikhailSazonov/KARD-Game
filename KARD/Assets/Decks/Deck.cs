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

[System.Serializable]
public class Deck
{
    public Race race = Race.Human;
    public string deck_name = "TestDeck";
    public List<Card> menu_cards = new List<Card>();

    CardCollection collection;

    public void StartTheGame()
    {
        if (menu_cards.Count != DeckMenuManager.MAX_CARDS)
        {
            throw new NotEnoughCardException();
        }
        collection = new CardCollection(menu_cards);
        collection.StartTheGame();
    }

    /// Menu functions
    public int AddCard(Card card)
    {
        if (menu_cards.Count == DeckMenuManager.MAX_CARDS)
        {
            return -1;
        }
        int cnt = menu_cards.Count;
        menu_cards.Add(card);
        return cnt;
    }

    public void DeleteCard(int idx)
    {
        if (idx < 0 || idx >= menu_cards.Count)
        {
            return;
        }
        else
        {
            menu_cards.RemoveAt(idx);
        }
    }

    public bool isFull()
    {
        return menu_cards.Count == DeckMenuManager.MAX_CARDS;
    }

    public int Size()
    {
        return menu_cards.Count;
    }

    public void Clear()
    {
        menu_cards.Clear();
    }

    /// Game functions
    public Card getNextCard()
    {
        return collection.getNextCard();
    }
}

class DeckNotFoundException : System.Exception
{}

class NotEnoughCardException : System.Exception
{}