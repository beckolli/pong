using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public Rigidbody2D rb;
    public Vector3 startPosition;
    // Start is called before the first frame update    

    private float startSpeed = 5;

    void Start()
    {
        startPosition = transform.position;
        Launch();
    }

    public void Reset()
    {
        rb.velocity = Vector2.zero;
        transform.position = startPosition;
        Launch();
    }

    // Update is called once per frame
    void Update()
    {
        SpeedLimit();
    }

    private void Launch()
    {
        float x = Random.Range(0, 2) == 0 ? -1 : 1;
        float y = Random.Range(0, 2) == 0 ? -1 : 1;
        rb.velocity = new Vector2(startSpeed * x, startSpeed * y);
    }

    private void SpeedLimit()
    {
        if (rb.velocity.x < 0)
        {
            if (rb.velocity.x > -1.5)
            {
                rb.velocity = new Vector2(-1.5f, rb.velocity.y);
            }

            if (rb.velocity.x < -20)
            {
                rb.velocity = new Vector2(-20f, rb.velocity.y);
            }
        }
        else
        {
            if (rb.velocity.x < 1.5)
            {
                rb.velocity = new Vector2(1.5f, rb.velocity.y);
            }

            if (rb.velocity.x > 20)
            {
                rb.velocity = new Vector2(20f, rb.velocity.y);
            }
        }
        if (rb.velocity.y < -20)
        {
            rb.velocity = new Vector2(rb.velocity.x, -20f);
        }

        if (rb.velocity.y > 20)
        {
            rb.velocity = new Vector2(rb.velocity.x, -20f);
        }
    }
}
