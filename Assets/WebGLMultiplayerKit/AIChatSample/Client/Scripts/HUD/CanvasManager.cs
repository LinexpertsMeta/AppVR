// ----------------------------------------------------------------------------------------------------------------------
// <copyright company="Rio 3D Studios">UDPNet Pro - Copyright (C) 2020 Rio 3D Studios</copyright>
// <author>Sebastiao Lucio - sebastiao@ice.ufjf.br</author>
// ----------------------------------------------------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;
using UnityEngine.UI;

/// <summary>
/// class to manage all the game's UI's and HUDs
/// </summary>
namespace  AIChatSample
{
 public class CanvasManager : MonoBehaviour
 {
	//useful for any script access this class without the need of object instance  or declare .
    public static  CanvasManager instance;

	[Header("Canvas HUD's :")]
	
	public GameObject hudSignIn; // set in inspector. stores the SignIn Panel
	
	public GameObject chatRoom; // set in inspector. stores the  chat Room Panel
	
	public Text txtLog; // set in inspector. stores the txtLog

    [Header("Input field Variables :")]
	public InputField ifLogin; // set in inspector. stores the login InputField

	public InputField inputFieldMessage;	

	[Header("Prefabs :")]

	public GameObject myMessagePrefab; // set in inspector. stores the user Prefab message game object
	
	public GameObject aiMessagePrefab; // set in inspector. stores the user Prefab message game object

    [Header("Contents :")]
	public GameObject contentMessages; // set in inspector. stores the content messages game object
	
    [HideInInspector]
	public int countMessages; //variable for controlling the number of messages on the screen
	
	[HideInInspector]
	public int maxDeleteMessage; //variable for controlling the number of messages on the screen

    [HideInInspector]
	public string currentHUD; // flag to mark the current screen
	
	ArrayList messages; // list to store all messages
	

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null) {

			DontDestroyOnLoad (this.gameObject);

			instance = this;
			
			messages = new ArrayList ();
			
			OpenScreen("login");


		}
		else
		{
			Destroy(this.gameObject);
		}
    }

  
	/// <summary>
	/// Opens the  current screen.
	/// </summary>
	/// <param name="_current">Current screen .</param>
	public void  OpenScreen(string _current)
	{
		switch (_current)
		{
		    case "login":
			currentHUD = _current;
			hudSignIn.SetActive(true);
	        chatRoom.SetActive(false);
			break;
			case "room":
			currentHUD = _current;
			hudSignIn.SetActive(false);
	        chatRoom.SetActive(true);
			break;
		
		
		}

	}



	public void SpawnMyMessage( string _message)
	{
	
	 
	  countMessages +=1;
	  
	  GameObject newMessage = Instantiate (myMessagePrefab) as GameObject;
	  newMessage.name = countMessages.ToString();
	  newMessage.GetComponent<Message>().txtMsg.text = _message;
      newMessage.transform.parent = contentMessages.transform;
	  newMessage.GetComponent<RectTransform> ().localScale = new Vector3 (1, 1, 1);	  	  
	  messages.Add (newMessage);
	  Debug.Log("messages.Count : "+messages.Count);
	   if (messages.Count > 7)
		{
		     ArrayList deleteMessages = new ArrayList();

			int j = 0;

			foreach(GameObject msg in messages )
			{
				if (j <= maxDeleteMessage) 
				{
                    deleteMessages.Add(msg);
				}
				j += 1;

			}
			
			foreach(GameObject msg in deleteMessages)
            {
			  Destroy (msg);
              messages.Remove(msg);
             }

		}
    
	}

	
	
	public void SpawnAIMessage( string _message)
	{
	
	 
	  countMessages +=1;
	  
	  GameObject newMessage = Instantiate (aiMessagePrefab) as GameObject;
	  newMessage.name = countMessages.ToString();
	   Debug.Log("_message : "+_message);
	  newMessage.GetComponent<Message>().txtMsg.text = _message;
      newMessage.transform.parent = contentMessages.transform;
	  newMessage.GetComponent<RectTransform> ().localScale = new Vector3 (1, 1, 1);	  	  
	  messages.Add (newMessage);
	  Debug.Log("messages.Count : "+messages.Count);
	   if (messages.Count > 7)
		{
		     ArrayList deleteMessages = new ArrayList();

			int j = 0;

			foreach(GameObject msg in messages )
			{
				if (j <= maxDeleteMessage) 
				{
                    deleteMessages.Add(msg);
				}
				j += 1;

			}
			
			foreach(GameObject msg in deleteMessages)
            {
			  Destroy (msg);
              messages.Remove(msg);
             }

		}
    
	}
	
 
}//END_OF_CLASS
}//END_OF_NAMESPACE