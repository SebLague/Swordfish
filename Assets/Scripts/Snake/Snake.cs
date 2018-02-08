using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour
{
    public Transform startPos;
    public int startSnakeSize = 5;
    public float speed = 5;
    public Material mat;
    public const float size = .15f;
    public LayerMask snakeMask;
    public LayerMask foodMask;

    public float wiggleSpeed = 2;
    public float wiggleDst = .2f;
    public bool wiggle;
    float wiggleAmountOld;
    float wiggleTime;
    public const float spacing = size;
    int numEaten;
    int maxLength = 100;
    int visIndex;

    Vector2 initialDirection = Vector2.right;
    Vector2 dirOld;

    public float smoothMoveTime = .1f;
    Vector2 velocity;
    Vector2 smoothVelocityRef;
    List<SnakeSegment> snake;
    ScreenAreas screen;

    void Start()
    {
        screen = FindObjectOfType<ScreenAreas>();
        CreateSnake(startSnakeSize);
    }

    // Update is called once per frame
    void Update()
    {
        wiggleTime += Time.deltaTime;

        Vector2 dir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (dir == Vector2.zero || dir == -dirOld)
        {
            dir = dirOld;
        }
        else
        {
            if (dir.sqrMagnitude > 1) // holding multiple keys; so pick only latest one
            {
                if (dir.x != dirOld.x)
                {
                    dir = Vector2.right * dir.x;
                }
                else if (dir.y != dirOld.y)
                {
                    dir = Vector2.up * dir.y;
                }
                else
                {
                    dir = dirOld;
                }
            }
            else
            {
                dirOld = dir;

            }
        }

        float wiggleAmount = Mathf.Sin(wiggleTime * wiggleSpeed) * wiggleDst;
        float deltaWiggle = wiggleAmount - wiggleAmountOld;
        wiggleAmountOld = wiggleAmount;
        Vector2 wiggleDir = new Vector2(-dir.y, dir.x);

        Vector2 targetVelocity = dir * speed;
        velocity = Vector2.SmoothDamp(velocity, targetVelocity, ref smoothVelocityRef, smoothMoveTime, float.MaxValue,Time.deltaTime);
        float wiggleFac = (wiggle) ? 1 : 0;
        Vector2 displacement = velocity * Time.deltaTime + wiggleDir * deltaWiggle*wiggleFac;

        float moveDst = displacement.magnitude;

        snake[0].Move(displacement);

        float buffer = .1f;
        // left
        if (snake[0].position.x + size / 2f < screen.minMaxX.x)
        {
            Vector2 newPos = new Vector2(screen.minMaxX.y + size / 2f - buffer, snake[0].position.y);
            //print("left: " + newPos);
            snake[0].Move(newPos - snake[0].position);
        }
        //right
		if (snake[0].position.x - size / 2f > screen.minMaxX.y)
		{
			//print("right");
            Vector2 newPos = new Vector2(screen.minMaxX.x - size / 2f + buffer, snake[0].position.y);
			snake[0].Move(newPos - snake[0].position);
		}
        //down
		if (snake[0].position.y + size / 2f < screen.minMaxY.x)
		{
            Vector2 newPos = new Vector2(snake[0].position.x, screen.minMaxY.y + size / 2f - buffer);
			snake[0].Move(newPos - snake[0].position);
		}
		//up
		if (snake[0].position.y - size / 2f > screen.minMaxY.y)
		{
            Vector2 newPos = new Vector2(snake[0].position.x, screen.minMaxY.x - size / 2f + buffer);
			snake[0].Move(newPos - snake[0].position);
		}

        for (int i = 1; i < snake.Count; i++)
        {
            snake[i].Follow(moveDst);
        }

        if (Physics2D.OverlapCircle(snake[0].position, size * .5f, snakeMask))
        {
            Debug.DrawRay(snake[0].position, Vector2.up * size * .5f, Color.red);
            OnDeath();
        }

        Collider2D food = Physics2D.OverlapCircle(snake[0].position, size * .5f, foodMask);
        if (food != null)
        {
            Eat(food.gameObject);
        }
    }

    void Eat(GameObject food) {
        Destroy(food);
        //GrowSnake();
        if (visIndex < maxLength)
        {
            snake[visIndex].SetVisible(true);
        }
        numEaten++;
        visIndex++;

    }

    void OnDeath()
    {
        print("Death");
    }

    void CreateSnake(int initialSize = 2)
    {
        dirOld = initialDirection;

        snake = new List<SnakeSegment>();
        snake.Add(CreateBodyPart(startPos.position, null));
        snake.Add(CreateBodyPart((Vector2)startPos.position - initialDirection * spacing, snake[0]));

        for (int i = 0; i < maxLength - 2; i++)
        {
            GrowSnake();
            if (i+2 > initialSize)
            {
                snake[i + 2].SetVisible(false);
            }
        }
        visIndex = initialSize;
    }

    SnakeSegment CreateBodyPart(Vector2 position, SnakeSegment parent)
    {
		GameObject g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        g.transform.localScale = Vector3.one * size;
        g.transform.parent = transform;
        g.GetComponent<MeshRenderer>().material = mat;
        DestroyImmediate(g.GetComponent<SphereCollider>());
        SnakeSegment p = new SnakeSegment(parent, position, g.transform);

        g.layer = LayerMask.NameToLayer("Snake");
        if (snake.Count > 1)
        {
            CircleCollider2D c = g.AddComponent<CircleCollider2D>();
            c.isTrigger = true;
        }


        return p;
    }

    void GrowSnake()
    {

        SnakeSegment tail = snake[snake.Count - 1];
        Vector2 forwardDir = (snake[snake.Count - 2].position - tail.position).normalized;

        Vector2 position = tail.position - forwardDir * spacing;

        snake.Add(CreateBodyPart(position,tail));
    }


    public class SnakeSegment
    {
        //const float distanceBetweenRecordings = 1;
        public bool debug;
        public Queue<Vector2> pastPositions;
        public Vector2 position;
        public Vector2 target;
        const float teleportThreshold = 3;
        SnakeSegment parentSegment;
        Transform t;

        public SnakeSegment(SnakeSegment parentSegment, Vector2 position, Transform t)
        {
            this.parentSegment = parentSegment;
            this.t = t;
            t.position = position;
            this.position = position;
            pastPositions = new Queue<Vector2>();
            pastPositions.Enqueue(position);

            if (parentSegment != null)
            {
                target = parentSegment.pastPositions.Dequeue();
            }
        }

        // This should only be used for the head of the snake
        public void Move(Vector2 moveAmount)
        {
            position += moveAmount;
            t.position = position;
            pastPositions.Enqueue(position);
            //Debug.Log("new pos: " + position + " move dst: " + moveAmount.magnitude);
        }


        public void Follow(float moveDst)
        {
			float moveDstRemaining = moveDst;
      

			while (moveDstRemaining > 0)
			{
                
				float dstToTarget = Vector2.Distance(position, target);
                if (dstToTarget > teleportThreshold)
                {
                    position = target;
                    dstToTarget = 0;
                }

				if (dstToTarget <= moveDstRemaining)
				{
					position = target;
					pastPositions.Enqueue(position);
					moveDstRemaining -= dstToTarget;

					target = parentSegment.pastPositions.Dequeue();


				}
				else
				{
					Vector2 newPos = position + (target - position).normalized * moveDstRemaining;

					position = newPos;
					pastPositions.Enqueue(position);
					moveDstRemaining = 0;
				}

			}

			t.position = position;
           
        }

        public void SetVisible(bool v)
        {
            t.gameObject.SetActive(v);
        }
      

    }

}
