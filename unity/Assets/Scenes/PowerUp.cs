using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;


public class PowerUp : MonoBehaviour
{
    public GameManager GameManager;    
    public bool FirePUUsed;
    public bool WallPUUsed;
    public long? WallPUTime = null;

    
    public void PowerUps()
    {
        if (GameManager.Player1Paddle.GetComponent<Paddle>().Opponent == false)
        {
            if (Input.GetKeyDown(KeyCode.E) && WallPUUsed == false)
            {
                WallPUStart();
            }
            if (WallPUTime < GameManager.PlayedTime)
            {
                WallPUEnd();
            }

            if (Input.GetKeyDown(KeyCode.F) && FirePUUsed == false)
            {
                FirePU();
            }
        }
    }

    //PU stands for Power Up
    void WallPUStart()
    {
        GameManager.MiddleWallPU.gameObject.SetActive(true);
        WallPUTime = GameManager.PlayedTime + 50000000;
    }

    void WallPUEnd()
    {
        GameManager.MiddleWallPU.gameObject.SetActive(false);
        WallPUUsed = true;
        GameManager.E.gameObject.GetComponent<Image>().color = new Color32(123, 123, 123, 255);
    }

    void FirePU()
    {
        GameManager.Ball.GetComponent<Ball>().Rigidbody.velocity = new Vector2(10f, 0f);
        FirePUUsed = true;
        GameManager.F.gameObject.GetComponent<Image>().color = new Color32(123, 123, 123, 255);
    }

    void PowerUpUpdate(bool firePUUsed, bool wallPUUsed)
    {
        FirePUUsed = firePuUsed;
        WallPUUsed = wallPuUsed;
    }

    public void SendPowerUp(bool firePUUsed, bool wallPUUsed)
    {
        var PowerUpDto = new PowerUpDto()
        {
            FirePUUsed = firePUUsed,
            WallPUUsed = wallPUUsed
        };
        var jsonString = JsonUtility.ToJson(paddleDto);

        GameManager.ServerClient.Send(jsonString);
    }
}