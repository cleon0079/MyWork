using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Reflection;

public class SetUp : EditorWindow
{
    [MenuItem("Tools/Setup Cloud Render Pipeline")]
    public static void ShowWindow()
    {
        GetWindow<SetUp>("Cloud Render Pipeline Setup");
    }

    void OnGUI()
    {
        GUILayout.Label("Cloud Render Pipeline Setup", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("This tool allows you to set up a Cloud Render Pipeline feature and a CloudContainer in the scene. \n\n"
            + "1. Click 'Create' to add a cloud rendering feature to your Universal Render Pipeline.\n"
            + "2. Click 'Remove' to remove the feature and the CloudContainer from the scene.\n\n"
            + "Run the game every time after you have Click on 'Create' and 'Remove'", MessageType.Info);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Create"))
        {
            var currentRenderPipeline = GraphicsSettings.currentRenderPipeline;

            if (currentRenderPipeline is UniversalRenderPipelineAsset)
            {
                AddCloudRenderPipeline();
                CreateCloudContainer();
            }
            else
            {
                Debug.LogError("The Render Pipeline you are using is not Universal Render Pipeline, please switch to URP.");
            }
        }

        if (GUILayout.Button("Remove"))
        {
            var currentRenderPipeline = GraphicsSettings.currentRenderPipeline;

            if (currentRenderPipeline is UniversalRenderPipelineAsset)
            {
                RemoveCloudRenderPipeline();
                RemoveCloudContainer();
            }
            else
            {
                Debug.LogError("The Render Pipeline you are using is not Universal Render Pipeline, please switch to URP.");
            }
        }

        GUILayout.EndHorizontal();
    }

    void AddCloudRenderPipeline()
    {
        var pipelineAsset = (UniversalRenderPipelineAsset)GraphicsSettings.currentRenderPipeline;

        FieldInfo rendererDataListField = typeof(UniversalRenderPipelineAsset).GetField("m_RendererDataList", BindingFlags.NonPublic | BindingFlags.Instance);
        ScriptableRendererData[] rendererDataList = (ScriptableRendererData[])rendererDataListField.GetValue(pipelineAsset);

        if (rendererDataList != null && rendererDataList.Length > 0)
        {
            ScriptableRendererData rendererData = rendererDataList[0];

            bool alreadyAdded = false;
            foreach (var feature in rendererData.rendererFeatures)
            {
                if (feature is CloudRenderPipeline)
                {
                    alreadyAdded = true;
                    break;
                }
            }

            if (!alreadyAdded)
            {
                var cloudFeature = ScriptableObject.CreateInstance<CloudRenderPipeline>();

                rendererData.rendererFeatures.Add(cloudFeature);
                AssetDatabase.AddObjectToAsset(cloudFeature, pipelineAsset);

                Debug.Log("CloudRenderPipeline is added to your render pipeline");

                EditorUtility.SetDirty(rendererData);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            else
            {
                Debug.LogWarning("CloudRenderPipeline is already in your render pipeline");
            }
        }
        else
        {
            Debug.LogError("There is no Renderer Data");
        }
    }

    void RemoveCloudRenderPipeline()
    {
        var pipelineAsset = (UniversalRenderPipelineAsset)GraphicsSettings.currentRenderPipeline;

        FieldInfo rendererDataListField = typeof(UniversalRenderPipelineAsset).GetField("m_RendererDataList", BindingFlags.NonPublic | BindingFlags.Instance);
        ScriptableRendererData[] rendererDataList = (ScriptableRendererData[])rendererDataListField.GetValue(pipelineAsset);

        if (rendererDataList != null && rendererDataList.Length > 0)
        {
            ScriptableRendererData rendererData = rendererDataList[0];

            for (int i = 0; i < rendererData.rendererFeatures.Count; i++)
            {
                if (rendererData.rendererFeatures[i] is CloudRenderPipeline)
                {
                    rendererData.rendererFeatures.RemoveAt(i);
                    Debug.Log("CloudRenderPipeline has been removed from your render pipeline");

                    EditorUtility.SetDirty(rendererData);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    break;
                }
            }
        }
        else
        {
            Debug.LogError("There is no Renderer Data to remove CloudRenderPipeline");
        }
    }

    void CreateCloudContainer()
    {
        GameObject cloudContainer = new GameObject("CloudContainer");

        cloudContainer.AddComponent<Container>();

        Debug.Log("The scene has added a CloudContainer with Cloud controler");
    }

    void RemoveCloudContainer()
    {
        GameObject cloudContainer = GameObject.Find("CloudContainer");
        if (cloudContainer != null)
        {
            DestroyImmediate(cloudContainer);
            Debug.Log("CloudContainer has been removed from the scene");
        }
        else
        {
            Debug.LogWarning("No CloudContainer found in the scene to remove");
        }
    }
}
