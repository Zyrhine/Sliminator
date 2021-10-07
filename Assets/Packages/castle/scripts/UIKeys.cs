using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class UIKeys : MonoBehaviour {

	public Light sunlight;

	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.F)) {

			if (sunlight.enabled)
				sunlight.enabled = false;
			else
				sunlight.enabled = true;
		}
		if (Input.GetKeyDown(KeyCode.Alpha1)) {

			SceneManager.LoadScene ("scene_sunset");
		}
		if (Input.GetKeyDown(KeyCode.Alpha2)) {

			SceneManager.LoadScene ("scene_winter");
		}

		if (Input.GetKeyDown(KeyCode.Alpha3)) {

			SceneManager.LoadScene ("scene_candle_room");
		} 
	}
}
