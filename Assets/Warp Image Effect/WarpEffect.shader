Shader "Custom/WarpEffect"
{
    Properties
    {
        [HideInInspector] _MainTex ("Texture", 2D) = "white" {}
		_NoiseTex ("Noise Texture (RG)", 2D) = "gray" {}
		_EffectTex ("Effect Texture", 2D) = "white" {}
		_EffectTint ("Effect Tint", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex VertexFunction
            #pragma fragment FragmentFunction

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
				float2 worldPos: TEXCOORD1;
            };

            sampler2D _MainTex;
			sampler2D _NoiseTex;
			sampler2D _EffectTex;
			fixed4 _EffectTint;
			float4 _WarpData;		// x, y: world position   z: warp radius start   w: warp radius end
			float4 _WorldEdgeData;	// x: edge x * 2    y: edge y * 2   z: edge x    w: edge y	

            v2f VertexFunction(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.worldPos = float2(o.worldPos.x * _WorldEdgeData.x - _WorldEdgeData.z,
									o.worldPos.y * _WorldEdgeData.y - _WorldEdgeData.w);
                return o;
            }


            fixed4 FragmentFunction(v2f i) : SV_Target
            {
				fixed4 effectColor;
				fixed applyDistortion = 0;
				fixed distortionScale = 0;
				fixed applyEffect = 0;


				//float2 pixelToCenter = _WarpData.xy - i.worldPos.xy;
				float2 pixelToCenter = _WarpData.xy - i.uv;
				pixelToCenter.y *= 0.5625;
				float pixelRadius = length(pixelToCenter);
				float2 pixelToCenterNormalized = pixelToCenter / pixelRadius;
				applyEffect = pixelRadius > _WarpData.z && pixelRadius < _WarpData.w;
				applyDistortion = applyEffect;
				float warpFactor = (pixelRadius - _WarpData.z) / (_WarpData.w - _WarpData.z);
				warpFactor = applyEffect ? warpFactor : 0;
				distortionScale = warpFactor > 0.5 ? (1 - warpFactor) : warpFactor;
				float2 effectUv = -pixelToCenterNormalized * warpFactor * 0.5 + float2(0.5, 0.5);
				effectColor = applyEffect ? tex2D(_EffectTex, effectUv) : 0;
				effectColor *= _EffectTint;
				effectColor.rgb *= effectColor.a;
				
				float2 noise = tex2D(_NoiseTex, i.uv).rg;
				noise = (noise * 2 - 1) / 10;
				noise *= distortionScale;
				float2 distortion = applyDistortion ? noise : 0;
				float2 finalUv = i.uv - distortion;

				fixed4 colorRaw = tex2D(_MainTex, finalUv);
				fixed4 finalColor = colorRaw + effectColor;
				return finalColor;
            }
            ENDCG
        }
    }
}
