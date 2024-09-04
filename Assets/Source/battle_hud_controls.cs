using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class battle_hud_controls : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI time;
    [SerializeField] TextMeshProUGUI wave;
    [SerializeField] TextMeshProUGUI kills;
    [SerializeField] TextMeshProUGUI kills_p1;
    [SerializeField] TextMeshProUGUI kills_p2;
    [SerializeField] Image p1_hp;
    [SerializeField] Image p2_hp;

    [SerializeField] private List<jiggla_ui> p1_jiggs;
    [SerializeField] private List<jiggla_ui> p2_jiggs;

    public character p1;
    public character p2;
    public AIOvermind aiii;

    private int p1_ks = 0;
    private int p2_ks = 0;

    void Start()
    {
        OnP1HealthChange(1f);
        OnP2HealthChange(1f);

        p1.OnDamage += OnP1HealthChange;
        p2.OnDamage += OnP2HealthChange;
        aiii.OnKill += OnKill;

        time.text = $"READY";
        kills.text = $"LET THEM EAT CAKE!";
        kills_p1.text = $"Wallops: 0";
        kills_p2.text = $"Thrashings: 0";
    }

    void Update()
    {
        int itime = Mathf.CeilToInt(aiii.WaveTime);

        time.text = $"{itime}s";
        wave.text = $"Wave: {aiii.total_waves}";
    }

    void OnKill(int killas, int id)
    {

        if (id == 0)
        {
            p1_ks++;
            kills_p1.text = $"Wallops: {p1_ks}";
            kills_p1.GetComponent<jiggla_ui>().vibration_timer = 1f;
        }
        else
        {
            p2_ks++;
            kills_p2.text = $"Thrashings: {p2_ks}";
            kills_p2.GetComponent<jiggla_ui>().vibration_timer = 1f;
        }

        kills.text = $"{killas} Revolting Peasants Denied Cake";
        kills.GetComponent<jiggla_ui>().vibration_timer = 1f;
    }

    void OnP1HealthChange(float pcnt)
    {
        p1_hp.fillAmount = pcnt;
        foreach (var jig in p1_jiggs)
            jig.vibration_timer = 1f;
    }

    void OnP2HealthChange(float pcnt)
    {
        p2_hp.fillAmount = pcnt;

        foreach (var jig in p2_jiggs)
            jig.vibration_timer = 1f;
    }

}
