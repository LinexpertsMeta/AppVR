using System;
using System.Collections.Generic;
using SocketIOClient;
using SocketIOClient.Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using TMPro;

public class Client : MonoBehaviour
{
    public SocketIOUnity socket;
    public TextMeshProUGUI ReceivedText;
    // Start is called before the first frame update
    void Start()
    {
        var uri = new Uri("http://localhost:11100");
        socket = new SocketIOUnity(uri, new SocketIOOptions
        {
            Query = new Dictionary<string, string>
                {
                    {"token", "UNITY" }
                }
            ,
            EIO = 4
            ,
            Transport = SocketIOClient.Transport.TransportProtocol.WebSocket,
            //ConnectionTimeout = new TimeSpan(0,0,0,4),
            //ReconnectionDelay = 1,
            //Reconnection = true
        }); ;
        socket.JsonSerializer = new NewtonsoftJsonSerializer();

        ///// reserved socketio events
        socket.OnConnected += (sender, e) =>
        {
            Debug.Log("socket.OnConnected");
            ReceivedText.text = "socket.OnConnected";
        };

        //socket.OnConnected += async (sender, e) =>
        //{
        //    // Emit a string
        //    await socket.EmitAsync("hello", "socket.io");
        //};

        socket.On("hellou", response =>
        {
            Debug.Log("hellou");
            Debug.Log("Response hellou: "+response);
        });

        socket.OnPing += (sender, e) =>
        {
            ReceivedText.text = "Ping";
            Debug.Log("Ping");
        };
        socket.OnPong += (sender, e) =>
        {
            ReceivedText.text = "Pong: " + e.TotalMilliseconds;
            Debug.Log("Pong: " + e.TotalMilliseconds);
            Debug.Log("Pong: " + e);
        };
        socket.OnDisconnected += (sender, e) =>
        {
            ReceivedText.text = "disconnect: " + e;
            Debug.Log("disconnect: " + e);
        };
        socket.OnReconnectAttempt += (sender, e) =>
        {
            ReceivedText.text = $"{DateTime.Now} Reconnecting: attempt = {e}";
            Debug.Log($"{DateTime.Now} Reconnecting: attempt = {e}");
        };
        ////

        socket.On("SPAWN_PLAYER", async response =>
        {
            //ReceivedText.text = "Spawning";
            Debug.Log("Spawning");

            await socket.EmitAsync("JOIN", "socket.io");
        });

        //ReceivedText.text = "Connecting....";
        Debug.Log("Connecting....");
        socket.Connect();
        Debug.Log(socket);

        Debug.Log("Try connect");
        socket.OnUnityThread("spin", (data) =>
        {
            //rotateAngle = 0;
            Debug.Log("thread");
        });

        ////ReceivedText.text = "";
        //socket.OnAnyInUnityThread((name, response) =>
        //{
        //    ReceivedText.text += "Received On. " + name + " : " + response.ToString() + "\n";
        //});
    }

    async public void ButtonClick()
    {
        Debug.Log("Presiono el boton");
        socket.EmitAsync("hello");

    }
}
