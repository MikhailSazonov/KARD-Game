using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ManaBar : MonoBehaviour
{
    public int plNo;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        var gm = GridManager.gridManagerSingletone;
        gameObject.GetComponentInChildren<TMP_Text>().text = gm.data[plNo].currentMana.ToString();
    }
}
