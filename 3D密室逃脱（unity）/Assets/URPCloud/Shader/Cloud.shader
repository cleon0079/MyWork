Shader "Unlit/Cloud"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 viewDir : TEXCOORD1;
            };


            sampler2D _MainTex;
            sampler2D _CameraDepthTexture;
            Texture2D<float4> _ShapeNoise;
            Texture3D<float4> _DetailNoise;
            Texture2D<float4> _BlueNoise;

            SamplerState sampler_ShapeNoise;
            SamplerState sampler_DetailNoise;
            SamplerState sampler_BlueNoise;

            float4 _MainTex_ST;

            float3 _ContainerMin, _ContainerMax;
            float _containerEdgeFadeDst;

            float _CloudScale, _detailNoiseScale;
            float3 _Wind, _detailNoiseWind;
            float _DensityThreshold, _DensityMultiplier;

            float _lightAbsorptionThroughCloud, _lightAbsorptionTowardSun, _darknessThreshold, _cloudSmooth;
            int _NumSteps, _numStepsLight;
            half4 _color;
            float _alpha, _RenderDistance, _detailNoiseWeight;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                float3 viewVector = mul(unity_CameraInvProjection, float4(v.uv * 2 - 1, 0, -1));
                o.viewDir = mul(unity_CameraToWorld, float4(viewVector, 0));

                return o;
            }

            // Convert UV into screen pixel space and scale them to get a lager UV for sampling the bule noise
            float2 squareUV(float2 uv) {
                float width = _ScreenParams.x;
                float height =_ScreenParams.y;
                float scale = 1000;
                float x = uv.x * width;
                float y = uv.y * height;
                return float2 (x/scale, y/scale);
            }

            // Calculate the distances at which a ray enters and exits the container and get the enter and exit point
            float2 rayBoxDst(float3 boundsMin, float3 boundsMax, float3 rayOrigin, float3 invRaydir) {
                // Calculate the points where the ray intersect with the container
                float3 tmin = min((boundsMin - rayOrigin) * invRaydir, (boundsMax - rayOrigin) * invRaydir);
                float3 tmax = max((boundsMin - rayOrigin) * invRaydir, (boundsMax - rayOrigin) * invRaydir);
                
                // Finds the enter and exit point of the ray
                float dstA = max(max(tmin.x, tmin.y), tmin.z);
                float dstB = min(tmax.x, min(tmax.y, tmax.z));

                return float2(max(0, dstA), max(0, dstB - max(0, dstA)));
            }

            // Calculates how dense the cloud is at any given position
            float sampleDensity(float3 pos)
            {
                // Get the uv from the wind and noise
                float3 uvw = pos * _CloudScale * 0.001 + _Wind.xyz * 0.1 * _Time.y * _CloudScale;
                float3 duvw = pos * _detailNoiseScale * 0.001 + _detailNoiseWind.xyz * 0.1 * _Time.y * _detailNoiseScale;

                // Calculate how close the point is to the edges of the container
                float dstFromEdgeX = min(_containerEdgeFadeDst, min(pos.x - _ContainerMin.x, _ContainerMax.x - pos.x));
                float dstFromEdgeY = min(_cloudSmooth, min(pos.y - _ContainerMin.y, _ContainerMax.y - pos.y));
                float dstFromEdgeZ = min(_containerEdgeFadeDst, min(pos.z - _ContainerMin.z, _ContainerMax.z - pos.z));
                float edgeWeight = min(dstFromEdgeZ, dstFromEdgeX) / _containerEdgeFadeDst;

                // Samples the noise of the cloud and detail
                float4 shape = _ShapeNoise.SampleLevel(sampler_ShapeNoise, uvw.xz, 0);
                float4 detail = _DetailNoise.SampleLevel(sampler_DetailNoise, duvw, 0);
                
                // How much influence the detail noise has compared to the shape noise
                float density = max(0, lerp(shape.x, detail.x, _detailNoiseWeight) - _DensityThreshold) * _DensityMultiplier;

                return density * edgeWeight * (dstFromEdgeY / _cloudSmooth);
            }

            // Calculates how much light is absorbed as it travel through the cloud
            float lightmarch(float3 position) {
                // Retrieves the direction of the primary light source in the scene
                float3 dirToLight = _WorldSpaceLightPos0.xyz;
                // Gives the total distance that the light ray travels through the cloud
                float dstInsideBox = rayBoxDst(_ContainerMin, _ContainerMax, position, 1 / dirToLight).y;
                
                // The Step Size for Raymarching
                float stepSize = dstInsideBox / _numStepsLight;
                float totalDensity = 0;

                // Raymarching Loop to Sample Cloud Density
                for (int step = 0; step < _numStepsLight; step ++) {
                    position += dirToLight * stepSize;
                    totalDensity += max(0, sampleDensity(position) * stepSize);
                }

                return _darknessThreshold + exp(-totalDensity * _lightAbsorptionTowardSun) * (1 - _darknessThreshold);
            }


            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                // Starting point of the ray
                float3 rayOrigin = _WorldSpaceCameraPos;
                // Direction in which the ray is cast
                float3 rayDir = i.viewDir / length(i.viewDir);

                // Whether the clouds should be rendered in front of or behind other objects
                float nonlin_depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
                float depth = LinearEyeDepth(nonlin_depth) * length(i.viewDir);

                // Checks where the ray intersects with the clouds container
                float dstToBox = rayBoxDst(_ContainerMin, _ContainerMax, rayOrigin, 1 / rayDir).x;
                float dstInsideBox = rayBoxDst(_ContainerMin, _ContainerMax, rayOrigin, 1 / rayDir).y;
                // The cloud rendering stops if the ray is too far away
                if(dstToBox + dstInsideBox > _RenderDistance) return col;

                // Random offset to the raymarching process to prevent visible artifacts
                float dstTravelled = _BlueNoise.SampleLevel(sampler_BlueNoise, squareUV(i.uv *3), 0) * 50;
                float stepSize = dstInsideBox / _NumSteps;

                float3 entryPoint = rayOrigin + rayDir * dstToBox;
                float transmittance = 1;
                float3 lightEnergy = 0;
                // Accumulating cloud density and calculating how much light gets through at each step.
                while (dstTravelled < min(depth - dstToBox, dstInsideBox)) {
                    rayOrigin = entryPoint + rayDir * dstTravelled;
                    float density = sampleDensity(rayOrigin);
                    
                    if (density > 0) {
                        float lightTransmittance = lightmarch(rayOrigin);
                        lightEnergy += density * stepSize * transmittance * lightTransmittance * 0.5f;
                        transmittance *= exp(-density * stepSize * _lightAbsorptionThroughCloud);
                    
                        if (transmittance < 0.1) {
                            break;
                        }
                    }
                    dstTravelled += stepSize;
                }

                // Cloud and background color
                float3 cloudCol = lightEnergy * _color;
                float3 col0 = col * transmittance + cloudCol;
                return float4(lerp(col, col0, smoothstep(_RenderDistance, _RenderDistance * 0.25f, dstToBox + dstInsideBox) * _alpha), 0);
            }
            ENDCG
        }
    }
}