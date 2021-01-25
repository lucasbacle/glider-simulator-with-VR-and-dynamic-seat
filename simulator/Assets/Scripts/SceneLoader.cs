using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
    This class allows to load the procedural Flight Scene with different parameters
 */
public class SceneLoader : MonoBehaviour
{

	public GameObject worldMenu;
	public GameObject mainMenu;

	public void Update()
	{
		
	}

	public void loadOcean() {

		MultiSceneValues.worldHeight = 100;

		MultiSceneValues.lastScene = SceneManager.GetActiveScene();
		SceneManager.LoadScene("proceduralFlight");
	}

	public void loadPlains() {

		MultiSceneValues.worldHeight = 200;

		MultiSceneValues.lastScene = SceneManager.GetActiveScene();
		SceneManager.LoadScene("proceduralFlight");
	}
	public void loadMountains()
	{
		MultiSceneValues.worldHeight = 600;

		MultiSceneValues.lastScene = SceneManager.GetActiveScene();
		SceneManager.LoadScene("proceduralFlight");
	}
}
