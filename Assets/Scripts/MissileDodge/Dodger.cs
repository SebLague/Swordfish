using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dodger : MonoBehaviour {

    public float speed = 5;
    public float smoothTime = .1f;

    Vector2 smoothV;
    public Vector2 currV { get; private set; }
    Rigidbody2D rb;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
        Vector2 targetV = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * speed;
        currV = Vector2.SmoothDamp(currV, targetV, ref smoothV, smoothTime,float.MaxValue,Time.deltaTime);
	}

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + currV * Time.fixedDeltaTime);
    }
}
