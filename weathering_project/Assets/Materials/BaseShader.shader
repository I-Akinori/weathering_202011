Shader "Custom/BaseShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_BumpMap ("Albedo (RGB)", 2D) = "normal" {}
		_RustLevel("RustLevel", Range(0,1)) = 0.5
        //_Glossiness ("Smoothness", Range(0,1)) = 0.5
        //_Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows vertex:vert

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
		sampler2D _BumpMap;

        struct Input
        {
			float4 vertColor;
            float2 uv_MainTex;
        };

		void vert(inout appdata_full v, out Input o) {
			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.vertColor = v.color;
		}

		half _RustLevel;
        //half _Glossiness;
        //half _Metallic;
        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
			_RustLevel = IN.vertColor.r;
			//_RustLevel = 1.0;
			// Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _RustLevel * 0.7 + fixed4(0.5, 0.5, 0.5, 1) * (1 - _RustLevel);
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = (1 - _RustLevel) * 0.5;
            o.Smoothness = (1 - _RustLevel) * 0.7;
            o.Alpha = c.a;

			fixed4 n = tex2D(_BumpMap, IN.uv_MainTex);

			o.Normal = UnpackScaleNormal(n, _RustLevel);
        }
        ENDCG
    }
    FallBack "Diffuse"
}
