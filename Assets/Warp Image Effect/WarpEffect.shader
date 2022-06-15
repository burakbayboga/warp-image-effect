Shader "Custom/WarpEffect"
{
    Properties
    {
        [HideInInspector] _MainTex ("Texture", 2D) = "white" {}

		// the noise texture to determine the warp directions
		_NoiseTex ("Noise Texture (RG)", 2D) = "gray" {}

		// effect texture to render on top of the warped area
		_EffectTex ("Effect Texture", 2D) = "white" {}
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
            };

            sampler2D _MainTex;
			sampler2D _NoiseTex;
			sampler2D _EffectTex;
			fixed4 _EffectTint;
			float _WarpStrength;
			float4 _WarpData;		// x, y: effect center in viewport space   z: warp radius start   w: warp radius end
			float _HeightToWidthRatio;
			float4 _EffectRotationFactors;

            v2f VertexFunction(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }


            fixed4 FragmentFunction(v2f i) : SV_Target
            {
				fixed4 effectColor;
				fixed distortionScale = 0;
				fixed applyEffect = 0;

				// get vector pointing from current pixel to the effect center
				float2 pixelToCenter = _WarpData.xy - i.uv;

				// adjust for aspect ratio
				pixelToCenter.y *= _HeightToWidthRatio;

				// pixelRadius is used to determine whether to apply the effect or not
				float pixelRadius = length(pixelToCenter);

				// pixelToCenterNormalized is used to sample the effect texture
				float2 pixelToCenterNormalized = pixelToCenter / pixelRadius;

				// if applyEffect is 1, the pixel will be warped, otherwise it will not touched.
				// determine if the current pixel is inside the effect area
				applyEffect = pixelRadius > _WarpData.z && pixelRadius < _WarpData.w;

				// warpFactor determines how much the pixel will be warped
				float warpFactor = (pixelRadius - _WarpData.z) / (_WarpData.w - _WarpData.z);

				// if pixel is outside of effect area, set warpFactor to zero
				warpFactor = applyEffect ? warpFactor : 0;

				// warp pixels that are on the outer side of the effect area in the opposite direction from the pixels that are on the inner side of the area
				distortionScale = warpFactor > 0.5 ? (1 - warpFactor) : warpFactor;

				// calculate uv coordinates for the effect texture
				// also apply rotation to the effect uv coordinates
				float2 centerToPixel = -pixelToCenterNormalized;
				float2 centerToPixelRotated;
				centerToPixelRotated.x = centerToPixel.x * _EffectRotationFactors.x - centerToPixel.y * _EffectRotationFactors.y;
				centerToPixelRotated.y = centerToPixel.x * _EffectRotationFactors.y + centerToPixel.y * _EffectRotationFactors.x;
				float2 effectUv = centerToPixelRotated * warpFactor * 0.5 + float2(0.5, 0.5);


				// sample the effect texture and apply color to it
				effectColor = applyEffect ? tex2D(_EffectTex, effectUv) : 0;
				effectColor *= _EffectTint;
				effectColor.rgb *= effectColor.a;
				
				// sample the noise texture to warp pixel positions and calculate the final uv coordinates
				float2 noise = tex2D(_NoiseTex, i.uv).rg;
				noise = (noise * 2 - 1) / 10;
				noise *= distortionScale * _WarpStrength;
				float2 distortion = applyEffect ? noise : 0;
				float2 finalUv = i.uv - distortion;

				// sample the final texture with updated uv coordinates
				fixed4 colorRaw = tex2D(_MainTex, finalUv);

				// add the effect color on top of updated final texture
				fixed4 finalColor = colorRaw + effectColor;
				return finalColor;
            }
            ENDCG
        }
    }
}
