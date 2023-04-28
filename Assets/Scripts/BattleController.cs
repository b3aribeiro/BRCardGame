using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleController : MonoBehaviour
{
    public int startingMana = 4, manaCap = 12;
    public int playerMana;
    private int currentPlayerMaxMana;

    public int startingCardsAmount = 3;
    public int cardToDrawPerTurn = 2;

    //enums are good to track game states <they are not actual variables, just references
    //LATER: rename enums accordingly
    public enum TurnOrder { playerActive, playerCardAttacks, enemyActive, enemyCardAttacks }
    public TurnOrder currentPhase; //now this is a variable

    public Transform discardPoint;

    /*Singleton Design Pattern: Creating one version of a script that all scripts can have access to*/
    public static BattleController instance;

    public int playerHealth, enemyHealth;
   
    private void Awake() 
    {
        instance = this;
    }

    void Start()
    {
        currentPlayerMaxMana = startingMana;
        ResetPlayerMana();
        DeckController.instance.DrawMultipleCards(startingCardsAmount);
        UIController.instance.SetPlayerHealthText(playerHealth);
        UIController.instance.SetEnemyHealthText(enemyHealth);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            AdvanceTurn();
        }
    }

    public void AdvanceTurn()
    {
        currentPhase++;

        //if currentPhase goes larger than length of enum, goes back to zero
        if((int)currentPhase >= System.Enum.GetValues(typeof(TurnOrder)).Length)
        {
            currentPhase = 0;
        }
        
        switch(currentPhase)
        {
            case TurnOrder.playerActive:
                
                if(currentPlayerMaxMana < manaCap) { currentPlayerMaxMana++; }
                ResetPlayerMana();

                UIController.instance.endTurnButton.SetActive(true);
                UIController.instance.drawCardButton.SetActive(true);

                DeckController.instance.DrawMultipleCards(cardToDrawPerTurn);

                break;

            case TurnOrder.playerCardAttacks:

                CPController.instance.PlayerAttack();

                break;

            case TurnOrder.enemyActive:

               EnemyController.instance.StartAction();
                
                break;
            
            case TurnOrder.enemyCardAttacks:

                CPController.instance.EnemyAttack();

                break;
        }
    }

    public void SpendPlayerMana(int amountToSpend)
    {
        playerMana = playerMana - amountToSpend;

        //shouldnt need this anyway but better safe than sorry
        if(playerMana < 0)
        {
            playerMana = 0;
        }

        UIController.instance.SetPlayerManaText(playerMana);
    }

    public void ResetPlayerMana()
    {
        playerMana = currentPlayerMaxMana;   
        UIController.instance.SetPlayerManaText(playerMana);
    }

    public void EndPlayerTurn()
    {
        UIController.instance.endTurnButton.SetActive(false);
        UIController.instance.drawCardButton.SetActive(false);
        AdvanceTurn();
    }

    public void DamagePlayer(int damageAmount)
    {
        if(playerHealth > 0)
        {
            playerHealth -= damageAmount;

            if(playerHealth <= 0)
            { 
                playerHealth = 0;
                //End Battle
            }

            UIController.instance.SetPlayerHealthText(playerHealth);
            UIDamageIndicator damageClone = Instantiate(UIController.instance.playerTakesDamage, UIController.instance.playerTakesDamage.transform.parent);
            damageClone.playerDamageTxt.text = damageAmount.ToString();
            damageClone.gameObject.SetActive(true);
        }
    }

    public void DamageEnemy(int damageAmount)
    {
        if(enemyHealth > 0)
        {
            enemyHealth -= damageAmount;

            if(enemyHealth <= 0)
            { 
                enemyHealth = 0;
                //End Battle
            }

            UIController.instance.SetEnemyHealthText(enemyHealth);
            UIDamageIndicator damageClone = Instantiate(UIController.instance.enemyTakesDamage, UIController.instance.enemyTakesDamage.transform.parent);
            damageClone.enemyDamageTxt.text = damageAmount.ToString();
            damageClone.gameObject.SetActive(true);
        }
    }

}
