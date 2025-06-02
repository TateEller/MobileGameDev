using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScreenBehavior : MainMenuBehavior
{
    /// <summary>
    /// If our game us currently paused
    /// </summary>
    public static bool paused;
    [Tooltip("Reference to the pause menu object to turn on/off")]
    public GameObject pauseMenu;
    [Tooltip("Reference to the on screen controls menu")]
    public GameObject onScreenControls;

    private void Start()
    {
        SetPauseMenu(false);
    }

    /// <summary>
    /// Reloads the current level, "restarting" the game
    /// </summary>
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Will turn our pause menu on or off
    /// </summary>
    /// <param name="isPaused"></param>
    public void SetPauseMenu(bool isPaused)
    {
        paused = isPaused;
        //if game is paues timeScale = 0, else = 1
        Time.timeScale = (paused) ? 0 : 1;
        pauseMenu.SetActive(paused);
        onScreenControls.SetActive(!paused);
    }
}