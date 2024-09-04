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

    private SpriteRenderer render;
    private character _character;

    private float vibrate_timer = 0f;
    private float scale_timer = 0f;

    void Start()
    {
        render = GetComponent<SpriteRenderer>();
        _character = GetComponentInParent<character>();
        _character.OnStateChange += OnStateChange;
        render.sprite = run;
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
                vb_progress * Mathf.PerlinNoise1D(15f * (Time.time + UnityEngine.Random.value)),
                vb_progress * Mathf.PerlinNoise1D(15f * (Time.time + UnityEngine.Random.value)),
                0f
            );
        }
        else
        {
            vibrate_timer = 0f;
            transform.localPosition = Vector3.zero;
        }

        switch (_character.State)
        {
            case CharState.moving:
            {

                break;
            }
            case CharState.attacking:
            case CharState.recoiling:
            case CharState.dying:
                break;
        }
    }

    void OnStateChange(CharState state)
    {
        switch (state)
        {
            case CharState.moving: 
                render.sprite = run;
                break;
            case CharState.attacking:
                vibrate_timer = 0.5f;
                render.sprite = attack;
                break;
            case CharState.recoiling:
                vibrate_timer = 0.5f;
                render.sprite = recoil;
                break;
            case CharState.dying:
                vibrate_timer = 0.5f;
                render.sprite = dead;
                break;
        }
    }
}
