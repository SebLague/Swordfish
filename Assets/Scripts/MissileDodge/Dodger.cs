using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dodger : MonoBehaviour {

    public float speed = 5;
    public float smoothTime = .1f;
    float size;

    Vector2 smoothV;
    public Vector2 currV { get; private set; }
    public Vector2 targetV { get; private set; }
    Rigidbody2D rb;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
        size = Snake.size;
        transform.localScale = Vector3.one * size;
	}
	
	// Update is called once per frame
	void Update () {
        targetV = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * speed;
        currV = Vector2.SmoothDamp(currV, targetV, ref smoothV, smoothTime,float.MaxValue,Time.deltaTime);

        /*
		float buffer = .1f;
		// left
        if (transform.position.x + size / 2f < screen.minMaxX.x)
		{
            transform.position = new Vector2(screen.minMaxX.y + size / 2f - buffer, transform.position.y);
		}
		//right
		if (transform.position.x - size / 2f > screen.minMaxX.y)
		{
			//print("right");
			transform.position = new Vector2(screen.minMaxX.x - size / 2f + buffer, transform.position.y);
		}
		//down
		if (transform.position.y + size / 2f < screen.minMaxY.x)
		{
			transform.position = new Vector2(transform.position.x, screen.minMaxY.y + size / 2f - buffer);
		}
		//up
		if (transform.position.y - size / 2f > screen.minMaxY.y)
		{
            transform.position = new Vector2(transform.position.x, screen.minMaxY.x - size / 2f + buffer);
		}
		*/
	}

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + currV * Time.fixedDeltaTime);
    }
}
