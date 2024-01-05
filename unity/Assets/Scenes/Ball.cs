using UnityEngine;

public class Ball : MonoBehaviour
{
    public GameManager GameManager;
    public Rigidbody2D Rigidbody;
    public Vector3 StartPosition;
    float _startSpeed = 5;

    public void Launch(float speedX, float speedY)
    {
        Rigidbody.velocity = new Vector2(_startSpeed * speedX, _startSpeed * speedY);
    }

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
        if (GameManager.IsStarted)
        {
            SpeedLimit();
        }
        else
        {
            Reset();
        }
    }

    void SpeedLimit()
    {
        if (GameManager.IsFinished == false)
        {
            if (GameManager.SpeedLimitTime <= GameManager.PlayedTime)
            {
                GameManager.HideSpeedLimit();
            }

            if (Rigidbody.velocity.x < 0)
            {
                if (Rigidbody.velocity.x > -1.5)
                {
                    Rigidbody.velocity = new Vector2(-1.5f, Rigidbody.velocity.y);
                    GameManager.ShowSpeedLimit();
                    GameManager.SpeedLimitTime = GameManager.PlayedTime + 10000000;
                }

                if (Rigidbody.velocity.x < -20)
                {
                    Rigidbody.velocity = new Vector2(-20f, Rigidbody.velocity.y);
                    GameManager.ShowSpeedLimit();
                    GameManager.SpeedLimitTime = GameManager.PlayedTime + 10000000;
                }
            }
            else
            {
                if (Rigidbody.velocity.x < 1.5)
                {
                    Rigidbody.velocity = new Vector2(1.5f, Rigidbody.velocity.y);
                    GameManager.ShowSpeedLimit();
                    GameManager.SpeedLimitTime = GameManager.PlayedTime + 10000000;
                }

                if (Rigidbody.velocity.x > 20)
                {
                    Rigidbody.velocity = new Vector2(20f, Rigidbody.velocity.y);
                    GameManager.ShowSpeedLimit();
                    GameManager.SpeedLimitTime = GameManager.PlayedTime + 10000000;
                }
            }
            if (Rigidbody.velocity.y < -20)
            {
                Rigidbody.velocity = new Vector2(Rigidbody.velocity.x, -20f);
                GameManager.ShowSpeedLimit();
                GameManager.SpeedLimitTime = GameManager.PlayedTime + 10000000;
            }

            if (Rigidbody.velocity.y > 20)
            {
                Rigidbody.velocity = new Vector2(Rigidbody.velocity.x, 20f);
                GameManager.ShowSpeedLimit();
                GameManager.SpeedLimitTime = GameManager.PlayedTime + 10000000;
            }
        }
    }
}
