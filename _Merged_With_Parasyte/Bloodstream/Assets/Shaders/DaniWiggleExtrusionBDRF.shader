Shader "Dani/WiggleExtrusionBDRF" {
	Properties {
		//_MainTex ("Base (RGB)", 2D) = "white" {}
		_Ramp2D ("BRDF Ramp", 2D) = "gray" {}
		_Amount ("Extrusion Amount", Range (-100,100)) = 0
		_TimeScale ("Time Scaling", Range(0,10)) = 0
       	_XAmount ("X Wiggle", Range(-1,1)) = 0
       	_YAmount ("Y Wiggle", Range(-1,1)) = 0
       	_ZAmount ("Z Wiggle", Range(-1,1)) = 0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Ramp vertex:vert
		#pragma target 3.0

		//sampler2D _MainTex;
		sampler2D _Ramp2D;

		struct Input {
			float2 uv_MainTex;
		};
		
		//Variables 
		float _XAmount;
		float _YAmount;
		float _ZAmount;
		float _TimeScale;
		float _Amount;
		void vert (inout appdata_full v) {
			v.vertex.xyz += v.normal * _Amount;

			float time = _TimeScale * _Time.y * 10;
         	float iny = v.vertex.y * _YAmount + time;
         	float inx = v.vertex.x * _XAmount + time;
         	float inz = v.vertex.z * _ZAmount + time;
         
         	float wiggleX = sin(inx) * _XAmount;
         	float wiggleY = sin(iny) * _YAmount;
         	float wiggleZ = cos(inz) * _ZAmount;
         
         	v.normal.x = v.normal.x + wiggleX;
         	v.normal.y = v.normal.y + wiggleY;
         	v.normal.z = v.normal.z + wiggleZ;
         
         	normalize(v.normal);
         	v.vertex.x = v.vertex.x + wiggleX;
         	v.vertex.y = v.vertex.y + wiggleY;
         	v.vertex.z = v.vertex.z + wiggleZ;	
			
		}
		
		half4 LightingRamp (SurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
		{
			float NdotL = dot (s.Normal, lightDir);
			float NdotE = dot (s.Normal, viewDir);
			
			float diff = (NdotL * 0.3) + 0.5;
			float2 brdfUV = float2(NdotE * .8, diff);
			float3 BRDF = tex2D(_Ramp2D, brdfUV.xy).rgb;
			
			float4 c;
			c.rgb = BRDF;
			c.a = s.Alpha;
			return c;	
		}
			
		void surf (Input IN, inout SurfaceOutput o)
		{
			half4 c = float4 (.5, .5, .5,1);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
