using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpEffectController : MonoBehaviour
{
	[HideInInspector]
	public Color effectTint = Color.white;

	// how much the pixels that are inside the affected area will be warped
	// values more than 1 will result in exaggerated visuals
	[HideInInspector]
	public float warpStrength = 1f;

	private Material warpMaterial;

	// warpData is passed to the shader and updated every frame to configure the effect
	private Vector4 warpData;

	private void Awake()
	{
		warpMaterial = Instantiate(Resources.Load("Warp Material") as Material);
		
		float height = Screen.height;
		float width = Screen.width;
		warpMaterial.SetFloat("_HeightToWidthRatio", height / width);

		warpMaterial.SetColor("_EffectTint", effectTint);
		warpMaterial.SetFloat("_WarpStrength", warpStrength);
	}

	// Apply the warp material on the rendered frame
	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		Graphics.Blit(source, destination, warpMaterial);
	}

	public void UpdateRadius(float radiusInner, float radiusOuter)
	{
		warpData.z = radiusInner;
		warpData.w = radiusOuter;
		warpMaterial.SetVector("_WarpData", warpData);
	}

	public void UpdateCenter(Vector2 center)
	{
		warpData.x = center.x;
		warpData.y = center.y;
		warpMaterial.SetVector("_WarpData", warpData);
	}

	public void UpdateRotation(Vector2 rotationFactors)
	{
		warpMaterial.SetVector("_EffectRotationFactors", rotationFactors);
	}

	// called by custom inspector to update effect at runtime
	public void UpdateEffectTint(Color _effectTint)
	{
		if (Application.isPlaying)
		{
			effectTint = _effectTint;
			warpMaterial.SetColor("_EffectTint", effectTint);
		}
	}

	// called by custom inspector to update effect at runtime
	public void UpdateWarpStrength(float _warpStrength)
	{
		if (Application.isPlaying)
		{
			warpStrength = _warpStrength;
			warpMaterial.SetFloat("_WarpStrength", warpStrength);
		}
	}
}
