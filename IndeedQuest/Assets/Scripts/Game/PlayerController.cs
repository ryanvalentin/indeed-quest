using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private bool _groundedPlayer;
    private Vector3 _playerVelocity;
    private const float _jumpHeight = 1.0f;
    private const float _gravityValue = -9.81f;

    public CharacterController Character;

    [Range(1f, 5f)]
    public float MoveSpeed = 2f;    

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateInputs();
    }

    private void UpdateInputs()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (Time.timeScale == 0)
                GameController.Instance.ResumeGame();
            else
                GameController.Instance.PauseGame();

            return;
        }

        _groundedPlayer = Character.isGrounded || transform.position.y <= 0;
        if (_groundedPlayer && _playerVelocity.y < 0)
        {
            _playerVelocity.y = 0f;
        }

        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        Character.Move(move * Time.deltaTime * MoveSpeed);

        if (move != Vector3.zero)
        {
            gameObject.transform.forward = move;
        }

        Character.Move(_playerVelocity * Time.deltaTime);
    }
}
