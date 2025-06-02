using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuBehavior : MonoBehaviour
{
    /// <summary>
    ///  Will load a new scene
    /// </summary>
    /// <param name="levelName"> the name of the level 
    /// </param>
    public void LoadLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }
}
