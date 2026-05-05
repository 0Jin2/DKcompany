Shader "Custom/Blur"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _BlurAmount ("Blur Amount", Range(0.0, 10.0)) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Overlay" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _BlurAmount;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float4 color = float4(0,0,0,0);

                // Blur offsets
                float2 offsets[9] = {
                    float2(-1, -1), float2(0, -1), float2(1, -1),
                    float2(-1, 0), float2(0, 0), float2(1, 0),
                    float2(-1, 1), float2(0, 1), float2(1, 1)
                };

                // Apply blur
                for (int j = 0; j < 9; j++)
                {
                    color += tex2D(_MainTex, uv + offsets[j] * _BlurAmount * 0.001);
                }
                color /= 9;

                return color;
            }
            ENDCG
        }
    }
}