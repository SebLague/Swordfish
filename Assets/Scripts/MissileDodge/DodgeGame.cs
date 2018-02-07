using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DodgeGame : MonoBehaviour {

    public Missile missilePrefab;
    public Transform[] possibleSpawns;

    float nextSpawnTime;


	void Update () {
        if (Time.time > nextSpawnTime)
        {
            nextSpawnTime = Time.time + 3;

            Missile missile = Instantiate(missilePrefab);
            missile.transform.position = possibleSpawns[Random.Range(0, possibleSpawns.Length)].position;
            missile.transform.parent = transform;
            missile.OnDeath += OnMissileDeath;
            missile.OnPlayerDeath += OnPlayerDeath;
        }
	}

    void OnMissileDeath()
    {

    }

    void OnPlayerDeath()
    {

    }
}
