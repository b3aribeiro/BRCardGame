using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Card", order = 1)]

//SO's don't run regular funcitons, it's just for reference
public class CardSO : ScriptableObject
{
    public string cardName;
    [TextArea] public string cardAction, cardLore;

    public int currentHealth, attackPower, manaCost;

    public Sprite cardSprite; 

}
