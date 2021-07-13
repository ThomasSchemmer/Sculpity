Shader "Custom/PointVisualizer"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Scale("Scale", Range(0, 1)) = 1
    }
        SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma geometry geo
            #pragma fragment frag
            #pragma target 5.0

            #include "UnityCG.cginc"


            static const float2 points[] = {
                0.1 * float2(-1, +1),
                0.1 * float2(+1, +1),
                0.1 * float2(+1, -1),
                0.1 * float2(-1, +1),
                0.1 * float2(+1, -1),
                0.1 * float2(-1, -1),
            };

            struct v2g
            {
                float4 vertex : SV_POSITION;
                float4 color : COLOR0;
            };

            struct g2f {
                float4 vertex: SV_POSITION;
                float4 color : COLOR0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            StructuredBuffer<float4> _Buffer;
            float _Scale;

            v2g vert (uint id : SV_VertexID)
            {
                v2g o;
                o.vertex = float4(_Buffer[id].xyz, 1);
                o.color = float4(_Buffer[id].w, 0, 0, 1);
                return o;
            }


            [maxvertexcount(6)]
            void geo(point v2g input[1], inout TriangleStream<g2f> outStream){
                float4 center = input[0].vertex;
                float3 up = normalize(mul(unity_ObjectToWorld, float4(0, 1, 0, 0)));
                float3 dir = normalize(WorldSpaceViewDir(center));
                float3 right = cross(up, dir);

                for (uint i = 0; i < 6; i++) {
                    g2f o;
                    float3 pos = 
                        center.xyz + 
                        points[i].x * right * _Scale + 
                        points[i].y * up * _Scale;
                    o.vertex = UnityObjectToClipPos(pos);
                    o.color = input[0].color;
                    outStream.Append(o);
                }
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
