using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class TurnManager
{
    public ExecutionUnit execution = new ExecutionUnit();

    public int cntAdd;

    public bool setAction(int x, int y, Creature creature, int addCnt)
    {
        int cur_player = GridManager.gridManagerSingletone.init_manager.getCreature().ownerPlayer;
        if (cur_player == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            return execution.Execute(x, y, creature);
        }
        else
        {
            execution.SetAction(x, y, creature, addCnt);
            return true;
        }
    }
}
