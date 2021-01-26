using UnityEngine;
using System.Collections;

public class Wiggle : MonoBehaviour {

	public float distance = .5f;
	public float speed = 0.5f;

	private Vector3 startPos, toPos;

	void randomToPos() {
		toPos = startPos;
		toPos.x += Random.Range(-distance, +distance);
		toPos.y += Random.Range(-distance, +distance);
	}

	// Use this for initialization
	void Start() {
		startPos = transform.position;
		InvokeRepeating("randomToPos", 0, .25f);
	}

	// Update is called once per frame
	void Update() {
		transform.position = Vector2.MoveTowards(transform.position, toPos, speed);
		//transform.position = Vector2.Lerp(transform.position, toPos, speed);
	}
}