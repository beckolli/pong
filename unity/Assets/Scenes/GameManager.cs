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
                UnityMainThreadDispatcher.Instance().Enqueue(() => paddle.OpponentUpdate(float.Parse(data)));                
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }

        return Task.CompletedTask;
    }


    private Socket _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    private byte[] _recieveBuffer = new byte[1024];

    private void SetupServer()
    {
        try
        {
            _clientSocket.Connect(new IPEndPoint(IPAddress.Loopback, 11000));
        }
        catch (SocketException ex)
        {
            Debug.Log(ex.Message);
        }

        _clientSocket.BeginReceive(_recieveBuffer, 0, _recieveBuffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);

    }

    private void ReceiveCallback(IAsyncResult result)
    {
        try
        {
            Player1Scored();
            //Check how much bytes are recieved and call EndRecieve to finalize handshake
            int recieved = _clientSocket.EndReceive(result);

            if (recieved <= 0)
                return;

            //Copy the recieved data into new buffer , to avoid null bytes
            byte[] recData = new byte[recieved];
            Buffer.BlockCopy(_recieveBuffer, 0, recData, 0, recieved);

            //Process data here the way you want , all your bytes will be stored in recData                    
            UnityMainThreadDispatcher.Instance().Enqueue(() => player2Paddle.GetComponent<Paddle>().OpponentUpdate(1));

            //paddle.OpponentUpdate(float.Parse(recData.ToString()));

            //Start receiving again
            _clientSocket.BeginReceive(_recieveBuffer, 0, _recieveBuffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
        }
        catch (Exception e)
        {
            Debug.Log("ReceiveCallback exception: " + e.Message + "\n" + e.StackTrace);
        }

    }

}
