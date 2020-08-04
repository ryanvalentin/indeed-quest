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
        FadeCanvas.alpha = 0f;
        FadeCanvas.gameObject.SetActive(false);
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
        StartCoroutine(FadeScreen(FadeSeconds, 1f, 0f));
    }

    public void ShowAbout()
    {

    }

    public void ExitGame()
    {
        Application.Quit();
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

        SceneManager.LoadScene(GameStartScene);
    }
}
