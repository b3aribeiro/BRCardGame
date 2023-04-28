using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public List<CardSO> deckToUse = new List<CardSO>();
    private List<CardSO> activeCards = new List<CardSO>();

    public static EnemyController instance;

    public CardData cardToSpawn;
    public Transform cardSpawnPoint;

    private void Awake() 
    {
        instance = this;
    }
    
    void Start()
    {
        SetupDeck();
    }

    void Update()
    {
        
    }

    public void SetupDeck()
    {
        activeCards.Clear(); //make sure list is empty

        List<CardSO> tempDeck = new List<CardSO>(); //create temporary deck
        tempDeck.AddRange(deckToUse);

        int iterations = 0;

        while(tempDeck.Count > 0 && iterations < 500)
        {
            int selected = Random.Range(0, tempDeck.Count); //Random returns a value whithin min. INCLUSIVE and max EXCLUSIVE
            activeCards.Add(tempDeck[selected]);
            tempDeck.RemoveAt(selected);

            iterations++; //this is a safe to make sure the while will never be infinite. it has a max of 500 iterations
        }
    }

    public void StartAction()
    {
        StartCoroutine(EnemyAction_CO());
    }

    IEnumerator EnemyAction_CO()
    {
        if(activeCards.Count == 0)
        {
            SetupDeck();
        }

        yield return new WaitForSeconds(.5f);

        List<CardPlacePoint> cardPoints = new List<CardPlacePoint>();
        cardPoints.AddRange(CPController.instance.enemyCardPoints);
        
        int randomPoint = Random.Range(0, cardPoints.Count);
        CardPlacePoint selectedPoint = cardPoints[randomPoint]; //Randomly pick a point from the list and place a card

        //Before placing, check if there's a card in that position
        //While there's a card but there's an empty slot... assign a new random point
        while (selectedPoint._cardData != null && cardPoints.Count > 0)
        {
            randomPoint = Random.Range(0, cardPoints.Count);
            selectedPoint = cardPoints[randomPoint];
            cardPoints.RemoveAt(randomPoint); //remove occupied point that can't be picked up
        }

        if (selectedPoint._cardData == null)
        {
            //if this spot we picked is really empty....
            CardData newCard = Instantiate(cardToSpawn, cardSpawnPoint.position, cardSpawnPoint.rotation);

            //for now, selected next card on list and remove from active card list.
            newCard._cardSO = activeCards[0];
            activeCards.RemoveAt(0);
            newCard.SetupCard();
            newCard.MoveToPoint(selectedPoint.transform.position, selectedPoint.transform.rotation);

            selectedPoint._cardData = newCard;
            newCard.assignedPlace = selectedPoint;
        }

        yield return new WaitForSeconds(.5f);

        BattleController.instance.AdvanceTurn();
    }
}
