Shader "MyCustoms/MarioCustoms/StarShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Speed ("Color Speed", Range(10, 20)) = 10
        _ColorPower ("Color Power", Range(0, 1)) = 0.5
        [KeywordEnum(Off, On)] _IsActive("Is Active", float) = 0
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
            // make fog work
            #pragma multi_compile_fog
            #pragma multi_compile _ISACTIVE_OFF _ISACTIVE_ON

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL0;
                float3 position_world : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Speed;
            float _ColorPower;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                o.normal = normalize(mul(unity_ObjectToWorld, float4(v.normal, 0))).xyz;
                o.position_world = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            void Unity_FresnelEffect_float(float3 Normal, float3 ViewDir, float Power, out float Out)
            {
                Out = pow((1.0 - saturate(dot(normalize(Normal), normalize(ViewDir)))), Power);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float speed = ceil(_Time.y * _Speed);
                float2 initial_color = float2(1, 1);
                float3 star_color = (float3(initial_color.xyx) + speed) + float3(0, 2, 4);
                star_color = cos(star_color) * _ColorPower;

                float3 normal_world = i.normal;
                float3 view_dir = normalize(_WorldSpaceCameraPos.xyz - i.position_world.xyz);

                float fresnel = 0;
                Unity_FresnelEffect_float(normal_world, view_dir, 1, fresnel);
                
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);

                #if _ISACTIVE_ON
                    return col + float4(star_color, 1) + fresnel;
                #else
                    return col;
                #endif
            }
            ENDCG
        }
    }
}
