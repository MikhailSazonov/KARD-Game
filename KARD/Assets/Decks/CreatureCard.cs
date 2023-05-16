using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureCard : Card
{
    public string creatureName;

    public Deck deck;

    public override void Play(GridManager manager, int tile_x, int tile_y)
    {
        manager.Spawn(creatureName, tile_x, tile_y, manager.init_manager.current_player);
    }
}
