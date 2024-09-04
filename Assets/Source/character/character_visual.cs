using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class character_visual : MonoBehaviour
{
    public Sprite run;
    public Sprite attack;
    public Sprite recoil;
    public Sprite dead;

    public AnimationCurve scale_x_curve;
    public AnimationCurve scale_y_curve;
    public float scale_duration = 0.7f;

    public bool flip = false;

    private SpriteRenderer render;
    private character _character;

    private float vibrate_timer = 0f;
    private float scale_timer = 0f;

    private bool going_left = true;

    void Start()
    {
        render = GetComponent<SpriteRenderer>();
        _character = GetComponentInParent<character>();
        _character.OnStateChange += OnStateChange;
        render.sprite = run;
        scale_timer += UnityEngine.Random.value;
        _character.OnMove += OnMove;
    }

    void Update()
    {
        //VIBRATION
        vibrate_timer -= Time.deltaTime;
        float vb_progress = Mathf.Pow(vibrate_timer, 4);
        if (vibrate_timer >= 0f)
        {
            float alpha = UnityEngine.Random.value;
            transform.localPosition = new Vector3(
                vb_progress * Mathf.PerlinNoise1D(150f * (Time.time + UnityEngine.Random.value)),
                vb_progress * Mathf.PerlinNoise1D(150f * (Time.time + UnityEngine.Random.value)),
                0f
            ) + Vector3.up;
        }
        else
        {
            vibrate_timer = 0f;
            transform.localPosition = Vector3.up;
        }

        switch (_character.State)
        {
            case CharState.moving:
            {
                scale_timer += Time.deltaTime / scale_duration;
                scale_timer %= 1f;

                float x = scale_x_curve.Evaluate(scale_timer);
                float y = scale_y_curve.Evaluate(scale_timer);

                transform.localScale = new Vector3(x, y, 1f);

                break;
            }
            case CharState.attacking:
            case CharState.recoiling:
            case CharState.dying:
                transform.localScale = Vector3.one;
                break;
        }

        Vector3 scale = transform.localScale;
        scale.x = (going_left ? 1f : -1f) * Mathf.Abs(scale.x);
        transform.localScale = scale;
    }
    void OnMove(Vector3 vel)
    {
        going_left = vel.x < 0f;

        if (flip)
            going_left = !going_left;
    }

    void OnStateChange(CharState state)
    {
        switch (state)
        {
            case CharState.moving: 
                render.sprite = run;
                break;
            case CharState.attacking:
                render.sprite = attack;
                break;
            case CharState.recoiling:
                vibrate_timer = 1f;
                render.sprite = recoil;
                break;
            case CharState.dying:
                vibrate_timer = 1f;
                render.sprite = dead;
                break;
        }
    }
}
