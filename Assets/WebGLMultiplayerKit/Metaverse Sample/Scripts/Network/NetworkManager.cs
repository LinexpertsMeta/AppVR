using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Text;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using Cinemachine;
using TMPro;
using Newtonsoft.Json;

/// <summary>
/// Network Manager class.
/// </summary>
///
namespace MetaverseSample
{

    public class NetworkManager : MonoBehaviour
    {
        public static NetworkManager instance; //useful for any gameObject to access this class without the need of instances her or you declare it

        static private readonly char[] Delimiter = new char[] { ':' };  //Variable that defines ':' character as separator

        [HideInInspector]
        public bool onLogged = false; //flag which is determined the player is logged in the game arena


        [HideInInspector]
        public bool onLoggedWithMetamask = false; //flag which is determined the player is logged in the game arena

        [HideInInspector]
        public GameObject localPlayer; //store localPlayer

        [HideInInspector]
        public string local_player_id;

        //store all players in game
        public Dictionary<string, PlayerManager> networkPlayers = new Dictionary<string, PlayerManager>();

        [Header("Local Player Prefab")]
        public GameObject[] playerPref; //store the local player prefabs

        [Header("Local Player Prefab")]
        public GameObject[] remotePlayerPref; //store the local player prefabs


        [Header("Spawn Points")]
        public Transform[] spawnPoints; //stores the spawn points


        [Header("Field Of View Variables")]
        public float defaultFOV;

        [HideInInspector]
        public bool isGameOver; // game over flag

        [HideInInspector]
        public string _inputAxisNameX;

        [HideInInspector]
        public string _inputAxisNameY;

        public string account;

        [Header("Cinemachine Camera Brain")]
        public GameObject camBrain;

        [Header("FreeLookCamera")]
        public CinemachineFreeLook cinemachineFreeLook;


        [DllImport("__Internal")] private static extern void DetectDevice();

        [DllImport("__Internal")] private static extern void MetamaskSignIn();

        [DllImport("__Internal")] private static extern void ConfirmTransaction(string _amount);

        public string currentZone;

        void Awake()
        {
            Application.ExternalEval("socket.isReady = true;");

            _inputAxisNameX = cinemachineFreeLook.m_XAxis.m_InputAxisName;
            _inputAxisNameY = cinemachineFreeLook.m_YAxis.m_InputAxisName;

        }

        // Use this for initialization
        void Start()
        {

            // if don't exist an instance of this class
            if (instance == null)
            {


                //it doesn't destroy the object, if other scene be loaded
                DontDestroyOnLoad(this.gameObject);
                instance = this;// define the class as a static variable


                Debug.Log("start mmo game");



            }
            else
            {
                //it destroys the class if already other class exists
                Destroy(this.gameObject);
            }

        }

        public void OnDetectDevice(string _device)
        {

            /*
             * _data =  desktop or mobile
             * 

            */

            Debug.Log("data: " + _device);


            CanvasManager.instance.SetUpCanvas(_device);

        }




        /// <summary>
        /// Prints the pong message which arrived from server.
        /// </summary>
        /// <param name="_msg">Message.</param>
        public void OnPrintPongMsg(string data)
        {

            /*
             * data.pack[0]= msg
            */

            var pack = data.Split(Delimiter);
            Debug.Log("received message: " + pack[0] + " from server by callbackID: PONG");
        }

        // <summary>
        /// sends ping message to server.
        /// </summary>
        public void EmitPing()
        {

            //hash table <key, value>
            Dictionary<string, string> data = new Dictionary<string, string>();

            //store "ping!!!" message in msg field
            data["msg"] = "ping!!!!";

            JSONObject jo = new JSONObject(data);

            //sends to the nodejs server through socket the json package
            Application.ExternalCall("socket.emit", "PING", new JSONObject(data));


        }



        // <summary>
        /// manages and switches user login.
        /// </summary>
        public void SignIn(string _method)
        {

            switch (_method)
            {
                case "metamask":
                    MetamaskSignIn();
                    break;
                case "guest":
                    CanvasManager.instance.OpenScreen(2);
                    break;
            }

        }






