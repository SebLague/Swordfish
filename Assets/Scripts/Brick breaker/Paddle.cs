using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paddle : MonoBehaviour
{

    public float speed = 3;
    public float xForce = 2;
    public float vUp = 10;
    float width;
    public bool isDummy;
    public Paddle paddleLeft;
    public Paddle paddleRight;
    Vector2 screenMinMax;

    float currV;
    public float smoothTime = .1f;
    float smoothV;

    void Start()
    {
        width = GetComponent<BoxCollider2D>().size.x * transform.localScale.x;
        screenMinMax = FindObjectOfType<ScreenAreas>().minMaxX;
    }


    void Update()
    {
        if (!isDummy)
        {
            float inputX = Input.GetAxisRaw("Horizontal");
            float targetV = inputX * Time.deltaTime * speed;
            currV = Mathf.SmoothDamp(currV, targetV, ref smoothV, smoothTime);
            transform.Translate(Vector2.right*currV);

            if (transform.position.x + width * .5f < screenMinMax.x)
            {
                transform.position = new Vector2(screenMinMax.y - width * .5f, transform.position.y);
            }
			else if (transform.position.x - width * .5f > screenMinMax.y)
			{
				transform.position = new Vector2(screenMinMax.x + width * .5f, transform.position.y);
			}

            paddleLeft.transform.position = transform.position + Vector3.left * (screenMinMax.y - screenMinMax.x);
            paddleRight.transform.position = transform.position + Vector3.right * (screenMinMax.y - screenMinMax.x);
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.tag == "Ball")
        {
            float offsetX = col.transform.position.x - transform.position.x;
            float percentX = offsetX / (width / 2f);
            col.rigidbody.velocity = new Vector2(percentX * xForce, vUp);
        }
    }
}
