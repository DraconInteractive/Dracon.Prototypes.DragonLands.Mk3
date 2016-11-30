using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuButtonScript : MonoBehaviour {
	public bool isNewGame, isContinue, isQuit;	

	void Awake () {
		if (isNewGame) {
			GetComponent<Button> ().onClick.AddListener (() => NewGame ());
		} else if (isContinue) {
			GetComponent<Button> ().onClick.AddListener (() => Continue ());
			if (PlayerPrefs.GetInt("HasSaveGame",0) == 0) {
				GetComponent<Button> ().interactable = false;
			}
		} else if (isQuit) {
			GetComponent<Button> ().onClick.AddListener (() => Quit ());
		}
	}

	void OnEnable () {
		if (isContinue) {
			if (PlayerPrefs.GetInt("HasSaveGame",0) == 0) {
				GetComponent<Button> ().interactable = false;
			}
		}

	}

	void NewGame () {
		PlayerPrefs.SetInt ("HasSaveGame", 1);
		print ("loading new game screen");
		SceneManager.LoadScene ("Character_Creation_Scene");
	}

	void Continue () {
		print ("loading previous game");
		SceneManager.LoadScene ("Scene 01");
	}

	void Quit () {
		print ("quit");
		Application.Quit();
	}
}
