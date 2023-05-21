using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using System;

[System.Serializable]
public class CreatureCard : Card
{
    public string creatureName;

    public override bool Play(int tile_x, int tile_y)
    {
        var gm = GridManager.gridManagerSingletone;
        int cur_pl = gm.init_manager.getCreature().ownerPlayer;
        var cur_hero = GridManager.gridManagerSingletone.heroes[PhotonNetwork.LocalPlayer.ActorNumber];
        if (Math.Abs(tile_x - cur_hero.x) > 2 || Math.Abs(tile_y - cur_hero.y) > 2)
        {
            return false;
        }
        if (gm.SpawnLocal(creatureName,
            tile_x, tile_y, cur_pl))
        {
            gm.data[cur_pl].currentMana -= manacost;
            var photonView = gm.GetComponent<PhotonView>();
            photonView.RPC("Spawn", RpcTarget.Others, creatureName, tile_x, tile_y, cur_pl);
            return true;
        }
        return false;
    }
}
