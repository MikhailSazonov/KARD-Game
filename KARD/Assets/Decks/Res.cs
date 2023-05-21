using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Res : MonoBehaviour
{
    public List<string> cardPathes;
    public List<string> cardNames;
    public List<string> creatureNames;
    public List<int> cardCosts;

    // Start is called before the first frame update
    void Start()
    {
        if (cardPathes.Count != cardNames.Count || cardNames.Count != cardCosts.Count)
        {
            throw new NotSynchronizedCardsResException();
        }
    }

    // Update is called once per frame
    void Update()
    {   
    }
}

class NotSynchronizedCardsResException : System.Exception
{}
