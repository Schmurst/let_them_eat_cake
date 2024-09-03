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
    public float movement_mult = 1.0f;
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
    dying,
}

public class character : MonoBehaviour
{
    [SerializeField] private int starting_hp = 1;
    [SerializeField] private float recoil_time = 1f;
    [SerializeField] private float death_time = 1f;

    public event Action<CharState> OnStateChange;

    Vector3 attack_direction = Vector3.back;

    CharacterController controller;
    private int hp = 0;
    Attack currentAttack = null;
    CharState state = CharState.moving;
    private float stateTime = 0f;

    private List<character> chars_hit_by_attack = new List<character>();

    public bool IsMovementAllowed => state == CharState.moving;
    public CharState State => state;
    void Start()
    {
        controller = GetComponent<CharacterController>();
        hp = starting_hp;
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
                    EnterState(CharState.moving);

                break;
            case CharState.recoiling:

                if (stateTime > recoil_time)
                    EnterState(CharState.moving);

                break;
            case CharState.dying:
                if (stateTime > death_time)
                {

                }
                break;
        }
    }

    void DoDamage(int damage)
    {
        hp -= damage;
        if (hp < 0)
        {
            EnterState(CharState.dying);
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

        //movement
        float progress = stateTime / currentAttack.duration;
        float speed = currentAttack.curve.Evaluate(progress);

        Vector3 move = attack_direction * Time.deltaTime * speed * currentAttack.movement_mult;
        controller.Move(move);

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
                    if (chars_hit_by_attack.Contains(target))
                        continue;
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
                        DoDamage(currentAttack.damage);
                        chars_hit_by_attack.Add(target);
                    }
                }
            }
        }
    }
    public void StartAttack(Attack _attack, Vector3 direction)
    {
        if (!CanEnterState(CharState.attacking))
            return ;

        EnterState(CharState.attacking);

        currentAttack = _attack;
        chars_hit_by_attack.Clear();
        attack_direction = Vector3.Normalize(direction);
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