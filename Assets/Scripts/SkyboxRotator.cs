using UnityEngine;
using System.Collections;

public class SkyboxRotator : MonoBehaviour
{
    [Header("Rotation Settings")]
    [Range(0.1f, 2.0f)]
    public float rotationSpeed = 0.3f;

    [Header("Skybox Materials")]
    public Material[] galaxyMaterials;

    [Header("Transition Settings")]
    [Range(0.5f, 5.0f)]
    public float transitionDuration = 2.0f;

    [Header("Score Thresholds")]
    public int[] transitionScores = { 10, 20 };

    private Material skyboxMaterial;
    private Skybox skyboxComponent;
    private bool isTransitioning = false;
    private int currentTransitionIndex = -1;

    private void Start()
    {
        // Cache component reference
        skyboxComponent = GetComponent<Skybox>();

        // Create instance material to avoid modifying the original asset
        skyboxMaterial = new Material(skyboxComponent.material);
        skyboxComponent.material = skyboxMaterial;

        // Validate configuration
        if (galaxyMaterials == null || galaxyMaterials.Length == 0)
        {
            Debug.LogWarning("No galaxy materials assigned to SkyboxRotator.");
        }

        if (transitionScores.Length > galaxyMaterials.Length)
        {
            Debug.LogWarning("More transition scores than galaxy materials. Extra scores will be ignored.");
        }
    }

    private void Update()
    {
        // Rotate skybox using texture offset
        skyboxMaterial.SetFloat("_Rotation", skyboxMaterial.GetFloat("_Rotation") + Time.deltaTime * rotationSpeed);

        // Check for transitions if we have materials to transition to
        if (!isTransitioning && galaxyMaterials.Length > 0)
        {
            CheckForTransition();
        }
    }

    private void CheckForTransition()
    {
        // Get current score once
        int currentScore = int.Parse(TileSpawner.Instance.Score.text);

        // Check each transition threshold
        for (int i = 0; i < Mathf.Min(galaxyMaterials.Length, transitionScores.Length); i++)
        {
            // Only process if we haven't transitioned to this index yet
            if (currentScore >= transitionScores[i] && i > currentTransitionIndex)
            {
                StartCoroutine(TransitionSkybox(galaxyMaterials[i]));
                currentTransitionIndex = i;
                return; // Exit after starting a transition
            }
        }
    }

    private IEnumerator TransitionSkybox(Material targetGalaxy)
    {
        if (targetGalaxy == null)
        {
            Debug.LogWarning("Target galaxy material is null. Skipping transition.");
            yield break;
        }

        isTransitioning = true;

        // Store original skybox properties
        Material originalSkybox = skyboxMaterial;
        Material targetSkybox = new Material(targetGalaxy);

        // Create a temporary material for the transition
        Material transitionMaterial = new Material(originalSkybox);
        skyboxComponent.material = transitionMaterial;

        // Get original and target colors/properties
        Color originalTint = originalSkybox.GetColor("_Tint");
        Color targetTint = targetSkybox.GetColor("_Tint");
        float originalExposure = originalSkybox.GetFloat("_Exposure");
        float targetExposure = targetSkybox.GetFloat("_Exposure");
        float currentRotation = originalSkybox.GetFloat("_Rotation");

        // Fade out phase
        float halfDuration = transitionDuration * 0.5f;
        yield return FadeSkybox(transitionMaterial, originalTint, Color.black, originalExposure, 0.1f, halfDuration);

        // Switch to target material when faded out
        transitionMaterial = new Material(targetSkybox);
        transitionMaterial.SetColor("_Tint", Color.black);
        transitionMaterial.SetFloat("_Exposure", 0.1f);
        transitionMaterial.SetFloat("_Rotation", currentRotation);
        skyboxComponent.material = transitionMaterial;

        // Fade in phase
        yield return FadeSkybox(transitionMaterial, Color.black, targetTint, 0.1f, targetExposure, halfDuration);

        // Ensure we end with the exact target material
        skyboxMaterial = new Material(targetGalaxy);
        skyboxComponent.material = skyboxMaterial;

        isTransitioning = false;
    }

    private IEnumerator FadeSkybox(Material material, Color startTint, Color endTint,
                                  float startExposure, float endExposure, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;

            // Apply fade
            material.SetColor("_Tint", Color.Lerp(startTint, endTint, t));
            material.SetFloat("_Exposure", Mathf.Lerp(startExposure, endExposure, t));

            // Continue rotation during transition
            float rotation = Time.deltaTime * rotationSpeed;
            material.SetFloat("_Rotation", material.GetFloat("_Rotation") + rotation);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure final values are exact
        material.SetColor("_Tint", endTint);
        material.SetFloat("_Exposure", endExposure);
    }
}