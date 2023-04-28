using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPController : MonoBehaviour
{
    public CardPlacePoint[] playerCardPoints, enemyCardPoints;

    public static CPController instance;
    public float timeBetweenAttackks = .5f;

    private void Awake() {
        instance = this;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    //EXTRA CHECK to make sure we dont keep a card with health 0
    public void CheckAsssignedCards()
    {
        //You use foreach when you don't care about position, but you need to check certain content
        //foreach(typeObject giveAname in array)
        foreach(CardPlacePoint point in enemyCardPoints)
        {
            if((point._cardData != null) && (point._cardData.currentHealth <= 0))
            {
                point._cardData = null;
            }
        }

        foreach(CardPlacePoint point in playerCardPoints)
        {
            if((point._cardData != null) && (point._cardData.currentHealth <= 0))
            {
                point._cardData = null;
            }
        }
    }

    public void PlayerAttack()
    {
        StartCoroutine(PlayerAttack_CO());
    }

    IEnumerator PlayerAttack_CO()
    {
        yield return new WaitForSeconds(timeBetweenAttackks);

        for(int i = 0; i < playerCardPoints.Length; i++)
        {
            //Check if there's a card in that position, 
            //then check if enemy has a card in the opposite position
            if (playerCardPoints[i]._cardData != null)
            {
                if(enemyCardPoints[i]._cardData != null)
                {
                    //Attack enemy card
                    enemyCardPoints[i]._cardData.DamageCard(playerCardPoints[i]._cardData.attackPower);
                    playerCardPoints[i]._cardData.cardAnimator.SetTrigger("Attack");
                } 
                else 
                {
                    //Attack enemy health
                    BattleController.instance.DamageEnemy(playerCardPoints[i]._cardData.attackPower);
                    
                }
                playerCardPoints[i]._cardData.cardAnimator.SetTrigger("Attack");
                yield return new WaitForSeconds(timeBetweenAttackks);
            }
        }

        CheckAsssignedCards();
        BattleController.instance.AdvanceTurn();
    }

    public void EnemyAttack()
    {
        StartCoroutine(EnemyAttack_CO());
    }

    IEnumerator EnemyAttack_CO()
    {
        yield return new WaitForSeconds(timeBetweenAttackks);

        for(int i = 0; i < enemyCardPoints.Length; i++)
        {
            //Check if there's a card in that position, 
            //then check if player has a card in the opposite position
            if (enemyCardPoints[i]._cardData != null)
            {
                if(playerCardPoints[i]._cardData != null)
                {
                    //Attack player card
                    playerCardPoints[i]._cardData.DamageCard(enemyCardPoints[i]._cardData.attackPower);
                    
                } 
                else 
                {
                    //Attack player health
                    BattleController.instance.DamagePlayer(enemyCardPoints[i]._cardData.attackPower);
                    
                }
                enemyCardPoints[i]._cardData.cardAnimator.SetTrigger("Attack");
                yield return new WaitForSeconds(timeBetweenAttackks);
            }
        }

        CheckAsssignedCards();
        BattleController.instance.AdvanceTurn();
    }

}
