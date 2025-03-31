using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SC_PlayerHUD : MonoBehaviour
{
    public Image healthImage;
    public SC_PlayerStateMachine player;

    void Update()
    {
        healthImage.fillAmount = (float)player.currentHP / (float)player.maxHP;
    }
}

