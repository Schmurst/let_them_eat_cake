using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ai : MonoBehaviour
{
    private AIOvermind mind;
    CharacterController controller;
    private bool isActive = false;



    void Start()
    {
        var flow = FindFirstObjectByType<battle_flow>();
        flow.OnCountDown += OnCountdown;
        flow.OnBattle += OnBattle;
        flow.OnDefeat += OnDefeat;

        controller = GetComponent<CharacterController>();
        mind = FindFirstObjectByType<AIOvermind>();
    }

    void Update()
    {
        //find players
        var players = FindObjectsByType<player_input>(FindObjectsSortMode.None);
        player_input best = null;
        float best_dist = float.MaxValue;
        Vector3 our_pos = transform.position;
        //get closest 
        foreach (var p in players)
        {
            float dist = Vector3.Distance(transform.position, our_pos);
            if (dist < best_dist)
            {
                best_dist = dist;
                best = p;
            }
        }

        //calculate velocity
        Vector3 to_target = best.transform.position - our_pos;
        Vector3 movement_vec = Vector3.Normalize(to_target);

        Vector3 velocity = movement_vec * mind.balance.speed * Time.deltaTime;
        controller.Move(velocity);
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
