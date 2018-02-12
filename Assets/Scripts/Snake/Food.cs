using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour {

    public event System.Action OnDisappear;
    public Transform ring;
    Material m;
    public float maxS = 2;
    public float speed = 1;
    float p;

    public float lifetime = 10;
    float time;
    float startScale;

    void Start()
    {
        startScale = transform.localScale.x;
        m = ring.GetComponent<MeshRenderer>().material;
    }

	void Update () {
        time += Time.deltaTime;
        p += Time.deltaTime * speed;
        ring.transform.localScale = Vector3.one * Mathf.Clamp01(p) * maxS;
        m.color = new Color(m.color.r, m.color.g, m.color.b, Mathf.Lerp(.7f,0,Mathf.Clamp01(p)));
        if (p > 1)
        {
            p = -.4f;
        }

        float lifePercent = time / lifetime;
        transform.localScale = Vector3.one * Mathf.Lerp(startScale, 0, lifePercent);

        if (lifePercent >= 1)
        {
            if (OnDisappear != null)
            {
                OnDisappear();
            }
            Destroy(gameObject);
        }
	}

}
