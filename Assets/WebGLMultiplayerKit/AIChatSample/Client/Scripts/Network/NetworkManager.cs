using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Text;
using UnityEngine.UI;
using System.Runtime.InteropServices;

/// <summary>
/// Class to manage the game client's network communication.
/// </summary>
/// 
namespace AIChatSample
{

public class NetworkManager : MonoBehaviour
{

	//useful for any gameObject to access this class without the need of instances her or you declare her
	public static NetworkManager instance;

	//flag which is determined the player is logged in the arena
	public bool onLogged = false;

    string local_player_id;


	//Variable that defines ':' character as separator
	static private readonly char[] Delimiter = new char[] {':'};
	
	
	
	void Awake()
	{
		Application.ExternalEval("socket.isReady = true;");

	}


    // Start is called before the first frame update
    void Start()
    {
        // if don't exist an instance of this class
		if (instance == null) {

		    //it doesn't destroy the object, if other scene be loaded
		    DontDestroyOnLoad (this.gameObject);
			
			instance = this;// define the class as a static variable
		
			
			 Debug.Log(" ------- chat started ------");
			

		}
		else
		{
			//it destroys the class if already other class exists
			Destroy(this.gameObject);
		}

    }
	
	

    

////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////[JOIN] [SPAWN AND RESPAWN] FUNCTIONS///////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	// method triggered by the BtnLogin button
	/// <summary>
	/// EmitToServers the player's name to server.
	/// </summary>
	public void EmitJoinRoom()
	{
	
	
		//hash table <key, value>	
		Dictionary<string, string> data = new Dictionary<string, string>();
	
		string msg = string.Empty;

		//Identifies with the name "JOIN", the notification to be transmitted to the server
		data["callback_name"] = "JOIN";
			
		data["name"] = CanvasManager.instance.ifLogin.text;
			
		//sends to the nodejs server through socket the json package
		Application.ExternalCall("socket.emit", data["callback_name"],new JSONObject(data));
		
		
	
		//obs: take a look in server script.
	}

	/// <summary>
	/// method to handle notification that arrived from the server.
	/// </summary>
	/// <remarks>
    /// Joins the local player in game.
    /// </remarks>
	/// <param name="_data">Data.</param>
	public void OnJoinGame(string data)
	{
		
		/*
		 * data.pack[0] = id (local player id)
		 * data.pack[1]= username (local player name)
		*/
	
	
			var pack = data.Split (Delimiter);
			
		    Debug.Log("Login successful, joining game");

		    // the local player now is logged
		    onLogged = true;
		
			Client client = new Client ();

			client.id = pack [0];//set client id

			client.name = pack [1];//set client name
			
			Debug.Log("player instantiated");
			
			//this is local player
			client.isLocalPlayer = true;
			
			local_player_id = client.id;

			//hide the lobby menu (the input field and join buton)
			CanvasManager.instance.OpenScreen("room");
			CanvasManager.instance.SpawnAIMessage("Ask me anything?");
			
			Debug.Log("player in game");
			
	
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
		Dictionary<string, string> data = new Dictionary<string, string>();
		
		string msg = string.Empty;

		//Identifies with the name "MESSAGE", the notification to be transmitted to the server
		data["callback_name"] = "MESSAGE";
		
		data ["message"] = CanvasManager.instance.inputFieldMessage.text;

		CanvasManager.instance.SpawnMyMessage(data ["message"]);
		
		CanvasManager.instance.inputFieldMessage.text = string.Empty;
			
		
		//sends to the nodejs server through socket the json package
		Application.ExternalCall("socket.emit", data["callback_name"],new JSONObject(data));
		
	
	}
	
	 /// <summary>
	/// method to handle notification that arrived from the server.
	/// </summary>	
	/// <param name="data">received package from server.</param>
	void OnReceiveMessage(string data)
	{
	
	/*
		 * data.pack[0]= message
		*/
		
      
		var pack = data.Split (Delimiter);
	  
		CanvasManager.instance.SpawnAIMessage(pack[0]);
			
		
	
			 
	}

	
	
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////


////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////DISCONNECTION FUNCTION////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
	

}//END_OF_CLASS
}//END_OF_NAMESPACE
