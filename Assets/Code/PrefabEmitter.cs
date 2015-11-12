using UnityEngine;
using System.Collections;

public class PrefabEmitter : MonoBehaviour {

    private GameObject root;
    public GameObject source;

	// Use this for initialization
    void Awake()
    {

        if (root == null)
        {
            for (GameObject p = this.transform.parent.gameObject; p != null; p = p.transform.parent.gameObject)
            {
                if (p.GetComponent<VisualCharacter>() != null)
                {
                    root = p;
                    break;
                }
            }
        }



        if (root.transform.localScale.x < 0)
        {
            this.gameObject.transform.eulerAngles = new Vector3(this.gameObject.transform.eulerAngles.x, 90f, this.gameObject.transform.eulerAngles.z);
        }
        else
        {
            this.gameObject.transform.eulerAngles = new Vector3(this.gameObject.transform.eulerAngles.x, 270f, this.gameObject.transform.eulerAngles.z);
        }

    }
	
	// Update is called once per frame
	void Update () {

        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[this.GetComponent<ParticleSystem>().particleCount];
	    this.GetComponent<ParticleSystem>().GetParticles(particles);


        if (particles != null && particles.Length > 0)
        {
            if (root.transform.localScale.x < 0)
            {
                source.transform.position = new Vector3(particles[0].position.x, particles[0].position.y, source.transform.position.z) + new Vector3(source.GetComponent<Renderer>().bounds.size.x / 2f, source.GetComponent<Renderer>().bounds.size.y / 2f, 0f);
            }
            else
            {
                source.transform.position = new Vector3(particles[0].position.x, particles[0].position.y, source.transform.position.z) + new Vector3(-source.GetComponent<Renderer>().bounds.size.x / 2f, source.GetComponent<Renderer>().bounds.size.y / 2f, 0f);
            }

            source.SetActive(true);
        }
        

	}
}
