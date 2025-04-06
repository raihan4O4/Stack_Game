using UnityEngine;
using System.Collections;

public class SkyboxRotator : MonoBehaviour
{
    [Header("Skybox Colors")]
    public Color[] skyboxColors;

    [Header("Transition Settings")]
    [Range(0.5f, 5.0f)]
    public float transitionDuration = 2.0f;

    [Header("Score Thresholds")]
    public int[] transitionScores = { 10, 20 };

    // Remove material references and add camera component
    private Camera cameraComponent;
    private bool isTransitioning = false;
    private int currentTransitionIndex = -1;

    private void Start()
    {
        // Get camera component instead of Skybox
        cameraComponent = GetComponent<Camera>();
        cameraComponent.clearFlags = CameraClearFlags.SolidColor;  // Add this line
        
        // Set initial color
        if (skyboxColors.Length > 0)
        {
            cameraComponent.backgroundColor = skyboxColors[0];
        }

        // Validate configuration
        if (skyboxColors == null || skyboxColors.Length == 0)
        {
            Debug.LogWarning("No skybox colors assigned to SkyboxRotator.");
        }

        if (transitionScores.Length > skyboxColors.Length)
        {
            Debug.LogWarning("More transition scores than skybox colors. Extra scores will be ignored.");
        }
    }

    private void Update()
    {
        // Check for transitions if we have colors to transition to
        if (!isTransitioning && skyboxColors.Length > 0)
        {
            CheckForTransition();
        }
    }

    private void CheckForTransition()
    {
        int currentScore = int.Parse(TileSpawner.Instance.Score.text);
        
        for (int i = 1; i < Mathf.Min(skyboxColors.Length, transitionScores.Length); i++)
        {
            if (currentScore >= transitionScores[i] && i > currentTransitionIndex)
            {
                StartCoroutine(TransitionSkyboxColor(skyboxColors[i]));
                currentTransitionIndex = i;
                return;
            }
        }
    }

    private IEnumerator TransitionSkyboxColor(Color targetColor)
    {
        isTransitioning = true;
        Color originalColor = cameraComponent.backgroundColor;
        
        // Change this line to properly start the coroutine
        yield return StartCoroutine(FadeColor(originalColor, targetColor, transitionDuration));
        
        isTransitioning = false;
    }

    private IEnumerator FadeColor(Color startColor, Color endColor, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            cameraComponent.backgroundColor = Color.Lerp(startColor, endColor, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        cameraComponent.backgroundColor = endColor;
    }
}