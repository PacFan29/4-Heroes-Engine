Shader "MatCap"
{
    Properties
    {
        _MatCap ("Mat Cap", 2D) = "white" {}
    }

    Subshader
    {
        Tags { "RenderType"="Opaque" }
        
        Pass
        {
            CGPROGRAM
           #include "UnityCG.cginc"
           #pragma vertex vert
           #pragma fragment frag

            sampler2D _MatCap;
                
            struct v2f
            {
                float4 pos  : SV_POSITION;
                half2 uv    : TEXCOORD0;
            };
                
            v2f vert (appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos (v.vertex);
                
                // カメラ座標系の法線を取得
                float3 normal = UnityObjectToWorldNormal(v.normal);
                normal = mul((float3x3)UNITY_MATRIX_V, normal);

                // 法線のxyを0～1に変換する
                o.uv = normal.xy * 0.5 + 0.5;

                return o;
            }
                
            float4 frag (v2f i) : COLOR
            {
                // カメラから見た法線のxyをそのままUVとして使う
                return tex2D(_MatCap, i.uv);
            }

            ENDCG
        }
    }
}