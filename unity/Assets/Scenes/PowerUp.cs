using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;


public class PowerUp : MonoBehaviour
{
    public GameManager GameManager;
    bool _firePUUsed;
    bool _wallPUUsed;
    long? _wallPUTime = null;

    void Start()
    {
        _firePUUsed = false;
        _wallPUUsed = false;
    }

    public void Update()
    {
        if (GameManager.Player1Paddle.GetComponent<Paddle>().Opponent == false)
        {
            if (Input.GetKeyDown(KeyCode.E) && _wallPUUsed == false)
            {
                _wallPUUsed = true;
                if (_wallPUUsed == true)
                {
                    WallPUStart();
                }
            }
            if (_wallPUTime < GameManager.PlayedTime)
            {
                WallPUEnd();
            }

            if (Input.GetKeyDown(KeyCode.F) && _firePUUsed == false)
            {
                _firePUUsed = true;
                if (_firePUUsed == true)
                {
                    FirePU();
                }
            }
            if (_wallPUTime != null && _wallPUTime > GameManager.PlayedTime)
            {
                SendPowerUp((bool)_firePUUsed, (bool)_wallPUUsed, (long)_wallPUTime);
            }
        }
    }

    //PU stands for Power Up
    void WallPUStart()
    {
        GameManager.MiddleWallPU.gameObject.SetActive(true);
        _wallPUTime = GameManager.PlayedTime + 50000000;
    }

    void WallPUEnd()
    {
        GameManager.MiddleWallPU.gameObject.SetActive(false);
        GameManager.E.gameObject.GetComponent<Image>().color = new Color32(123, 123, 123, 255);
    }

    void FirePU()
    {
        GameManager.Ball.GetComponent<Ball>().Rigidbody.velocity = new Vector2(10f, 0f);
        GameManager.F.gameObject.GetComponent<Image>().color = new Color32(123, 123, 123, 255);
    }

    public void PowerUpUpdate(bool firePUUsed, bool wallPUUsed, long wallPUTime)
    {
        _firePUUsed = firePUUsed;
        _wallPUUsed = wallPUUsed;
        _wallPUTime = wallPUTime;
    }

    public void SendPowerUp(bool firePUUsed, bool wallPUUsed, long wallPUTime)
    {
        var powerUpDto = new PowerUpDto()
        {
            FirePUUsed = firePUUsed,
            WallPUUsed = wallPUUsed,
            WallPUTime = wallPUTime
        };
        var jsonString = JsonUtility.ToJson(powerUpDto);

        _ = GameManager.ServerClient.SendAsync(jsonString);
    }
}