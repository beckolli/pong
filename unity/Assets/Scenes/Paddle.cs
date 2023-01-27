using System.Collections;
using System.Collections.Generic;
using System.Text;
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
            // looks at the input (w/s)
            _movement = Input.GetAxisRaw("Vertical");
            if (_nextMovement == null && _nextMovementStartTime == null && _movement != _pastMovement)
            {
                _nextMovementStartTime = GameManager.PlayedTime + 1000000;
                _nextMovement = _movement;
                SendPaddleMovment((float)_nextMovement, (long)_nextMovementStartTime);
                _pastMovement = _movement;
            }
        }
        if (_nextMovement != null && _nextMovementStartTime != null && _nextMovementStartTime <= GameManager.PlayedTime)
        {
            Rigidbody.velocity = new Vector2(Rigidbody.velocity.x, (float)_nextMovement * _speed);
            _nextMovementStartTime = null;
            _nextMovement = null;
        }
    }

    public void OpponentPaddleUpdate(float nextMovement, long nextMovementStartTime)
    {
        _nextMovement = nextMovement;
        _nextMovementStartTime = nextMovementStartTime;
    }

    public void SendPaddleMovment(float nextMovment, long nextMovementStartTime)
    {
        var paddleDto = new PaddleDto()
        {
            NextMovement = nextMovment,
            NextMovementStartTime = nextMovementStartTime
        };
        var jsonString = JsonUtility.ToJson(paddleDto);

        GameManager.ServerClient.Send(jsonString);
    }
}
