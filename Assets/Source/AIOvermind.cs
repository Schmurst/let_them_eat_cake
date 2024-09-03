using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIOvermind : MonoEditorDebug
{
    [SerializeField] private ai ai_prefab;

    [SerializeField] private List<GameObject> spawn_locations;
    [SerializeField] private Transform alive_tr;
    [SerializeField] private Transform dead_tr;

    [Serializable]
    public class Balance
    {
        public float speed = 4f;
    }

    public Balance balance;

    private bool isActive = false;
    // Start is called before the first frame update
    void Start()
    {
        var flow = FindFirstObjectByType<battle_flow>();
        flow.OnCountDown += OnCountdown;
        flow.OnBattle += OnBattle;
        flow.OnDefeat += OnDefeat;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCountdown()
    {
        isActive = false;
    }
    void OnBattle()
    {
        isActive = true;

        SpawnAI(spawn_locations[0].transform.position);
    }

    [EditorDebugMethod]
    void SpawnAI(Vector3 position)
    {
        var ai = GameObject.Instantiate(ai_prefab);
        ai.transform.SetParent(alive_tr);
        ai.transform.position = position;
    }

    void OnDefeat()
    {
        isActive = false;
    }
}
