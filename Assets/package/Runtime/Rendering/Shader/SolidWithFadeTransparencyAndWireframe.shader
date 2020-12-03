/*
 * Copyright (c) 2019 Robotic Eyes GmbH software
 *
 * THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
 * KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
 * PARTICULAR PURPOSE.
 *
 */

Shader "RoboticEyes/RexFile/SolidWithFadeTransparencyAndWireframe"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}

        _WireframeColor("Wireframe Color", Color) = (0, 0, 0)
        _WireframeSmoothing("Wireframe Smoothing", Range(0, 10)) = 1
        _WireframeThickness("Wireframe Thickness", Range(0, 10)) = 1
    }
    SubShader
    {
        Tags { "Queue" = "Geometry" "RenderType"="Transparent" }
        LOD 200

        Pass{
            // Pre-Pass to write onto Z-Buffer
			ZWrite On
			ColorMask 0
			Cull Back
		}

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows noforwardadd alpha:fade vertex:vert nolightmap

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            float3 vertexColor; // Vertex color stored here by vert() method
            float3 barycentricCoords;
        };

        fixed _RexFadeAlpha = 1.0;
        fixed4 _Color;

        float3 _WireframeColor;
        float _WireframeSmoothing;
        float _WireframeThickness;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

		void vert (inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input,o);
            o.vertexColor = v.color; // Save the Vertex Color in the Input for the surf() method
            o.barycentricCoords = v.texcoord1.xyz;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb * IN.vertexColor;

            //Apply Wireframe
            float3 baryCoords = IN.barycentricCoords;
            float3 deltas = fwidth(baryCoords);
            float3 smoothing = deltas * _WireframeSmoothing;
            float3 thickness = deltas * _WireframeThickness;
            baryCoords = smoothstep(thickness, thickness + smoothing, baryCoords);
            float wireBlend = min(baryCoords.x, min(baryCoords.y, baryCoords.z));
            o.Albedo = lerp(_WireframeColor, o.Albedo, wireBlend);

            // Metallic and smoothness come from slider variables
            o.Metallic = 0.0;
            o.Smoothness = 0.0;
            o.Alpha = c.a * _RexFadeAlpha;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
