using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public SceneReference MainMenuScene;

    public RoomScene[] Rooms;

    /// <summary>
    /// Message handler for changing player room
    /// </summary>
    /// <param name="room">Name of the room - should match the name established in the <see cref="RoomScene"/> in this game manager.</param>
    public void OnChangeScene(string room)
    {
        for (int i = 0; i < Rooms.Length; i++)
        {
            if (room == Rooms[i].Name)
            {
                SwitchRoom(Rooms[i]);
                break;
            }
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    private void LoadScenes()
    {

    }

    private void SwitchRoom(RoomScene room)
    {
        // TODO: Find
    }

    [System.Serializable]
    public class RoomScene
    {
        public string Name;

        public SceneReference Scene;
    }
}
