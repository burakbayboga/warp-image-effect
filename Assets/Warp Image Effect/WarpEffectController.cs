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
	private Vector2 warpCenter = new Vector2(-10f, -1f);
	private float warpTimer;

	private void Awake()
	{
		warpMaterial = Instantiate(warpMaterialReference);
		
		float height = Screen.height;
		float width = Screen.width;
		Vector2 worldTopRight = Camera.main.ScreenToWorldPoint(new Vector2(width, height));
		Vector4 worldEdgeData = new Vector4(worldTopRight.x * 2f, worldTopRight.y * 2f,
												worldTopRight.x, worldTopRight.y);
		warpMaterial.SetVector("_WorldEdgeData", worldEdgeData);

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

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		Graphics.Blit(source, destination, warpMaterial);
	}

	public void SetWarpCenter(Vector2 _warpCenter)
	{
		warpCenter = Camera.main.ScreenToViewportPoint(_warpCenter);
		//warpCenter = _warpCenter;
		warpData.x = warpCenter.x;
		warpData.y = warpCenter.y;
		warpTimer = 0f;
	}
}
