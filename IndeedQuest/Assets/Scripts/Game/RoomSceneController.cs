using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Manager for a single room
/// </summary>
public class RoomSceneController : MonoBehaviour
{
    public RoomPortal[] Portals;

    public Renderer[] Renderers;

    // Start is called before the first frame update
    private void Start()
    {
        List<Renderer> renderers = new List<Renderer>();
        var rootObjects = gameObject.scene.GetRootGameObjects();
        for (int i = 0; i < rootObjects.Length; i++)
        {
            renderers.AddRange(rootObjects[i].GetComponentsInChildren<Renderer>());
        }
        Renderers = renderers.ToArray();
        GameController.Instance.RegisterRoom(this);
    }

    // Update is called once per frame
    void Update()
    {
        
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

        Debug.Log($"Portal position: {pos} from {portalId} in scene {gameObject.scene.name}");

        return pos;
    }
}
