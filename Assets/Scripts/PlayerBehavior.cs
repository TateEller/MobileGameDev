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

    // Start is called before the first frame update
    void Start()
    {
        // Get access to our Rigidbody component
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// FixedUpdate is a prime place to put physics
    /// calculations happening over a period of time.
    /// </summary>
    void FixedUpdate()
    {
        // Check if we're moving to the side
        var horizontalSpeed = Input.GetAxis("Horizontal") * dodgeSpeed;
        rb.AddForce(horizontalSpeed, 0, rollSpeed);
    }
}
