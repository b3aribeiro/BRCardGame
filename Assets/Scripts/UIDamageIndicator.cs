using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIDamageIndicator : MonoBehaviour
{
    
    public TMP_Text playerDamageTxt, enemyDamageTxt;
    public float dmgTxtMoveSpeed;
    public float lifetime = 3f;

    private RectTransform thisRect;

    void Start()
    {
        Destroy(gameObject, lifetime);
        thisRect = GetComponent<RectTransform>();
    }

    void Update()
    {
        thisRect.anchoredPosition += new Vector2(0f, -dmgTxtMoveSpeed * Time.deltaTime);
    }
}
