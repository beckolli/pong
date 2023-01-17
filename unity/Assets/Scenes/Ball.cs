using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public Rigidbody2D Rigidbody;
    public Vector3 StartPosition;
    // Start is called before the first frame update

    private float _startSpeed = 5;

    public GameManager GameManager;

    void Start()
    {
        StartPosition = transform.position;
        Launch();
    }

    public void Reset()
    {
        Rigidbody.velocity = Vector2.zero;
        transform.position = StartPosition;
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
        Rigidbody.velocity = new Vector2(_startSpeed * x, _startSpeed * y);
    }

    private void SpeedLimit()
    {
        
        if(GameManager.SpeedLimitTime <= GameManager.PlayedTime)
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
            Rigidbody.velocity = new Vector2(Rigidbody.velocity.x, -20f);
            GameManager.ShowSpeedLimit();
            GameManager.SpeedLimitTime = GameManager.PlayedTime + 10000000;
        }
    }
}
