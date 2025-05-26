using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerBehavior : MonoBehaviour
{
    /// <summary>
    /// A reference to the Rigidbody component
    /// </summary> 
    private Rigidbody rb;

    [Tooltip("How fast the ball moves left/right")]
    public float dodgeSpeed = 5;

    [Tooltip("How fast the ball moves forward automatically")]
    [Range(0, 10)]
    public float rollSpeed = 5;

    public enum MobileHorizMovement
    {
        Accelerometer,
        ScreenTouch
    }
    [Tooltip("What horizontal movement type should be used")]
    public MobileHorizMovement horizMovement = MobileHorizMovement.Accelerometer;

    [Header("Swipe Properties")]
    [Tooltip("How far will the player move upon swiping")]
    public float swipeMove = 2f;
    [Tooltip("How far must the player swipe before we willexecute the action (in inches)")]
    private float minSwipeDistance = 0.25f;
    /// <summary>
    /// Used to hold the value that converts minSwipeDistance to pixels
    /// </summary>
    private float minSwipeDistancePixels;
    /// <summary>
    /// Stores the starting postion of of mobile touch event
    /// </summary>
    private Vector2 touchStart;

    [Header("Scaling Properties")]
    [Tooltip("The minimum size (in Unity units) that the player should be")]
    public float minScale = 0.5f;
    [Tooltip("The maximum size (in Unity units) that the player should be")]
    public float maxScale = 3.0f;
    /// <summary>
    /// The current scale of the player
    /// </summary>
    private float currentScale = 1;


    void Start()
    {
        // Get access to our Rigidbody component
        rb = GetComponent<Rigidbody>();
        minSwipeDistancePixels = minSwipeDistance * Screen.dpi;
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    private void Update()
    {
        /* Check if we are running either in the Unity
           editor or in a
         * standalone build.*/
#if UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_EDITOR
        /* If the mouse is tapped */
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 screenPos = new Vector2(
                Input.mousePosition.x,
                    Input.mousePosition.y);
            TouchObjects(screenPos);
        }
        /* Check if we are running on a mobile device */
#elif UNITY_IOS || UNITY_ANDROID
        /* Check if Input has registered more than
           zero touches */
        if (Input.touchCount > 0)
        {
            /* Store the first touch detected */
            Touch touch = Input.touches[0];
            TouchObjects(touch.position);
            SwipeTeleport(touch);
            ScalePlayer();
        }
#endif
    }

    /// <summary>
    /// FixedUpdate is a prime place to put physics
    /// calculations happening over a period of time.
    /// </summary>
    void FixedUpdate()
    {
        //Check if we're moving
        var horizontalSpeed = Input.GetAxis("Horizontal") * dodgeSpeed;

        //Check if running inUnity editor or stand alone build
#if UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_EDITOR
        //if mouse is held down/screen tap
        if (Input.GetMouseButton(0))
        {
            var screenPos = Input.mousePosition;
            horizontalSpeed = CalculateMovement(screenPos);
        }
        //check if running on mobile
#elif UNITY_IOS || UNITY_ANDROID
        switch (horizMovement)
        {
            case MobileHorizMovement.Accelerometer:
                //move based on accelerometer
                horizontalSpeed = Input.acceleration.x * dodgeSpeed;
                break;
            case MobileHorizMovement.ScreenTouch:
                //check for more than zero touches
                if (Input.touchCount > 0)
                {
                    //store first touch
                    var firstTouch = Input.touches[0];
                    var screenPos = firstTouch.position;
                    horizontalSpeed = CalculateMovement(screenPos);
                }
                break;
        }
#endif
        rb.AddForce(horizontalSpeed, 0, rollSpeed);
    }

    /// <summary>
    /// figure out where to move the player
    /// </summary>
    /// <param name="screenPos"> 
    /// The position the player has touched/clicked </param>
    /// <return> The direction to move in the X axis</return>
    private float CalculateMovement(Vector3 screenPos)
    {
        //get cam
        var cam = Camera.main;
        //convert mouse pos to a 0-1 range
        var viewPos = cam.ScreenToViewportPoint(screenPos);
        float xMove = 0;
        if(viewPos.x < 0.5f)
        {
            //move right
            xMove = -1;
        }
        else
        {
            //move left
            xMove = 1;
        }
        return xMove * dodgeSpeed;
    }

    /// <summary>
    /// will teleport the player left or right
    /// </summary>
    /// <param name="touch"> current touch event </param>
    private void SwipeTeleport(Touch touch)
    {
        //check if touch just started
        if(touch.phase == TouchPhase.Began)
        {
            //set touchStart
            touchStart = touch.position;
        }
        else if (touch.phase == TouchPhase.Ended)
        {
            //get pos of touch ended
            Vector2 touchEnd = touch.position;
            //calculate difference between beginning and end on x
            float x = touchEnd.x - touchStart.x;

            //if not far enough, dont teleport
            if(Mathf.Abs(x) < minSwipeDistancePixels)
            {
                return;
            }

            Vector3 moveDirection;
            if (x < 0)
            {
                //move left
                moveDirection = Vector3.left;
            }
            else
            {
                //move right
                moveDirection = Vector3.right;
            }

            RaycastHit hit;
            //only move if player wouldn't hit something
            if(!rb.SweepTest(moveDirection, out hit, swipeMove))
            {
                //move the player
                var movement = moveDirection * swipeMove;
                var newPos = rb.position + movement;
                rb.MovePosition(newPos);
            }
        }
    }

    /// <summary> 
    /// Will scale the player based on pinching
    /// </summary>
    private void ScalePlayer()
    {
        //must have two touches
        if(Input.touchCount != 2)
        {
            return;
        }
        else
        {
            //store touches
            Touch touch0 = Input.touches[0];
            Touch touch1 = Input.touches[1];
            Vector2 t0Pos = touch0.position;
            Vector2 t0Delta = touch0.deltaPosition;
            Vector2 t1Pos = touch1.position;
            Vector2 t1Delta = touch1.deltaPosition;

            //find the previous frame position of each touch
            Vector2 t0Prev = t0Pos - t0Delta;
            Vector2 t1Prev = t1Pos - t1Delta;

            //find distance between touches
            float prevTDeltaMag = (t0Prev - t1Prev).magnitude;
            float tDeltaMag = (t0Pos - t1Pos).magnitude;

            //find difference in the distances between frames
            float deltaMagDiff = prevTDeltaMag - tDeltaMag;

            //keep change constitent
            float newScale = currentScale;
            newScale -= (deltaMagDiff * Time.deltaTime);

            //ensure new value is valid
            newScale = Mathf.Clamp(newScale, minScale, maxScale);

            //update scale
            transform.localScale = Vector3.one * newScale;
            //set current scale for next frame
            currentScale = newScale;
        }
    }

    /// <summary>
    /// Will determine if we are touching a game object
    /// and if so call events for it
    /// </summary>
    /// <param name="screenPos">The position of the touch
    /// in screen space</param>
    private static void TouchObjects(Vector2 screenPos)
    {
        /* Convert the position into a ray */
        Ray touchRay = Camera.main.ScreenPointToRay(screenPos);
        RaycastHit hit;
        /* Create a LayerMask that will collide with all possible channels */
        int layerMask = ~0;
        /* Are we touching an object with a collider? */
        if (Physics.Raycast(touchRay, out hit, Mathf.Infinity, layerMask, QueryTriggerInteraction.Ignore))
        {
            /* Call the PlayerTouch function if it exists on a component attached to this object */
            hit.transform.SendMessage("PlayerTouch", SendMessageOptions.DontRequireReceiver);
        }
    }
    /// <summary>
    /// Will determine if we are touching a game object
    /// and if so call events for it
    /// </summary>
    /// <param name="touch">Our touch event</param> 
    private static void TouchObjects(Touch touch)
    {
        // Convert the position into a ray
        Ray touchRay =
        Camera.main.ScreenPointToRay(touch.position);
        RaycastHit hit;
        // Create a LayerMask that will collide with all
        // possible channels
        int layerMask = ~0;
        // Are we touching an object with a collider?
        if (Physics.Raycast(touchRay, out hit, Mathf.Infinity, layerMask, QueryTriggerInteraction.Ignore))
        {
            // Call the PlayerTouch function if it exists on a
            // component attached to this object
            hit.transform.SendMessage("PlayerTouch",
                SendMessageOptions.DontRequireReceiver);
        }
    }
}