        //call be  OnClickJoinBtn() method from CanvasManager class
        /// <summary>
        /// Emits the player's information to the server.
        /// </summary>
        /// <param name="_login">Login.</param>
        public void EmitJoin()
        {
            //hash table <key, value>
            Dictionary<string, string> data = new Dictionary<string, string>();


            //makes the draw of a point for the player to be spawn
            int index = Random.Range(0, spawnPoints.Length);

            //send the position point to server
            string msg = string.Empty;


            data["name"] = CanvasManager.instance.inputLogin.text;

            data["publicAddress"] = "none";

            if (onLoggedWithMetamask)
            {
                data["publicAddress"] = CanvasManager.instance.myPublicAdrr;
            }

            //store player's skin
            data["model"] = AvatarSelector.instance.indexSelectorHead.ToString() + ":" + AvatarSelector.instance.indexSelectorHair.ToString() +
                ":" + AvatarSelector.instance.indexSelectorBody.ToString() + ":" + AvatarSelector.instance.indexSelectorColor.ToString();
            data["posX"] = spawnPoints[index].position.x.ToString();
            data["posY"] = spawnPoints[index].position.y.ToString();
            data["posZ"] = spawnPoints[index].position.z.ToString();

            //sends to the nodejs server through socket the json package
            Application.ExternalCall("socket.emit", "JOIN", new JSONObject(data));


            Debug.Log("join sended");

            //obs: take a look in server script.
        }

        /// <summary>
        /// Joins the local player in game.
        /// </summary>
        /// <param name="_data">Data.</param>
        public void OnJoinGame(string data)
        {


            /*
             * pack[0] = id (local player id)
             * pack[1]= name (local player name)
             * pack[2] = position.x (local player position x)
             * pack[3] = position.y (local player position ...)
             * pack[4] = position.z (local player position ...)
             * pack[5] = model.head
             * pack[6] = model.hair
             * pack[7] = model.body
             * pack[8] = model.color

            */

            Debug.Log("Login successful, joining game");

            var pack = data.Split(Delimiter);
            Debug.Log("pack: " + data);

            // the local player now is logged
            onLogged = true;


            // take a look in NetworkPlayer.cs script
            PlayerManager newPlayer;

            // newPlayer = GameObject.Instantiate( local player avatar or model, spawn position, spawn rotation)
            newPlayer = GameObject.Instantiate(playerPref[int.Parse(pack[7])],
                    new Vector3(UtilsClass.StringToFloat(pack[2]), UtilsClass.StringToFloat(pack[3]),
                        UtilsClass.StringToFloat(pack[4])), Quaternion.identity).GetComponent<PlayerManager>();

            newPlayer.SetModel(int.Parse(pack[5]), int.Parse(pack[6]));
            newPlayer.SetModelColor(int.Parse(pack[5]), int.Parse(pack[8]), int.Parse(pack[7]));

            Debug.Log("player instantiated");

            account = pack[0];

            newPlayer.id = pack[0];

            newPlayer.name = pack[1];

            //this is local player
            newPlayer.isLocalPlayer = true;

            //now local player online in the arena
            newPlayer.isOnline = true;

            //set local player's 3D text with his name
            newPlayer.Set3DName(pack[1]);

            //puts the local player on the list
            networkPlayers[pack[0]] = newPlayer;

            localPlayer = networkPlayers[pack[0]].gameObject;

            local_player_id = pack[0];


            camBrain.GetComponent<Camera>().enabled = true;

            SetCinemachineFreeLookTarget(newPlayer.transform);

            //hide the lobby menu (the input field and join buton)
            CanvasManager.instance.OpenScreen(1);
            CharacterChoiceManager.instance.HideModels();

            DetectDevice();
            Debug.Log("player in game");

        }


        public void SetCinemachineFreeLookTarget(Transform _target)
        {

            cinemachineFreeLook.LookAt = _target;

            cinemachineFreeLook.Follow = _target;



        }

