using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;


[System.Serializable]
[KnownType(typeof(CreatureCard))]
public abstract class Card
{
    public string cardName;
    public string imagePath;
    public int manacost;
    public Race race;

    public abstract bool Play(int tile_x, int tile_y);
}


[System.Serializable]
public class CardCollection
{
    public List<Card> data;
    int iterator_idx;

    public CardCollection(List<Card> takeData)
    {
        data = takeData;
    }

    public void StartTheGame()
    {
        Shuffle();
        iterator_idx = 0;
    }

    public void Shuffle()
    {
        System.Random rng = new System.Random();
        int n = data.Count;
        while (n > 1)
        {
            int k = rng.Next(n--);
            Card value = data[k];
            data[k] = data[n];
            data[n] = value;
        }
    }

    public Card getNextCard()
    {
        if (iterator_idx == data.Count)
        {
            return null;
        }
        return data[iterator_idx++];
    }
}