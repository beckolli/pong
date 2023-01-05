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
    float? pastMovement;
    float? nextMovement;
    long? nextMovementStartTime;
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
            // fragt die aktuelle Taste ab (Pfeil hoch/runter)
            movement = Input.GetAxisRaw("Vertical2"); 
            if (movement != pastMovement)
            {
                nextMovementStartTime = gameManager.PlayedTime + 1000000;
                nextMovement = movement;
                gameManager.serverClient.SendPaddleMovment(movement);
                pastMovement = movement;
            }
        }
        if(nextMovement != null && nextMovementStartTime != null && nextMovementStartTime <= gameManager.PlayedTime) 
        {
            rb.velocity = new Vector2(rb.velocity.x, (float)nextMovement * speed);
            nextMovementStartTime = null;
            nextMovement = null;
        }
    }
    public void OpponentUpdate(float movement)
    {
    }
}
