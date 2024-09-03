using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public enum EButton
{
    A, B,
    X, Y,
    Count
}

public class player_input : MonoEditorDebug
{
    [SerializeField] private int player_index = 0;
    [SerializeField] private float speed = 5f;

    [SerializeField] Attack fan_attack;

    CharacterController controller;
    character _character;

    Vector3 prevMovement = Vector3.zero;

    private Dictionary<KeyCode, EButton> keyButtonMap;
    private bool isActive = false;
    private string stick_input_x;
    private string stick_input_y;
    [ExposeInInspector("Is active: ")] bool IsActive => isActive;


    void Start()
    {
        var flow = FindFirstObjectByType<battle_flow>();
        flow.OnCountDown += OnCountdown;
        flow.OnBattle += OnBattle;
        flow.OnDefeat += OnDefeat;

        keyButtonMap = new Dictionary<KeyCode, EButton>()
        {
            { player_index == 0 ? KeyCode.Joystick1Button0 : KeyCode.Joystick2Button0, EButton.A },
            { player_index == 0 ? KeyCode.Joystick1Button1 : KeyCode.Joystick2Button1, EButton.B },
            { player_index == 0 ? KeyCode.Joystick1Button2 : KeyCode.Joystick2Button2, EButton.X },
            { player_index == 0 ? KeyCode.Joystick1Button3 : KeyCode.Joystick2Button3, EButton.Y },
        };

        stick_input_x = player_index == 0 ? "joystick_1_x" : "joystick_2_x";
        stick_input_y = player_index == 0 ? "joystick_1_y" : "joystick_2_y";

        controller = GetComponent<CharacterController>();
        _character = GetComponent<character>();
    }

    // Update is called once per frame
    void Update()
    {
        float x_axis = Input.GetAxis(stick_input_x);
        float y_axis = Input.GetAxis(stick_input_y);

        float frame_movement = speed * Time.deltaTime;
        Vector3 motion = new Vector3(x_axis, 0f, y_axis) * frame_movement;

        if (isActive)
        {
            if (_character.IsMovementAllowed)
            {
                controller.Move(motion);
            }

            HashSet<EButton> active_buttons = new HashSet<EButton>();
            foreach (var kvp in keyButtonMap)
                if (Input.GetKeyDown(kvp.Key))
                    active_buttons.Add(kvp.Value);

            if (active_buttons.Count > 0)
            {
                if (active_buttons.Contains(EButton.A))
                {
                    if (motion == Vector3.zero)
                        motion = prevMovement;
                    _character.StartAttack(fan_attack, motion);
                }
            }
        }

        prevMovement = motion;
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
