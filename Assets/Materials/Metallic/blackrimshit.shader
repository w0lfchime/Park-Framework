Shader "Custom/RimLightingWithRainbowOutline"
{
    Properties
    {
        _Color ("Main Color", Color) = (0, 0, 0, 1)
        _RimColor ("Rim Color", Color) = (1, 1, 1, 1)
        _RimPower ("Rim Power", Range(0.1, 8)) = 3
        _OutlineWidth ("Outline Width", Range(0.001, 0.1)) = 0.02
        _RainbowSpeed ("Rainbow Speed", Range(0.1, 10)) = 1
        _OutlineEnabled ("Outline Enabled (1 = On, 0 = Off)", Range(0, 1)) = 1
        _OutlineFadeStrength ("Outline Fade Strength", Range(0, 1)) = 1
        _LightSpeed ("Light Speed", Range(0.1, 5)) = 1
        _LightRange ("Light Range", Range(0.1, 3)) = 1
        _LightIntensityMin ("Light Intensity Min", Range(0, 2)) = 0.5
        _LightIntensityMax ("Light Intensity Max", Range(0, 2)) = 1.5
        _PulseSpeed ("Pulse Speed", Range(0.1, 5)) = 1
        _DarkHoldOffset ("Dark Hold Offset", Range(0, 1)) = 0.3
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        
        // Outline Pass (Inverted Hull) with Fading Effect
        Pass
        {
            Name "OUTLINE"
            Tags { "LightMode" = "Always" }
            Cull Front
            ZWrite On
            ZTest LEqual

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f {
                float4 pos : SV_POSITION;
            };

            float _OutlineWidth;
            float _RainbowSpeed;
            float _OutlineEnabled;
            float _OutlineFadeStrength;
            float _PulseSpeed;
            float _DarkHoldOffset;

            fixed4 GetRainbowColor(float t)
            {
                float3 rainbow = 0.5 + 0.5 * float3(
                    sin(t),
                    sin(t + 2.094),
                    sin(t + 4.188)
                );
                return fixed4(rainbow, 1);
            }

            v2f vert (appdata_t v)
            {
                v2f o;

                if (_OutlineEnabled > 0.5) 
                {
                    float3 norm = normalize(mul((float3x3)UNITY_MATRIX_IT_MV, v.normal));
                    v.vertex.xyz += norm * _OutlineWidth;
                }

                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float time = _Time.y;

                // Calculate pulse intensity (same as rim lighting)
                float pulse = sin(time * _PulseSpeed) - _DarkHoldOffset;
                pulse = saturate((pulse + 1.0) * 0.5); // Remaps -1 to 1 into 0 to 1

                // Adjust rainbow alpha based on pulse intensity
                float outlineAlpha = lerp(1.0, 0.0, pulse * _OutlineFadeStrength);

                float timeColor = _Time.y * _RainbowSpeed;
                fixed4 rainbowColor = GetRainbowColor(timeColor);
                rainbowColor.a *= outlineAlpha; // Apply fading effect

                return rainbowColor;
            }
            ENDCG
        }

        // Main Pass (Unlit Rim Lighting with Oscillating & Pulsing Light)
        Pass
        {
            Tags { "LightMode"="Always" }
            Cull Back
            ZWrite On
            ZTest LEqual

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float3 viewDir : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
            };

            fixed4 _Color;
            fixed4 _RimColor;
            float _RimPower;
            float _LightSpeed;
            float _LightRange;
            float _LightIntensityMin;
            float _LightIntensityMax;
            float _PulseSpeed;
            float _DarkHoldOffset;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.viewDir = normalize(WorldSpaceViewDir(v.vertex));
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float time = _Time.y;

                // Oscillating light position (back & forth in front of the object)
                float3 lightPos = float3(0, 0, 1) * (_LightRange * sin(time * _LightSpeed));

                // Pulsing intensity with extended dark phase
                float pulse = sin(time * _PulseSpeed) - _DarkHoldOffset;
                pulse = saturate((pulse + 1.0) * 0.5); // Remaps -1 to 1 into 0 to 1
                float lightIntensity = lerp(_LightIntensityMin, _LightIntensityMax, pulse);

                // Convert normal to view space
                float3 normalVS = normalize(i.worldNormal);

                // Light direction
                float3 lightDir = normalize(lightPos);

                // Rim lighting effect
                float rim = 1.0 - saturate(dot(i.viewDir, normalVS));
                rim = pow(rim, _RimPower) * lightIntensity; // Apply pulsing brightness

                // Final color output
                fixed4 rimColor = _RimColor * rim;
                fixed4 baseColor = _Color;
                return baseColor + rimColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
