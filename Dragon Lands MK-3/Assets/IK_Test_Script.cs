using UnityEngine;
using System.Collections;

public class IK_Test_Script : MonoBehaviour {

	Player_Script ps;
	// Use this for initialization
	void Start () {
		ps = Player_Script.player.GetComponent<Player_Script> ();
	}
	
	void Interact () {
		ps.ikActive = true;
		ps.ikLookObj = this.gameObject.transform;
		ps.ikRightHandObj = transform.GetChild (0);
	}
}
