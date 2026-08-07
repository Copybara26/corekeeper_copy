using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    public string SceneName;


    public void Scene_Change()
    {
        SceneManager.LoadScene(SceneName);
    }
}
