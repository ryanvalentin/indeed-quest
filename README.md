# indeed-quest
 Indeed Quest Hackathon Summer 2020

## Dependencies

- Unity 2019.4.6f (LTS): https://unity3d.com/get-unity/download
- If you want to build the project, you'll also need to download the WebGL build tools (when installing Unity)

## Definitions

**Room** - This is a single Unity scene where the player can move about, e.g. the "Lunch Area" is one room.

**Portal** - These are links between rooms and describe to the game controller which scene to load next, and where to place the player. There's a portal on either end of the Room and they're linked by ID.

**Interactable** - This is a simple object that can be inspected. You can optionally pick up that object if allowed.

**Collectable** - This is an interactable that can also be picked up from the environment and placed into the player's inventory.

**NPC** - This is an extension of an interactable which allows for things like dialogue, quests, etc.

## Game Structure

The game is broken apart into different Rooms (represented as scenes). The GameController class handles the loading and unloading of these scenes and the objects within. Each Room scene can be loaded independently, but in order to function like a game, the main GameSceneContainer scene loaded ahead of time.

## Adding Objects

To add a new Interactable or NPC, you'll need a few things:

1. A short title
2. A slightly longer description
3. An icon - this should be a pixelated look and be at least 256x256 pixels in dimensions, and a power of 2 (512x, 1024x, etc.)

First you'll need to either create or pick an existing profile. A profile is a static data file which can be used for multiple unique objects. These live under *Assets/Data/Profiles/(NPCs/Interactables)*.

If you don't see a profile for a character/object you want, right click inside the correct folder and in the context menu, go to *Create -> Indeed -> Interactable Profile (or) NPC Profile*. This will create a file in the directory and on the right in the inspector, you'll be able to customize it.

In the *Assets/Prefabs* folder there's several prototypes you can use to build your object. Drag and drop one into the scene you want it in, and then you can assign the profile you created/selected in the inspector.

## Testing the Game

Open the main scene by double clicking *Assets/Scenes/MainMenuScene* to load the menu. Then click the "Play" button on the top. Now you'll be able to test the game from the beginning.
