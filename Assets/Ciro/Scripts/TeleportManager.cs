using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NewtonVR;

public class TeleportManager : MonoBehaviour
{
	public GameObject tpSpherePrefab;

	private List<GameObject> tpSpheres;
	private int spheresNumber = 50;
	private bool collisionFound = false;
	private Vector3 teleportPosition;

	Palette m_Palette; 

	private void Awake()
	{
		//setup all spheres and disable their renderer
		tpSpheres = new List<GameObject> ();
		for (int i = 0; i < spheresNumber; i++)
		{
			tpSpheres.Add(Instantiate<GameObject>(tpSpherePrefab));
			tpSpheres [i].transform.SetParent (transform);
			tpSpheres [i].GetComponent<Renderer> ().enabled = false;
		}
		m_Palette = GetComponent<Palette> (); 
	}

	void Update ()
	{
		if (NVRPlayer.Instance.LeftHand.UseButtonPressed)
		{
			

			Transform handTransform = NewtonVR.NVRPlayer.Instance.LeftHand.transform;

			collisionFound = false;
			Vector3 nextPosition = handTransform.position + handTransform.forward * .1f;
			float gravity = .3f;
			for(int i = 0; i<spheresNumber; i++)
			{
				nextPosition += (handTransform.forward + (Vector3.down * gravity)) * .1f;

				tpSpheres [i].transform.position = nextPosition;

				tpSpheres [i].GetComponent<Renderer> ().enabled = !collisionFound;

				//check if the current sphere position is colliding with anything
				if (!collisionFound
					&& Physics.CheckSphere (nextPosition, .1f))
				{
					collisionFound = true;

					Vector3 playerOffsetFromCenter = NVRPlayer.Instance.transform.position - NVRPlayer.Instance.Head.transform.position;
					playerOffsetFromCenter.y = 0;

					teleportPosition = nextPosition + playerOffsetFromCenter;



					//	teleportPosition.y = 0f;
					//if (teleportPosition.y < 0.01f)
					//	teleportPosition.y = 0.01f;
				}

				gravity += .05f;
			}
		}

		if (NVRPlayer.Instance.LeftHand.UseButtonUp)
		{
			
			DeactivateSpheres();

			if (collisionFound) {
				//teleport
				NVRPlayer.Instance.transform.position = teleportPosition;
				//m_Palette.transform.Translate (teleportPosition - NVRPlayer.Instance.transform.position ); 
				//m_Palette.transform.TransformVector (teleportPosition); 
			}
		}
	}

	private void DeactivateSpheres()
	{
		for (int i = 0; i < spheresNumber; i++)
		{
			tpSpheres [i].GetComponent<Renderer> ().enabled = false;
		}
	}
}
