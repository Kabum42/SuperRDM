using UnityEngine;
using System.Collections;

public class ColliderScript : MonoBehaviour {

	//public bool isPlayer = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter(Collision collision) {
		checkCollision (collision);
	}
	
	void OnCollisionStay(Collision collision) {
		checkCollision (collision);
	}

	void OnTriggerEnter(Collider collider) {
		checkTrigger (collider);
	}
	
	void OnTriggerStay(Collider collider) {
		checkTrigger (collider);
	}
	
	void checkCollision(Collision collision) {

		if (this.gameObject.GetComponentInParent<CompetitorScript> ()) {

			if (collision.gameObject.tag == "Rojo" && this.gameObject.GetComponentInParent<CompetitorScript>().shredding && collision.gameObject.transform.GetComponent<Renderer>().material.GetFloat("_Amount") == 0f) {
				collision.gameObject.transform.GetComponent<Renderer>().material.SetFloat("_Amount", Time.deltaTime);
				collision.gameObject.transform.GetComponent<Rigidbody>().isKinematic = true;
				collision.gameObject.transform.GetComponent<Rigidbody>().angularVelocity = new Vector3 (0,0,0);

				/*
				if (!this.gameObject.GetComponent<AudioSource>().isPlaying) {
					this.gameObject.GetComponent<AudioSource>().Play();
				}
				*/

				float sound = Random.Range(0f, 100f);

				if (sound <= (100f/4f)*1f) {
					collision.gameObject.GetComponent<GlobuloRojoScript>().burst1.Play();
				}
				else if (sound <= (100f/4f)*2f) {
					collision.gameObject.GetComponent<GlobuloRojoScript>().burst2.Play();
				}
				else if (sound <= (100f/4f)*3f) {
					collision.gameObject.GetComponent<GlobuloRojoScript>().burst3.Play();
				}
				else if (sound <= (100f/4f)*4f) {
					collision.gameObject.GetComponent<GlobuloRojoScript>().burst4.Play();
				}

				this.gameObject.GetComponentInParent<CompetitorScript>().currentRush = this.gameObject.GetComponentInParent<CompetitorScript>().defaultRush;
				this.gameObject.GetComponentInParent<CompetitorScript>().redEffect = this.gameObject.GetComponentInParent<CompetitorScript>().redEffectDefault;
				this.gameObject.GetComponentInParent<CompetitorScript>().redBonusTotal += this.gameObject.GetComponentInParent<CompetitorScript>().redBonusPerRed;
				if (this.gameObject.GetComponentInParent<CompetitorScript>().redBonusTotal >= this.gameObject.GetComponentInParent<CompetitorScript>().redBonusCap) {
					this.gameObject.GetComponentInParent<CompetitorScript>().redBonusTotal = this.gameObject.GetComponentInParent<CompetitorScript>().redBonusCap;
				}
			}

			/*
			else if (collision.gameObject.tag == "Robot") {
				if (collision.gameObject.GetComponentInParent<CompetitorScript>().personalT > (this.gameObject.GetComponentInParent<CompetitorScript>().personalT +0.01f) && this.gameObject.GetComponentInParent<CompetitorScript>().shredding && collision.gameObject.GetComponentInParent<CompetitorScript>().dead <= 0) {
					collision.gameObject.GetComponentInParent<CompetitorScript>().dead = 1f;
					collision.gameObject.GetComponentInParent<CompetitorScript>().main.slowMo = 0.5f;
				}
				//this.GetComponentInParent<MovePlayer>().collidingWhite = true;
			}
			*/

		}
		
	}

	void checkTrigger(Collider collider) {

		if (this.gameObject.GetComponentInParent<CompetitorScript>() && this.gameObject.GetComponentInParent<CompetitorScript>().dead == 0f) {
			if (collider.gameObject.tag == "Blanco") {
				this.GetComponentInParent<CompetitorScript>().collidingWhite = true;
			}

			else if (collider.gameObject.tag == "Robot" && collider.gameObject.GetComponentInParent<CompetitorScript>().dead == 0f) {
				
				//Debug.Log("TRIGGER");
				
				if (collider.gameObject.GetComponentInParent<CompetitorScript>().personalT > (this.gameObject.GetComponentInParent<CompetitorScript>().personalT +0.01f) && this.gameObject.GetComponentInParent<CompetitorScript>().shredding && collider.gameObject.GetComponentInParent<CompetitorScript>().dead <= 0) {

					collider.gameObject.GetComponentInParent<CompetitorScript>().dead = 1f;
					if (this.transform.parent.tag == "Player" || collider.transform.parent.tag == "Player") {
						collider.gameObject.GetComponentInParent<CompetitorScript>().main.slowMo = 0.35f;
					}
					this.gameObject.GetComponentInParent<CompetitorScript>().currentRush = this.gameObject.GetComponentInParent<CompetitorScript>().defaultRush;
					this.gameObject.GetComponentInParent<CompetitorScript>().redBonusTotal = 3f;
					this.gameObject.GetComponentInParent<CompetitorScript>().turbo.Play();
				}
				//this.GetComponentInParent<MovePlayer>().collidingWhite = true;
			}
		}


	}


}
