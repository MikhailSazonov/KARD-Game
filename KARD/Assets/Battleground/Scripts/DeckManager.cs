using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using Photon.Pun;

public class DeckManager : MonoBehaviour
{
    public GameObject play1;
    public GameObject play2;
    public GameObject play3;
    public GameObject play4;

    List<Card> cards = new List<Card>();

    public int numClicked = -1;

    public bool played = false;

    static public string hero_name;

    static public Deck deck;

    static public DeckManager deckManagerSingletone;

    // static public bool inited = false;

    static public Card cardClicked = null;

    // Start is called before the first frame update
    void Start()
    {
        play1.SetActive(false);
        play2.SetActive(false);
        play3.SetActive(false);
        play4.SetActive(false);

        deckManagerSingletone = this;

        cards.Add(null);
        cards.Add(null);
        cards.Add(null);
        cards.Add(null);
    }

    public static void Init()
    {
        deck = DeckMenuManager.preserved_decks[DeckMenuManager.current_deck_idx];

        deck.StartTheGame();

        switch (deck.race)
        {
            case Race.Human:
                hero_name = "Races/Humans/Hero/HeroHuman";
                break;
            case Race.Demon:
                hero_name = "Races/Demons/Hero/HeroDemon";
                break;
            case Race.Undead:
                hero_name = "Races/Undead/Hero/HeroUndead";
                break;
            case Race.Elf:
                hero_name = "Races/Elves/Hero/HeroElf";
                break;
        }   
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1) && !played)
        {
            cardClicked = null;
            numClicked = -1;
            GridManager.gridManagerSingletone.SetBackTheColor();
            GridManager.gridManagerSingletone.init_manager.getCreature().LightenTheCells();
        }
    }

    public void PlayCard1()
    {
        if (!played)
        {
            var gm = GridManager.gridManagerSingletone;
            int cur_pl = gm.init_manager.getCreature().ownerPlayer;
            if (gm.data[cur_pl].currentMana < cards[0].manacost)
            {
                return;
            }
            LightenTheCells();
            cardClicked = cards[0];
            numClicked = 0;
        }
    }

    public void PlayCard2()
    {
        if (!played)
        {
            var gm = GridManager.gridManagerSingletone;
            int cur_pl = gm.init_manager.getCreature().ownerPlayer;
            if (gm.data[cur_pl].currentMana < cards[1].manacost)
            {
                return;
            }
            LightenTheCells();
            cardClicked = cards[1];
            numClicked = 1;
        }
    }

    public void PlayCard3()
    {
        if (!played)
        {
            var gm = GridManager.gridManagerSingletone;
            int cur_pl = gm.init_manager.getCreature().ownerPlayer;
            if (gm.data[cur_pl].currentMana < cards[2].manacost)
            {
                return;
            }
            LightenTheCells();
            cardClicked = cards[2];
            numClicked = 2;
        }
    }

    public void PlayCard4()
    {
        if (!played)
        {
            var gm = GridManager.gridManagerSingletone;
            int cur_pl = gm.init_manager.getCreature().ownerPlayer;
            if (gm.data[cur_pl].currentMana < cards[3].manacost)
            {
                return;
            }
            LightenTheCells();
            cardClicked = cards[3];
            numClicked = 3;
        }
    }

    public void DrawCard()
    {
        Card card = deck.getNextCard();
        if (card == null)
        {
            return;
        }
        int idx = -1;
        for (int i = 0; i < 4; ++i)
        {
            if (cards[i] == null)
            {
                idx = i;
                break;
            }
        }
        if (idx == -1)
        {
            return;
        }

        Debug.LogFormat("Filling the slot {0}", idx);
        fillTheSlot(idx, card);
        numClicked = -1;
        played = false;
    }

    public void FinishTurn()
    {
        if (played)
        {
            Debug.LogFormat("Releasing the slot {0}", numClicked);
            ReleaseTheSlot(numClicked);
        }
    }

    void fillTheSlot(int s, Card card)
    {
        switch (s)
        {
            case 0:
                play1.SetActive(true);
                Debug.Log(card.imagePath);
                play1.GetComponent<Image>().sprite = Resources.Load<Sprite>(card.imagePath);
                break;
            case 1:
                play2.SetActive(true);
                Debug.Log(card.imagePath);
                play2.GetComponent<Image>().sprite = Resources.Load<Sprite>(card.imagePath);
                break;
            case 2:
                play3.SetActive(true);
                Debug.Log(card.imagePath);
                play3.GetComponent<Image>().sprite = Resources.Load<Sprite>(card.imagePath);
                break;
            case 3:
                play4.SetActive(true);
                Debug.Log(card.imagePath);
                play4.GetComponent<Image>().sprite = Resources.Load<Sprite>(card.imagePath);
                break;
        }
        cards[s] = card;
    }

    void ReleaseTheSlot(int s)
    {
        switch (s)
        {
            case 0:
                play1.SetActive(false);
                break;
            case 1:
                play2.SetActive(false);
                break;
            case 2:
                play3.SetActive(false);
                break;
            case 3:
                play4.SetActive(false);
                break;
        }
        cards[s] = null;
    }

    public void LightenTheCells()
    {
        GridManager.gridManagerSingletone.SetBackTheColor();
        var hero_current = GridManager.gridManagerSingletone.heroes[PhotonNetwork.LocalPlayer.ActorNumber];
        for (int dx = -2; dx <= 2; ++dx)
        {
            for (int dy = -2; dy <= 2; ++dy)
            {
                int x = hero_current.x + dx;
                int y = hero_current.y + dy;
                if (x < 0 || x >= GridManager.WIDTH || y < 0 || y >= GridManager.HEIGHT)
                {
                    continue;
                }
                if (GridManager.gridManagerSingletone.tiles_info[x][y].standing == null)
                {
                    Creature.Recolor(x, y);
                }
            }
        }
    }
}
