using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    private Scene _currentRoomScene;

    private Dictionary<string, List<NPCController>> _npcReferences;

    private float _timeSinceStart = 0f;

    public static GameController Instance;

    public SceneReference MainMenuScene;

    public CanvasGroup FadeCanvas;

    public AudioMixer Mixer;

    public float FadeSeconds = 1f;

    public SceneReference[] InitialScenes;

    public PlayerController Player;

    public Text TimeText;

    public Text ScoreText;

    public GameObject PauseMenu;

    [Tooltip("The multiplier applied to each second to represent time in-game. e.g. a value of 60 would mean 1 second = 1 minute.")]
    [Min(1f)]
    public float TimeScale = 60f;

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
        StartCoroutine(RunSwitchRoomRoutine(portal.Scene));
    }

    public void ResumeGame()
    {
        PauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    public void PauseGame()
    {
        PauseMenu.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ReturnToMenu()
    {
        StartCoroutine(RunReturnToMenuRoutine());
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// Method to manage NPCs in the game, globally.
    /// </summary>
    /// <param name="controller"></param>
    public void RegisterNPC(NPCController controller)
    {
        var controllerGameObject = controller.gameObject;
        string sceneName = controllerGameObject.scene.name;
        DontDestroyOnLoad(controllerGameObject);

        if (!_npcReferences.ContainsKey(sceneName))
            _npcReferences.Add(sceneName, new List<NPCController>());

        _npcReferences[sceneName].Add(controller);
    }

    private void Start()
    {
        Instance = this;

        _npcReferences = new Dictionary<string, List<NPCController>>();

        // Put up our black canvas.
        FadeCanvas.gameObject.SetActive(true);
        FadeCanvas.alpha = 1f;
        if (Mixer != default)
            Mixer.SetFloat("MasterVolume", 0f.ToNormalizedVolume());

        // Load our additional scenes.
        LoadInitialScenes();

        // Make sure Pause Menu is hidden because we'll show instructions first.
        PauseMenu.SetActive(false);

        // Now reveal the scene.
        StartCoroutine(RunFadeScreenRoutine(FadeSeconds, 0f, FadeSeconds));
    }

    private void Update()
    {
        UpdateGameClock();
    }

    private void UpdateGameClock()
    {
        const float timeBase = 32400f; // 9:00 in seconds
        const float eightHours = timeBase + 28800f; // 8 hours in seconds
        _timeSinceStart += Time.deltaTime;

        float gameTime = timeBase + (_timeSinceStart * TimeScale);

        if (gameTime > eightHours)
            EndGame();

        TimeSpan ts = TimeSpan.FromSeconds(gameTime);
        TimeText.text = $"{new DateTime(2020, 01, 01, ts.Hours, ts.Minutes, ts.Seconds):hh:mm tt}";
    }

    private void EndGame()
    {
        PauseGame();
    }

    private void LoadInitialScenes()
    {
        for (int i = 0; i < InitialScenes.Length; i++)
        {
            SceneManager.LoadScene(InitialScenes[i], LoadSceneMode.Additive);
        }
        _currentRoomScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
    }

    private IEnumerator RunReturnToMenuRoutine()
    {
        FadeCanvas.gameObject.SetActive(true);
        FadeCanvas.alpha = 0f;

        yield return RunFadeScreenRoutine(1f, 1f, 0f);

        // Destroy all NPCs
        foreach (var npc in _npcReferences.Values)
        {
            for (int i = 0; i < npc.Count; i++)
            {
                Destroy(npc[i]);
            }
        }

        Time.timeScale = 1f;

        // Replace everything with menu screen.
        SceneManager.LoadScene(MainMenuScene, LoadSceneMode.Single);
    }

    private IEnumerator RunSwitchRoomRoutine(SceneReference scene)
    {
        IsTransitioning = true;

        // Fade screen
        yield return RunFadeScreenRoutine(0.2f, 1f, 0f);

        // Hide current NPCs
        if (_npcReferences.ContainsKey(_currentRoomScene.name))
        {
            foreach (var npc in _npcReferences[_currentRoomScene.name])
            {
                npc.Hide();
            }
        }

        // Unload current room
        var unload = SceneManager.UnloadSceneAsync(_currentRoomScene);

        // Wait for operations to complete
        while (!unload.isDone)
            yield return null;

        // Load new room additively
        SceneManager.LoadScene(scene, LoadSceneMode.Additive);

        _currentRoomScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);

        // Show new NPCs
        if (_npcReferences.ContainsKey(_currentRoomScene.name))
        {
            foreach (var npc in _npcReferences[_currentRoomScene.name])
            {
                npc.Show();
            }
        }

        // Pause for a moment here because we have to wait for a cycle for the new scene to call Start() and
        // allow us to access the new RoomSceneContoller instance.
        while (RoomSceneController.Current == null)
            yield return null;

        // Move the player to the start position of that room
        Player.transform.position = RoomSceneController.Current.GetEntryPointForPortal(LastPortalId);

        // Reveal new scene
        yield return RunFadeScreenRoutine(0.2f, 0f, 0f);

        IsTransitioning = false;
    }

    private IEnumerator RunFadeScreenRoutine(float fadeTimeSeconds, float targetAlpha, float initialWait)
    {
        if (initialWait > 0f)
            yield return new WaitForSeconds(initialWait);

        float currentTime = 0f;
        float startAlpha = FadeCanvas.alpha;
        while (currentTime < fadeTimeSeconds)
        {
            currentTime += Time.unscaledDeltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, currentTime / fadeTimeSeconds);
            FadeCanvas.alpha = newAlpha;

            // Also fade audio (this is the inverted value).
            if (Mixer != default)
                Mixer.SetFloat("MasterVolume", (1f - newAlpha).ToNormalizedVolume());

            yield return null;
        }

        FadeCanvas.gameObject.SetActive(FadeCanvas.alpha > 0);
    }
}
