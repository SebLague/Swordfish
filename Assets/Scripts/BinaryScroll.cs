using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BinaryScroll : MonoBehaviour {

    public TextMesh[] t;
    public float speed = 3;
    Vector2 minMaxY;
    public int lines = 10;
    public int chars = 20;

	// Use this for initialization
	void Start () {
        minMaxY = FindObjectOfType<ScreenAreas>().minMaxY;

        foreach (TextMesh m in t)
        {
            string bin = "";
            for (int i = 0; i < lines; i++)
            {
                for (int j = 0; j < chars; j++)
                {
                    bin += Random.Range(0, 2) + "";
                }
                bin += "\n";
            }
            m.text = bin;
        }
	}
	
	// Update is called once per frame
	void Update () {
        for (int i = 0; i < 2; i++)
        {
            TextMesh m = t[i];
            m.transform.position += Vector3.down * speed * Time.deltaTime;
            if (m.transform.position.y < minMaxY.x)
            {
                float h = m.GetComponent<MeshRenderer>().bounds.size.y;
                m.transform.position = new Vector3(m.transform.position.x, t[(i + 1) % 2].transform.position.y + h);
            }
        }
	}
}
