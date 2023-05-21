using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HpBar : MonoBehaviour
{
    public int plNo;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (GridManager.gridManagerSingletone.heroes[plNo])
        {
            gameObject.GetComponentInChildren<TMP_Text>().text = GridManager.gridManagerSingletone.heroes[plNo].currentHp.ToString();
        }
        else
        {
            gameObject.GetComponentInChildren<TMP_Text>().text = "";
        }
    }
}
