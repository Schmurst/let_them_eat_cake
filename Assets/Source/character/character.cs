using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.TextCore.Text;

[Serializable]
public class Attack
{
    public float duration = 1f;
    public AnimationCurve curve;
    public int damage = 1;
    public bool hitEnemies = true;
    public int ticks = 3;
    public float arc_degrees = 120f;
    public float arc_radius = 2f;
}

public enum CharState
{
    moving,
    attacking,
    recoiling,
    dying
}

public class character : MonoBehaviour
{
    public event Action<CharState> OnStateChange;

    CharacterController controller;

    Attack currentAttack = null;
    CharState state = CharState.moving;
    private float stateTime = 0f;

    public bool IsMovementAllowed => state == CharState.moving;
    public CharState State => state;
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        stateTime += Time.deltaTime;
        switch (state)
        {
            case CharState.moving:
                break;
            case CharState.attacking:
                ProcessAttack();

                if (currentAttack == null)
                    EnterState(state);

                break;
            case CharState.recoiling:
                break;
            case CharState.dying:
                break;
        }
    }
    void ProcessAttack()
    {
        //end it
        if (stateTime > currentAttack.duration)
        {
            currentAttack = null;
            return;
        }

        //did we just tick?
        float tick_length = currentAttack.duration / currentAttack.ticks;
        for (int i = 0; i < currentAttack.ticks; i++)
        {
            float threshold = tick_length * i;
            float lower = threshold - Time.deltaTime;

            if (stateTime > lower && stateTime <= threshold)
            {
                var potentials = FindObjectsByType<character>(FindObjectsSortMode.None);

                foreach (var target in potentials)
                {
                    if (currentAttack.hitEnemies && target.GetComponent<ai>() == null)
                        continue;
                    if (!currentAttack.hitEnemies && target.GetComponent<player_input>() == null)
                        continue;

                    Vector3 to_target = target.transform.position - transform.position;
                    float dist = Vector3.Distance(target.transform.position, transform.position);

                    if (dist > currentAttack.arc_radius)
                        continue;

                    float dot = Vector3.Dot(transform.forward, Vector3.Normalize(to_target));
                    if (dot > Mathf.Cos(Mathf.Deg2Rad * currentAttack.arc_radius))
                    {
                        
                    }
                }
            }
        }
    }

    public void StartAttack(Attack _attack)
    {
        currentAttack = _attack;
    }

    public bool EnterState(CharState _state)
    {
        if (!CanEnterState(_state))
            return false;

        stateTime = 0f;
        state = _state;

        if (OnStateChange != null)
            OnStateChange(state);

        return true;
    }

    bool CanEnterState(CharState _state)
    {
        switch (state)
        {
            case CharState.moving:
                return true;
            case CharState.attacking:
                switch (_state)
                {
                    case CharState.moving: return true;
                    case CharState.attacking: return false;
                    case CharState.recoiling: return false;
                    case CharState.dying: return true;
                }
                break;
            case CharState.recoiling:
                switch (_state)
                {
                    case CharState.moving: return false;
                    case CharState.attacking: return false;
                    case CharState.recoiling: return false;
                    case CharState.dying: return true;
                }
                break;
            case CharState.dying:
                return false;
        }

        return false;
    }
}