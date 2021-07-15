Shader "Custom/TriangleShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Center("Center", Vector) = (0,0,0,0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Cull Off

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
            float3 _Center;

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
                float3 pos0 = _Buffer[id].a - _Center;
                float3 pos1 = _Buffer[id].b - _Center;
                float3 pos2 = _Buffer[id].c - _Center;
                float3 normal = (pos0 + pos1 + pos2) / 3;
                float3 worldNormal = mul(unity_ObjectToWorld, float4(normal, 0));

                o0.vertex = UnityObjectToClipPos(_Buffer[id].a);
                o1.vertex = UnityObjectToClipPos(_Buffer[id].b);
                o2.vertex = UnityObjectToClipPos(_Buffer[id].c);

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
