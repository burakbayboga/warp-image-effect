using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WarpEffectController))]
public class Warp_Pulse : MonoBehaviour
{
	[Range(0.05f, 0.3f)]
	public float warpThickness = 0.05f;
	public float effectRotationSpeed = 400f;
	[Range(0.05f, 0.6f)]
	public float maxRadius = 0.3f;
	public float timeToMaxRadius = 2.2f;

	private WarpEffectController warpController;

	private float warpStartTime = 0f;
	private float currentEffectRotation = 0f;

	private bool shrinking;

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

			warpStartTime = Time.time;
			shrinking = false;
		}

		UpdateRadius();
		UpdateRotation();
	}

	private void UpdateRadius()
	{
		float timePassed = Time.time - warpStartTime;
		float t = timePassed / timeToMaxRadius;
		float radiusInner;
		if (shrinking)
		{
			radiusInner = Mathf.Lerp(maxRadius, 0f, t);
		}
		else
		{
			radiusInner = Mathf.Lerp(0f, maxRadius, t);
		}
		float radiusOuter = radiusInner + warpThickness;

		warpController.UpdateRadius(radiusInner, radiusOuter);

		if (timePassed >= timeToMaxRadius)
		{
			shrinking = !shrinking;
			warpStartTime = Time.time;
		}
	}

	private void UpdateRotation()
	{
		if (shrinking)
		{
			currentEffectRotation += Time.deltaTime * effectRotationSpeed;
		}
		else
		{
			currentEffectRotation -= Time.deltaTime * effectRotationSpeed;
		}

		float effectRotationRadians = currentEffectRotation * Mathf.Deg2Rad;
		Vector2 effectRotationFactors = new Vector2(Mathf.Cos(effectRotationRadians), Mathf.Sin(effectRotationRadians));

		warpController.UpdateRotation(effectRotationFactors);
	}
}
