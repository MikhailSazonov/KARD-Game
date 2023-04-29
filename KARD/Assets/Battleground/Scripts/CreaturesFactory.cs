using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CreaturesFactory
{
    public Dictionary<int, Creature> creatures_register;
    public Dictionary<string, int> creatures_codes;

    public void RegisterNewCreature<T>(int code, string name) where T : Creature, new()
    {
        creatures_codes.Add(name, code);
        creatures_register.Add(code, new T());
    }

    public CreaturesFactory()
    {
        creatures_register = new Dictionary<int, Creature>();
        creatures_codes = new Dictionary<string, int>();

        RegisterNewCreature<HeroHuman>(0, "HeroHuman");
    }
}
