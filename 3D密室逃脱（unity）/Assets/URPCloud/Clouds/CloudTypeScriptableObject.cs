using UnityEngine;

[CreateAssetMenu(fileName = "CloudTypeData", menuName = "ScriptableObjects/CloudTypeData", order = 1)]
public class CloudTypeScriptableObject : ScriptableObject
{
    [Header("Cloud Settings")]
    [Tooltip("Cloud color")]
    public Color CloudColor = new Color(1, 1, 1, 1);
    [Tooltip("Wind direction and speed")]
    public Vector3 Wind = new Vector3(1, 0, 0);
    [Tooltip("Controlling how detailed the cloud rendering is.")]
    [Range(10, 50)] public int Steps = 30;
    [Tooltip("Number of light steps affecting cloud light scattering.")]
    [Range(8, 30)] public int LightSteps = 15;
    [Tooltip("The overall size of the cloud in the scene.")]
    [Range(1, 15)] public float CloudScale = 1;
    [Tooltip("Controls the smoothness of cloud edges.")]
    [Range(1, 50)] public float CloudSmooth = 5;
    [Tooltip("Amount of light absorbed as it passes through the cloud.")]
    [Range(0, 1)] public float LightAbsorptionThroughCloud = 0.15f;
    [Tooltip("Distance over which the cloud fades at the edges.")]
    [Range(50, 100)] public float ContainerEdgeFadeDst = 45;
    [Tooltip("The threshold for cloud density.")]
    [Range(0.1f, 0.5f)] public float DensityThreshold = 0.25f;
    [Tooltip("Multiplier for cloud density")]
    [Range(1, 3)] public float DensityMultiplier = 1;
    [Tooltip("Controls how much light is absorbed when looking toward the sun.")]
    [Range(0.1f, 0.35f)] public float LightAbsorptionTowardSun = 0.25f;
    [Tooltip("Controls the darkness threshold for cloud shadows.")]
    [Range(0.1f, 0.25f)] public float DarknessThreshold = 0.1f;
    [Tooltip("Cloud transparency.")]
    [Range(0.7f, 1)] public float alpha = 1;

    [Header("Detail Cloud Settings")]
    [Tooltip("Wind of detailed cloud.")]
    public Vector3 DetailCloudWind = new Vector3(0.5f, 0, 0);
    [Tooltip("Weight of detailed cloud.")]
    [Range(0, 1)] public float detailCloudWeight = 0.24f;
    [Tooltip("Scale for the detailed cloud.")]
    [Range(1, 3)] public float DetailCloudScale = 1;
}