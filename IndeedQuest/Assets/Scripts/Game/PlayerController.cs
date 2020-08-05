using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
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
        float moveHor = Input.GetAxis("Horizontal");
        float moveVer = Input.GetAxis("Vertical");
        Vector3 playerVel = new Vector3(moveHor * speed, 0, moveVer * speed);
        jobbyBody.velocity = playerVel;
    }
}
