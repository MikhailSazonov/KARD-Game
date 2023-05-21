using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(InputField))]
public class DeckInput : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Edit()
    {
        var inputText = gameObject.GetComponent<TMPro.TMP_InputField>();
        DeckMenuManager.EditDeckName(inputText.text);
    }
}
