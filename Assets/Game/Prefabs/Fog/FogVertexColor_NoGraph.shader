Shader "Custom/FogVertexColor_NoGraph"
{
    Properties
    {
        _FogTexture("FogTexture", 2D) = "red" {}
        _WorldSize("WorldSize", Float) = 1
        _ChunkSize("ChunkSize", Float) = 1
        _VoxelSize("VoxelSize", Float) = 1
        _MinPosition("MinPosition", Vector) = (0, 0, 0, 0)
    }

        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            LOD 200

            CGPROGRAM
            // Surface Shader with Standard lighting model
            #pragma surface surf Standard fullforwardshadows addshadow
            #pragma target 3.0

            // VR and GPU Instancing Support
            //#pragma multi_compile_instancing
            //#pragma instancing_options renderinglayer

            sampler2D _FogTexture;
            float _WorldSize;
            float _ChunkSize;
            float _VoxelSize;
            float3 _MinPosition;

            struct Input
            {
                float3 worldPos;
                float4 color : COLOR; // Vertex color
                UNITY_VERTEX_INPUT_INSTANCE_ID // Required for stereo
            };

            void surf(Input IN, inout SurfaceOutputStandard o)
            {
                UNITY_SETUP_INSTANCE_ID(IN); // Required for stereo

                float2 posXZ = IN.worldPos.xz;
                float2 minXZ = _MinPosition.xz;
                float2 worldSpan = (_WorldSize * _ChunkSize) * _VoxelSize/* / 4*/;

                float2 uv = (posXZ - minXZ) / worldSpan;
                uv = 1.0 - uv; // Invert for sampling

                fixed4 texSample = tex2D(_FogTexture, uv);
                fixed4 vertexColor = IN.color;

                // Albedo = texture * vertex color
                o.Albedo = (1 - texSample.a) * vertexColor.rgb;
            }

            ENDCG
        }

            FallBack "Diffuse"
}