using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Card
{
    public abstract void Play(GridManager manager, int tile_x, int tile_y);
}


public class CardCollection
{
    public List<Card> data;
    int iterator_idx;

    public CardCollection(List<Card> data)
    {
        this.data = data;
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