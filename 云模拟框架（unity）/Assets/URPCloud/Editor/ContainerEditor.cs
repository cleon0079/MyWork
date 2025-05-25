using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Container))]
public class ContainerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Container container = (Container)target;

        DrawDefaultInspector();

        string cloudTypeName = container.cloudType.ToString();
        string[] assetGuids = AssetDatabase.FindAssets(cloudTypeName + " t:CloudTypeScriptableObject");

        if (assetGuids.Length > 0)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(assetGuids[0]);
            CloudTypeScriptableObject selectedCloudData = AssetDatabase.LoadAssetAtPath<CloudTypeScriptableObject>(assetPath);

            if (selectedCloudData != null)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Cloud Settings for " + cloudTypeName, EditorStyles.boldLabel);

                selectedCloudData.CloudColor = EditorGUILayout.ColorField("Cloud Color", selectedCloudData.CloudColor);
                selectedCloudData.Steps = EditorGUILayout.IntSlider("Steps", selectedCloudData.Steps, 10, 50);
                selectedCloudData.LightSteps = EditorGUILayout.IntSlider("Light Steps", selectedCloudData.LightSteps, 8, 30);
                selectedCloudData.CloudScale = EditorGUILayout.Slider("Cloud Scale", selectedCloudData.CloudScale, 1, 15);
                selectedCloudData.CloudSmooth = EditorGUILayout.Slider("Cloud Smooth", selectedCloudData.CloudSmooth, 1, 50);
                selectedCloudData.Wind = EditorGUILayout.Vector3Field("Wind", selectedCloudData.Wind);
                selectedCloudData.LightAbsorptionThroughCloud = EditorGUILayout.Slider("Light Absorption Through Cloud", selectedCloudData.LightAbsorptionThroughCloud, 0, 1);
                selectedCloudData.ContainerEdgeFadeDst = EditorGUILayout.Slider("Container Edge Fade Distance", selectedCloudData.ContainerEdgeFadeDst, 50, 100);
                selectedCloudData.DensityThreshold = EditorGUILayout.Slider("Density Threshold", selectedCloudData.DensityThreshold, 0.1f, 0.5f);
                selectedCloudData.DensityMultiplier = EditorGUILayout.Slider("Density Multiplier", selectedCloudData.DensityMultiplier, 1, 3);
                selectedCloudData.LightAbsorptionTowardSun = EditorGUILayout.Slider("Light Absorption Toward Sun", selectedCloudData.LightAbsorptionTowardSun, 0.1f, 0.35f);
                selectedCloudData.DarknessThreshold = EditorGUILayout.Slider("Darkness Threshold", selectedCloudData.DarknessThreshold, 0.1f, 0.25f);
                selectedCloudData.alpha = EditorGUILayout.Slider("Alpha", selectedCloudData.alpha, 0.7f, 1);
                selectedCloudData.detailCloudWeight = EditorGUILayout.Slider("Detail Cloud Weight", selectedCloudData.detailCloudWeight, 0, 1);
                selectedCloudData.DetailCloudScale = EditorGUILayout.Slider("Detail Cloud Scale", selectedCloudData.DetailCloudScale, 1, 3);
                selectedCloudData.DetailCloudWind = EditorGUILayout.Vector3Field("Detail Cloud Wind", selectedCloudData.DetailCloudWind);

                EditorUtility.SetDirty(selectedCloudData);
            }
            else
            {
                EditorGUILayout.HelpBox("No Cloud Data available for this CloudType.", MessageType.Warning);
            }
        }
        else
        {
            EditorGUILayout.HelpBox("No CloudTypeScriptableObject found for " + cloudTypeName, MessageType.Warning);
        }
    }
}