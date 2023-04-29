using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Tile : MonoBehaviour
{
    public GridManager gridManager;
    
    [HideInInspector] public int x;
    [HideInInspector] public int y;
    // Start is called before the first frame update
    void Start()
    {
           
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseDown()
    {
        Debug.LogFormat("Tile {0} {1} clicked!", x, y);
        gridManager.Go(x, y);
    }
}
