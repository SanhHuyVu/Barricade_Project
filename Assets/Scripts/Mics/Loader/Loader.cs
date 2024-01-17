using UnityEngine.SceneManagement;

public static class Loader
{
    public enum Scene { GamePlay, MainMenuScene, LoadingScene }
    private static Scene targetScene;

    public static void Load(Scene targetScene)
    {
        Loader.targetScene = targetScene;

        SceneManager.LoadSceneAsync(Scene.LoadingScene.ToString());
    }

    public static void LoaderCallback()
    {
        SceneManager.LoadSceneAsync(targetScene.ToString());
    }
}
