using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A behaviour that specifies a relationship between two rooms
/// </summary>
public class RoomPortal : MonoBehaviour
{
    public string Id = Guid.NewGuid().ToString();

    public SceneReference Scene;

    public Vector3 StartOffset = Vector3.forward;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (GameController.Instance.IsTransitioning)
            return;

        // Transition to the next scene.
        if (other.CompareTag("Player") && GameController.Instance.IsPlayerInSameRoom(gameObject))
            GameController.Instance.OnPortalTrigger(this);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position + StartOffset, Vector3.one * 0.1f);
    }
}
