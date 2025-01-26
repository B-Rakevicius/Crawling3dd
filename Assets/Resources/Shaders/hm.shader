Shader "Custom/Dithering"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _DitherPattern ("Dither Pattern", 2D) = "white" {}
        _PatternSize ("Pattern Size", Float) = 8.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

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
            };

            sampler2D _MainTex;
            sampler2D _DitherPattern;
            float _PatternSize;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                float luminance = dot(col.rgb, float3(0.299, 0.587, 0.114));
                float2 ditherUV = i.uv * _ScreenParams.xy / _PatternSize;
                float dither = tex2D(_DitherPattern, ditherUV).r;
                float threshold = luminance + dither - 0.5;
                float finalColor = step(threshold, 0.5);
                return fixed4(finalColor, finalColor, finalColor, 1.0);
            }
            ENDCG
        }
    }
}