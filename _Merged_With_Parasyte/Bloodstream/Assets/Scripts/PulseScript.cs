using UnityEngine;
using System.Collections;

public class PulseScript : MonoBehaviour {

	VolumetricLines.VolumetricMultiLineBehavior multiLine;

	private GameObject aux;
	private float personalT = 0f;
	private float positionInCircle = 0f;

	// Use this for initialization
	void Start () {

		aux = new GameObject();
		multiLine = this.gameObject.GetComponent<VolumetricLines.VolumetricMultiLineBehavior> ();
	
	}
	
	// Update is called once per frame
	void Update () {

		//personalT += Time.deltaTime*2f;
		personalT = GameObject.Find ("CentralPointPlayer").GetComponent<CompetitorScript> ().personalT -0.5f;

		this.gameObject.transform.position = GameObject.Find ("CentralPointPlayer").transform.position + new Vector3(0, 0, 10f);
		
		for (int i = 0; i < multiLine.m_lineVertices.Length; i++) {
			
			float personalAngle = positionInCircle*360f;
			
			Vector3 personalPoint = GameObject.Find("Main").GetComponent<MainScript>().getPointBSpline(personalT + i/10f);
			aux.transform.position = personalPoint;
			Vector3 direction = GameObject.Find("Main").GetComponent<MainScript>().getPointBSpline(personalT + i/10f +0.0001f) -personalPoint;
			
			Vector3 personalOffset = aux.transform.up * Mathf.Sin(personalAngle*Mathf.Deg2Rad) * 18.0f + aux.transform.right * Mathf.Cos (personalAngle*Mathf.Deg2Rad) * 19.5f;
			
			multiLine.m_lineVertices[i] = personalPoint -this.gameObject.transform.position +personalOffset;
			//multiLine.m_lineVertices[i] = personalPoint +personalOffset;
		}
	
	}

	public void generatePulse(float initialT, float auxPositionInCircle, float delay) {



		if (!multiLine.m_dynamic) { 
			multiLine.m_dynamic = true; 
			for (int i = 0; i < multiLine.m_lineVertices.Length; i++) {

			}
		}

	
		//personalT = initialT;
		positionInCircle = auxPositionInCircle;



	}

}
