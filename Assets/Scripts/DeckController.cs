using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckController : MonoBehaviour
{
    //change name to availableDeck. These is all the cards in your game.
    public List<CardSO> deckToUse = new List<CardSO>();
    //change name to activeCards. These are the cards you will be loading in your deck to avoid repetables.
    private List<CardSO> activeCards = new List<CardSO>();

    public CardData cardToSpawn;
    public Transform spawnPoint;

    public int drawCardCost = 2; //LATER: Delete/Change this if you don't want player drawing by click

    public float waitBetweenDrawTime = .5f;

    public static DeckController instance;

    private void Awake() {
        instance = this;
    }

    void Start()
    {
        SetupDeck();
    }

    void Update()
    {
        //if(Input.GetKeyDown(KeyCode.T))
        //{
        //    DrawCardToHand();
        //}
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

    public void DrawCardToHand()
    {
        if(activeCards.Count == 0)
        {
            SetupDeck();
        }

        //Instantiate(Object original, Tranform parent); is how you create a copy of an object
        //So which object you're copying and where you want it to spawn
        CardData newCard = Instantiate(cardToSpawn, spawnPoint.position, spawnPoint.rotation); //whereaver our deckcontroller is in the world
        newCard._cardSO = activeCards[0];
        newCard.SetupCard();

        activeCards.RemoveAt(0);

        HandController.instance.AddCardToHand(newCard);
    }

    //LATER: Change name for Check Mana
    public void DrawCardForMana()
    {
        if(BattleController.instance.playerMana >= drawCardCost)
        {
            DrawCardToHand();
            BattleController.instance.SpendPlayerMana(drawCardCost);
        } 
        else
        {
            UIController.instance.ShowManaWarning();
        }
    }

    public void DrawMultipleCards(int amountToDraw)
    {
        StartCoroutine(DrawMultiple_CO(amountToDraw));
    }

    IEnumerator DrawMultiple_CO(int amountToDraw)
    {
        for(int i = 0; i < amountToDraw; i++)
        {
            DrawCardToHand();
            yield return new WaitForSeconds(waitBetweenDrawTime);
        }
    }
}
