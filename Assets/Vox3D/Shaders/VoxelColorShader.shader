Shader "Custom/VoxelColorShader"
{
    Properties
    {
        
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque"}
        CGPROGRAM

        #pragma surface surf Lambert

        #include "UnityCG.cginc"
            
        struct Input {
            float4 vertex : POSITION;
            float4 color : COLOR;
        };

        void surf(Input IN, inout SurfaceOutput o) {
            o.Albedo = IN.color;
        }
            
        ENDCG
    }
    FallBack "Diffuse"
}
