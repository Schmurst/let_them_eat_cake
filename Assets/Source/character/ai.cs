using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ai : MonoBehaviour
{
    private AIOvermind mind;
    CharacterController controller;
    private character character;
    private bool isActive = false;


    void Start()
    {
        var flow = FindFirstObjectByType<battle_flow>();
        flow.OnCountDown += OnCountdown;
        flow.OnBattle += OnBattle;
        flow.OnDefeat += OnDefeat;

        character = GetComponent<character>();
        controller = GetComponent<CharacterController>();
        mind = FindFirstObjectByType<AIOvermind>();

        character.OnStateChange += OnStateChange;
    }

    void Update()
    {
        //find players
        var players = FindObjectsByType<player_input>(FindObjectsSortMode.None);
        player_input target = null;
        float dist_to_target = float.MaxValue;
        Vector3 our_pos = transform.position;
        //get closest 
        foreach (var p in players)
        {
            float dist = Vector3.Distance(transform.position, our_pos);
            if (dist < dist_to_target)
            {
                dist_to_target = dist;
                target = p;
            }
        }

        //calculate velocity
        Vector3 to_target = target.transform.position - our_pos;
        Vector3 movement_vec = Vector3.Normalize(to_target);

        Vector3 velocity = movement_vec * mind.balance.speed * Time.deltaTime;
        controller.Move(velocity);

        //attack check
        if (dist_to_target < mind.balance.attack_dist)
        {
            character.EnterState(CharState.attacking);
        }
    }

    void OnStateChange(CharState state)
    {
        switch (state)
        {
            case CharState.moving:
                break;
            case CharState.attacking:
            {
                // send move info
                
                // damage check

                // 

                break;
            }
            case CharState.recoiling:
                break;
            case CharState.dying:
                break;
        }
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
