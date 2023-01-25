using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Pong.Unity.Scenes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    Socket _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

    [Header("Ball")]
    public GameObject Ball;
    public GameObject SpeedLimitText;

    [Header("Player1")]
    public GameObject Player1Goal;
    public GameObject Player1Paddle;

    [Header("Player2")]
    public GameObject Player2Goal;
    public GameObject Player2Paddle;

    [Header("UnityMainThreadDispatcher")]
    public GameObject UnityMainThreadDispatcher;

    [Header("Score UI")]
    public GameObject Player1Text;
    public GameObject Player2Text;

    public int Player1Score;
    public int Player2Score;

    public ServerClient ServerClient;

    [Header("Time")]
    // Played time in ticks (10.000.000 in 1 second)
    public long PlayedTime = 0;
    public long SpeedLimitTime = 0;
    public long StartTime;

    [Header("Timer")]
    public bool IsFinished = false;
    public GameObject TimerText;
    long _minutes;
    long _seconds;
    long _timeInTimer;

    [Header("Win Texts")]
    public GameObject Player1WonText;
    public GameObject Player2WonText;
    public GameObject TieText;

    [Header("Power-Ups")]
    public GameObject E;
    public GameObject F;
    public GameObject MiddleWallPU;
    bool _firePUUsed;
    bool _wallPUUsed;
    long? _wallPUTime = null;

    [Header("Server")]
    public GameObject ConnectionText;
    public bool IsStarted = false;

    public void Player1Scored()
    {
        Player1Score++;
        Player1Text.GetComponent<TextMeshProUGUI>().text = Player1Score.ToString();
        ResetPosition();
    }

    public void Player2Scored()
    {
        Player2Score++;
        Player2Text.GetComponent<TextMeshProUGUI>().text = Player2Score.ToString();
        ResetPosition();
    }

    private void ResetPosition()
    {
        Ball.GetComponent<Ball>().Reset();
        Player1Paddle.GetComponent<Paddle>().Reset();
        Player2Paddle.GetComponent<Paddle>().Reset();
        HideSpeedLimit();
    }

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 120;
        ServerClient = new ServerClient();
        ServerClient.Connect();
        _clientSocket = ServerClient.Socket;
        Paddle opponentPaddle = Player2Paddle.GetComponent(typeof(Paddle)) as Paddle;
        new Task(() => OpponentPaddleUpdateAsync(opponentPaddle)).Start();
        StartTime = DateTime.UtcNow.Ticks;

        MiddleWallPU.gameObject.SetActive(false);
        Player1WonText.gameObject.SetActive(false);
        Player2WonText.gameObject.SetActive(false);
        SpeedLimitText.gameObject.SetActive(false);
        TieText.gameObject.SetActive(false);
        _firePUUsed = false;
        _wallPUUsed = false;
        ConnectionText.gameObject.SetActive(false);
        // while(Player2.Connection != true)
        // {
        //      Game.Pause();
        // }
    }

    // Update is called once per frame
    void Update()
    {
        //Win Conditions
        if (Player2Score == 20)
        {
            Player2Won();
        }
        else
        if (Player1Score == 20)
        {
            Player1Won();
        }
        if (IsFinished == false)
        {
            PlayedTime = DateTime.UtcNow.Ticks - StartTime;
            Timer();
            if (_timeInTimer <= 0)
            {
                IsFinished = true;
            }
            if (IsFinished)
            {
                TimerEnd();
            }
            // Power-Ups
            if (Player1Paddle.GetComponent<Paddle>().Opponent == false)
            {
                if (Input.GetKeyDown(KeyCode.E) && _wallPUUsed == false)
                {
                    WallPUStart();
                }
                if (_wallPUTime < PlayedTime)
                {
                    WallPUEnd();
                }

                if (Input.GetKeyDown(KeyCode.F) && _firePUUsed == false)
                {
                    FirePU();
                }
            }
        }
    }

    public Task OpponentPaddleUpdateAsync(Paddle paddle)
    {
        while (true)
        {
            var bytes = new byte[1024];
            var bytesCount = ServerClient.Socket.ReceiveAsync(bytes, SocketFlags.None).GetAwaiter().GetResult();
            try
            {
                var data = Encoding.ASCII.GetString(bytes, 0, bytesCount);
                var paddleDto = JsonUtility.FromJson<PaddleDto>(data);
                global::UnityMainThreadDispatcher.Instance().Enqueue(() => paddle.OpponentUpdate(paddleDto.NextMovement, paddleDto.NextMovementStartTime));
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }

        return Task.CompletedTask;
    }

    public void ShowSpeedLimit()
    {
        SpeedLimitText.gameObject.SetActive(true);
    }
    public void HideSpeedLimit()
    {
        SpeedLimitText.gameObject.SetActive(false);
    }
    void Timer()
    {
        _timeInTimer = 180 - (PlayedTime / 10000000);
        _seconds = _timeInTimer % 60;
        _minutes = _timeInTimer / 60;
        TimerText.GetComponent<TextMeshProUGUI>().text = "Timer: " + _minutes.ToString() + ":" + _seconds.ToString();
        if ((_seconds = _timeInTimer % 60) < 10)
        {
            TimerText.GetComponent<TextMeshProUGUI>().text = "Timer: " + _minutes.ToString() + ":0" + _seconds.ToString();
        }
    }

    void Player2Won()
    {
        Player2WonText.gameObject.SetActive(true);
        IsFinished = true;
        Ball.GetComponent<Ball>().Rigidbody.velocity = new Vector2(0f, 0f);
    }

    void Player1Won()
    {
        Player1WonText.gameObject.SetActive(true);
        IsFinished = true;
        Ball.GetComponent<Ball>().Rigidbody.velocity = new Vector2(0f, 0f);
    }

    void TimerEnd()
    {
        HideSpeedLimit();
        if (Player1Score > Player2Score)
        {
            Player1Won();
        }
        else
            if (Player2Score > Player1Score)
        {
            Player2Won();
        }
        else
        {
            IsFinished = true;
            Ball.GetComponent<Ball>().Rigidbody.velocity = new Vector2(0f, 0f);
            TieText.gameObject.SetActive(true);
        }
    }

    //PU stands for Power Up
    void WallPUStart()
    {
        MiddleWallPU.gameObject.SetActive(true);
        _wallPUTime = PlayedTime + 50000000;
    }

    void WallPUEnd()
    {
        MiddleWallPU.gameObject.SetActive(false);
        _wallPUUsed = true;
        E.gameObject.GetComponent<Image>().color = new Color32(123, 123, 123, 255);
    }

    void FirePU()
    {
        Ball.GetComponent<Ball>().Rigidbody.velocity = new Vector2(10f, 0f);
        _firePUUsed = true;
        F.gameObject.GetComponent<Image>().color = new Color32(123, 123, 123, 255);
    }
}
