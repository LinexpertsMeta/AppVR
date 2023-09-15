using System;
using System.Collections.Generic;
using SocketIOClient;
using SocketIOClient.Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using TMPro;
using System.Threading.Tasks;

namespace MetaverseSample
{
    public class Client : MonoBehaviour
    {
        public static Client instance;

        public string local_player_id;

        [HideInInspector]
        public bool onLogged = false;

        static private readonly char[] Delimiter = new char[] { ':' };

        public SocketIOUnity socket;
        public TextMeshProUGUI ReceivedText;

        public Transform spawnPoint;
        public TMP_InputField input;

        public Dictionary<string, PlayerManager> networkPlayers = new Dictionary<string, PlayerManager>();

        [Header("Remote Player Prefab")]
        public GameObject[] remotePlayerPref; //store the local player prefabs
                                              // Start is called before the first frame update
        public GameObject prefab;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(this);
            }
        }
        void Start()
        {
            var uri = new Uri("http://metaverso.linexperts.com:8002");
            //var uri = new Uri("https://metaverso.linexperts.com:8001");
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
                Debug.Log("Response hellou: " + response);
                ReceivedText.text = response.ToString();
            });

            socket.OnPing += (sender, e) =>
            {
                //ReceivedText.text = "Ping";
                Debug.Log("Ping");
            };
            socket.OnPong += (sender, e) =>
            {
                //ReceivedText.text = "Pong: " + e.TotalMilliseconds;
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
                Debug.Log($"{DateTime.Now} Reconnecting: attempt = {e}");
                ReceivedText.text = $"{DateTime.Now} Reconnecting: attempt = {e}";
            };
            ////


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



            ////////////////////////////////////////////////////////////////////////////
            /////////////////////////// Eventos Propios ////////////////////////////////
            ////////////////////////////////////////////////////////////////////////////
            socket.On("SPAWN_PLAYER", response =>
            {
                Debug.Log("Spawning");
                PimDeWitte.UnityMainThreadDispatcher.UnityMainThreadDispatcher.Instance().
                Enqueue(() => SpawnPlayer(response.GetValue<string>(0), response.GetValue<string>(1),
                response.GetValue<string>(2), response.GetValue<string>(3), response.GetValue<string>(4),
                response.GetValue<string>(5)));

                //GameObject.Instantiate(remotePlayerPref[0], new Vector3(0, 0, 0), Quaternion.identity);
            });

            socket.On("UPDATE_MOVE_AND_ROTATE", response =>
            {
                string pack = response.GetValue<string>(0) + ":" + response.GetValue<string>(1) + ":" +
                response.GetValue<string>(2) + ":" + response.GetValue<string>(3) + ":" + response.GetValue<string>(4);
                PimDeWitte.UnityMainThreadDispatcher.UnityMainThreadDispatcher.Instance().
                Enqueue(() => OnUpdateMoveAndRotate(pack));
            });

            socket.On("UPDATE_PLAYER_ANIMATOR", response =>
            {
                string pack = response.GetValue<string>(0) + ":" + response.GetValue<string>(1) + ":" +
                response.GetValue<string>(2) + ":" + response.GetValue<string>(3);
                PimDeWitte.UnityMainThreadDispatcher.UnityMainThreadDispatcher.Instance().
                Enqueue(() => OnUpdateAnim(pack));
            });

            socket.On("JOIN_SUCCESS", response =>
            {
                PimDeWitte.UnityMainThreadDispatcher.UnityMainThreadDispatcher.Instance().
                Enqueue(() => local_player_id = response.GetValue<string>(0));
            });
        }

        async public void ButtonClick()
        {
            Debug.Log("Presiono el boton");
            string name = input.text;
            await socket.EmitAsync("hello", name);

        }

        public async void EmitJoin()
        {
            PlayerInfo player = new PlayerInfo(input.text, "none",
                0 + ":" + 0 + ":" + 0 + ":" + 0, spawnPoint.position.x.ToString(),
                spawnPoint.position.y.ToString(), spawnPoint.position.z.ToString());
            await socket.EmitAsync("JOIN", JsonUtility.ToJson(player));
            onLogged = true;
        }

        private void SpawnPlayer(string id, string name, string posX, string posY, string posZ, string model)
        {

            var pack = model.Split(Delimiter);
            bool alreadyExist = false;

            //verify all players to avoid duplicates
            if (networkPlayers.ContainsKey(id))
            {
                alreadyExist = true;
            }
            if (!alreadyExist)
            {
                Debug.Log("received spawn network player");




                // newPlayer = GameObject.Instantiate( network player avatar or model, spawn position, spawn rotation)
                PlayerManager newPlayer = GameObject.Instantiate(remotePlayerPref[int.Parse(pack[0])],
                    new Vector3(UtilsClass.StringToFloat(posX), UtilsClass.StringToFloat(posY),
                        UtilsClass.StringToFloat(posZ)), Quaternion.identity).GetComponent<PlayerManager>();
                newPlayer.SetModel(int.Parse(pack[0]), int.Parse(pack[1]));
                newPlayer.SetModelColor(int.Parse(pack[0]), int.Parse(pack[3]), int.Parse(pack[2]));

                newPlayer.id = id;

                newPlayer.name = name;

                newPlayer.isLocalPlayer = false; //it is not the local player

                newPlayer.isOnline = true; //set network player online in the arena

                newPlayer.Set3DName(name); //set the network player 3D text with his name

                newPlayer.gameObject.name = name;

                networkPlayers[id] = newPlayer;
            }
        }

        private void OnUpdateMoveAndRotate(string data)
        {
            /*
             * data.pack[0] = id (network player id)
             * data.pack[1] = position.x
             * data.pack[2] = position.y
             * data.pack[3] = position.z
             * data.pack[4] = "rotation"
            */

            var pack = data.Split(Delimiter);

            if (networkPlayers.ContainsKey(pack[0]))
            {

                PlayerManager netPlayer = networkPlayers[pack[0]];

                //update with the new position
                netPlayer.UpdatePosition(new Vector3(
                    UtilsClass.StringToFloat(pack[1]), UtilsClass.StringToFloat(pack[2]), UtilsClass.StringToFloat(pack[3])));
                Vector4 _rot = UtilsClass.StringToVector4(pack[4]);
                //update new player rotation
                netPlayer.UpdateRotation(new Quaternion(_rot.x, _rot.y, _rot.z, _rot.w));

            }


        }

        void OnUpdateAnim(string data)
        {


            /*
             * data.pack[0] = id (network player id)
             * data.pack[1] = key
             * data.pack[2] = value
            */

            var pack = data.Split(Delimiter);
            //	Debug.Log("pack[1]: "+pack[1] );
            //	Debug.Log("pack[2]: "+pack[2] );

            if (onLogged)
            {
                //find network player by your id
                if (!networkPlayers.ContainsKey(pack[0]))
                {
                    return;
                }
                PlayerManager netPlayer = networkPlayers[pack[0]];
                //updates current animation

                switch (pack[3])
                {
                    case "float":
                        netPlayer._animator.SetFloat(int.Parse(pack[1]), UtilsClass.StringToFloat(pack[2]));
                        break;
                    case "bool":
                        netPlayer._animator.SetBool(int.Parse(pack[1]), bool.Parse(pack[2]));
                        break;
                }

                //   Debug.Log("animation updated");

            }


        }

        public async void EmitMoveAndRotate(Transform user_transform)
        {
            PlayerInfoTransform player = new PlayerInfoTransform(user_transform.position.x.ToString(),
                user_transform.position.y.ToString(), user_transform.position.z.ToString(),
                user_transform.rotation.x + ";" + user_transform.rotation.y + ";" + user_transform.rotation.z + ";" + user_transform.rotation.w);

            string pack = JsonUtility.ToJson(player);

            await socket.EmitAsync("MOVE_AND_ROTATE", pack);
        }

    }


    public class PlayerInfoTransform
    {
        public string posX;
        public string posY;
        public string posZ;
        public string rotation;

        public PlayerInfoTransform(string posX, string posY, string posZ, string rotation)
        {
            this.posX = posX;
            this.posY = posY;
            this.posZ = posZ;
            this.rotation = rotation;
        }
    }

}
