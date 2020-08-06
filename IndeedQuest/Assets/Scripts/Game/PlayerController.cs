using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody jobbyBody;
    private float horizontalInput;
    private float verticalInput;

    [HideInInspector]
    public float speed;

    void Start()
    {
        jobbyBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
    }

    private void FixedUpdate()
    {
        walk();
    }

    void walk() 
    {
        // Locks player movement when we change scenes because the controller
        // will manually move the player into a position.
        if (GameController.Instance.IsTransitioning)
        {
            jobbyBody.isKinematic = true;
            return;
        }
        else if (jobbyBody.isKinematic)
        {
            jobbyBody.isKinematic = false;
        }
        
        Vector3 playerVel = new Vector3(horizontalInput * speed, 0, verticalInput * speed);
        jobbyBody.velocity = playerVel;
    }
}
