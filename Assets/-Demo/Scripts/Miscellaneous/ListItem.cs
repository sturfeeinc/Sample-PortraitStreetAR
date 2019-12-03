using UnityEngine;
using UnityEngine.UI;

public class ListItem : MonoBehaviour {

	public Text NameText;
	public Text CheckinsText;

	void Start () {
		
	}

	public void SetLocationData(string name, int checkins)
	{
		NameText.text = name;
		CheckinsText.text = checkins.ToString ();
	}

	public void ClearData()
	{
		NameText.text = "";
		CheckinsText.text = "";
		gameObject.SetActive (false);
	}
}
