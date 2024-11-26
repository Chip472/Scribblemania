Shader "Custom/SuctionEffect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}       // The texture to apply the effect
        _Center ("Suction Center", Vector) = (0.5, 0.5, 0.0, 0.0) // Center of suction
        _Strength ("Suction Strength", Float) = 1.0 // How intense the suction is
        _TimeFactor ("Time Factor", Float) = 0.0    // For animation
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float2 _Center;
            float _Strength;
            float _TimeFactor;

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

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float2 WarpUV(float2 uv, float2 center, float strength, float time)
            {
                float2 offset = uv - center;
                float distance = length(offset);

                // Suction effect formula
                float warpAmount = pow(distance, strength) * 0.5 * sin(time);
                return uv - offset * warpAmount;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Get time for animation
                float animTime = _TimeFactor + _Time.y;

                // Warp UVs
                float2 warpedUV = WarpUV(i.uv, _Center.xy, _Strength, animTime);

                // Sample the texture
                return tex2D(_MainTex, warpedUV);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
