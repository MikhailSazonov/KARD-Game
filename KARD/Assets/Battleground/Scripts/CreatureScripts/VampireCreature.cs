using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class VampireCreature : Creature
{
    public virtual void Attack(Creature other)
    {
        double add = (currentAttack - other.currentDefense) * 1.2;
        double dmg = Math.Max(currentDamage * 0.2, currentDamage + add);
        animator.SetTrigger("Hit");
        other.TakeDamage(dmg);
        currentHp = (int)Math.Min(hp, currentHp + dmg * 0.2f);
    }
}
