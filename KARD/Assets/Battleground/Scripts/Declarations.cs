using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class Declarations : MonoBehaviour
{
    public GameObject sprite;
    public TMP_Text textComponent;

    bool end = false;
    int counter = 0;

    void Start()
    {

    }

    public bool isEnd()
    {
        return end;
    }

    void Update()
    {
        if (end)
        {
            ++counter;
            if (counter == 900)
            {
                PhotonNetwork.LeaveRoom();
            }
        }
    }

    public void Disable()
    {
        sprite.GetComponent<Renderer>().enabled = false;
    }

    public void Enable(string text)
    {
        if (string.IsNullOrEmpty(textComponent.text))
        {
            textComponent.text = text;
        }
        sprite.GetComponent<Renderer>().enabled = true;
        end = true;
        // DeckManager.inited = false;
    }
}
