﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawner : MonoBehaviour {

	public GameObject main;
	public GameObject spawnSource;
	public List<GameObject> objects = new List<GameObject>();
	public int current = 0;

	public GameObject particleSource;
	public List<GameObject> particles = new List<GameObject>();
	public int current_particle;

	float circle = 0;

	public float defaultCooldown = 0.1f;
	public float cooldown = 0.1f;
	public bool circularPattern = false;

	public float lastSpawnedT = 0f;

	// Use this for initialization
	void Start () {
		/*
		GameObject aux;

		for (int i = 0; i < 50; i++) {
			Vector3 previousPosition = this.transform.position;
			this.transform.position = new Vector3(0, 0, 0);
			aux = Instantiate (spawnSource);
			this.transform.position = previousPosition;
			particles.Add(aux);
		}
		*/
	
	}
	
	// Update is called once per frame
	void Update () {

		circle += Time.deltaTime * 360f;
		if (circle > 360) { circle -= 360; }

		if (this.gameObject.tag == "SpawnerBlanco") {
			for (int i = 0; i < objects.Count; i++) {

				float aux_circle = circle + Mathf.PerlinNoise(0.71f*i, 0.42f*i)*360f;
				float aux_circle2 = circle + Mathf.PerlinNoise(0.21f*i, 0.12f*i)*360f;
				float aux_circle3 = circle + Mathf.PerlinNoise(0.81f*i, 0.32f*i)*360f;

				objects[i].transform.FindChild("Nucleus").GetComponent<Renderer>().material.SetFloat("_Amount", Mathf.Sin(aux_circle*Mathf.Deg2Rad)*(0.5f));		
				objects[i].transform.FindChild("Nucleus2").GetComponent<Renderer>().material.SetFloat("_Amount", Mathf.Cos(aux_circle2*Mathf.Deg2Rad)*(0.5f));
				objects[i].transform.FindChild("Nucleus3").GetComponent<Renderer>().material.SetFloat("_Amount", Mathf.Cos(aux_circle3*Mathf.Deg2Rad)*(0.5f));

				objects[i].transform.FindChild("Nucleus").localEulerAngles = new Vector3 (Mathf.PerlinNoise(0.33f*i, 0.15f*i)*360f -circle, Mathf.PerlinNoise(0.42f*i, 0.85f*i)*360f -circle, Mathf.PerlinNoise(0.67f*i, 0.21f*i)*360f -circle);
				objects[i].transform.FindChild("Nucleus2").localEulerAngles = new Vector3 (Mathf.PerlinNoise(1.37f*i, 3.15f*i)*360f + circle, Mathf.PerlinNoise(0.59f*i, 0.18f*i)*360f + circle, Mathf.PerlinNoise(0.82f*i, 0.76f*i)*360f + circle);
				objects[i].transform.FindChild("Nucleus2").localEulerAngles = new Vector3 (Mathf.PerlinNoise(1.17f*i, 2.41f*i)*360f + circle, Mathf.PerlinNoise(0.50f*i, 0.37f*i)*360f + circle, Mathf.PerlinNoise(0.33f*i, 0.62f*i)*360f + circle);

			}
		}

		if (this.gameObject.tag == "SpawnerRojo") {
			for (int i = 0; i < objects.Count; i++) {
				if (objects[i].gameObject.transform.GetComponent<Renderer>().material.GetFloat("_Amount") > 0f) {
					float maximum = 7f;
					float newAmount = Mathf.Lerp(objects[i].gameObject.transform.GetComponent<Renderer>().material.GetFloat("_Amount"), maximum, Time.deltaTime*2f);
					objects[i].transform.GetComponent<Renderer>().material.SetFloat("_Amount", newAmount);
					objects[i].transform.GetComponent<Renderer>().material.SetFloat("_Alpha", 1f - (objects[i].gameObject.transform.GetComponent<Renderer>().material.GetFloat("_Amount")/maximum));
				
				
					if (objects[i].gameObject.transform.GetComponent<Renderer>().material.GetFloat("_Amount") < 3f) {
						objects[i].transform.GetComponent<ParticleSystem>().enableEmission=true;
					}
					else {
						objects[i].transform.GetComponent<ParticleSystem>().enableEmission=false;
					}

				}
				
			}
		}







		this.transform.position = main.GetComponent<MainScript> ().getPointBSpline (main.GetComponent<MainScript> ().player.GetComponentInParent<CompetitorScript>().personalT + 2.0f);
		Vector3 direction = main.GetComponent<MainScript> ().getPointBSpline (main.GetComponent<MainScript> ().player.GetComponentInParent<CompetitorScript>().personalT + 2.0001f) - this.transform.position;
		this.transform.LookAt (direction);

		if (circularPattern) {
			this.transform.localPosition += -this.transform.right *10f*Mathf.Sin (Mathf.Deg2Rad * circle);
			this.transform.localPosition += this.transform.up * 10f*Mathf.Cos (Mathf.Deg2Rad * circle);
		}

		Vector3 auxPosition = new Vector3 (Random.Range (-10f, 10f), Random.Range (-10f, 10f), 0f);
		auxPosition.Normalize ();
		this.transform.localPosition += -this.transform.right *auxPosition.x*10f;
		this.transform.localPosition += this.transform.up *auxPosition.y*10f;


		if (cooldown > 0 && !main.GetComponent<MainScript>().menu && main.GetComponent<MainScript>().player.GetComponentInParent<CompetitorScript>().personalT < main.GetComponent<MainScript>().endingT + 1f) {
			cooldown -= Time.deltaTime;
		}

		if (cooldown <= 0 && lastSpawnedT < main.GetComponent<MainScript> ().player.GetComponentInParent<CompetitorScript>().personalT +1.9f) {
			cooldown = defaultCooldown;
			spawn ();
			lastSpawnedT = main.GetComponent<MainScript> ().player.GetComponentInParent<CompetitorScript>().personalT +2.0f;
		}

	}
	

	public void spawn() {

		GameObject aux;

		if (objects.Count < 50) {
			Vector3 previousPosition = this.transform.position;
			this.transform.position = new Vector3(0, 0, 0);
			aux = Instantiate (spawnSource);
			this.transform.position = previousPosition;
			objects.Add(aux);

			if (aux.tag == "Blanco") {
				// DESACTIVAR COLISIONES CON LOS COMPETIDORES
				for (int i = 0; i < main.GetComponent<MainScript>().competitors.Count; i++) {

					Collider colliderB = null;
					if (main.GetComponent<MainScript>().competitors[i].GetComponent<Collider>()) {
						colliderB = main.GetComponent<MainScript>().competitors[i].GetComponent<Collider>();
					}
					else {
						colliderB = main.GetComponent<MainScript>().competitors[i].transform.FindChild("Robot").GetComponent<Collider>();
					}

					Physics.IgnoreCollision(aux.GetComponent<Collider>(), colliderB);
				}

				aux.GetComponent<Rigidbody>().angularVelocity = new Vector3 (Random.Range(-1f,1f),Random.Range(-1f,1f),Random.Range(-1f,1f));

				main.GetComponent<MainScript>().whites.Add(aux);

			}
			else if (aux.tag == "Rojo") {
				main.GetComponent<MainScript>().reds.Add(aux);
				aux.transform.eulerAngles=new Vector3 (Random.Range(0f,360f),Random.Range(0f,360f),Random.Range(0f,360f));
				aux.transform.GetComponent<Rigidbody>().angularVelocity = new Vector3 (Random.Range(-5f,5f),Random.Range(-5f,5f),Random.Range(-5f,5f));
				aux.gameObject.transform.GetComponent<Rigidbody>().isKinematic = false;
				aux.transform.GetComponent<Renderer>().material.SetFloat("_Alpha", 1f);
			}

		} else {
			aux = objects[current];
			aux.transform.GetComponent<Renderer>().material.SetFloat("_Amount", 0f);
			aux.transform.eulerAngles=new Vector3 (Random.Range(0f,360f),Random.Range(0f,360f),Random.Range(0f,360f));
			if (aux.gameObject.tag == "Rojo") {
				aux.gameObject.transform.GetComponent<Rigidbody>().isKinematic = false;
				aux.transform.GetComponent<Renderer>().material.SetFloat("_Alpha", 1f);
			}
			aux.transform.GetComponent<Rigidbody>().angularVelocity = new Vector3 (Random.Range(-5f,5f),Random.Range(-5f,5f),Random.Range(-5f,5f));

			current++;
			if (current == objects.Count) { current = 0; }
		}

		aux.transform.GetComponent<Rigidbody> ().velocity = new Vector3 (0, 0, 0);
		aux.transform.localPosition = new Vector3 (0, 0, 0);
		aux.transform.rotation = this.transform.rotation;
		aux.transform.position = this.transform.position;

	}

	public void releaseParticles(Vector3 location) {

		for (int i = 0; i < 10; i++) {

			particles[current_particle].transform.position = location;
			particles[current_particle].transform.GetComponent<Rigidbody>().velocity = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), -1f);

			current_particle++;
			if (current_particle >= particles.Count) {
				current_particle = 0;
			}
		}

	}

}