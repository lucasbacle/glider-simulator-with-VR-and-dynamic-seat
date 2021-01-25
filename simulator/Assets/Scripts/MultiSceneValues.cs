using UnityEngine.SceneManagement;

/* simple static class to store values across scenes*/
static class MultiSceneValues
{
	public static int worldHeight = 0;
	public static Scene lastScene;
}