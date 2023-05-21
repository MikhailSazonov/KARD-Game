using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DeckMenuManager : MonoBehaviour
{
    public List<DeckDecorator> decks;
    public List<CardDecorator> cards_in_deck;
    public List<CardDecorator> cards;

    public static List<Deck> preserved_decks;
    public static Deck current_deck;
    public static int current_deck_idx;
    public static readonly int MAX_CARDS = 9;

    public Res humanRes;
    public Res undeadRes;
    public Res demonRes;
    public Res elvesRes;

    // Start is called before the first frame update
    void Start()
    {
        DeckSerializer.Deserialize();
        SetRes(humanRes, Race.Human);
        ReassignDecks();
        for (int i = 0; i < 5; ++i)
        {
            decks[i].idx = i;
        }
    }

    void ReassignDecks()
    {
        for (int i = 0; i < 5; ++i)
        {
            if (preserved_decks[i] != null)
            {
                this.decks[i].SetDeck(preserved_decks[i]);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void EditDeckName(string new_name)
    {
        current_deck.deck_name = new_name;
    }

    public void Exit()
    {
        SceneManager.LoadScene(0);
    }

    public void Clear()
    {
        OnCancelCard8();
        OnCancelCard7();
        OnCancelCard6();
        OnCancelCard5();
        OnCancelCard4();
        OnCancelCard3();
        OnCancelCard2();
        OnCancelCard1();
        OnCancelCard0();
    }

    public void SaveDeck()
    {
        int idx = -1;
        if (!string.IsNullOrEmpty(current_deck.deck_name) && current_deck.isFull())
        {
            for (int i = 0; i < 5; ++i)
            {
                if (decks[i].isFree() && idx == -1 ||
                        idx != -1 && !decks[i].isFree() && decks[i].getName() == current_deck.deck_name)
                {
                    idx = i;
                }
            }
        }
        if (idx != -1)
        {
            decks[idx].SetDeck(current_deck);
        }
    }

    void LoadCard(Card card)
    {
        if (current_deck.AddCard(card) > -1)
        {
            foreach (var c in cards_in_deck)
            {
                if (c.isFree())
                {
                    c.SetCard(card);
                    return;
                }
            }
        }
    }

    void RmCard(int idx)
    {
        cards_in_deck[idx].RemoveCard();
        if (idx >= 0 && idx < current_deck.Size())
        {
            current_deck.DeleteCard(idx);
        }
    }

    public void OnClickCard0()
    {
        LoadCard(cards[0].getCard());
    }

    public void OnClickCard1()
    {
        LoadCard(cards[1].getCard());
    }

    public void OnClickCard2()
    {
        LoadCard(cards[2].getCard());
    }

    public void OnClickCard3()
    {
        LoadCard(cards[3].getCard());
    }

    public void OnClickCard4()
    {
        LoadCard(cards[4].getCard());
    }

    public void OnClickCard5()
    {
        LoadCard(cards[5].getCard());
    }

    public void OnClickCard6()
    {
        LoadCard(cards[6].getCard());
    }

    public void OnClickCard7()
    {
        LoadCard(cards[7].getCard());
    }

    public void OnClickCard8()
    {
        LoadCard(cards[8].getCard());
    }

    public void OnCancelCard0()
    {   
        RmCard(0);
    }

    public void OnCancelCard1()
    {
        RmCard(1);
    }

    public void OnCancelCard2()
    {
        RmCard(2);
    }

    public void OnCancelCard3()
    {
        RmCard(3);
    }

    public void OnCancelCard4()
    {
        RmCard(4);
    }

    public void OnCancelCard5()
    {
        RmCard(5);
    }

    public void OnCancelCard6()
    {
        RmCard(6);
    }

    public void OnCancelCard7()
    {
        RmCard(7);
    }

    public void OnCancelCard8()
    {
        RmCard(8);
    }

    public void SetIdx(int idx)
    {
        Clear();
        current_deck_idx = idx;
        for (int i = 0; i < MAX_CARDS; ++i)
        {
            LoadCard(preserved_decks[idx].menu_cards[i]);
        }
    }

    public void ChooseDeck1()
    {
        SetIdx(0);
    }

    public void ChooseDeck2()
    {
        SetIdx(1);
    }

    public void ChooseDeck3()
    {
        SetIdx(2);
    }

    public void ChooseDeck4()
    {
        SetIdx(3);
    }

    public void ChooseDeck5()
    {
        SetIdx(4);
    }

    public void CancelDeck1()
    {
        decks[0].RemoveDeck();
    }

    public void CancelDeck2()
    {
        decks[1].RemoveDeck();
    }

    public void CancelDeck3()
    {
        decks[2].RemoveDeck();
    }

    public void CancelDeck4()
    {
        decks[3].RemoveDeck();
    }

    public void CancelDeck5()
    {
        decks[4].RemoveDeck();
    }

    public void SetRes(Res res, Race race)
    {
        current_deck = new Deck();
        current_deck.race = race;

        foreach (var car in cards)
        {
            car.RemoveCard();
        }
        Clear();

        int total = res.cardPathes.Count;

        for (int i = 0; i < total; ++i)
        {
            CreatureCard next = new CreatureCard();
            next.creatureName = res.creatureNames[i];
            next.cardName = res.cardNames[i];
            next.imagePath = res.cardPathes[i];
            next.manacost = res.cardCosts[i];
            cards[i].SetCard(next);
        }
    }

    public void SetResFromMenu(int value)
    {
        switch (value)
        {
            case 0:
                SetRes(humanRes, Race.Human);
                break;
            case 1:
                SetRes(elvesRes, Race.Elf);
                break;
            case 2:
                SetRes(demonRes, Race.Demon);
                break;
            case 3:
                SetRes(undeadRes, Race.Undead);
                break;
        }
    }
}
