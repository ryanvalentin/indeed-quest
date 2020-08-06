using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody jobbyBody;
    private float horizontalInput;
    private float verticalInput;

    [HideInInspector]
    public float speed;

    /* status code 
        0: front
        1: back
        2: left
        3: right */
    public Animator playeranime;

    void Start()
    {
        jobbyBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        if (Input.GetKey(KeyCode.DownArrow)) 
        {
            playeranime.SetInteger("status", 0);
        }
        if (Input.GetKey(KeyCode.UpArrow)) 
        {
            playeranime.SetInteger("status", 1);
        }
        if (Input.GetKey(KeyCode.LeftArrow)) 
        {
            playeranime.SetInteger("status", 2);
        }
        if (Input.GetKey(KeyCode.RightArrow)) 
        {
            playeranime.SetInteger("status", 3);
        }

    }

    private void FixedUpdate()
    {
        Walk();
    }

    void Walk() 
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
