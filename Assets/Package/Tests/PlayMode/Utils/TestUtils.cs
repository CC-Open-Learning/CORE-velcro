using UnityEngine.SceneManagement;

public static class TestUtils
{
    /// <summary>
    /// This method unloads the previous test scene async and creates a new one for the next test
    /// </summary>
    /// <param name="sceneCounter">int to append on the end of the scene name to keep names unique</param>
    /// <param name="scenename">Name of the scene. This had to be added in order to keep all tests passing when ran together as scene unload is async</param>
    /// <returns></returns>
    public static int ClearScene(int sceneCounter, string scenename)
    {
        SceneManager.SetActiveScene(SceneManager.CreateScene(scenename + sceneCounter));

        if (sceneCounter > 0)
        {
            SceneManager.UnloadSceneAsync(scenename + (sceneCounter - 1));
        }

        return ++sceneCounter;
    }
}