using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour {
    public Transform ring;
    Material m;
    public float maxS = 2;
    public float speed = 1;
    float p;

    void Start()
    {
        m = ring.GetComponent<MeshRenderer>().material;
    }

	void Update () {
        p += Time.deltaTime * speed;
        ring.transform.localScale = Vector3.one * Mathf.Clamp01(p) * maxS;
        m.color = new Color(m.color.r, m.color.g, m.color.b, Mathf.Lerp(.7f,0,Mathf.Clamp01(p)));
        if (p > 1)
        {
            p = -.4f;
        }
	}
}
