using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIOvermind : MonoBehaviour
{
    [SerializeField] private ai ai_prefab;

    private bool isActive = false;
    // Start is called before the first frame update
    void Start()
    {
        var flow = FindFirstObjectByType<battle_flow>();
        flow.OnCountDown += OnCountdown;
        flow.OnBattle += OnBattle;
        flow.OnDefeat += OnDefeat;
    }

    // Update is called once per frame
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
