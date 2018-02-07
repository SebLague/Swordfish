using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour
{
    public Transform startPos;
    public int startSnakeSize = 5;
    public float speed = 5;
    public Material mat;
    public float size = .3f;
    public LayerMask snakeMask;
    public LayerMask foodMask;

    public float wiggleSpeed = 2;
    public float wiggleDst = .2f;
    float wiggleAmountOld;
    float wiggleTime;
    float spacing;
    int numEaten;
    int maxLength = 100;
    int visIndex;

    Vector2 initialDirection = Vector2.right;
    Vector2 dirOld;

    public float smoothMoveTime = .1f;
    Vector2 velocity;
    Vector2 smoothVelocityRef;
    List<SnakeSegment> snake;

    void Start()
    {
        spacing = size;
        CreateSnake(startSnakeSize);
    }

    // Update is called once per frame
    void Update()
    {
        wiggleTime += Time.deltaTime;

        Vector2 dir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (dir == Vector2.zero || dir == -dirOld || dir.sqrMagnitude > 1)
        {
            dir = dirOld;
        }
        else
        {
            dirOld = dir;
        }

        float wiggleAmount = Mathf.Sin(wiggleTime * wiggleSpeed) * wiggleDst;
        float deltaWiggle = wiggleAmount - wiggleAmountOld;
        wiggleAmountOld = wiggleAmount;
        Vector2 wiggleDir = new Vector2(-dir.y, dir.x);

        Vector2 targetVelocity = dir * speed;
        velocity = Vector2.SmoothDamp(velocity, targetVelocity, ref smoothVelocityRef, smoothMoveTime, float.MaxValue,Time.deltaTime);
        Vector2 displacement = velocity * Time.deltaTime + wiggleDir * deltaWiggle;
        float moveDst = displacement.magnitude;

        snake[0].Move(displacement);
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
        }


        public void Follow(float moveDst)
        {
			float moveDstRemaining = moveDst;

			while (moveDstRemaining > 0)
			{
				float dstToTarget = Vector2.Distance(position, target);

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
