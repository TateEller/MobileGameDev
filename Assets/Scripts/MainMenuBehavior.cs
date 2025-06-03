using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuBehavior : MonoBehaviour
{
    public TextMeshProUGUI highscoreText;

    private void Start()
    {
        highscoreText.text = $"High Score: {PlayerPrefs.GetInt("score").ToString()}";
    }

    /// <summary>
    ///  Will load a new scene
    /// </summary>
    /// <param name="levelName"> the name of the level 
    /// </param>
    public void LoadLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }

    //reset highscore
    public void ResetHighScore()
    {
        PlayerPrefs.DeleteAll();
        highscoreText.text = ("High Score: 0");
    }
}