        /// <summary>
        /// Raises the spawn player event.
        /// </summary>
        /// <param name="_msg">Message.</param>
        void OnSpawnPlayer(string data)
        {

            /*
             * pack[0] = id (network player id)
             * pack[1]= name
             * pack[2] = position.x
             * pack[3] = position.y
             * pack[4] = position.z
             * pack[5] = model.head
             * pack[6] = model.hair
             * pack[7] = model.body
             * pack[8] = model.color
            */

            var pack = data.Split(Delimiter);

            bool alreadyExist = false;

            //verify all players to avoid duplicates 
            if (networkPlayers.ContainsKey(pack[0]))
            {
                alreadyExist = true;
            }
            if (!alreadyExist)
            {
                Debug.Log("received spawn network player");





                //PlayerManager newPlayer;

                // newPlayer = GameObject.Instantiate( network player avatar or model, spawn position, spawn rotation)
                PlayerManager newPlayer = GameObject.Instantiate(remotePlayerPref[int.Parse(pack[7])],
                    new Vector3(UtilsClass.StringToFloat(pack[2]), UtilsClass.StringToFloat(pack[3]),
                        UtilsClass.StringToFloat(pack[4])), Quaternion.identity).GetComponent<PlayerManager>();

                newPlayer.SetModel(int.Parse(pack[5]), int.Parse(pack[6]));
                newPlayer.SetModelColor(int.Parse(pack[5]), int.Parse(pack[8]), int.Parse(pack[7]));


                Debug.Log("player spawned");

                newPlayer.id = pack[0];

                newPlayer.name = pack[1];

                newPlayer.isLocalPlayer = false; //it is not the local player

                newPlayer.isOnline = true; //set network player online in the arena

                newPlayer.Set3DName(pack[1]); //set the network player 3D text with his name

                newPlayer.gameObject.name = pack[0];

                networkPlayers[pack[0]] = newPlayer; //puts the network player on the list
                if (CanvasManager.instance.contentUsers.gameObject.activeInHierarchy)
                {
                    NetworkManager.instance.EmitGetUsersList();
                }
            }


        }





        /// <summary>
        /// send player's position and rotation to the server
        /// </summary>
        /// <param name="data"> package with player's position and rotation</param>
        public void EmitMoveAndRotate(Dictionary<string, string> data)
        {

            JSONObject jo = new JSONObject(data);

            //sends to the nodejs server through socket the json package
            Application.ExternalCall("socket.emit", "MOVE_AND_ROTATE", new JSONObject(data));

        }



        /// <summary>
        /// Update the network player position and rotation to local player.
        /// </summary>
        /// <param name="_msg">Message.</param>
        void OnUpdateMoveAndRotate(string data)
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

        /// <summary>
        /// Emits the local player camera to Server.js.
        /// </summary>
        /// <param name="_animation">animation's name.</param>
        public void EmitToggleCamera()
        {
            Dictionary<string, string> data = new Dictionary<string, string>();

            data["local_player_id"] = localPlayer.GetComponent<PlayerManager>().id;
            JSONObject jo = new JSONObject(data);

            Application.ExternalCall("socket.emit", "TOGGLE_CAMERA", new JSONObject(data));

        }

        public void OnToggleCamera(string data)
        {
            var pack = data.Split(Delimiter);

            if (networkPlayers.ContainsKey(pack[0]))
            {
                PlayerManager netPlayer = networkPlayers[pack[0]];
                netPlayer.ToggleCameraOnline();
            }
        }

        /// <summary>
        /// Metodo para enviar la textura de la camara, se crean dos diccionarios y luego se unen en otro hecho en formato de
        /// <string, object>, el resultado que llegara al javascript es dos json dentro de un json mas grande
        /// </summary>
        /// <param name="TextureArray"></param>
        public void EmitUpdateCamera(byte[] TextureArray)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data["local_player_id"] = localPlayer.GetComponent<PlayerManager>().id;


            Dictionary<string, byte[]> data2 = new Dictionary<string, byte[]>();
            data2["texture"] = TextureArray;

            Dictionary<string, object> jsonObject = new Dictionary<string, object>();
            jsonObject.Add("dictionary1", data);
            jsonObject.Add("dictionary2", data2);

            string json = JsonConvert.SerializeObject(jsonObject);
            Application.ExternalCall("socket.emit", "UPDATE_CAMERA", json);
        }

        public void OnUpdateCameraPlayer(string data)
        {
            var pack = data.Split(Delimiter);
            if (networkPlayers.ContainsKey(pack[0]))
            {
                PlayerManager netPlayer = networkPlayers[pack[0]];

                netPlayer.UpdateCamera(pack[1]);
            }

        }

        public void EmitMegafono()
        {
            Application.ExternalCall("socket.emit", "Megafono");

        }
        public void EmitMegafonoOff()
        {
            Application.ExternalCall("socket.emit", "MegafonoOff");

        }

