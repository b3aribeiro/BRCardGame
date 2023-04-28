using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardData : MonoBehaviour
{
    public CardSO _cardSO;
    private HandController _handController;

    public bool isPlayer;

    public int currentHealth, attackPower, manaCost;

    public TMP_Text cardName, cardAction, cardLore;
    public TMP_Text healthTxt, attackTxt, manaTxt;

    public Image characterArt;

    public Vector3 targetPoint;
    public Quaternion targetRot;

    public float cardMoveSpeed = 4f;
    public float cardRotationSpeed = 540f; //velocity for rotation is an angle 540f

    public bool isOnHand;
    public int handPosition;

    private bool isSelected;
    private Collider cardCollider;

    public LayerMask whatIsDesktop, whatIsPlacement;
    private bool justPressed;

    public CardPlacePoint assignedPlace;

    public Animator cardAnimator;

    void Start()
    {
        if (targetPoint == Vector3.zero) //this is to set enemy cards in the correct position
        {
            targetPoint = transform.position;
            targetRot = transform.rotation;
        }
        SetupCard();
        _handController = FindObjectOfType<HandController>();
        cardCollider = GetComponent<Collider>();
    }

    void Update()
    {
        //Lerp gradually changes values, slowing in halfs (so 100/50/25/12/6/3/1)
        //Lerp is (currentPosition, GoToPosition, linearInterpolationValue)
        //deltaTime corrects the speed for frame changes
        transform.position = Vector3.Lerp(transform.position, targetPoint, cardMoveSpeed * Time.deltaTime); 
        //RotateTowards is the way to do a Lerp for rotation, but consistent all the way
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, cardRotationSpeed * Time.deltaTime);

        if (isSelected)
        {
            //To make the card move to arena, we convert a point (based on mouse position) to a ray
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            //RaycastHit detects if the ray is hitting something else
            RaycastHit hit;
            //Raycast has a start and end point
            //(origin, direction/end point, maxDistance, colliders/mask)
            if(Physics.Raycast(ray, out hit, 100f, whatIsDesktop))
            {
                MoveToPoint(hit.point + new Vector3(0f, 2f, 0f), Quaternion.identity);
            }

            //(0) is left click | (1) is right click | (2) is middle
            if(Input.GetMouseButtonDown(1))
            {
                ReturnToHand();
            }

            if(Input.GetMouseButtonDown(0) && justPressed == false)
            {
                if(Physics.Raycast(ray, out hit, 100f, whatIsPlacement) && BattleController.instance.currentPhase == BattleController.TurnOrder.playerActive)
                {
                    //assign card to the point... store the point clicked as temporary value
                    CardPlacePoint selectedPoint = hit.collider.GetComponent<CardPlacePoint>();
                    if (selectedPoint._cardData == null && selectedPoint.isPlayerPoint)
                    {
                        if(BattleController.instance.playerMana >= manaCost)
                        {
                            BattleController.instance.SpendPlayerMana(manaCost);

                            selectedPoint._cardData = this; //this = card
                            assignedPlace = selectedPoint; //we will need the card to know which point they are going tp

                            MoveToPoint(selectedPoint.transform.position, Quaternion.identity);
                            isOnHand = false;
                            isSelected = false;

                            _handController.RemoveCardFromHand(this);
                        }
                        else 
                        {
                            ReturnToHand();
                            UIController.instance.ShowManaWarning();
                        }


                    }
                    else 
                    {
                        ReturnToHand();
                    }
                } else 
                {
                    ReturnToHand();
                }
            }
        }

        justPressed = false;
    }
    //This is copying info from the CardSO
    public void SetupCard()
    {
        cardName.text = _cardSO.cardName;
        cardAction.text = _cardSO.cardAction;
        cardLore.text = _cardSO.cardLore;
        
        characterArt.sprite = _cardSO.cardSprite; 

        currentHealth = _cardSO.currentHealth;
        attackPower = _cardSO.attackPower;
        manaCost = _cardSO.manaCost;
 
        UpdateCardDisplay();   
    }

    public void UpdateCardDisplay()
    {
        healthTxt.text = currentHealth.ToString();
        attackTxt.text = attackPower.ToString();
        manaTxt.text = manaCost.ToString();   
    }

    //Function for card "animation"/movement
    //MoveToPoint needs transform coordinates and rotation.
    public void MoveToPoint(Vector3 thePoint, Quaternion theRotation)
    {
        targetPoint = thePoint;
        targetRot = theRotation;
    }
    
    private void OnMouseOver() 
    {
        if(isOnHand && isPlayer)
        {
            //Bring card up once mouse over
            //Quarternion.identiy is the same as Vector3.zero
            MoveToPoint(_handController.cardPositions[handPosition] + new Vector3(0f, 1f, .5f), Quaternion.identity); 
        }
    }

    private void OnMouseExit() 
    {
        if(isOnHand && isPlayer)
        {
            //Put card back to original position
            MoveToPoint(_handController.cardPositions[handPosition], _handController.minPos.rotation); 
        }
    }

    //Once clicking cards on hand...
    private void OnMouseDown() 
    {
        if(isOnHand && BattleController.instance.currentPhase == BattleController.TurnOrder.playerActive && isPlayer)
        {
            isSelected = true;
            cardCollider.enabled = false;

            justPressed = true;
        }
    }

    public void ReturnToHand()
    {
        isSelected = false;
        cardCollider.enabled = true;
        //Put card back to original position
        MoveToPoint(_handController.cardPositions[handPosition], _handController.minPos.rotation); 
    }

    //LATER: Change to TakeDamageFrom or ApplyDamage
    public void DamageCard(int damageAmount)
    {
        currentHealth -= damageAmount;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            assignedPlace._cardData = null;
            MoveToPoint(BattleController.instance.discardPoint.position, BattleController.instance.discardPoint.rotation);
            cardAnimator.SetTrigger("Jump");
            Destroy(gameObject, 3f);
        }

        cardAnimator.SetTrigger("Hurt");
        UpdateCardDisplay(); 
    }
}
