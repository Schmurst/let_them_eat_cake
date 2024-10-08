using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jiggla_ui : MonoBehaviour
{
    public  float vibration_timer = 0f;
    Vector2 initial_ap = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        RectTransform rt = GetComponent<RectTransform>();
        initial_ap = rt.anchoredPosition;
    }

    void Update()
    {
        RectTransform rt = GetComponent<RectTransform>();
        vibration_timer -= Time.deltaTime;
        float vb_progress = Mathf.Pow(vibration_timer, 4);
        if (vibration_timer >= 0f)
        {
            rt.anchoredPosition = initial_ap + 50f * new Vector2(
                vb_progress * (2f* Mathf.PerlinNoise1D(10f * (Time.time + UnityEngine.Random.value)) -1f),
                vb_progress * (2f * Mathf.PerlinNoise1D(10f * (Time.time + UnityEngine.Random.value)) - 1f)
            );
        }
        else
        {
            vibration_timer = 0f;
            rt.anchoredPosition = initial_ap;
        }
    }
}
