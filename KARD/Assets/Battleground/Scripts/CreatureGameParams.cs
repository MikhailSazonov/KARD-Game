using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureGameParams
{
    public int hp;
    public int damage;
    public int attack;
    public int defense;
    public int speed;
    public int shots;
    public int initiative;
    public List<Effect> effects;

    [HideInInspector]
    public int currentHp;

    [HideInInspector]
    public int currentDamage;

    [HideInInspector]
    public int currentAttack;

    [HideInInspector]
    public int currentDefense;

    [HideInInspector]
    public int currentSpeed;

    [HideInInspector]
    public int currentShots;

    [HideInInspector]
    public int currentInitiative;
}
