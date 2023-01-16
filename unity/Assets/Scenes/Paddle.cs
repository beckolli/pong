using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paddle : MonoBehaviour
{
    public bool Opponent;
    public GameManager GameManager;

    public Rigidbody2D Rigidbody;
    public Vector3 StartPosition;
    private float _movement;
    private float _speed = 10;
    private float? _pastMovement;
    private float? _nextMovement;
    private long? _nextMovementStartTime;
    // Start is called before the first frame update
    void Start()
    {
        StartPosition = transform.position;
    }

    public void Reset()
    {
        Rigidbody.velocity = Vector2.zero;
        transform.position = StartPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (Opponent == false)
        {
            // fragt die aktuelle Taste ab (Pfeil hoch/runter)
            _movement = Input.GetAxisRaw("Vertical"); 
            if (_nextMovement == null && _nextMovementStartTime == null && _movement != _pastMovement)
            {
                _nextMovementStartTime = GameManager.PlayedTime + 1000000;
                _nextMovement = _movement;
                GameManager.ServerClient.SendPaddleMovment((float)_nextMovement, (long)_nextMovementStartTime);
                _pastMovement = _movement;
            }
        }
        if(_nextMovement != null && _nextMovementStartTime != null && _nextMovementStartTime <= GameManager.PlayedTime) 
        {
            Rigidbody.velocity = new Vector2(Rigidbody.velocity.x, (float)_nextMovement * _speed);
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
