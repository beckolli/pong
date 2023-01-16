using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Pong.Unity.Scenes
{
    public class ServerClient
    {
        byte[] _bytes = null;

        public Socket Socket;

        public void Connect()
        {
            _bytes = new byte[1024];
            try
            {
                IPHostEntry host = Dns.GetHostEntry("localhost");
                IPAddress ipAddress = host.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);

                Socket = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect the socket to the remote endpoint. Catch any errors.

                Socket.Connect(remoteEP);
            }
            catch (Exception)
            {
            }
        }

        public void SendPaddleMovment(float nextMovment, long nextMovementStartTime)
        {
            var paddleDto = new PaddleDto()
            {
                NextMovement = nextMovment,
                NextMovementStartTime = nextMovementStartTime
            };
            var jsonString = JsonUtility.ToJson(paddleDto);
            var data = Encoding.ASCII.GetBytes(jsonString);
            
            SocketAsyncEventArgs socketAsyncData = new SocketAsyncEventArgs();
            socketAsyncData.SetBuffer(data, 0, data.Length);
            Socket.SendAsync(socketAsyncData);
        }

    }
}