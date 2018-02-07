using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpawner : MonoBehaviour {

    //string chars = "!@£$%^&*()_+œ∑´†¥^øπ“æ…¬˚∆˙©ƒ∂ßå`Ω≈ç√∫~µ≤≥÷¿˘¯˜ˆı◊ÇÙÛŸÅÍÎÏÌÓÆØÈËÁÊÂ‰„Œ⁄‹›ﬁﬂ‡°·‚—±§1234567890";
    string chars = "@%¿¿¿¿Ÿ¥∆°çµ";
    BoxCollider2D[] screens;
    public TextMesh foodPrefab;
    float nextSpawnTime;

    void Start()

    {
        screens = FindObjectOfType<ScreenAreas>().allScreens;
    }

    void Update()
    {
        if (Time.time > nextSpawnTime)
        {
            nextSpawnTime = Time.time + 6;
            TextMesh snack = Instantiate(foodPrefab);
            snack.text = Build();
            snack.transform.position = RandomPoint();
            snack.transform.parent = transform;
        }
    }

    Vector2 RandomPoint()
    {
        int i = Random.Range(0,screens.Length);
        Bounds b = screens[i].bounds;
        b.Expand(-.5f);

        return new Vector2(Random.Range(b.min.x, b.max.x), Random.Range(b.min.y, b.max.y));
    }

    string Build(int len = 2)
    {
        string s = "";
        for (int i = 0; i < len; i++)
        {
            s += chars[Random.Range(0, chars.Length)];
        }
        return s;
    }
}
