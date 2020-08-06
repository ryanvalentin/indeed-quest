using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody jobbyBody;
    private float horizontalInput;
    private float verticalInput;

    private bool isWalking = false;

    [HideInInspector]
    public float speed;

    /* status code 
        1: front
        2: back
        3: left
        4: right */
    public Animator playeranime;

    void Start()
    {
        jobbyBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        isWalking = false;
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        if (Input.GetKey(KeyCode.DownArrow)) 
        {
            isWalking = true;
            playeranime.SetInteger("status", -1);
        }
        if (Input.GetKey(KeyCode.UpArrow)) 
        {
            isWalking = true;
            playeranime.SetInteger("status", -2);
        }
        if (Input.GetKey(KeyCode.LeftArrow)) 
        {
            isWalking = true;
            playeranime.SetInteger("status", -3);
        }
        if (Input.GetKey(KeyCode.RightArrow)) 
        {
            isWalking = true;
            playeranime.SetInteger("status", -4);
        }
        if (!isWalking) {
            int prev_status = playeranime.GetInteger("status");
            if (prev_status < 0) 
            {
                playeranime.SetInteger("status", -prev_status);
            }
            else 
            {
                playeranime.SetInteger("status", prev_status);
            }
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