        /// <summary>
        /// Emits the local player animation to Server.js.
        /// </summary>
        /// <param name="_animation">animation's name.</param>
        public void EmitAnimation(int _key, string _value, string _type)
        {
            //hash table <key, value>
            Dictionary<string, string> data = new Dictionary<string, string>();

            data["local_player_id"] = localPlayer.GetComponent<PlayerManager>().id;

            data["key"] = _key.ToString();

            data["value"] = _value;

            data["type"] = _type;

            JSONObject jo = new JSONObject(data);

            //sends to the nodejs server through socket the json package
            Application.ExternalCall("socket.emit", "ANIMATION", new JSONObject(data));


        }


        /// <summary>
        ///  Update the network player animation to local player.
        /// </summary>
        /// <param name="data">package received from server with player id and  animation's name</param>
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

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////// USERS LIST UPDATES/////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        public void EmitGetUsersList()
        {

            CanvasManager.instance.ClearUsersList();

            //sends to the nodejs server through socket the json package
            Application.ExternalCall("socket.emit", "GET_USERS_LIST");


        }

        void OnClearUsersList()
        {
            CanvasManager.instance.ClearUsersList();
        }

        void OnUpdateUsersList(string data)
        {

            /*
             * pack[0] = id
             * pack[1] = name
             * pack[2] = public address
            */

            var pack = data.Split(Delimiter);


            //Debug.Log("received best players from server ...");
            //Debug.Log("id: "+pack[0]);
            //Debug.Log("name: "+pack[1]);
            //Debug.Log("public address: "+pack[2]);

            CanvasManager.instance.SetUpUser(pack[0], pack[1], pack[2]);

        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////



        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////MESSAGE FUNCTIONS////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// method to emit message to the server.
        /// </summary>
        public void EmitMessage()
        {
            if (string.IsNullOrWhiteSpace(CanvasManager.instance.inputFieldMessage.text))
            {
                return;
            }
            Dictionary<string, string> data = new Dictionary<string, string>();

            string msg = string.Empty;

            //Identifies with the name "MESSAGE", the notification to be transmitted to the server
            data["callback_name"] = "MESSAGE";

            data["id"] = local_player_id;

            data["message"] = CanvasManager.instance.inputFieldMessage.text;

            CanvasManager.instance.inputFieldMessage.text = string.Empty;


            //sends to the nodejs server through socket the json package
            Application.ExternalCall("socket.emit", data["callback_name"], new JSONObject(data));


        }

        /// <summary>
        /// method to handle notification that arrived from the server.
        /// </summary>	
        /// <param name="data">received package from server.</param>
        void OnReceiveMessage(string data)
        {

            /*
                 * data.pack[0] = id (network player id)
                 * data.pack[1]= message
                */



            var pack = data.Split(Delimiter);


            if (local_player_id.Equals(pack[0]))
            {

                CanvasManager.instance.SpawnMyMessage(pack[1]);

            }
            else
            {
                CanvasManager.instance.SpawnNetworkMessage(networkPlayers[pack[0]].name, pack[1]);
            }


        }



        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////PRIVATE CHAT FUNCTIONS////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        /// <summary>
        /// method to emit message to the server.
        /// </summary>
        public void EmitOpenChatBox(string _player_id)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();

            string msg = string.Empty;

            //Identifies with the name "MESSAGE", the notification to be transmitted to the server
            data["callback_name"] = "SEND_OPEN_CHAT_BOX";

            data["player_id"] = _player_id;

            //sends to the nodejs server through socket the json package
            Application.ExternalCall("socket.emit", data["callback_name"], new JSONObject(data));


        }


        /// <summary>
        /// method to handle notification that arrived from the server.
        /// </summary>	
        /// <param name="data">received package from server.</param>
        void OnReceiveOpenChatBox(string data)
        {

            /*
                 * data.pack[0] = host id 
                 * data.pack[1]= guest id
                */



            var pack = data.Split(Delimiter);

            if (local_player_id.Equals(pack[0]))
            {
                //spawn new chatbox
                CanvasManager.instance.SpawnChatBox(pack[0], pack[0], pack[1], networkPlayers[pack[1]].name);
            }
            else
            {
                CanvasManager.instance.SpawnChatBox(pack[0], pack[1], pack[0], networkPlayers[pack[0]].name);
            }


        }


        /// <summary>
        /// method to emit message to the server.
        /// </summary>
        public void EmitPrivateMessage(string _message, string _chat_box_id, string _gest_id, string _host_id)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();

