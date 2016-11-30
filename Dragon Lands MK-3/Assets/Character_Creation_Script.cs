using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class Character_Creation_Script : MonoBehaviour {

	public Button createButton;
	public InputField nameInput;
	public Dropdown sexDrop;
	public Slider weightSlider;
	public GameObject menu_Character_m;
	public MORPH3D.M3DCharacterManager m3dCM;

	private float weight;

	void Awake () {
		createButton.GetComponent<Button> ().onClick.AddListener (() => Create ());
	}

	void Create () {
		PlayerPrefs.SetInt ("PlayerSex", sexDrop.value);
		PlayerPrefs.SetString ("PlayerName", nameInput.text);
		PlayerPrefs.SetFloat ("PlayerWeight", weight);
		SceneManager.LoadScene ("Scene 01");
	}

	public void SetWeight (float t) {
		print ("Changed weight value");
		m3dCM.SetBlendshapeValue ("FBMHeavy", t);
		weight = t;
	}
}
