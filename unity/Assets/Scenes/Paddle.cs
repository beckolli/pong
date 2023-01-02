using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paddle : MonoBehaviour
{
    public bool opponent;
    public GameManager gameManager;

    public Rigidbody2D rb;
    public Vector3 startPosition;
    private float movement;
    private float speed = 10;
    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
    }

    public void Reset()
    {
        rb.velocity = Vector2.zero;
        transform.position = startPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (opponent == false)
        {
            movement = Input.GetAxisRaw("Vertical2");
            gameManager.serverClient.SendPaddleMovment(movement);

            rb.velocity = new Vector2(rb.velocity.x, movement * speed);
        }
    }
    public void OpponentUpdate(float movement)
    {
        rb.velocity = new Vector2(rb.velocity.x, movement * speed);
    }
}
