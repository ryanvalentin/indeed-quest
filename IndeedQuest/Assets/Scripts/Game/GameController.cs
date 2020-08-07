using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    //
    // Static

    public static GameController Instance;

    public const float DAY_LENGTH = 28800f;
    public const float START_TIME = 32400f; // 9:00 in seconds
    public const float END_TIME = START_TIME + DAY_LENGTH; // +8 hours in seconds

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

    public Gradient SkyGradientOverTime;

    public AudioSource MusicSource;

    public AudioSource EffectsSource;

    [SerializeField]
    private float _timeSinceStart;

    //
    // Private variables

    private Scene _currentRoomScene;

    private Dictionary<string, List<NPCController>> _npcReferences = new Dictionary<string, List<NPCController>>();
    private List<NPCController> _allNPCs = new List<NPCController>();

    private Dictionary<string, RoomSceneController> _roomReferences = new Dictionary<string, RoomSceneController>();

    private Dictionary<string, List<CollectibleController>> _collectibleReferences = new Dictionary<string, List<CollectibleController>>();
    private List<CollectibleController> _allCollectibles = new List<CollectibleController>();

    //
    // Public getters

    public string LastPortalId { get; private set; }

    public bool IsTransitioning { get; private set; }

    public float TimeSinceStart
    {
        get { return _timeSinceStart; }
        private set { _timeSinceStart = value; }
    }

    public float ScaledGameTime { get; private set; }

    public int CurrentScore { get; private set; } = 0;

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

    public void OnPopupTrigger(string title, string description, Sprite icon, GameObject sender)
    {
        Dialogue.Show(sender, title, description, icon);
    }

    public void OnInventoryTrigger(string title, string description, Sprite icon, GameObject sender)
    {
        Dialogue.Show(sender, title, description, icon, "Close", "Drop Item");
    }

    public void OnCollectiblePopupTrigger(string title, string description, Sprite icon, GameObject sender)
    {
        Dialogue.Show(sender, title, description, icon, "Close", "Take");
    }

    public void OnQuestPopupTrigger(string title, string description, Sprite icon, GameObject sender)
    {
        Dialogue.Show(sender, title, description, icon, "Decline", "Accept");
    }

    public void OnActiveQuestPopupTrigger(string title, string description, Sprite icon, GameObject sender)
    {
        Dialogue.Show(sender, title, description, icon, "Close", "Unaccept");
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
        _allNPCs.Add(controller);
    }

    public void RegisterRoom(RoomSceneController controller)
    {
        var controllerGameObject = controller.gameObject;
        string sceneName = controllerGameObject.scene.name;
        if (!_roomReferences.ContainsKey(sceneName))
            _roomReferences.Add(sceneName, controller);
    }

    public void RegisterCollectible(CollectibleController controller)
    {
        var controllerGameObject = controller.gameObject;
        string sceneName = controllerGameObject.scene.name;
        if (!_collectibleReferences.ContainsKey(sceneName))
            _collectibleReferences.Add(sceneName, new List<CollectibleController>());

        _collectibleReferences[sceneName].Add(controller);
        _allCollectibles.Add(controller);
    }

    private void Start()
    {
        Instance = this;

        StartCoroutine(RunLoadStartRoutine());

        // Clone skybox material so we don't leave permanent changes to the file
        RenderSettings.skybox = Instantiate(RenderSettings.skybox);
    }

    private void LateUpdate()
    {
        UpdateScore();
        UpdateGameClock();
        UpdateSkybox();
    }

    private void UpdateSkybox()
    {
        // Change color of the skybox over time.
        float normalizedTime = Normalize(ScaledGameTime, START_TIME, END_TIME);
        Color newColor = SkyGradientOverTime.Evaluate(normalizedTime);
        RenderSettings.skybox.SetColor("_Tint", newColor);
    }

    private float Normalize(float value, float min, float max)
    {
	    float normalized = (value - min) / (max - min);

        return normalized;
    }

    /// <summary>
    /// Runs through all the NPCs and sets their quest data.
    /// </summary>
    private IEnumerator RunInitializeQuestTimelineRoutine()
    {
        // Wait a moment for all NPCs to be registered before we determine their quests.
        yield return new WaitForEndOfFrame();

        // Get list of random quests to assign.
        var questsToAssign = Profile.RandomQuests.Where(q => q != default).ToArray();

        // Randomize the list of NPCs who don't have a permanent quest and take the number of quests to assign.
        var rand = new System.Random();
        var chosenNPCs = _allNPCs
            .Where(n => n.Quest == default)
            .OrderBy(x => rand.Next())
            .Take(questsToAssign.Length)
            .ToArray();

        float questIntervals = DAY_LENGTH / questsToAssign.Length;
        for (int i = 0; i < chosenNPCs.Length; i++)
        {
            var questCopy = Instantiate(questsToAssign[i]);

            // Set a random time to trigger it within a random time within the bucket of the day.
            float lowerTimeRange = questIntervals * i;
            float upperTimeRange = lowerTimeRange + questIntervals;
            float triggerTime = UnityEngine.Random.Range(lowerTimeRange, upperTimeRange);
            questCopy.StartTime = triggerTime;

            questCopy.IsComplete = false;
            questCopy.Owner = chosenNPCs[i];
            chosenNPCs[i].Quest = questCopy;

            Debug.Log($"Assigned NPC \"{chosenNPCs[i].name}\" the quest \"{questCopy.Name}\" to be triggered at {questCopy.StartTime}");
        }
    }

    private void ConfigurePlayer()
    {
        Player.speed = Profile.PlayerMoveSpeed;
    }

    private void UpdateScore()
    {
        ScoreText.text = $"{CurrentScore:N0}";
    }

    private void UpdateGameClock()
    {        
        ScaledGameTime = START_TIME;
        if (GameHasStarted)
        {
            // Only start ticking time when the player has started the game.

            TimeSinceStart += Time.deltaTime;

            ScaledGameTime += (TimeSinceStart * Profile.TimeScale);

            if (ScaledGameTime > END_TIME)
                EndGame();
        }

        TimeSpan ts = TimeSpan.FromSeconds(ScaledGameTime);
        TimeText.text = $"{new DateTime(2020, 01, 01, ts.Hours, ts.Minutes, ts.Seconds):hh:mm tt}";
    }

    private void EndGame()
    {
        PauseGame(showMenu: false);

        string gameOverText = Profile.DefaultGameOverText;
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
        OnPopupTrigger(Profile.PlayerName, gameOverText, Profile.PlayerAvatar, gameObject);
    }

    private IEnumerator RunLoadStartRoutine()
    {
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

        if (Profile.Mixer != default)
            Profile.Mixer.SetFloat("MasterVolume", 1f.ToNormalizedVolume());

        // Play the music
        MusicSource.loop = true;
        MusicSource.clip = Profile.MusicLoop;
        MusicSource.Play();

        // Lastly show the instruction
        OnPopupTrigger(Profile.PlayerName, Profile.IntroductionText, Profile.PlayerAvatar, gameObject);

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

            yield return null;
        }

        FadeCanvas.gameObject.SetActive(FadeCanvas.alpha > 0);
    }
}
