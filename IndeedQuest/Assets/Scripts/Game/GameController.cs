﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    //
    // Static

    public static GameController Instance;

    //
    // Properties

    public GameplayProfile Profile;

    public SceneReference MainMenuScene;

    public CanvasGroup FadeCanvas;

    public DialoguePopup Dialogue;

    public SceneReference[] InitialScenes;

    public PlayerController Player;

    public Text TimeText;

    public Text ScoreText;

    public GameObject PauseMenu;

    //
    // Private variables

    private Scene _currentRoomScene;

    private Dictionary<string, List<NPCController>> _npcReferences;

    private Dictionary<string, RoomSceneController> _roomReferences;

    public string LastPortalId { get; private set; }

    public bool IsTransitioning { get; private set; }

    public float TimeSinceStart { get; private set; } = 0f;

    public int CurrentScore { get; private set; } = 0;

    public QuestProfile ActiveQuest { get; private set; }

    public bool GameHasStarted { get; private set; } = false;

    public bool IsPlayerInSameRoom(GameObject gameObject)
    {
        return gameObject.scene.name == _currentRoomScene.name;
    }

    public void OnPortalTrigger(RoomPortal portal)
    {
        LastPortalId = portal.Id;
        StartCoroutine(RunSwitchRoomRoutine(portal.Scene));
    }

    public void OnContributeToScore(int score)
    {
        CurrentScore += score;
    }

    public void OnStartQuest(QuestProfile quest)
    {
        ActiveQuest = quest;
    }

    public void OnCompleteQuest()
    {
        // TODO: Store the active quest somewhere for reference later?

        ActiveQuest = null;
    }

    public void ResumeGame()
    {
        PauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    public void PauseGame(bool showMenu = true)
    {
        if (showMenu)
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

    public void OnPopupTrigger(string title, string description, Sprite icon)
    {
        Dialogue.Show(title, description, icon);
    }

    public void OnQuestPopupTrigger(string title, string description, Sprite icon)
    {
        Dialogue.Show(title, description, icon, "Decline", "Accept");
    }

    /// <summary>
    /// Method to manage NPCs in the game, globally.
    /// </summary>
    /// <param name="controller"></param>
    public void RegisterNPC(NPCController controller)
    {
        var controllerGameObject = controller.gameObject;
        string sceneName = controllerGameObject.scene.name;
        if (!_npcReferences.ContainsKey(sceneName))
            _npcReferences.Add(sceneName, new List<NPCController>());
        
        _npcReferences[sceneName].Add(controller);
    }

    public void RegisterRoom(RoomSceneController controller)
    {
        var controllerGameObject = controller.gameObject;
        string sceneName = controllerGameObject.scene.name;
        if (!_roomReferences.ContainsKey(sceneName))
            _roomReferences.Add(sceneName, controller);
    }

    private void Start()
    {
        Instance = this;

        StartCoroutine(RunLoadStartRoutine());
    }

    private void Update()
    {
    }

    private void LateUpdate()
    {
        UpdateScore();
        UpdateGameClock();
    }

    /// <summary>
    /// Runs through all the NPCs and sets their quest data.
    /// </summary>
    private IEnumerator RunInitializeQuestTimelineRoutine()
    {
        // Wait a moment for all NPCs to be registered before we determine their quests.
        yield return new WaitForEndOfFrame();

        // TODO: Loop through all the NPCs and set their quest data.
    }

    private void ConfigurePlayer()
    {
        Player.speed = Profile.PlayerMoveSpeed;
    }

    private void UpdateScore()
    {
        ScoreText.text = $"{CurrentScore:N0} people we've helped get jobs";
    }

    private void UpdateGameClock()
    {
        const float timeBase = 32400f; // 9:00 in seconds
        const float eightHours = timeBase + 28800f; // 8 hours in seconds

        float gameTime = timeBase;
        if (GameHasStarted)
        {
            // Only start ticking time when the player has started the game.

            TimeSinceStart += Time.deltaTime;

            gameTime += (TimeSinceStart * Profile.TimeScale);

            if (gameTime > eightHours)
                EndGame();
        }

        TimeSpan ts = TimeSpan.FromSeconds(gameTime);
        TimeText.text = $"{new DateTime(2020, 01, 01, ts.Hours, ts.Minutes, ts.Seconds):hh:mm tt}";
    }

    private void EndGame()
    {
        PauseGame(showMenu: false);

        string gameOverText = Profile.DefaultGameOverText;
        // TODO: Analyze profile to determine how the game should end.
        for (int i = 0; i < Profile.EndResults.Length; i++)
        {
            var result = Profile.EndResults[i];

            if (CurrentScore.CompareValues(result.ScoreCondition.Comparison, result.ScoreCondition.Value))
            {
                gameOverText = result.GameOverText;
                break;
            }
        }

        // Subscribe to when the player clicks the close button to go back to the main menu.
        Dialogue.OnSecondaryClick.AddListener(ReturnToMenu);

        // Show end dialogue.
        OnPopupTrigger(Profile.PlayerName, gameOverText, Profile.PlayerAvatar);
    }

    private IEnumerator RunLoadStartRoutine()
    {
        _npcReferences = new Dictionary<string, List<NPCController>>();
        _roomReferences = new Dictionary<string, RoomSceneController>();

        // Put up our black canvas.
        FadeCanvas.gameObject.SetActive(true);
        FadeCanvas.alpha = 1f;
        if (Profile.Mixer != default)
            Profile.Mixer.SetFloat("MasterVolume", 0f.ToNormalizedVolume());

        // Make sure dialogues hidden because we'll show instructions first.
        PauseMenu.SetActive(false);
        Dialogue.Hide();

        // Load initial scenes
        for (int i = 0; i < InitialScenes.Length; i++)
        {
            var task = SceneManager.LoadSceneAsync(InitialScenes[i], LoadSceneMode.Additive);

            while (!task.isDone)
                yield return null;

            var loadedScene = SceneManager.GetSceneByPath(InitialScenes[i]);
            if (i > 0)
            {
                // Hide renderers except for the initial scene
                ToggleSceneRenderers(loadedScene.name, visible: false);
            }
            else
            {
                _currentRoomScene = loadedScene;
            }
        }

        ConfigurePlayer();

        // Start game loop initializations.
        yield return RunInitializeQuestTimelineRoutine();

        // Now reveal the scene.
        yield return RunFadeScreenRoutine(Profile.GameFadeTimeSeconds, 0f, Profile.GameFadeTimeSeconds);

        // Lastly show the instruction
        OnPopupTrigger(Profile.PlayerName, Profile.IntroductionText, Profile.PlayerAvatar);

        GameHasStarted = true;
    }

    private void ToggleSceneRenderers(string name, bool visible)
    {
        for (int i = 0; i < _roomReferences[name].Renderers.Length; i++)
        {
            _roomReferences[name].Renderers[i].enabled = visible;
        } 
    }

    private IEnumerator RunReturnToMenuRoutine()
    {
        FadeCanvas.gameObject.SetActive(true);
        FadeCanvas.alpha = 0f;

        yield return RunFadeScreenRoutine(1f, 1f, 0f);

        Time.timeScale = 1f;

        // Replace everything with menu screen.
        SceneManager.LoadScene(MainMenuScene, LoadSceneMode.Single);
    }

    private IEnumerator RunSwitchRoomRoutine(SceneReference scene, bool fadeTransition = true)
    {
        IsTransitioning = true;

        // Fade screen
        if (fadeTransition)
            yield return RunFadeScreenRoutine(Profile.SceneTransitionTimeSeconds, 1f, 0f);

        // Hide current renderers
        ToggleSceneRenderers(_currentRoomScene.name, visible: false);

        _currentRoomScene = SceneManager.GetSceneByPath(scene);

        // Show new renderers
        ToggleSceneRenderers(_currentRoomScene.name, visible: true);

        // Move the player to the start position of that room
        Player.transform.position = _roomReferences[_currentRoomScene.name].GetEntryPointForPortal(LastPortalId);

        // Reveal new scene
        if (fadeTransition)
            yield return RunFadeScreenRoutine(Profile.SceneTransitionTimeSeconds, 0f, 0f);

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
            if (Profile.Mixer != default)
                Profile.Mixer.SetFloat("MasterVolume", (1f - newAlpha).ToNormalizedVolume());

            yield return null;
        }

        FadeCanvas.gameObject.SetActive(FadeCanvas.alpha > 0);
    }
}
