using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerData
{
    public string hero_name;

    public Deck deck;

    public struct CreaturesParams {
        public int StartX;
        public float RotateY;

        public CreaturesParams(int X, float Y)
        {
            StartX = X;
            RotateY = Y;
        }
    }

    [HideInInspector] public CreaturesParams player_params;

    public int currentMana;
    public int maxMana;

    public PlayerData(int StartX, float RotateY)
    {
        player_params = new CreaturesParams(StartX, RotateY);
        Deck[] decks = GameObject.FindObjectsOfType<Deck>();
        foreach (Deck next_deck in decks)
        {
            if (next_deck.deck_name == Deck.choosen_deck_name)
            {
                deck = next_deck;
                break;
            }
        }
        if (deck is null)
        {
            throw new DeckNotFoundException();
        }

        currentMana = 0;
        switch (deck.race)
        {
            case Race.Human:
                hero_name = "HeroHuman";
                break;
            case Race.Demon:
                hero_name = "HeroDemon";
                break;
            case Race.Undead:
                hero_name = "HeroUndead";
                break;
            case Race.Elf:
                hero_name = "HeroElf";
                break;
        }
    }

    void RenewMana(int turn)
    {
        currentMana = Math.Min(turn + 1, maxMana);
    }
}
