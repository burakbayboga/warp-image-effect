using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(Warp_Stationary))]
public class Warp_StationaryInspector : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		Warp_Stationary warp_stationary = target as Warp_Stationary;

		EditorGUI.BeginChangeCheck();
		float _radiusInner = EditorGUILayout.Slider("Inner Radius", warp_stationary.radiusInner, 0f, 0.5f);
		if (EditorGUI.EndChangeCheck())
		{
			Undo.RecordObject(warp_stationary, "inner radius");
			EditorUtility.SetDirty(warp_stationary);
			warp_stationary.radiusInner = _radiusInner;
			warp_stationary.UpdateInnerRadius(_radiusInner);
		}

		EditorGUI.BeginChangeCheck();
		float _warpThickness = EditorGUILayout.Slider("Warp Thickness", warp_stationary.warpThickness, 0.05f, 0.3f);
		if (EditorGUI.EndChangeCheck())
		{
			Undo.RecordObject(warp_stationary, "warp thickness");
			EditorUtility.SetDirty(warp_stationary);
			warp_stationary.warpThickness = _warpThickness;
			warp_stationary.UpdateWarpThickness(_warpThickness);
		}
	}
}
#endif
