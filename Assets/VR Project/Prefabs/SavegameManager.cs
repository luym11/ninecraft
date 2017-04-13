using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavegameManager : MonoBehaviour
{
    public GameObject[] Prefabs;

    public void SaveGame()
    {
        GameObject[] bricks = GameObject.FindGameObjectsWithTag("Brick");

        SaveData data = new SaveData();
        data.Bricks = new BrickSaveData[bricks.Length];
        
        for( int i = 0; i < bricks.Length; i++ )
        {
            data.Bricks[i] = new BrickSaveData();
            data.Bricks[i].BrickPositionSave = bricks[i].transform.position;
			data.Bricks [i].BrickScaleSave = bricks [i].transform.localScale; 
			//
			data.Bricks [i].BrickRocationSave = bricks [i].transform.rotation; 
			data.Bricks [i].BrickTypeSave = (int)bricks [i].GetComponent<Brick> ().BrickTypeSave; 
			Rigidbody r1 = (Rigidbody)bricks [i].GetComponent<Rigidbody> (); 
			data.Bricks [i].IsKinematicSave = r1.isKinematic; 
        }

        string jsonData = JsonUtility.ToJson(data, true);

        PlayerPrefs.SetString("SavegameData", jsonData);

        
    }

    public void LoadGame()
    {
        string jsonData = PlayerPrefs.GetString("SavegameData");

        SaveData data = JsonUtility.FromJson<SaveData>(jsonData);

        for ( int i = 0; i < data.Bricks.Length; ++i )
        {
			GameObject newBrick = Instantiate(Prefabs[data.Bricks[i].BrickTypeSave]); 
			newBrick.transform.position = data.Bricks[i].BrickPositionSave;
			newBrick.transform.localScale = data.Bricks [i].BrickScaleSave; 
			//
			newBrick.transform.rotation = data.Bricks[i].BrickRocationSave;
			Rigidbody r2 = (Rigidbody) newBrick.GetComponent<Rigidbody> ();
			r2.isKinematic = data.Bricks [i].IsKinematicSave;
        }
    }
}
