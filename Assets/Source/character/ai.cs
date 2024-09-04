using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ai : MonoBehaviour
{
    private AIOvermind mind;
    CharacterController controller;
    private character _character;
    private bool isActive = false;

    private float deathTime = 0f;
    private float spawn_cooldown = 0.1f;

    private float speed = 0f;
    void Start()
    {
        var flow = FindFirstObjectByType<battle_flow>();
        flow.OnCountDown += OnCountdown;
        flow.OnBattle += OnBattle;
        flow.OnDefeat += OnDefeat;

        _character = GetComponent<character>();
        controller = GetComponent<CharacterController>();
        mind = FindFirstObjectByType<AIOvermind>();

        _character.OnStateChange += OnStateChange;

        controller.enabled = false;
        speed = (UnityEngine.Random.value * 2f - 1f) * 0.3f * mind.balance.speed + mind.balance.speed;

        Vector3 v = transform.position;
        v.y = 1f;
        transform.position = v;
    }

    void Update()
    {
        if (spawn_cooldown > 0f)
        {
            spawn_cooldown -= Time.deltaTime;
            if (spawn_cooldown < 0f)
            {
                controller.enabled = true;
            }
        }

        switch (_character.State)
        {
            case CharState.moving:
                //find players
                var target = GetClosestPlayer(out var dist_to_target, out Vector3 to_target);
                if (target)
                {
                    //calculate velocity
                    Vector3 movement_vec = Vector3.Normalize(to_target);
                    Vector3 velocity = movement_vec * speed * Time.deltaTime;

                    if (_character.IsMovementAllowed)
                        controller.SimpleMove(velocity / Time.deltaTime);

                    //attack check
                    if (dist_to_target < mind.balance.attack_distance)
                    {
                        _character.StartAttack(mind.balance.attack, to_target);
                    }
                }
                break;
            case CharState.attacking:
                break;
            case CharState.recoiling:

                break;
            case CharState.dying:
                deathTime += Time.deltaTime;

                if (deathTime > mind.balance.death_time) 
                    mind.Kill(this);
                break;
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
