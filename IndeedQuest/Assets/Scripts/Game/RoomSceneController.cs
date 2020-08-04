using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manager for a single room
/// </summary>
public class RoomSceneController : MonoBehaviour
{
    public static RoomSceneController Current;

    public RoomPortal[] Portals;

    // Start is called before the first frame update
    void Start()
    {
        Current = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        Current = null;
    }

    public Vector3 GetEntryPointForPortal(string portalId)
    {
        Vector3 pos = Vector3.zero;
        for (int i = 0; i < Portals.Length; i++)
        {
            if (Portals[i].Id == portalId)
            {
                pos = Portals[i].transform.position + Portals[i].StartOffset;

                break;
            }
        }

        return pos;
    }
}
