using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ExecutionUnit
{
    public struct CreatureAction
    {
        public int x;
        public int y;
        public Creature creature;
    }

    int counter;

    public CreatureAction action = new CreatureAction();

    public void Reset()
    {
        counter = 0;
    }

    public bool Execute(int x, int y, Creature creature)
    {
        var creature_go = GridManager.gridManagerSingletone.init_manager.getCreature();
        if (!creature_go.Go(x, y))
        {
            return false;
        }
        if (creature != null)
        {
            creature_go.creature_to_hit = creature;
            creature = null;
        }
        return true;
    }

    // CardPlayed is 1 or 2, depends on if the card was played on another client,
    // i.e. do we need to wait for it
    public void SetAction(int x, int y, Creature creature, int cardPlayed)
    {
        counter += cardPlayed; 
        if (counter == 2)
        {
            counter = 0;
            Execute(x, y, creature);
        }
        action.x = x;
        action.y = y;
        action.creature = creature;
    }

    public void incCounter()
    {
        counter += 1;
        if (counter == 2)
        {
            counter = 0;
            // Imitate the delay between card play and movement
            // Wait will freeze the animations -> weird looking
            // ad-hoc: spinlock
            for (int i = 0; i < 100000; ++i) {}
            Execute(action.x, action.y, action.creature);
        }
    }

}
