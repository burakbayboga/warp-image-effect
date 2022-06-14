using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(WarpEffectController))]
public class WarpEffectControllerInspector : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		WarpEffectController warpController = target as WarpEffectController;

		EditorGUI.BeginChangeCheck();
		Color _effectTint = EditorGUILayout.ColorField("Effect Tint", warpController.effectTint);
		if (EditorGUI.EndChangeCheck())
		{
			Undo.RecordObject(warpController, "effect tint");
			EditorUtility.SetDirty(warpController);
			warpController.effectTint = _effectTint;
			warpController.UpdateEffectTint(_effectTint);
		}

		EditorGUI.BeginChangeCheck();
		float _warpStrength = EditorGUILayout.Slider("Warp Strength", warpController.warpStrength, 1f, 3f);
		if (EditorGUI.EndChangeCheck())
		{
			Undo.RecordObject(warpController, "warp strength");
			EditorUtility.SetDirty(warpController);
			warpController.warpStrength = _warpStrength;
			warpController.UpdateWarpStrength(_warpStrength);
		}

	}
}
#endif
