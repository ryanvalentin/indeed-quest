using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public CanvasGroup FadeCanvas;

    public AudioMixer Mixer;

    public float FadeSeconds = 1f;

    public SceneReference GameStartScene;

    // Start is called before the first frame update
    private void Start()
    {
        FadeCanvas.alpha = 1f;
        FadeCanvas.gameObject.SetActive(true);
        StartCoroutine(RunFadeScreenRoutine(FadeSeconds, 0f, 0f, startGame: false));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        FadeCanvas.gameObject.SetActive(true);
        FadeCanvas.alpha = 0f;
        if (Mixer != default)
            Mixer.SetFloat("MasterVolume", 1f.ToNormalizedVolume());
        StartCoroutine(RunFadeScreenRoutine(FadeSeconds, 1f, 0f, startGame: true));
    }

    public void ShowAbout()
    {

    }

    public void ExitGame()
    {
        Application.Quit();
    }

    private IEnumerator RunFadeScreenRoutine(float fadeTimeSeconds, float targetAlpha, float initialWait, bool startGame)
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

        FadeCanvas.gameObject.SetActive(FadeCanvas.alpha > 0f);

        if (startGame)
            SceneManager.LoadScene(GameStartScene);
    }
}
