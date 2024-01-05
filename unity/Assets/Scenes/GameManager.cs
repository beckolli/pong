using System;
using System.Threading.Tasks;
using Pong.Unity.Scenes;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    long _minutes;
    long _seconds;
    long _timeInTimer;
    public PowerUp PowerUp;
    public int PlayerNumber;

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

    [Header("Win Texts")]
    public GameObject Player1WonText;
    public GameObject Player2WonText;
    public GameObject TieText;

    [Header("Power-Ups")]
    public GameObject E;
    public GameObject F;
    public GameObject MiddleWallPU;

    [Header("Server")]
    public GameObject ConnectionText;
    public bool IsStarted = false;

    public void HideSpeedLimit()
    {
        SpeedLimitText.SetActive(false);
    }

    public void Player1Scored()
    {
        Player1Score++;
        Player1Text.GetComponent<TextMeshProUGUI>().text = Player1Score.ToString();
        GoalReset();
    }

    public void Player2Scored()
    {
        Player2Score++;
        Player2Text.GetComponent<TextMeshProUGUI>().text = Player2Score.ToString();
        GoalReset();
    }

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 120;
        ServerClient = new ServerClient();
        new Task(() => ServerClient.ConnectAsync().Wait()).Start();
        while (ServerClient?.State != System.Net.WebSockets.WebSocketState.Open)
        {
            Debug.Log("Waiting for Server connection ...");
            Task.Delay(1000);
        }

        new Task(() => OpponentUpdateAsync().Wait()).Start();
        StartHide();
        Timer();

        ServerClient.SendAsync("test").Wait();
    }

    // Update is called once per frame
    void Update()
    {
        // doesn't start until the second player connects        
        if (IsStarted)
        {
            WinConditions();
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
        }
    }

    public async Task OpponentUpdateAsync()
    {
        Debug.Log($"Start OpponentUpdateAsync");
        while (true)
        {
            var data = await ServerClient.ReceiveAsync();
            try
            {
                if (data.Contains("PlayerNumber"))
                {
                    var gameStart = JsonUtility.FromJson<GameStartDto>(data);
                    global::UnityMainThreadDispatcher.Instance().Enqueue(() => GameStart(gameStart));
                }
                else
                if (data.Contains("Movement"))
                {
                    var paddleDto = JsonUtility.FromJson<PaddleDto>(data);
                    global::UnityMainThreadDispatcher.Instance().Enqueue(() =>
                        (Player2Paddle.GetComponent(typeof(Paddle)) as Paddle)
                            .OpponentPaddleUpdate(paddleDto.NextMovement, paddleDto.NextMovementStartTime));
                }
                else
                if (data.Contains("PU"))
                {
                    var powerUpDto = JsonUtility.FromJson<PowerUpDto>(data);
                    global::UnityMainThreadDispatcher.Instance().Enqueue(() =>
                        PowerUp.PowerUpUpdate(powerUpDto.FirePUUsed, powerUpDto.WallPUUsed, powerUpDto.WallPUTime));
                }
                else
                if (data.Contains("Speed"))
                {
                    var ballLaunchdto = JsonUtility.FromJson<BallLaunchDto>(data);
                    global::UnityMainThreadDispatcher.Instance().Enqueue(() =>
                        (Ball.GetComponent(typeof(Ball)) as Ball)
                            .Launch(ballLaunchdto.SpeedX, ballLaunchdto.SpeedY));
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }
    }

    public void ShowSpeedLimit()
    {
        SpeedLimitText.SetActive(true);
    }

    void GameFinish()
    {
        IsFinished = true;
        IsStarted = false;
        GoalReset();
        Ball.GetComponent<Ball>().Rigidbody.velocity = new Vector2(0f, 0f);
    }

    void GameStart(GameStartDto gameStart)
    {
        PlayerNumber = gameStart.PlayerNumber;
        StartTime = DateTime.UtcNow.Ticks;
        Player1Score = 0;
        Player2Score = 0;
        ConnectionText.SetActive(false);
        IsFinished = false;
        IsStarted = true;
        Ball.GetComponent<Ball>().Launch(gameStart.BallX, gameStart.BallY);
        if (gameStart.PlayerNumber == 1)
        {
            Player1Paddle.GetComponent<Paddle>().RightPaddle = false;
            Player2Paddle.GetComponent<Paddle>().RightPaddle = true;
        }
        else
        {
            Player1Paddle.GetComponent<Paddle>().RightPaddle = true;
            Player2Paddle.GetComponent<Paddle>().RightPaddle = false;
        }
    }

    void GoalReset()
    {
        Ball.GetComponent<Ball>().Reset();
        Player1Paddle.GetComponent<Paddle>().Reset();
        Player2Paddle.GetComponent<Paddle>().Reset();
        HideSpeedLimit();
    }

    void Player1Won()
    {
        Player1WonText.SetActive(true);
        Ball.GetComponent<Ball>().Rigidbody.velocity = new Vector2(0f, 0f);
    }

    void Player2Won()
    {
        Player2WonText.SetActive(true);
        Ball.GetComponent<Ball>().Rigidbody.velocity = new Vector2(0f, 0f);
    }

    void StartHide()
    {
        ConnectionText.SetActive(true);
        MiddleWallPU.SetActive(false);
        Player1WonText.SetActive(false);
        Player2WonText.SetActive(false);
        SpeedLimitText.SetActive(false);
        TieText.SetActive(false);
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
            TieText.SetActive(true);
        }
        GameFinish();
    }

    void WinConditions()
    {
        if (Player2Score == 20)
        {
            Player2Won();
            GameFinish();
        }
        else
        if (Player1Score == 20)
        {
            Player1Won();
            GameFinish();
        }
    }

}
