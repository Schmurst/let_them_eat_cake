using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class AIOvermind : MonoEditorDebug
{
    [SerializeField] private ai ai_prefab;

    [SerializeField] private Transform alive_tr;
    [SerializeField] private Transform dead_tr;

    public event Action<int, int> OnKill;
    [Serializable] public class Spawn
    {
        public GameObject spawn_loc;
        public int initial_spawn_count = 5;
        public float initial_delay = 2f;
        public int wave_spawn = 2;
        public float wave_delay = 2f;
    }
    [Serializable] public class Wave
    {
        public List<Spawn> spawns;
        public float duration;
        public float speed_mult = 1f;
    }
    [Serializable] public class Balance
    {
        public float speed = 4f;
        public float attack_distance = 2f;
        public Attack attack;
        public float death_time = 2f;
    }

    public class RunTimeSpawn
    {
        public int amount= 0;
        public float count_down = 0f;
    }

    [ExposeInInspector("wave id: ")] public int WaveIdx => wave_idx;
    [ExposeInInspector("remaining wave time: ")] public float WaveTime
    {
        get
        {
            Wave w = currentWave;
            return w.duration - wave_time;
        }
    }

    public List<Wave> waves; 
    public Balance balance;

    private List<ai> alive_ai = new List<ai>();
    int wave_idx = 0;
    public int total_waves = 0;
    List<RunTimeSpawn> spawnTimes = new List<RunTimeSpawn>();

    private bool isActive = false;
    private int total_kills = 0;
    public Wave currentWave => waves[wave_idx];

    private float wave_time = 0;
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
        if (isActive)
        {
            var wave = waves[wave_idx];

            wave_time += Time.deltaTime;
            if (wave_time > wave.duration)
            {
                wave_idx = ++wave_idx % waves.Count;
                StartWave();
            }

            for (int i = 0; i < spawnTimes.Count; i++)
            {
                spawnTimes[i].count_down -= Time.deltaTime;
                if (spawnTimes[i].count_down < 0f)
                {
                    var wv = wave.spawns[i];
                    for (int j = 0; j < spawnTimes[i].amount; j++)
                    {
                        int idx = j % wv.spawn_loc.transform.childCount;
                        Transform tr = wv.spawn_loc.transform.GetChild(idx);
                        alive_ai.Add(SpawnAI(tr.position)); 
                    }

                    spawnTimes[i].count_down = wv.wave_delay;
                    spawnTimes[i].amount = wv.wave_spawn;
                }
            }
        }
    }
    public void Kill(ai _ai, int id)
    {
        alive_ai.Remove(_ai);
        Destroy(_ai.gameObject);

        if (OnKill != null)
            OnKill(++total_kills, id);
    }

    void OnCountdown()
    {
        isActive = false;
    }
    void OnBattle()
    {
        isActive = true;

        //start waves
        StartWave();
    }
    private void StartWave()
    {
        spawnTimes.Clear();

        Wave wv = waves[wave_idx];
        wave_time = 0f;
        foreach (var sp in wv.spawns)
        {
            var jam = new RunTimeSpawn();
            jam.count_down = sp.initial_delay;
            jam.amount = sp.initial_spawn_count;
                
            spawnTimes.Add(jam);
        }

        total_waves++;
    }

    [EditorDebugMethod]
    ai SpawnAI(Vector3 position)
    {
        var ai = GameObject.Instantiate(ai_prefab);
        ai.transform.SetParent(alive_tr, true);
        ai.transform.position = position;
        var _char = ai.GetComponent<CharacterController>();

        return ai;
    }

    void OnDefeat()
    {
        isActive = false;
    }
}
