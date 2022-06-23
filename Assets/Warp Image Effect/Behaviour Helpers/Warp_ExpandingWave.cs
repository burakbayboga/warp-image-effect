using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WarpEffectController))]
public class Warp_ExpandingWave : MonoBehaviour
{
	[Range(0.05f, 0.3f)]
	public float warpThickness = 0.05f;
	public float effectRotationSpeed;
	public float expandSpeed = 0.3f;
	public float waveInterval = 2.5f;

	private WarpEffectController warpController;

	private float warpTimer = 0f;
	private float currentEffectRotation = 0f;

	private void Awake()
	{
		warpController = GetComponent<WarpEffectController>();
	}

	private void Start()
	{
		warpController.UpdateCenter(new Vector2(100f, 100f));
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Vector2 center = Camera.main.ScreenToViewportPoint(Input.mousePosition);
			warpController.UpdateCenter(center);

			warpTimer = 0f;
		}

		UpdateRadius();
		UpdateRotation();
	}

	private void UpdateRadius()
	{
		warpTimer = (warpTimer + Time.deltaTime) % waveInterval;
		float radiusInner = warpTimer * expandSpeed;
		float radiusOuter = radiusInner + warpThickness;

		warpController.UpdateRadius(radiusInner, radiusOuter);
	}

	private void UpdateRotation()
	{
		currentEffectRotation += Time.deltaTime * effectRotationSpeed;
		float effectRotationRadians = currentEffectRotation * Mathf.Deg2Rad;
		Vector2 effectRotationFactors = new Vector2(Mathf.Cos(effectRotationRadians), Mathf.Sin(effectRotationRadians));

		warpController.UpdateRotation(effectRotationFactors);
	}
}
