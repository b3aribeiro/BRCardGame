using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    
    public TMP_Text playerManaTxt;
    public TMP_Text playerHealthTxt, enemyHealthTxt;

    public GameObject manaWarningTxt; //since just changing active state, we want the entire object
    public float warningTime = 2f;
    private float warningCounter;

    public GameObject drawCardButton;
    public GameObject endTurnButton;

    public UIDamageIndicator playerTakesDamage, enemyTakesDamage;

    /*Singleton Design Pattern: Creating one version of a script that all scripts can have access to*/
    public static UIController instance;

    private void Awake() {
        instance = this;
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (warningCounter > 0)
        {
            warningCounter -= Time.deltaTime; /* warningCounter = warningCounter - Time.deltaTime; */

            if(warningCounter <= 0)
            {
                manaWarningTxt.SetActive(false);
            }
        }
    }

    //LATER: change name to UpdatePlayerManaText()
    public void SetPlayerManaText(int manaAmount)
    {
        playerManaTxt.text = "Mana: " + manaAmount;
    }

    public void ShowManaWarning()
    {
        manaWarningTxt.SetActive(true);
        warningCounter = warningTime;
    }

    //might delete this later
    public void DrawCardThroughButton()
    {
        DeckController.instance.DrawCardForMana();
    }

    public void EndPlayerTurnButton()
    {
        BattleController.instance.EndPlayerTurn();
    }

    public void SetPlayerHealthText(int healthAmount)
    {
        playerHealthTxt.text = "Player Health: " + healthAmount;
    }

     public void SetEnemyHealthText(int healthAmount)
    {
        enemyHealthTxt.text = "Enemy Health: " + healthAmount;
    }

}
