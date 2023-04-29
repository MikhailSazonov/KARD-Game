using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.IO;
using System;

public class Creature : MonoBehaviour
{
    public int hp;
    public int damage;
    public int attack;
    public int defense;
    public int speed;
    public int shots;
    public List<Effect> effects;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
