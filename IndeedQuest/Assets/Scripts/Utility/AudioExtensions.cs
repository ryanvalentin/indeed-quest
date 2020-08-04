using UnityEngine;

public static class AudioExtensions
{
    /// <summary>
    /// Returns a scaled audio volume given a normalized input (0, 1)
    /// </summary>
    public static float LerpAudio(float value)
    {
        value = Mathf.Clamp01(value);
        const float muteValue = -80f;
        const float minValue = -30f;
        const float maxValue = 0f;
        if (value < 0.01f)
            return muteValue;
        return Mathf.Lerp(minValue, maxValue, value);
    }

    public static float ToNormalizedVolume(this float value)
    {
        return LerpAudio(value);
    }
}
