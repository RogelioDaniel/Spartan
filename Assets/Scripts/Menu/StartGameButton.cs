using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGameButton : MonoBehaviour
{
    public string gameSceneName = "Principal";

    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }
}

