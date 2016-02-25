using UnityEngine;
using System.Collections;

public class LookAtCamera : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void LateUpdate () {

		this.transform.LookAt (Camera.main.transform.position);
		//this.transform.eulerAngles = new Vector3 (0f, 0f, 0f);
	
	}
}
