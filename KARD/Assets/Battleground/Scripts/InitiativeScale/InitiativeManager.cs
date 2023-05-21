using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;
using System.Threading;


public class InitiativeManager
{
    int turn = 0;
    int creatureId = 0;

    PlayerData nthPlayerData(int n)
    {
        return GridManager.gridManagerSingletone.data[n];
    }

    PlayerData currentPlayerData()
    {
        return GridManager.gridManagerSingletone.data[PhotonNetwork.PlayerList[turn % 2].ActorNumber];
    }

    public void Init()
    {
        turn = 0;
        nthPlayerData(1).SwapQueues();
        nthPlayerData(2).SwapQueues();
        var gm = GridManager.gridManagerSingletone; 
        gm.heroes[PhotonNetwork.PlayerList[0].ActorNumber] = gm.tiles_info[2][3].standing;   
        gm.heroes[PhotonNetwork.PlayerList[1].ActorNumber] = gm.tiles_info[12][3].standing;
        var creature = getCreature(); 
        GridManager.gridManagerSingletone.turn_manager.cntAdd = 2;
        GridManager.gridManagerSingletone.turn_manager.execution.Reset();
        if (creature.ownerPlayer == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            creature.LightenTheCells();
        }
    }

    public Creature getCreature()
    {
        return currentPlayerData().getCreature();
    }

    public void TurnEnd()
    {
        GridManager.gridManagerSingletone.SetBackTheColor();
        currentPlayerData().FinishTurn();
        ++turn;
        GridManager.gridManagerSingletone.turn_manager.cntAdd = 2;
        GridManager.gridManagerSingletone.turn_manager.execution.Reset();
        var creature = getCreature();
        Debug.Log(string.Format("Now goes creature {0} by owner {1}", creature.name, creature.ownerPlayer));
        if (creature.ownerPlayer == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            creature.LightenTheCells();
        }
    }

    public void AddSummoned(Creature creature)
    {
        creature.id = creatureId;
        creatureId += 1;
        int owner = creature.ownerPlayer;
        nthPlayerData(owner).AddSummoned(creature);
    }

    public void RemoveSummoned(Creature creature)
    {
        int owner = creature.ownerPlayer;
        nthPlayerData(owner).RemoveSummoned(creature);
    }

    public bool Ready()
    {
        return nthPlayerData(1).initiative_queue.Count + 
        nthPlayerData(1).initiative_queue_back.Count + 
        nthPlayerData(2).initiative_queue.Count + 
        nthPlayerData(2).initiative_queue_back.Count >= 2;
    }

}
