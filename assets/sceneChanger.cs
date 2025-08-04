using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public string BasicScene;

    public void ChangeScene()
    {
        SceneManager.LoadScene(BasicScene);
    }
}
