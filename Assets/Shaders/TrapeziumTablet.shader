Shader "Custom/TrapeziumTablet"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Transparency ("Transparency", Range(0, 1)) = 1.0
        _TopWidth ("Top Width Factor", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        LOD 200

        Blend SrcAlpha OneMinusSrcAlpha

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
            float4 _MainTex_ST;
            float _Transparency;
            float _TopWidth;

            v2f vert (appdata_t v)
            {
                v2f o;

                // Manipulate vertex positions to create a trapezium
                float4 worldPos = v.vertex;

                if (worldPos.y > 0.0) // Top edge
                {
                    worldPos.x *= _TopWidth; // Narrow the top edge
                }

                o.vertex = UnityObjectToClipPos(worldPos);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Sample texture and apply transparency
                fixed4 col = tex2D(_MainTex, i.uv);
                col.a *= _Transparency; // Control transparency via slider
                return col;
            }
            ENDCG
        }
    }
}
