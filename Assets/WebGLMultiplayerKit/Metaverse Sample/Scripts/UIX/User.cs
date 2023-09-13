using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MetaverseSample{
/// <summary>
/// User.
/// </summary>
public class User : MonoBehaviour
{
	
	public string id;

	public string publicAddress;

	public string name;

	public TextMeshProUGUI txtName;


	public void OpenUserOption()
	{
		CanvasManager.instance.OpenUserOptions(id,name, publicAddress);
	}



}//END_CLASS
}//END_NAMESPACE
