using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DodgeGame : Task {

    public Missile missilePrefab;
    public Transform[] possibleSpawns;

    float nextSpawnTime;
    public int duration = 64;
    public TextMesh countdownUI;
    float timeRemaining;
    public int max = 20;
    Transform player;
    int curr;
    float delay;
    public Transform defaultPlayerSpawn;
    public GameObject playerPrefab;
    float startTime;
	public override void EnterEasyMode_Debug()
	{
		base.EnterEasyMode_Debug();
        duration = 3;
        timeRemaining = duration;
	}


	void Start()

    {
        startTime = Time.time;
        timeRemaining = duration;
        if (FindObjectOfType<Dodger>() == null)
        {
            Instantiate(playerPrefab, defaultPlayerSpawn.position, Quaternion.identity, transform);
        }
        player = FindObjectOfType<Dodger>().transform;
        delay = (duration - 10) / (float)(max);
    }

	protected override void Update()
	{
		base.Update();
        if (!taskOver)
        {
            float percent = 1 - Mathf.Clamp01(timeRemaining / duration);

            if (Time.time > nextSpawnTime && curr < max)
            {
                curr++;
                Vector2 pos = Vector2.zero;
                float thresh = 8;
                int sI = Random.Range(0, possibleSpawns.Length);
                for (int i = 0; i < possibleSpawns.Length; i++)
                {
                    pos = possibleSpawns[(i + sI) % possibleSpawns.Length].position;
                    if (Vector2.Distance(pos, player.position) > thresh)
                    {
                        break;
                    }

                }
                float currDelay = delay;
                if (curr == 1)
                {
                    currDelay = 4;
                }
                nextSpawnTime = Time.time + delay;

                Missile missile = Instantiate(missilePrefab);
                missile.transform.position = pos;
                missile.transform.parent = transform;
                missile.OnDeath += OnMissileDeath;
                missile.OnPlayerDeath += OnPlayerDeath;
            }

            if (Time.time - startTime > 1)
            {
                timeRemaining -= Time.deltaTime;
            }
            timeRemaining = Mathf.Clamp(timeRemaining, 0, duration);
            countdownUI.text = Mathf.RoundToInt(timeRemaining) + "";

            if (timeRemaining <= 0)
            {
                TaskCompleted();
            }
        }
	}

    void OnMissileDeath()
    {

    }

    void OnPlayerDeath()
    {
        if (!taskOver)
        {
            Destroy(player.gameObject);
            TaskFailed();
        }
    }
}
