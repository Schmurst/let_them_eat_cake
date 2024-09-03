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
        var target = GetClosestPlayer(out var dist_to_target, out Vector3 to_target);

        //calculate velocity
        Vector3 movement_vec = Vector3.Normalize(to_target);

        Vector3 velocity = movement_vec * mind.balance.speed * Time.deltaTime;

        if (character.IsMovementAllowed)
            controller.Move(velocity);

        //attack check
        if (dist_to_target < mind.balance.attack_distance)
        {
            character.StartAttack(mind.balance.attack, to_target);
        }
    }

    private player_input GetClosestPlayer(out float dist_to_target, out Vector3 to_target)
    {
        var players = FindObjectsByType<player_input>(FindObjectsSortMode.None);
        player_input target = null;
        dist_to_target = float.MaxValue;
        Vector3 our_pos = transform.position;
        //get closest 
        foreach (var p in players)
        {
            float dist = Vector3.Distance(p.transform.position, our_pos);
            if (dist < dist_to_target)
            {
                dist_to_target = dist;
                target = p;
            }
        }

        to_target = target.transform.position - transform.position;
        return target;
    }

    void OnStateChange(CharState state)
    {
        switch (state)
        {
            case CharState.moving:
                break;
            case CharState.attacking:
            {
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
