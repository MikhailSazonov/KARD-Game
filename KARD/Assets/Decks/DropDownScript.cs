using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropDownScript : MonoBehaviour
{
    public DeckMenuManager mg;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnDropDown()
    {
        mg.SetResFromMenu(GetComponent<TMPro.TMP_Dropdown>().value);
    }
}
