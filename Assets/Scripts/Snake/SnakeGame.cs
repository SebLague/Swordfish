using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeGame : Task {

    BoxCollider2D[] screens;
    public GameObject foodGOPrefab;
    public Vector2 timeBetweenSpawns;
    public int numToSpawn = 32;
    public int maxNumOnScreen = 5;
    float nextSpawnTime;
    int numLeftToSpawn;
    int numEaten;
    int numOnScreen;
    public TextMesh numRemText;
    public Transform endPos;

	public override void EnterEasyMode_Debug()
	{
		base.EnterEasyMode_Debug();
        numToSpawn = 1;
	}

    private void Awake()
    {
       
    }

    void Start()

    {
		numLeftToSpawn = numToSpawn;
        numRemText.text = numToSpawn+"";
        numRemText.gameObject.SetActive(true);

        screens = FindObjectOfType<ScreenAreas>().allScreens;
        FindObjectOfType<Snake>().OnEat += OnEat;
        FindObjectOfType<Snake>().OnDeathEvent += TaskFailed;

    }

	protected override void Update()
	{
		base.Update();

        numRemText.text = (numToSpawn-numEaten)+"";
        if (numOnScreen < maxNumOnScreen && Time.time > nextSpawnTime && numLeftToSpawn>0)
        {
            if (numLeftToSpawn > 1 || numOnScreen == 0) // dont spawn last until all gone
            {
                numOnScreen++;
                numLeftToSpawn--;
                nextSpawnTime = Time.time + Random.Range(timeBetweenSpawns.x, timeBetweenSpawns.y);
                GameObject snack = Instantiate(foodGOPrefab, (numLeftToSpawn == 0) ? (Vector2)endPos.position : RandomPoint(), Quaternion.identity);

                snack.transform.parent = transform;
                snack.transform.localEulerAngles = Vector3.up * 180;
                snack.GetComponent<Food>().OnDisappear += OnDisappear;
            }
        }
    }

    void OnDisappear()
    {
        numOnScreen--;
        numLeftToSpawn++;
    }
    void OnEat()
    {
        numOnScreen--;
        numEaten++;

        if (numEaten >= numToSpawn)
        {
            TaskCompleted();
            FindObjectOfType<Snake>().Success();
        }
       
    }

  

    Vector2 RandomPoint()
    {
      
        int mainScreenIndex = 4;
        int i = mainScreenIndex;
        if (Random.Range(0, 100) < 80)
        {
            i = Random.Range(0, screens.Length);
        }

        Bounds b = screens[i].bounds;
        b.Expand(-.5f);

        return new Vector2(Random.Range(b.min.x, b.max.x), Random.Range(b.min.y, b.max.y));
    }

}
