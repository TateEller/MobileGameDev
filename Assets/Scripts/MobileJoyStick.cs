using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; //IDragHandler, IEndDragHandler

public class MobileJoyStick : MonoBehaviour, IDragHandler, IEndDragHandler
{
    /// <summary>
    /// Reference to this object's RectTransform
    /// </summary>
    RectTransform rt;

    /// <summary>
    /// Original position of the stick used to calculate the offset of movement
    /// </summary>
    Vector2 originalAnchored;

    /// <summary>
    /// get the value of the joystick in a -1 to 1 manner
    /// </summary>
    public Vector2 axisValue;

    private void Start()
    {
        rt = GetComponent<RectTransform>();
        originalAnchored = rt.anchoredPosition;
    }

    /// <summary> 
    /// will allow the user to move the joystick
    /// </summary>
    /// <param name="eventData"> info about the movement
    /// we are onyl using position</param>
    public void OnDrag(PointerEventData eventData)
    {
        //we use parent info since the joystick moves
        var parent = rt.parent.GetComponent<RectTransform>();
        var parentSize = parent.rect.size;
        var parentPoint = eventData.position - parentSize;

        //calculate the point relative to the parent's local space
        Vector2 localPoint = parent.InverseTransformPoint(parentPoint);

        //calc what new anchor should be
        Vector2 newAnchorPos = localPoint - originalAnchored;

        //prevent the stick moving too far
        newAnchorPos = Vector2.ClampMagnitude(newAnchorPos, parentSize.x / 2);
        rt.anchoredPosition = newAnchorPos;

        //update the axis value the the new pos
        axisValue = newAnchorPos / (parentSize.x / 2);
    }

    /// <summary>
    /// Will be called when the player lets go of the stick
    /// </summary>
    /// <param name="eventData"> info about the movement
    /// unused</param>
    public void OnEndDrag(PointerEventData eventData)
    {
        //reset stick pos
        rt.anchoredPosition = Vector3.zero;
        axisValue = Vector2.zero;
    }
}
