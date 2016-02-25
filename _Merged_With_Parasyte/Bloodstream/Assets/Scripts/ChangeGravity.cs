using UnityEngine;
using System.Collections;

public class ChangeGravity : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
		Physics.gravity = new Vector3 (0, -9.81f, 0);

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
