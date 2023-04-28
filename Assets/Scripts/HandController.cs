using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    public List<CardData> heldCards = new List<CardData>();

    public Transform minPos, maxPos;
    public List<Vector3> cardPositions = new List<Vector3>();

    public static HandController instance;

    private void Awake() 
    {
        instance = this;
    }

    void Start()
    {
        SetCardPositionsInHand();
    }

    void Update()
    {
        
    }

    //Store the positions where the cards will move to
    public void SetCardPositionsInHand()
    {
        
        cardPositions.Clear();
        Vector3 distBetPoints = Vector3.zero;

        //if it's an array[] we use .Length
        //if it's a list<> we use .Count
        if(heldCards.Count > 1)
        {
            distBetPoints = (maxPos.position - minPos.position) / (heldCards.Count - 1);
        }

        for(int i = 0; i < heldCards.Count; i++)
        {
            cardPositions.Add(minPos.position + (distBetPoints * i));
            //heldCards[i].transform.position = cardPositions[i];
            heldCards[i].transform.rotation = minPos.rotation; //for not overlapping 

            heldCards[i].MoveToPoint(cardPositions[i], minPos.rotation);

            heldCards[i].isOnHand = true;
            heldCards[i].handPosition = i;
        }
    }

    public void RemoveCardFromHand(CardData cardToRemove)
    {
        //EXTRA check to prevent problems in the future
        if (heldCards[cardToRemove.handPosition] == cardToRemove)
        {
            heldCards.RemoveAt(cardToRemove.handPosition);
        }
        else 
        {
            Debug.LogError("Card at position " + cardToRemove.handPosition + " is not the card being removed from handController");
        }

        SetCardPositionsInHand();
    }

    public void AddCardToHand(CardData cardToAdd)
    {
        heldCards.Add(cardToAdd);
        SetCardPositionsInHand();
    }

    //Missing a function that limits the number of cards you can hold (also in deckcontroller)
}

