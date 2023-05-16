using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class InitiativeManager
{
    SortedSet<Tuple<int, Creature>> initiative_queue;
    SortedSet<Tuple<int, Creature>> initiative_queue_back;
    Creature current_creature;
    [HideInInspector] public int current_player;

    [HideInInspector] int turn;

    public void Init()
    {
        if (initiative_queue.Count == 0)
        {
            Debug.LogError("No creatures detected from initiative manager!");
            return;
        }
        turn = 0;
        current_creature = initiative_queue.Min.Item2;
    }

    public Creature getCreature()
    {
        return current_creature;
    }

    public void TurnEnd()
    {
        initiative_queue_back.Add(Tuple.Create(current_creature.cr_params.currentInitiative, current_creature));
        initiative_queue.Remove(Tuple.Create(current_creature.cr_params.currentInitiative, current_creature));
        if (initiative_queue.Count == 0)
        {
            ++turn;
            var inter = initiative_queue_back;
            initiative_queue_back = initiative_queue;
            initiative_queue = inter;
        }
        current_creature = initiative_queue.Min.Item2;
    }

    public void AddSummoned(Creature creature)
    {
        initiative_queue_back.Add(Tuple.Create(creature.cr_params.currentInitiative, creature));
    }

}
