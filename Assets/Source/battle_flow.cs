using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class battle_flow : MonoBehaviour
{
    public event Action OnCountDown;
    public event Action OnBattle;
    public event Action OnDefeat;

    [SerializeField] private float countDownDuration = 1.0f;

    public enum Phase
    {
        countDown,
        battle,
        waveEnd,
        defeat
    }

    Phase phase = Phase.countDown;
    float countDownTimer = 0;
    [ExposeInInspector()] public Phase CurrentPhase => phase;

    void Start()
    {
        countDownTimer = countDownDuration;
    }

    void Update()
    {
        switch (phase)
        {
            case Phase.countDown:
                countDownTimer -= Time.deltaTime;
                if (countDownTimer <= 0)
                {
                    EnterState(Phase.battle);
                }
                break;
            case Phase.battle:
                break;
            case Phase.waveEnd:
                break;
            case Phase.defeat:
                break;
        }
    }

    void EnterState(Phase _phase)
    {
        switch (_phase)
        {
            case Phase.countDown:
                if (OnCountDown != null)
                    OnCountDown();
                break;
            case Phase.battle:
                if (OnBattle != null)
                    OnBattle();
                break;
            case Phase.waveEnd:
                break;
            case Phase.defeat:
                break;
        }

        phase = _phase;
    }

    IEnumerator DoNextFrame(Action action)
    {
        yield return null;
        action();
    }
}
