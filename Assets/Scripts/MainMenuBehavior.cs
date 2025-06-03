using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuBehavior : MonoBehaviour
{
    public TextMeshProUGUI highscoreText;
    public GameObject ScreenCanvas, PhysicalCanvas;

    private void Start()
    {
        highscoreText.text = $"High Score: {PlayerPrefs.GetInt("score")}";

        //slide in the menu
        SlideMenuIn(ScreenCanvas);
        SlideMenuIn(PhysicalCanvas);

        Time.timeScale = 1;
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

    /// <summary>
    /// will move an object from left to center
    /// </summary>
    /// <param name="obj"> UI element to move</param>
    public void SlideMenuIn(GameObject obj)
    {
        obj.SetActive(true);
        var rt = obj.GetComponent<RectTransform>();
        if (rt)
        {
            //set position offscreen
            var pos = rt.position;
            pos.x = -Screen.width / 2;
            rt.position = pos;

            //move to center
            var tween = LeanTween.moveX(rt, 0, 1.5f);
            tween.setEase(LeanTweenType.easeInOutExpo);
            tween.setIgnoreTimeScale(true);
        }
    }

    /// <summary>
    /// will move offscreen right
    /// </summary>
    /// <param name="obj"> UI element to move</param>
    public void SlideMenuOut(GameObject obj)
    {
        var rt = obj.GetComponent<RectTransform>();
        if (rt)
        {
            var tween = LeanTween.moveX(rt, Screen.width / 2, 0.5f);
            tween.setEase(LeanTweenType.easeOutQuad);
            tween.setIgnoreTimeScale(true);
            tween.setOnComplete(() =>
            {
                obj.SetActive(false);
            });
        }
    }
}
