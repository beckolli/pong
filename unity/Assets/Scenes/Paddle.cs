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
    float? _pastMovement;
    float? _nextMovement;
    long? _nextMovementStartTime;
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
            movement = Input.GetAxisRaw("Vertical"); 
            if (_nextMovement == null && _nextMovementStartTime == null && movement != _pastMovement)
            {
                _nextMovementStartTime = gameManager.PlayedTime + 1000000;
                _nextMovement = movement;
                gameManager.serverClient.SendPaddleMovment((float)_nextMovement, (long)_nextMovementStartTime);
                _pastMovement = movement;
            }
        }
        if(_nextMovement != null && _nextMovementStartTime != null && _nextMovementStartTime <= gameManager.PlayedTime) 
        {
            rb.velocity = new Vector2(rb.velocity.x, (float)_nextMovement * speed);
            _nextMovementStartTime = null;
            _nextMovement = null;
        }
    }

    public void OpponentUpdate(float nextMovement, long nextMovementStartTime)
    {
        _nextMovement = nextMovement;
        _nextMovementStartTime = nextMovementStartTime;
    }
}
