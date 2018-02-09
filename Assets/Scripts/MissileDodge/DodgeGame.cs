using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DodgeGame : MonoBehaviour {

    public Missile missilePrefab;
    public Transform[] possibleSpawns;
    public Vector2 delayStartEnd;

    float nextSpawnTime;
    public int duration = 64;
    public TextMesh countdownUI;
    float timeRemaining;
    public int max = 20;
    Transform player;
    int curr;

    void Start()
    {
        timeRemaining = duration;
        player = FindObjectOfType<Dodger>().transform;
    }

	void Update () {
        float percent = 1-Mathf.Clamp01(timeRemaining / duration);
       
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
            nextSpawnTime = Time.time + Mathf.Lerp(delayStartEnd.x, delayStartEnd.y, percent);

            Missile missile = Instantiate(missilePrefab);
            missile.transform.position = pos;
            missile.transform.parent = transform;
            missile.OnDeath += OnMissileDeath;
            missile.OnPlayerDeath += OnPlayerDeath;
        }

        timeRemaining -= Time.deltaTime;
        timeRemaining = Mathf.Clamp(timeRemaining, 0, duration);
        countdownUI.text = Mathf.RoundToInt(timeRemaining) + "";
	}

    void OnMissileDeath()
    {

    }

    void OnPlayerDeath()
    {
        Debug.Break();
    }
}
