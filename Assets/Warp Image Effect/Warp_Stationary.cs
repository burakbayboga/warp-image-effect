using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WarpEffectController))]
public class Warp_Stationary : MonoBehaviour
{
	public float effectRotationSpeed;

	[HideInInspector]
	public float radiusInner;

	[HideInInspector]
	public float warpThickness;

	private WarpEffectController warpController;

	private float currentEffectRotation = 0f;

	private void Awake()
	{
		warpController = GetComponent<WarpEffectController>();
	}

	private void Start()
	{
		warpController.UpdateCenter(new Vector2(0.5f, 0.5f));
		float radiusOuter = radiusInner + warpThickness;
		warpController.UpdateRadius(radiusInner, radiusOuter);
	}

	private void Update()
	{
		if (Input.GetMouseButton(0))
		{
			Vector2 center = Camera.main.ScreenToViewportPoint(Input.mousePosition);
			warpController.UpdateCenter(center);
		}

		UpdateRotation();
	}

	private void UpdateRotation()
	{
		currentEffectRotation += Time.deltaTime * effectRotationSpeed;
		float effectRotationRadians = currentEffectRotation * Mathf.Deg2Rad;
		Vector2 effectRotationFactors = new Vector2(Mathf.Cos(effectRotationRadians), Mathf.Sin(effectRotationRadians));

		warpController.UpdateRotation(effectRotationFactors);
	}

	public void UpdateInnerRadius(float _radiusInner)
	{
		radiusInner = _radiusInner;

		if (Application.isPlaying)
		{
			float radiusOuter = radiusInner + warpThickness;
			warpController.UpdateRadius(radiusInner, radiusOuter);
		}
	}

	public void UpdateWarpThickness(float _warpThickness)
	{
		warpThickness = _warpThickness;
		
		if (Application.isPlaying)
		{
			float radiusOuter = radiusInner + warpThickness;
			warpController.UpdateRadius(radiusInner, radiusOuter);
		}
	}
}
