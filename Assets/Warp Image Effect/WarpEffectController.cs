using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpEffectController : MonoBehaviour
{
	public Material warpMaterialReference;
	public float warpFrequency;
	public float warpSpeed;
	[Range(0f, 0.3f)]
	public float warpThickness;

	[HideInInspector]
	public Color effectTint;

	// how much the pixels that are inside the affected area will be warped
	// values more than 1 will result in exaggerated visuals
	[HideInInspector]
	public float warpStrength;

	private Material warpMaterial;

	// warpData is passed to the shader and updated every frame to configure the effect
	private Vector4 warpData;
	private Vector2 warpCenter = new Vector2(0.5f, 0.5f);
	private float warpTimer;

	private void Awake()
	{
		warpMaterial = Instantiate(warpMaterialReference);
		
		float height = Screen.height;
		float width = Screen.width;
		warpMaterial.SetFloat("_HeightToWidthRatio", height / width);

		warpData.x = warpCenter.x;
		warpData.y = warpCenter.y;
		warpMaterial.SetColor("_EffectTint", effectTint);
		warpMaterial.SetFloat("_WarpStrength", warpStrength);
	}

	// Apply the warp material on the rendered frame
	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		Graphics.Blit(source, destination, warpMaterial);
	}

	private void Update()
	{
		GetMouseInput();

		// effect area calculations are done here to simplify the shader itself
		warpTimer = (warpTimer + Time.deltaTime) % warpFrequency;
		float radiusStart = warpTimer * warpSpeed;
		float radiusEnd = radiusStart + warpThickness;
		warpData.z = radiusStart;
		warpData.w = radiusEnd;
		warpMaterial.SetVector("_WarpData", warpData);
	}

	public void UpdateEffectTint(Color _effectTint)
	{
		if (Application.isPlaying)
		{
			effectTint = _effectTint;
			warpMaterial.SetColor("_EffectTint", effectTint);
		}
	}

	public void UpdateWarpStrength(float _warpStrength)
	{
		if (Application.isPlaying)
		{
			warpStrength = _warpStrength;
			warpMaterial.SetFloat("_WarpStrength", warpStrength);
		}
	}

	private void GetMouseInput()
	{
		if (Input.GetMouseButtonDown(0))
		{
			SetWarpCenter(Input.mousePosition);
		}
	}


	public void SetWarpCenter(Vector2 _warpCenter)
	{
		warpCenter = Camera.main.ScreenToViewportPoint(_warpCenter);
		warpData.x = warpCenter.x;
		warpData.y = warpCenter.y;
		warpTimer = 0f;
	}
}
