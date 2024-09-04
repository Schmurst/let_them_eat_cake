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
    public int ticks = 1;
    public float delay = 0.25f;
    public float arc_degrees = 120f;
    public float arc_radius = 2f;
    public float cooldown = 0.5f;
}
public enum CharState
{
    moving,
    attacking,
    recoiling,
    dying,
}

public class character : MonoEditorDebug
{
    [SerializeField] private int starting_hp = 1;
    [SerializeField] private float recoil_time = 1f;
    [SerializeField] private float death_time = 1f;
    [SerializeField] private GameObject blood_vfx_prefab;

    public event Action<CharState> OnStateChange;
    public event Action<Vector3> OnMove;
    public event Action<float> OnDamage;

    Vector3 attack_direction = Vector3.back;

    CharacterController controller;
    private int hp = 0;
    Attack currentAttack = null;
    CharState state = CharState.moving;
    private float stateTime = 0f;

    private List<character> chars_hit_by_attack = new List<character>();

    private int attack_frames_done = 0;
    private float attack_timer = 0f;
    private float damage_countdown = 0f;
    private float attack_cooldown = 0f;

    public float StateTime => stateTime;
    [ExposeInInspector("HP:")] public int HP => hp;
    [ExposeInInspector("can move:")] public bool IsMovementAllowed => state == CharState.moving;
    [ExposeInInspector("state:")] public CharState State => state;
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
                {
                    if (hp > 0)
                        EnterState(CharState.moving);
                    else
                        EnterState(CharState.dying);
                }

                break;
            case CharState.dying:
                break;
        }

        attack_cooldown -= Time.deltaTime;
        damage_countdown -= Time.deltaTime;
    }

    public void Move(Vector3 velocity)
    {
        controller.Move(velocity);

        if (OnMove != null)
            OnMove(velocity);
    }

    void DoDamage(int damage)
    {
        if (state == CharState.recoiling)
            return;

        if (damage_countdown > 0f)
            return;

        damage_countdown = 0.8f;
        hp -= damage;
        if (OnDamage != null)
            OnDamage(hp/(float)starting_hp);

        EnterState(CharState.recoiling);
    }

    void ProcessAttack()
    {
        //end it
        if (stateTime > currentAttack.duration)
        {
            attack_cooldown = currentAttack.cooldown;
            currentAttack = null;
            return;
        }

        //movement
        float progress = stateTime / currentAttack.duration;
        float speed = currentAttack.curve.Evaluate(progress);

        Vector3 move = attack_direction * Time.deltaTime * speed * currentAttack.movement_mult;
        Move(move);

        //did we just tick?
        bool do_attack = false;
        if (currentAttack.ticks > attack_frames_done)
        {
            if (stateTime > attack_timer)
            {
                do_attack = true;
                attack_timer += currentAttack.delay;
                attack_frames_done++;
            }
        }

        if (do_attack)
        {
            var potentials = FindObjectsByType<character>(FindObjectsSortMode.None);
            foreach (var target in potentials)
            {
                if (target == this)
                    continue;
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

                float dot = Vector3.Dot(attack_direction, Vector3.Normalize(to_target));
                if (dot > Mathf.Cos(Mathf.Deg2Rad * currentAttack.arc_degrees))
                {
                    target.DoDamage(currentAttack.damage);
                    chars_hit_by_attack.Add(target);
                }
            }
        }
    }

    void ResetAttack()
    {
        attack_frames_done = 0;
        attack_timer = 0f;
        currentAttack = null;
    }

    public void StartAttack(Attack _attack, Vector3 direction)
    {
        if (!CanEnterState(CharState.attacking))
            return ;

        EnterState(CharState.attacking);

        attack_frames_done = 0;
        attack_timer = _attack.delay;
        currentAttack = _attack;
        chars_hit_by_attack.Clear();
        attack_direction = Vector3.Normalize(direction);
    }

    public bool EnterState(CharState _state)
    {
        if (!CanEnterState(_state))
            return false;

        //leave old one
        switch (state)
        {
            case CharState.attacking:
                ResetAttack();
                break;
        }

        stateTime = 0f;
        state = _state;

        switch (_state)
        {
            case CharState.recoiling:
            {
                var go =Instantiate(blood_vfx_prefab, transform);
                go.transform.position = transform.position;

                StartCoroutine(Co_DoAfterSeconds(() =>
                {
                    Destroy(go);
                },5f));
                break;
            }
        }

        if (OnStateChange != null)
            OnStateChange(state);

        return true;
    }

    IEnumerator Co_DoAfterSeconds(Action action, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        action();
    }

    bool CanEnterState(CharState _state)
    {
        switch (state)
        {
            case CharState.moving:
                switch (_state)
                {
                    case CharState.moving: return true;
                    case CharState.attacking: return attack_cooldown < 0f;
                    case CharState.recoiling: return true;
                    case CharState.dying: return true;
                }
                break;
            case CharState.attacking:
                switch (_state)
                {
                    case CharState.moving: return true;
                    case CharState.attacking: return false;
                    case CharState.recoiling: return true;
                    case CharState.dying: return true;
                }
                break;
            case CharState.recoiling:
                switch (_state)
                {
                    case CharState.moving: return true;
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