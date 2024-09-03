using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ai : MonoBehaviour
{


    private bool isActive = false;

    void Start()
    {
        var flow = FindFirstObjectByType<battle_flow>();
        flow.OnCountDown += OnCountdown;
        flow.OnBattle += OnBattle;
        flow.OnDefeat += OnDefeat;
    }

    void Update()
    {

    }

    void OnCountdown()
    {
        isActive = false;
    }
    void OnBattle()
    {
        isActive = true;
    }
    void OnDefeat()
    {
        isActive = false;
    }
}
