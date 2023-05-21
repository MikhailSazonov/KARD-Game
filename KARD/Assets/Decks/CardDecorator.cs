using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CardDecorator : MonoBehaviour
{
    public GameObject button;
    public CreatureCard card;

    // Start is called before the first frame update
    void Start()
    {
        RemoveCard();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetCard(Card new_card)
    {
        button.SetActive(true);
        card = new CreatureCard();
        card.creatureName = ((CreatureCard)new_card).creatureName;
        card.cardName = new_card.cardName;
        card.imagePath = new_card.imagePath;
        card.manacost = new_card.manacost;
        button.GetComponent<Image>().sprite = Resources.Load<Sprite>(card.imagePath);
    }

    public void RemoveCard()
    {
        button.SetActive(false);
        card = null;
    }

    public bool isFree()
    {
        return card == null;
    }

    public Card getCard()
    {
        return card;
    }
}
