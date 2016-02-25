using UnityEngine;
using System.Collections;

public class GlobuloRojoScript : MonoBehaviour {

	public AudioSource burst1;
	public AudioSource burst2;
	public AudioSource burst3;
	public AudioSource burst4;

	// Use this for initialization
	void Start () {

		burst1 = gameObject.AddComponent<AudioSource> ();
		burst1.clip = Resources.Load ("Burst_1") as AudioClip;
		burst1.volume = 1f;
		burst1.spatialBlend = 1f;
		burst1.minDistance = 30f;
		burst1.maxDistance = 100f;

		burst2 = gameObject.AddComponent<AudioSource> ();
		burst2.clip = Resources.Load ("Burst_2") as AudioClip;
		burst2.volume = 1f;
		burst2.spatialBlend = 1f;
		burst2.minDistance = 30f;
		burst2.maxDistance = 100f;

		burst3 = gameObject.AddComponent<AudioSource> ();
		burst3.clip = Resources.Load ("Burst_3") as AudioClip;
		burst3.volume = 1f;
		burst3.spatialBlend = 1f;
		burst3.minDistance = 30f;
		burst3.maxDistance = 100f;

		burst4 = gameObject.AddComponent<AudioSource> ();
		burst4.clip = Resources.Load ("Burst_4") as AudioClip;
		burst4.volume = 1f;
		burst4.spatialBlend = 1f;
		burst4.minDistance = 30f;
		burst4.maxDistance = 100f;
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