            string msg = string.Empty;

            //Identifies with the name "MESSAGE", the notification to be transmitted to the server
            data["callback_name"] = "PRIVATE_MESSAGE";

            data["chat_box_id"] = _chat_box_id;

            data["guest_id"] = _gest_id;

            data["message"] = _message;

            data["host_id"] = _host_id;

            CanvasManager.instance.inputFieldPrivateMessage.text = string.Empty;


            //sends to the nodejs server through socket the json package
            Application.ExternalCall("socket.emit", data["callback_name"], new JSONObject(data));


        }

        /// <summary>
        /// method to handle notification that arrived from the server.
        /// </summary>	
        /// <param name="data">received package from server.</param>
        void OnReceivePrivateMessage(string data)
        {

            /*
                 * data.pack[1] = host
                 * data.pack[2] = message
                 * data.pack[3] = guest
                 * currentUserAtr = _chat_box_id + ":" + host_id + ':' + message + ":" + another_host_id + ":" + another_guest_id;
                */



            var pack = data.Split(Delimiter);

            Debug.Log("pack[0]: " + pack[0]);
            Debug.Log("pack[1]: " + pack[1]);
            Debug.Log("pack[2]: " + pack[2]);
            Debug.Log("pack[3]: " + pack[3]);

            if (CanvasManager.instance.chatBoxes.ContainsKey(pack[0]))
            {
                if (local_player_id.Equals(pack[1]))
                {
                    foreach (ChatBox chat in CanvasManager.instance.chatBoxs)
                    {
                        if((chat.host_id == pack[3] && chat.guest_id == pack[4])||(chat.host_id == pack[4] && chat.guest_id == pack[3]))
                        {
                            chat.SpawnMyMessage(pack[2]);
                            if (!chat.gameObject.activeInHierarchy)
                            {
                                chat.gameObject.SetActive(true);
                            }
                        }
                    }
                }
                else
                {
                    foreach (ChatBox chat in CanvasManager.instance.chatBoxs)
                    {
                        if ((chat.host_id == pack[3] && chat.guest_id == pack[4]) || (chat.host_id == pack[4] && chat.guest_id == pack[3]))
                        {
                            if (local_player_id.Equals(pack[1]))
                            {
                                chat.SpawnNetworkMessage(networkPlayers[pack[1]].name, pack[2]);
                            }
                            else
                            {
                                chat.SpawnNetworkMessage(networkPlayers[pack[3]].name, pack[2]);
                            }
                            if (!chat.gameObject.activeInHierarchy)
                            {
                                chat.gameObject.SetActive(true);
                            }
                        }
                    }
                }
            }


        }

        /// <summary>
        /// method to emit message to the server.
        /// </summary>
        public void EmitConfirmTransaction(string _id_to, string _amount)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();

            string msg = string.Empty;

            //Identifies with the name "MESSAGE", the notification to be transmitted to the server
            data["callback_name"] = "CONFIRM_TRANSACTION";

            data["idTo"] = _id_to;

            data["amount"] = _amount;

            //sends to the nodejs server through socket the json package
            Application.ExternalCall("socket.emit", data["callback_name"], new JSONObject(data));


        }

        void OnConfirmTransaction(string data)
        {

            /*
             * data.pack[0] = amount

            */

            var pack = data.Split(Delimiter);

            Debug.Log("amount: " + pack[0]);

            ConfirmTransaction(pack[0]);


        }


        void OnUpdateUserVoiceInfo(string data)
        {

            /*
             * data.pack[0] = id

            */

            var pack = data.Split(Delimiter);
            //Debug.Log("name: "+networkPlayers[pack[0]].name);

            CanvasManager.instance.if_CurrentUserNameVoice.text = networkPlayers[pack[0]].name + " is talking";

            StartCoroutine("ClearCurrentVoiceText");




        }


        IEnumerator ClearCurrentVoiceText()
        {

            yield return new WaitForSeconds(3f); // wait for set reload time

            CanvasManager.instance.if_CurrentUserNameVoice.text = string.Empty;


        }

        /// <summary>
        /// method to emit message to the server.
        /// </summary>
        public void EmitMuteAllUsers()
        {

            Dictionary<string, string> data = new Dictionary<string, string>();

            //Identifies with the name "MESSAGE", the notification to be transmitted to the server
            data["callback_name"] = "MUTE_ALL_USERS";

            //sends to the nodejs server through socket the json package
            Application.ExternalCall("socket.emit", data["callback_name"]);


        }

        /// Emits the local player mute request  to Server.js.
        /// </summary>
        public void EmitAudioMute()
        {

            //sends to the nodejs server through socket the json package
            Application.ExternalCall("socket.emit", "AUDIO_MUTE");


        }





        /// <summary>
        /// method to emit message to the server.
        /// </summary>
        public void EmitRemoveAllUsersMute()
        {

            Dictionary<string, string> data = new Dictionary<string, string>();

            //Identifies with the name "MESSAGE", the notification to be transmitted to the server
            data["callback_name"] = "REMOVE_MUTE_ALL_USERS";

            //sends to the nodejs server through socket the json package
            Application.ExternalCall("socket.emit", data["callback_name"]);


        }


        /// <summary>
        /// method to emit message to the server.
        /// </summary>
        public void EmitMuteUser(string _id)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();

            string msg = string.Empty;

            //Identifies with the name "MESSAGE", the notification to be transmitted to the server
            data["callback_name"] = "ADD_MUTE_USER";

            data["id"] = _id;

            //sends to the nodejs server through socket the json package
            Application.ExternalCall("socket.emit", data["callback_name"], new JSONObject(data));


        }

        /// <summary>
        /// method to emit message to the server.
        /// </summary>
        public void EmitRemoveMuteUser(string _id)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();

            string msg = string.Empty;

            //Identifies with the name "MESSAGE", the notification to be transmitted to the server
            data["callback_name"] = "REMOVE_MUTE_USER";

            data["id"] = _id;

            //sends to the nodejs server through socket the json package
            Application.ExternalCall("socket.emit", data["callback_name"], new JSONObject(data));


        }




        /// <summary>
        /// inform the local player to destroy offline network player
        /// </summary>
        /// <param name="_msg">Message.</param>
        //desconnect network player
        void OnUserDisconnected(string data)
        {

            /*
             * data.pack[0] = id (network player id)
            */

            var pack = data.Split(Delimiter);


            if (networkPlayers.ContainsKey(pack[0]))
            {

                Debug.Log("Desconectado1");
                if (networkPlayers[pack[0]].isSharing)
                {
                    Debug.Log("Desconectado4");
                }

                if(networkPlayers[pack[0]].currentZone == currentZone)
                {
                    Debug.Log("Desconectado5");
                }

                if (networkPlayers[pack[0]].isSharing && networkPlayers[pack[0]].currentZone == currentZone)
                {
                    Debug.Log("Desconectado2");
                    List<ShareScreen> screens = ScreensManager.instance.screens;
                    foreach (ShareScreen s in screens)
                    {
                        if (s.zoneToTransmit == networkPlayers[pack[0]].currentZone)
                        {
                            Debug.Log("Desconectado3");
                            s.StopSharing();
                        }
                    }
                }
                //destroy network player by your id
                Destroy(networkPlayers[pack[0]].gameObject);


                //remove from the dictionary
                networkPlayers.Remove(pack[0]);
            }

        }

        public void EmitUserSharingScreen(bool state)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data["ID"] = local_player_id;
            data["state"] = state ? "true" : "false";
            Application.ExternalCall("socket.emit", "UserSharingScreen", new JSONObject(data));
        }

        public void OnUserSharingScreen(string data)
        {
            string[] pack = data.Split(Delimiter);
            if (networkPlayers.ContainsKey(pack[0]))
            {
                networkPlayers[pack[0]].isSharing = pack[1] == "true" ? true : false;
            }
        }

        public void OnEmitVideoPlay()
        {
            Application.ExternalCall("socket.emit", "VIDEO_PLAY");
        }

        public void OnVideoPlay()
        {
            VideoManager.instance.Play();
        }

        public void OnEmitVideoPause()
        {
            Application.ExternalCall("socket.emit", "VIDEO_PAUSE");
        }

        public void OnVideoPause()
        {
            VideoManager.instance.Pause();
        }

        public void OnEmitMusicPlay()
        {
            Application.ExternalCall("socket.emit", "MUSIC_PLAY");
        }

        public void OnMusicPlay()
        {
            //MusicManager.instance.PlayMusic();
        }

        public void OnEmitMusicPause()
        {
            Application.ExternalCall("socket.emit", "MUSIC_PAUSE");
        }

        public void OnMusicPause()
        {
            //MusicManager.instance.PauseMusic();
        }

        public void EmitSitPlayer(int idChari)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();

            data["chair"] = idChari.ToString();

            JSONObject jo = new JSONObject(data);

            Application.ExternalCall("socket.emit", "CHAIR", new JSONObject(data));
        }

        public void OnChariFilled(string chairID)
        {
            int auxIDChair = int.Parse(chairID);
            //SeatsManager.instance.ToggleChairByArray(true, auxIDChair);
        }

        public void EmitGetUpPlayer(int idChari)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();

            data["chair"] = idChari.ToString();

            JSONObject jo = new JSONObject(data);

            Application.ExternalCall("socket.emit", "CHAIR_OUT", new JSONObject(data));
        }

        public void OnChariFilledOut(string chairID)
        {
            int auxIDChair = int.Parse(chairID);
            //SeatsManager.instance.ToggleChairByArray(false, auxIDChair);
        }

        public void EmitToggleGameObject(GameObject objectToToggle)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();

            data["object"] = objectToToggle.name;
            if (objectToToggle.activeSelf)
            {
                objectToToggle.SetActive(false);
                data["state"] = "False";
            }
            else
            {
                objectToToggle.SetActive(true);
                data["state"] = "True";
            }

            Application.ExternalCall("socket.emit", "TOGGLE_OBJECT", new JSONObject(data));
        }
        public void OnToggleActivationObject(string data)
        {
            var pack = data.Split(Delimiter);
            bool state = pack[1] == "True" ? true : false;
            if (pack[0] == "Door1")
            {
                Doors.instance.ToggleDoor(0, state);
            }
            if (pack[0] == "Door2")
            {
                Doors.instance.ToggleDoor(1, state);
            }
            if (pack[0] == "Door3")
            {
                Doors.instance.ToggleDoor(2, state);
            }
            if (pack[0] == "Door4")
            {
                Doors.instance.ToggleDoor(3, state);
            }
            if (pack[0] == "Door5")
            {
                Doors.instance.ToggleDoor(4, state);
            }
            if (pack[0] == "Door6")
            {
                Doors.instance.ToggleDoor(5, state);
            }
        }

        public void EmitTurnOffCamera()
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data["local_player_id"] = localPlayer.GetComponent<PlayerManager>().id;

            Application.ExternalCall("socket.emit", "TURNOFF_CAMERA", new JSONObject(data));
        }

        public void OnTurnOffCamera(string data)
        {
            var pack = data;
            if (networkPlayers.ContainsKey(data))
            {
                PlayerManager netPlayer = networkPlayers[pack];
                netPlayer.TurnOffCamera();
            }
        }
        public void EmitShareScreen()
        {
            Application.ExternalCall("socket.emit", "TOGGLE_SHARESCREEN");
        }
        public void EmitShareScreenOFF()
        {
            Application.ExternalCall("socket.emit", "TOGGLE_SHARESCREEN_OFF", currentZone);
        }


        public void OnShareScreen(string data)
        {
            string[] pack = data.Split(":zona:");
            List<ShareScreen> screens = ScreensManager.instance.screens;
            foreach (ShareScreen s in screens)
            {
                if (s.zoneToTransmit == pack[1])
                {
                    string[] pngdata = pack[0].Split(",");
                    s.OnUpdateTextureScreen(pngdata[1]);
                }
            }
            //ShareScreen.instance.OnUpdateTextureScreen(pack[1]);
        }

        public void OnStopSharingScreen(string data)
        {
            List<ShareScreen> screens = ScreensManager.instance.screens;
            foreach (ShareScreen s in screens)
            {
                if (s.zoneToTransmit == data)
                {
                    s.StopSharing();
                    EmitUserSharingScreen(false);
                    PlayerManager.instance.isSharing = false;
                }
            }
            //ShareScreen.instance.StopSharing();
        }

        public void EmitChangeZone(string zona)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data["ID"] = local_player_id;
            data["currentZone"] = zona;
            currentZone = zona;
            Application.ExternalCall("socket.emit", "Changing_Zone", new JSONObject(data));
        }

        public void OnUserChangeZone(string data)
        {
            string[] pack = data.Split(Delimiter);
            if (networkPlayers.ContainsKey(pack[0]))
            {
                networkPlayers[pack[0]].currentZone = pack[1];
            }
        }

    }
}