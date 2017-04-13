using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NewtonVR;
using UnityEngine.Events;
using System.Collections;

public class PaletteItem : NVRInteractableItem
{
	[System.Serializable]
	public class PickedUpEvent : UnityEvent<int>
	{

	};

	public PickedUpEvent onPickedUp;
	public int iconId;
	private int counter; 

	private bool stillAttached = true;

	//called by the Palette on creation
	public void Setup (int _id)
	{
		iconId = _id;
	}

	public override void BeginInteraction (NVRHand hand)
	{
		if (stillAttached) {
			//detached for the first time
			transform.SetParent (null, true);
			transform.localScale = Vector3.one * .2f;
			base.UpdateColliders ();

			//Palette will create another one
			onPickedUp.Invoke (iconId);

			stillAttached = false;
		}

		base.BeginInteraction (hand);
	}

	 
	public override void InteractingUpdate (NVRHand hand)
	{
		
		if (hand.Inputs [NVRButtons.Touchpad].IsPressed && counter >= 50) {
			if (hand.Inputs [NVRButtons.Touchpad].Axis.y > 0  && transform.localScale.x  <= 1.51f) {
				transform.localScale = transform.localScale * 3f; 
			}
			if (hand.Inputs [NVRButtons.Touchpad].Axis.y < 0 && transform.localScale.x  >= 0.51f) {
				transform.localScale = transform.localScale / 3f; 
			}
			// Debug.Log ("Press" + counter); 
			counter = 0;
		}

		counter++;
		// Debug.Log ("Normale" + counter + hand.Inputs [NVRButtons.Touchpad].IsPressed); 
	}

	public void DestroyBrick(){
		
		// do sth
		if(stillAttached == true){
			return; 
		}


		Destroy (gameObject);
			
	}
}
