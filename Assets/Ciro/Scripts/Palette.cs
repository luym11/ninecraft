using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Palette : MonoBehaviour
{
	public GameObject[] prefabs;

	private GameObject[] spawnedIcons;
	private float distanceBetweenIcons = .2f;

	private void Awake ()
	{
		spawnedIcons = new GameObject[prefabs.Length];
	}

	void Start ()
	{
		for (int i = 0; i < prefabs.Length; i++) {
			CreateIcon (i);
		}
	}

	public void OnItemDetached (int itemId)
	{
		//remove listener from detached object
		spawnedIcons [itemId].GetComponent<PaletteItem> ().onPickedUp.RemoveListener (OnItemDetached);

		//create substitute object after a delay to avoid collisions
		StartCoroutine(CreateSubstituteIcon(itemId));

		//Debug.Log ("Item detached " + itemId);
	}

	private IEnumerator CreateSubstituteIcon(int itemId)
	{
		yield return new WaitForSeconds(1f);

		CreateIcon(itemId);
	}

	private void CreateIcon (int itemId)
	{
		spawnedIcons [itemId] = Instantiate<GameObject> (prefabs [itemId]); //spawn new object
		spawnedIcons [itemId].transform.SetParent (this.transform, false); //reparent it
		float x = (-distanceBetweenIcons * prefabs.Length * .5f) + distanceBetweenIcons * itemId + (distanceBetweenIcons * .5f); //calculate the X
		spawnedIcons [itemId].transform.localPosition = new Vector3 (x, .2f, .1f); //assign position
		spawnedIcons [itemId].transform.localScale = Vector3.one * .1f; //assign scale

		spawnedIcons [itemId].GetComponent<PaletteItem> ().Setup (itemId);
		spawnedIcons [itemId].GetComponent<PaletteItem> ().onPickedUp.AddListener (OnItemDetached); //assign listener
		//spawnedIcons[itemId].tag = "Brick"; 
	
	}


	public void SayHi()
	{
		Debug.Log ("hi");
	}
}
