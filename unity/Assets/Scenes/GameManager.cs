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

public class GameManager : MonoBehaviour
{
    Socket _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

    [Header("Ball")]
    public GameObject ball;

    [Header("Player1")]
    public GameObject player1Paddle;
    public GameObject player1Goal;

    [Header("Player2")]
    public GameObject player2Paddle;
    public GameObject player2Goal;

    [Header("UnityMainThreadDispatcher")]
    public GameObject unityMainThreadDispatcher;

    [Header("Score UI")]
    public GameObject player1Text;
    public GameObject player2Text;

    public int player1Score;
    public int player2Score;

    public ServerClient serverClient;

    public long StartTime;

    // Played time in ticks
    public long PlayedTime = 0;

    public void Player1Scored()
    {
        player1Score++;
        player1Text.GetComponent<TextMeshProUGUI>().text = player1Score.ToString();
        ResetPosition();
    }

    public void Player2Scored()
    {
        player2Score++;
        player2Text.GetComponent<TextMeshProUGUI>().text = player2Score.ToString();
        ResetPosition();
    }

    private void ResetPosition()
    {
        ball.GetComponent<Ball>().Reset();
        player1Paddle.GetComponent<Paddle>().Reset();
        player2Paddle.GetComponent<Paddle>().Reset();
    }

    // Start is called before the first frame update
    void Start()
    {
        serverClient = new ServerClient();
        serverClient.Connect();
        _clientSocket = serverClient.socket;
        Paddle opponentPaddle = player2Paddle.GetComponent(typeof(Paddle)) as Paddle;
        new Task(() => OpponentPaddleUpdateAsync(opponentPaddle)).Start();
        StartTime = DateTime.UtcNow.Ticks;
    }

    // Update is called once per frame
    void Update()
    {
        PlayedTime = DateTime.UtcNow.Ticks - StartTime;
    }

    public Task OpponentPaddleUpdateAsync(Paddle paddle)
    {
        while (true)
        {
            var bytes = new byte[1024];
            var bytesCount = serverClient.socket.ReceiveAsync(bytes, SocketFlags.None).GetAwaiter().GetResult();
            try
            {
                var data = Encoding.ASCII.GetString(bytes, 0, bytesCount);
                var paddleDto = JsonUtility.FromJson<PaddleDto>(data);
                UnityMainThreadDispatcher.Instance().Enqueue(() => paddle.OpponentUpdate(paddleDto.NextMovement, paddleDto.NextMovementStartTime));
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }

        return Task.CompletedTask;
    }
}
