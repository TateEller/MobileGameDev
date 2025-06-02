using UnityEngine;

public class UISafeAreaHandler : MonoBehaviour
{
    RectTransform panel;

    private void Start()
    {
        panel = GetComponent<RectTransform>();
    }

    private void Update()
    {
        Rect area = Screen.safeArea;

        //pixel size in screen space of the whole screen
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);

        //set anchors to percentages of the screen used
        panel.anchorMin = area.position / screenSize;
        panel.anchorMax = (area.position + area.size) / screenSize;
    }
}
