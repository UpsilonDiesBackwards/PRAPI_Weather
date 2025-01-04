Shader "Unlit/CloudShader"
{
    Properties
    {
        _MainTex ("Texture", 3D) = "white" {}
        _Alpha ("Alpha", float) = 0.02
        _StepSize ("Step Size", float) = 0.01
        _ShadowThreshold ("Shadow Threshold", float) = 0.1
        _Transmittance ("Transmittance", float) = 0.5
        _LightAbsorb ("Light Absorbtion", float) = 0.1
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        Blend One OneMinusSrcAlpha
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            // Maximum number of raymarching samples
            #define MAX_STEP_COUNT 64
            #define MAX_LIGHT_STEPS 16

            // Allowed floating point inaccuracy
            #define EPSILON 0.00001f

            #define PI 3.1415926f

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 objectVertex : TEXCOORD0;
                float3 vectorToSurface : TEXCOORD1;
            };

            sampler3D _MainTex;
            float4 _MainTex_ST;
            float _Alpha;
            float _StepSize;
            float _ShadowThreshold;
            float _Transmittance;
            float _LightAbsorb;

            float RayLeighScatter(float3 mainRay, float3 lightRay)
            {
                float3 rayAngle = dot(mainRay, lightRay);
                float scatterAmount = ((3 * PI)/16)*(1+(cos(rayAngle)*cos(rayAngle)));
                return scatterAmount;
            }

            float Falloff(float3 position)
            {
                float distance = position/sqrt(position);
                float calcFalloff = smoothstep(0, 1, position);
                return calcFalloff;
            }

            v2f vert (appdata v)
            {
                v2f o;

                // Vertex in object space. This is the starting point for the raymarching.
                o.objectVertex = v.vertex;

                // Calculate vector from camera to vertex in world space
                float3 worldVertex = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.vectorToSurface = worldVertex - _WorldSpaceCameraPos;

                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float density = 0;
                float transmission = 0;
                float lightAccumulation = 0;
                float finalLight = 0;

                float3 lightDirection = float3(50, -30, 0);

                // Start raymarching at the front surface of the object
                float3 rayOrigin = i.objectVertex;

                // Use vector from camera to object surface to get ray direction
                float3 rayDirection = mul(unity_WorldToObject, float4(normalize(i.vectorToSurface), 1));

                float4 colour = float4(1, 1, 1, 1);
                float3 samplePos = rayOrigin;

                // Raymarch through object space
                for (int i = 0; i < MAX_STEP_COUNT; i++)
                {
                    samplePos += rayDirection * _StepSize;

                    //Cloud voxel position sampling
                    float sampledDensity = tex3D(_MainTex, samplePos+float3(_Time.y,_Time.y,_Time.y));
                    density += sampledDensity*_Alpha;

                    //Light loop
                    float3 lightSamplePos = samplePos;

                    for (int j = 0; j < MAX_LIGHT_STEPS; j++)
                    {
                        lightSamplePos += -lightDirection * _StepSize;
                        float lightDensity = tex3D(_MainTex, lightSamplePos) * (1 - Falloff(lightSamplePos));
                        lightAccumulation += lightDensity * RayLeighScatter(rayDirection, lightDirection);
                    }

                    //Transmittance and shadow calculations
                    float lightTransmission = exp(-lightAccumulation);
                    float shadow = _ShadowThreshold + (lightTransmission * (1 - _ShadowThreshold));
                    finalLight += _Alpha * _Transmittance * shadow;
                    _Transmittance *= exp(-density*_LightAbsorb);
                }

                transmission = exp(-density);
                float3 result = float3(finalLight, transmission, _Transmittance);

                return colour * finalLight;
            }
            ENDCG
        }
    }
}