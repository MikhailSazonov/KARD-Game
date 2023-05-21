using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

public class PlayerData
{
    public SortedSet<Creature> initiative_queue;
    public SortedSet<Creature> initiative_queue_back;
    Creature current_creature;

    int round = 0;

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
    public int maxMana = 10;

    public PlayerData(int StartX, float RotateY)
    {
        player_params = new CreaturesParams(StartX, RotateY);
        initiative_queue = new SortedSet<Creature>();
        initiative_queue_back = new SortedSet<Creature>();
        current_creature =  null;

        currentMana = 1;
    }

    public Creature getCreature()
    {
        current_creature = initiative_queue.Min;
        return current_creature;
    }

    public void SwapQueues()
    {
        var inter_q = initiative_queue;
        initiative_queue = initiative_queue_back;
        initiative_queue_back = inter_q;
    }

    void FinishRound()
    {
        SwapQueues();
        ++round;
        currentMana = Math.Min(round + 1, maxMana);
        if (current_creature.ownerPlayer == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            DeckManager.deckManagerSingletone.DrawCard();
        }
    }

    public void AddSummoned(Creature creature)
    {
        initiative_queue_back.Add(creature);
    }
    
    public void Print()
    {
        foreach(var cr in initiative_queue)
        {
            Debug.LogFormat("{0} {1}", cr.creatureName, cr.ownerPlayer);
        }
        foreach(var cr in initiative_queue_back)
        {
            Debug.LogFormat("{0} {1}", cr.creatureName, cr.ownerPlayer);
        }
    }

    public void RemoveSummoned(Creature creature)
    {
        initiative_queue.Remove(creature);
        initiative_queue_back.Remove(creature);
    }

    public void FinishTurn()
    {
        initiative_queue.Remove(current_creature);
        initiative_queue_back.Add(current_creature);
        Debug.LogFormat("Players: {0} {1}", current_creature.ownerPlayer, PhotonNetwork.LocalPlayer.ActorNumber);
        if (current_creature.ownerPlayer == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            DeckManager.deckManagerSingletone.FinishTurn();
        }
        if (initiative_queue.Count == 0)
        {
            FinishRound();
        }
    }
}
