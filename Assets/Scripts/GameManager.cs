using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    private int lives;
    private int score;
    private int level;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);//stays across levels
        NewGame();
    }
    private void NewGame()
    {
        lives = 3;
        score = 0;

        //Load level
        LoadLevel(1);
    }
    private void LoadLevel(int index)
    {
        level = index;
        Camera camera = Camera.main;
        if (camera != null)
        {
            camera.cullingMask = 0;
        }
        Invoke(nameof(LoadScene), 1f);

    }
    private void LoadScene()
    {
        SceneManager.LoadScene(level);
    }

    public void LevelComplete()
    {
        score += 1000;
        //Load next Level
        int nextlevel = level + 1;
        if (nextlevel < SceneManager.sceneCountInBuildSettings)
        {
            LoadLevel(nextlevel);
        }
        else
        {
            LoadLevel(1);
        }

    }
    public void LevelFailed()
    {
        lives--;
        if (lives <= 0)
        {
            NewGame();
        }
        else
        {
            //Reload
            LoadLevel(level);
        }

    }
}
