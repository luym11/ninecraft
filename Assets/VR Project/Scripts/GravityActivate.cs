using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NewtonVR;

public class GravityActivate : MonoBehaviour {
	// use global variable, only use with objects that exist once
	public static GravityActivate Instance;

	private AudioSource audioSource;
	public AudioClip[] clips;
	private int currentClip = 0;

	GameObject[] allBricks; 
	public bool kinematicEnabled;
	void Awake(){
		kinematicEnabled = true; 
		Instance = this;
		audioSource = GetComponentInParent<AudioSource> ();
	}
	void Update(){
		if (NVRPlayer.Instance.LeftHand.HoldButtonDown) {

			kinematicEnabled = !kinematicEnabled;

			audioSource.Stop ();
			//play some music
			if (currentClip == 0) {
				audioSource.clip = clips [1];
				currentClip = 1;
			} else {
				audioSource.clip = clips [0];
				currentClip = 0;
			}
			audioSource.Play ();

			// Disable kinematic for all bricks
			Debug.Log("God says, let there be gravity. ");
			allBricks = GameObject.FindGameObjectsWithTag ("Brick");
			int brickNum; 
			brickNum = allBricks.Length;
		//	Debug.Log ("brickNum", brickNum);
			for (int i = 0; i < brickNum; i++) {
				GameObject o = allBricks [i];
				Rigidbody r = (Rigidbody) o.GetComponent<Rigidbody> ();
				r.isKinematic = false;

			}


		}
	}
}
