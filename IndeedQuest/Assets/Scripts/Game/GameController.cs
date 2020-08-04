using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    private Scene _currentRoomScene;

    public static GameController Instance;

    public SceneReference MainMenuScene;

    public CanvasGroup FadeCanvas;

    public AudioMixer Mixer;

    public float FadeSeconds = 1f;

    public SceneReference[] InitialScenes;

    public PlayerController Player;

    public string LastPortalId
    {
        get;
        private set;
    }

    public bool IsTransitioning
    {
        get;
        private set;
    }

    public void OnPortalTrigger(RoomPortal portal)
    {
        LastPortalId = portal.Id;
        StartCoroutine(SwitchRoom(portal.Scene));
    }

    private void Start()
    {
        Instance = this;

        // Put up our black canvas.
        FadeCanvas.gameObject.SetActive(true);
        FadeCanvas.alpha = 1f;
        if (Mixer != default)
            Mixer.SetFloat("MasterVolume", 0f.ToNormalizedVolume());

        // Load our additional scenes.
        LoadInitialScenes();

        // Now reveal the scene.
        StartCoroutine(FadeScreen(FadeSeconds, 0f, FadeSeconds));
    }

    private void Update()
    {
        
    }

    private void LoadInitialScenes()
    {
        for (int i = 0; i < InitialScenes.Length; i++)
        {
            SceneManager.LoadScene(InitialScenes[i], LoadSceneMode.Additive);
        }
        _currentRoomScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
    }

    private IEnumerator SwitchRoom(SceneReference scene)
    {
        IsTransitioning = true;

        // Fade screen
        yield return FadeScreen(0.2f, 1f, 0f);

        // Unload current room
        var unload = SceneManager.UnloadSceneAsync(_currentRoomScene);

        // Wait for operations to complete
        while (!unload.isDone)
            yield return null;

        // Load new room additively
        SceneManager.LoadScene(scene, LoadSceneMode.Additive);

        _currentRoomScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);

        // Pause for a moment here because we have to wait for a cycle for the new scene to call Start() and
        // allow us to access the new RoomSceneContoller instance.
        while (RoomSceneController.Current == null)
            yield return null;

        // Move the player to the start position of that room
        Player.transform.position = RoomSceneController.Current.GetEntryPointForPortal(LastPortalId);

        // Reveal new scene
        yield return FadeScreen(0.2f, 0f, 0f);

        IsTransitioning = false;
    }

    private IEnumerator FadeScreen(float fadeTimeSeconds, float targetAlpha, float initialWait)
    {
        if (initialWait > 0f)
            yield return new WaitForSeconds(initialWait);

        float currentTime = 0f;
        float startAlpha = FadeCanvas.alpha;
        while (currentTime < fadeTimeSeconds)
        {
            currentTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, currentTime / fadeTimeSeconds);
            FadeCanvas.alpha = newAlpha;

            // Also fade audio (this is the inverted value).
            if (Mixer != default)
                Mixer.SetFloat("MasterVolume", (1f - newAlpha).ToNormalizedVolume());

            yield return null;
        }
    }
}
