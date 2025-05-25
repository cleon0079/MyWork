using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine;
using UnityEditor;


public class CloudRenderPipeline : ScriptableRendererFeature
{
    public enum CloudType
    {
        Cirrus, Cirrocumulus, Cirrostratus,
        Altocumulus, Altostratus,
        Stratocumulus, Stratus, Cumulus, Cumulonimbus, Nimbostratus
    }

    [System.Serializable]
    public class Settings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingSkybox;
        public float RenderDistance = 1000;

        [HideInInspector] public Material material;
        [HideInInspector] public Container container;

        [HideInInspector] public Texture2D CloudNoiseTexure;
        [HideInInspector] public Texture3D DetailCloudNoiseTexure;
        [HideInInspector] public Texture2D BlueNoiseTexure;
        [HideInInspector] public Texture2D HighLevelCloudNoiseTexure;

        [HideInInspector] public CloudType cloudType;
        [HideInInspector] public CloudTypeScriptableObject[] cloudTypeData = new CloudTypeScriptableObject[10];
    }

    public class CloudSettings
    {
        public Color CloudColor = Color.white;
        public int Steps = 30;
        public int LightSteps = 15;
        public float CloudScale = 1;
        public float CloudSmooth = 5;
        public Vector3 Wind = Vector3.right;
        public float LightAbsorptionThroughCloud = 0.15f;
        public float ContainerEdgeFadeDst = 45;
        public float DensityThreshold = 0.25f;
        public float DensityMultiplier = 1;
        public float LightAbsorptionTowardSun = 0.25f;
        public float DarknessThreshold = 0.1f;
        public float alpha = 1;

        public float detailCloudWeight = 0.24f;
        public float DetailCloudScale = 1;
        public Vector3 DetailCloudWind = new Vector3(0.5f, 0, 0);

        public Vector3 containerMin = new Vector3(-250, 50, -250);
        public Vector3 containerMax = new Vector3(250, 80, 250);
    }

    public Settings settings = new Settings();
    public CloudSettings cloudSettings = new CloudSettings();

    class Pass : ScriptableRenderPass
    {
        private RenderTargetIdentifier source;
        private RenderTargetHandle tempTexture;
        private string profilerTag;
        public Settings settings;
        public CloudSettings cloudSettings;

        public void Setup(RenderTargetIdentifier source) => this.source = source;

        public Pass(string profilerTag, Settings settings, CloudSettings cloudSettings)
        {
            this.profilerTag = profilerTag;
            this.settings = settings;
            this.cloudSettings = cloudSettings;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            cmd.GetTemporaryRT(tempTexture.id, cameraTextureDescriptor);
            ConfigureTarget(tempTexture.Identifier());
            ConfigureClear(ClearFlag.All, Color.black);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (settings.material == null) return;

            CommandBuffer cmd = CommandBufferPool.Get(profilerTag);
            cmd.Clear();

            if (settings.cloudTypeData != null && settings.cloudTypeData[0] != null)
            {
                CloudRenderPipeline.SetCloudParameters(settings.cloudType, cloudSettings, settings.cloudTypeData);
            }


            settings.container = FindObjectOfType<Container>();
            if (settings.container != null)
            {
                settings.cloudType = settings.container.cloudType;
            }

            if (settings.container != null)
            {
                cloudSettings.containerMin = new Vector3(
                    settings.container.transform.position.x - (settings.container.containerSize.x / 2f), 
                    settings.container.transform.position.y - (settings.container.containerSize.y / 2f), 
                    settings.container.transform.position.z - (settings.container.containerSize.z / 2f));
                cloudSettings.containerMax = new Vector3(
                    settings.container.transform.position.x + (settings.container.containerSize.x / 2f), 
                    settings.container.transform.position.y + (settings.container.containerSize.y / 2f), 
                    settings.container.transform.position.z + (settings.container.containerSize.z / 2f));
            }


            ApplyMaterialProperties(cloudSettings, settings);
            cmd.Blit(source, tempTexture.Identifier());
            cmd.Blit(tempTexture.Identifier(), source, settings.material, 0);
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }

        private void ApplyMaterialProperties(CloudSettings cloudSettings, Settings settings)
        {
            if (settings.cloudType == CloudType.Cirrus || settings.cloudType == CloudType.Cirrostratus)
            {
                settings.material.SetTexture("_ShapeNoise", settings.HighLevelCloudNoiseTexure);
            }
            else
            {
                settings.material.SetTexture("_ShapeNoise", settings.CloudNoiseTexure);

            }
            settings.material.SetTexture("_DetailNoise", settings.DetailCloudNoiseTexure);
            settings.material.SetFloat("_alpha", cloudSettings.alpha);
            settings.material.SetColor("_color", cloudSettings.CloudColor);
            settings.material.SetVector("_ContainerMin", cloudSettings.containerMin);
            settings.material.SetVector("_ContainerMax", cloudSettings.containerMax);
            settings.material.SetFloat("_CloudScale", Mathf.Abs(cloudSettings.CloudScale));
            settings.material.SetVector("_Wind", cloudSettings.Wind);
            settings.material.SetFloat("_detailNoiseScale", Mathf.Abs(cloudSettings.DetailCloudScale));
            settings.material.SetVector("_detailNoiseWind", cloudSettings.DetailCloudWind);
            settings.material.SetFloat("_containerEdgeFadeDst", Mathf.Abs(cloudSettings.ContainerEdgeFadeDst));
            settings.material.SetFloat("_detailNoiseWeight", cloudSettings.detailCloudWeight);
            settings.material.SetFloat("_DensityThreshold", cloudSettings.DensityThreshold);
            settings.material.SetFloat("_DensityMultiplier", Mathf.Abs(cloudSettings.DensityMultiplier));
            settings.material.SetInteger("_NumSteps", cloudSettings.Steps);
            settings.material.SetFloat("_lightAbsorptionThroughCloud", cloudSettings.LightAbsorptionThroughCloud);
            settings.material.SetInteger("_numStepsLight", cloudSettings.LightSteps);
            settings.material.SetFloat("_lightAbsorptionTowardSun", cloudSettings.LightAbsorptionTowardSun);
            settings.material.SetFloat("_darknessThreshold", cloudSettings.DarknessThreshold);
            settings.material.SetFloat("_cloudSmooth", cloudSettings.CloudSmooth);
            settings.material.SetTexture("_BlueNoise", settings.BlueNoiseTexure);
            settings.material.SetFloat("_RenderDistance", settings.RenderDistance);
        }
    }


    Pass pass;
    public override void Create()
    {
        LoadAssets();
        pass = new Pass("CloudRenderPipeline", settings, cloudSettings)
        {
            renderPassEvent = settings.renderPassEvent
        };
        name = "CloudRenderPipeline";
        pass.settings = settings;
        pass.cloudSettings = cloudSettings;
        pass.renderPassEvent = settings.renderPassEvent;

        if (settings.cloudTypeData != null && settings.cloudTypeData[0] != null)
        {
            CloudRenderPipeline.SetCloudParameters(settings.cloudType, cloudSettings, settings.cloudTypeData);
        }
    }



    public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
    {
        var cameraColorTargetIdent = renderer.cameraColorTarget;
        pass.Setup(cameraColorTargetIdent);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(pass);
    }

    public static void SetCloudParameters(CloudType cloudType, CloudSettings cloudSettings, CloudTypeScriptableObject[] cloudTypeData)
    {
        int id = (int)cloudType;

        cloudSettings.CloudColor = cloudTypeData[id].CloudColor;
        cloudSettings.Steps = cloudTypeData[id].Steps;
        cloudSettings.LightSteps = cloudTypeData[id].LightSteps;
        cloudSettings.CloudScale = cloudTypeData[id].CloudScale;
        cloudSettings.CloudSmooth = cloudTypeData[id].CloudSmooth;
        cloudSettings.Wind = cloudTypeData[id].Wind;
        cloudSettings.LightAbsorptionThroughCloud = cloudTypeData[id].LightAbsorptionThroughCloud;
        cloudSettings.ContainerEdgeFadeDst = cloudTypeData[id].ContainerEdgeFadeDst;
        cloudSettings.DensityThreshold = cloudTypeData[id].DensityThreshold;
        cloudSettings.DensityMultiplier = cloudTypeData[id].DensityMultiplier;
        cloudSettings.LightAbsorptionTowardSun = cloudTypeData[id].LightAbsorptionTowardSun;
        cloudSettings.DarknessThreshold = cloudTypeData[id].DarknessThreshold;
        cloudSettings.alpha = cloudTypeData[id].alpha;

        cloudSettings.detailCloudWeight = cloudTypeData[id].detailCloudWeight;
        cloudSettings.DetailCloudScale = cloudTypeData[id].DetailCloudScale;
        cloudSettings.DetailCloudWind = cloudTypeData[id].DetailCloudWind;
    }

    private void LoadAssets()
    {
#if UNITY_EDITOR
        settings.material = AssetDatabase.LoadAssetAtPath<Material>("Assets/URPCloud/Shader/Unlit_Cloud.mat");
        settings.CloudNoiseTexure = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/URPCloud/Noise/CloudNoiseTexture.png");
        settings.DetailCloudNoiseTexure = AssetDatabase.LoadAssetAtPath<Texture3D>("Assets/URPCloud/Noise/CloudsDetailNoise.asset");
        settings.HighLevelCloudNoiseTexure = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/URPCloud/Noise/HighCloudNoiseTexture.png");
        settings.BlueNoiseTexure = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/URPCloud/Noise/BlueNoise.png");

        for (int i = 0; i < settings.cloudTypeData.Length; i++)
        {
            string cloudTypeName = CloudType.GetName(typeof(CloudType), i);
            settings.cloudTypeData[i] = AssetDatabase.LoadAssetAtPath<CloudTypeScriptableObject>($"Assets/URPCloud/Clouds/{cloudTypeName}.asset");
        }
#endif
    }
}


