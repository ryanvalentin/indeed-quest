using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float speed;
    private Rigidbody jobbyBody;
    // Start is called before the first frame update
    void Start()
    {
        jobbyBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        walk();
    }

    void walk() 
    {
        // Locks player movement when we change scenes because the controller
        // will manually move the player into a position.
        if (GameController.Instance.IsTransitioning)
            return;

        float moveHor = Input.GetAxis("Horizontal");
        float moveVer = Input.GetAxis("Vertical");
        Vector3 playerVel = new Vector3(moveHor * speed, 0, moveVer * speed);
        jobbyBody.velocity = playerVel;
    }
}
