using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RandomGenerator
{
    ulong next;
    ulong seed;

    const ulong DEGREE = 12329;
    const ulong SEED_DEGREE = 37;
    const ulong REMAINDER = 1000003;
    const ulong INIT_PRIME = 1327;

    public void SetSeed(ulong new_seed)
    {
        if (new_seed == 0)
        {
            new_seed += 13;
        }
        seed = new_seed;
        next = Pow(seed, INIT_PRIME);
    }

    public ulong GenerateNext()
    {
        ulong mb_next = (Pow(next, DEGREE) * Pow(seed, SEED_DEGREE)) % REMAINDER;
        if (mb_next == next)
        {
            mb_next++;
        }
        next = mb_next;
        return next;
    }

    public ulong Pow(ulong a, ulong b)
    {
        if (b == 0)
        {
            return 1;
        }
        if (b % 2 == 0)
        {
            ulong half = Pow(a, b / 2); 
            return half * half;
        }
        return a * Pow(a, b - 1);
    }
}
