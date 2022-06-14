using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpEffectController : MonoBehaviour
{
	public Material warpMaterialReference;
	public float warpFrequency;
	public float warpSpeed;
	public float warpThickness;

	private Color _effectTint;

	private Material warpMaterial;

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
	}

	private void Update()
	{
		GetMouseInput();
		warpTimer = (warpTimer + Time.deltaTime) % warpFrequency;
		float radiusStart = warpTimer * warpSpeed;
		float radiusEnd = radiusStart + warpThickness;
		warpData.z = radiusStart;
		warpData.w = radiusEnd;
		warpMaterial.SetVector("_WarpData", warpData);
	}

	private void GetMouseInput()
	{
		if (Input.GetMouseButtonDown(0))
		{
			SetWarpCenter(Input.mousePosition);
		}
	}

	// Apply the warp material on the rendered frame
	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		Graphics.Blit(source, destination, warpMaterial);
	}

	public void SetWarpCenter(Vector2 _warpCenter)
	{
		warpCenter = Camera.main.ScreenToViewportPoint(_warpCenter);
		warpData.x = warpCenter.x;
		warpData.y = warpCenter.y;
		warpTimer = 0f;
	}
}
