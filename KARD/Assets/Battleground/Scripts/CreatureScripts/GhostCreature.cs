using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GhostCreature : Creature
{
    public override void TakeDamage(double dmg)
    {
        System.Random random = new System.Random();
        int randomNumber = random.Next(1, 11);
        if (randomNumber > 5.0f)
        {
            Debug.Log("Miss!");
            return;
        }
        currentHp -= (int)dmg;
        Debug.Log(string.Format("Hit taken! current hp: {0}", currentHp));
        if (currentHp <= 0)
        {
            currentHp = 0;
            animator.SetTrigger("Die");
            GridManager.gridManagerSingletone.DieCallback(this);
        }
        else
        {
            animator.SetTrigger("TakeHit");
        }
    }
}
