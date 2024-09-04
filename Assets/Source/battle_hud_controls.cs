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
    [SerializeField] Image p1_hp;
    [SerializeField] Image p2_hp;

    [SerializeField] private List<jiggla_ui> p1_jiggs;
    [SerializeField] private List<jiggla_ui> p2_jiggs;

    public character p1;
    public character p2;
    public AIOvermind aiii;

    void Start()
    {
        OnP1HealthChange(1f);
        OnP2HealthChange(1f);

        p1.OnDamage += OnP1HealthChange;
        p2.OnDamage += OnP2HealthChange;
        aiii.OnKill += OnKill;
    }

    void Update()
    {
        int itime = Mathf.CeilToInt(aiii.WaveTime);

        time.text = $"{itime}s";
    }

    void OnKill(int killas)
    {
        kills.text = $"{killas} Stinking Peasants Denied Cake";
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
