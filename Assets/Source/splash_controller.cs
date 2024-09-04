using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class splash_controller : MonoBehaviour
{
    public jiggla_ui button;
    void Start()
    {
        button.vibration_timer = 1f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Joystick1Button0))
        {
            SceneManager.LoadScene(1);
        }


        if (button.vibration_timer <= 0f)
        {
            button.vibration_timer = 1f;
        }
    }
}
