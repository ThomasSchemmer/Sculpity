Shader "Custom/TriangleShader"
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
            #pragma geometry geo

            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc" 

            struct v2g
            {
                float4 id : COLOR0;
            };

            struct g2f {
                float4 vertex: SV_POSITION;
                float4 color : COLOR0;
            };

            struct Triangle {
                float3 a;
                float3 b;
                float3 c;
            };

            StructuredBuffer<Triangle> _Buffer;

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2g vert(uint id : SV_VertexID)
            {
                v2g o;
                o.id = float4(id, 0, 0, 0);
                return o;
            }


            [maxvertexcount(3)]
            void geo(point v2g input[1], inout TriangleStream<g2f> outStream) {
                g2f o0, o1, o2;
                uint id = (uint)input[0].id.x;
                o0.vertex = UnityObjectToClipPos(_Buffer[id].a);
                o1.vertex = UnityObjectToClipPos(_Buffer[id].b);
                o2.vertex = UnityObjectToClipPos(_Buffer[id].c);

                float3 a = normalize(o0.vertex - o1.vertex);
                float3 b = normalize(o2.vertex - o1.vertex);
                float3 worldNormal = cross(a, b);
                half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
                o0.color = nl * _LightColor0;
                o1.color = nl * _LightColor0;
                o2.color = nl * _LightColor0;

                outStream.Append(o0);
                outStream.Append(o1);
                outStream.Append(o2);
                outStream.RestartStrip();
            }

            fixed4 frag (g2f i) : SV_Target
            {
                return i.color;
            }
            ENDCG
        }
    }
}
