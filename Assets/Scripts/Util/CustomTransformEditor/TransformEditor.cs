#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Util.CustomTransformEditor
{
	[CustomEditor(typeof(Transform))]
	public class TransformEditor : Editor
	{
		private static bool _globalFoldOut;

		private object _localRotationGUI; // 사용 이유 : 안쓰면 Euler.x 짐벌락 걸림

		private Texture _refreshTexture;
		private string _texturePath;
		private Transform _transform;
		private MethodInfo _transGuiOnEnableMethod;
		private MethodInfo _transGuiRotationFieldMethod;

		private void OnEnable()
		{
			_transform = target as Transform;

			_refreshTexture =
				AssetDatabase.LoadAssetAtPath("Assets/Scripts/Util/CustomTransformEditor/EditorResources/Refresh.png",
					typeof(Texture2D)) as Texture2D;

			// 치트키 : 기존 TransformEditor로부터 RotationField 빌려쓰기
			if (_localRotationGUI == null)
			{
				var bunch = TransformEditorHelper.GetTransformRotationGUI();
				_localRotationGUI = bunch.rotationGUI;
				_transGuiOnEnableMethod = bunch.OnEnable;
				_transGuiRotationFieldMethod = bunch.RotationField;
			}

			_transGuiOnEnableMethod.Invoke(_localRotationGUI, new object[]
			{
				serializedObject.FindProperty("m_LocalRotation"), EditorGUIUtility.TrTextContent("Local Rotation")
			});
		}

		/***********************************************************************
		*                               Public Methods
		***********************************************************************/

		#region .

		public static void LoadGlobalFoldOutValue(bool value)
		{
			_globalFoldOut = value;
		}

		#endregion

		/***********************************************************************
		*                               Inspector Methods
		***********************************************************************/

		#region .

		public override void OnInspectorGUI()
		{
			// 1. Local Transform
			DrawDefaultTransformInspector();

			EditorGUILayout.Space();

			// 2. Global Transform
			EditorGUI.BeginChangeCheck();
			if (_globalFoldOut == EditorGUILayout.Foldout(_globalFoldOut, "Global"))
			{
				DrawGlobalTransformInspector();
				EditorGUILayout.Space();
			}

			if (EditorGUI.EndChangeCheck()) TransformEditorHelper.SaveGlobalFoldOutPref(_globalFoldOut);

			//base.serializedObject.ApplyModifiedProperties();
		}

		private void DrawDefaultTransformInspector()
		{
			// Reset Button
			var oldBgColor = GUI.backgroundColor;
			var oldTxtColor = GUI.contentColor;
			GUI.backgroundColor = Color.green;
			GUI.contentColor = Color.white;
			if (GUILayout.Button("Reset"))
			{
				_transform.localPosition = Vector3.zero;
				_transform.localScale = Vector3.one;

				TransformUtils.SetInspectorRotation(_transform, Vector3.zero);
			}

			GUI.backgroundColor = oldBgColor;
			GUI.contentColor = oldTxtColor;

			//base.serializedObject.Update();
			// ==================================== Local Position =========================================
			EditorGUILayout.BeginHorizontal();

			Undo.RecordObject(_transform, "Transform Local Position Changed");

			if (_globalFoldOut)
				_transform.localPosition = Vector3E4Round(_transform.localPosition);

			// Local Position Field
			_transform.localPosition = EditorGUILayout.Vector3Field("Local Position", _transform.localPosition);

			// Refresh Button
			DrawRefreshButton(Color.green, () => _transform.localPosition = Vector3.zero);

			EditorGUILayout.EndHorizontal();
			// ==================================== Local Rotation =========================================
			EditorGUILayout.BeginHorizontal();

			Undo.RecordObject(_transform, "Transform Local Rotation Changed");

			// Local Rotation Field
			_transGuiRotationFieldMethod.Invoke(_localRotationGUI, new object[] { });

			// 0~360 값 벗어나면 제한
			var exposedLocalEulerAngle = TransformUtils.GetInspectorRotation(_transform);
			if (exposedLocalEulerAngle.x < 0f || exposedLocalEulerAngle.y < 0f || exposedLocalEulerAngle.z < 0f ||
			    exposedLocalEulerAngle.x > 360f || exposedLocalEulerAngle.y > 360f || exposedLocalEulerAngle.z > 360f)
				TransformUtils.SetInspectorRotation(_transform, _transform.localEulerAngles);

			// Refresh Button
			DrawRefreshButton(Color.green, () => TransformUtils.SetInspectorRotation(_transform, Vector3.zero));

			EditorGUILayout.EndHorizontal();
			// ==================================== Local Scale =========================================
			EditorGUILayout.BeginHorizontal();

			Undo.RecordObject(_transform, "Transform Local Scale Changed");

			if (_globalFoldOut)
				_transform.localScale = Vector3E4Round(_transform.localScale);

			// Local Scale Field
			_transform.localScale = EditorGUILayout.Vector3Field("Local Scale", _transform.localScale);

			// Refresh Button
			DrawRefreshButton(Color.green, () => _transform.localScale = Vector3.one);

			EditorGUILayout.EndHorizontal();
		}

		private void DrawGlobalTransformInspector()
		{
			// Reset Button
			var oldBgColor = GUI.backgroundColor;
			var oldTxtColor = GUI.contentColor;
			GUI.backgroundColor = Color.blue;
			GUI.contentColor = Color.white;
			if (GUILayout.Button("Reset"))
			{
				_transform.position = Vector3.zero;
				_transform.eulerAngles = Vector3.zero;
				ChangeGlobalScale(Vector3.one);
			}

			GUI.backgroundColor = oldBgColor;
			GUI.contentColor = oldTxtColor;

			// ==================================== Global Position =========================================
			EditorGUILayout.BeginHorizontal();

			Undo.RecordObject(_transform, "Transform Global Position Changed");
			_transform.position = EditorGUILayout.Vector3Field("Global Position", Vector3E4Round(_transform.position));

			// Refresh Button
			DrawRefreshButton(Color.blue, () => _transform.position = Vector3.zero);

			EditorGUILayout.EndHorizontal();

			// ==================================== Global Rotation =========================================
			EditorGUILayout.BeginHorizontal();

			Undo.RecordObject(_transform, "Transform Global Rotation Changed");

			var globalRot = _transform.eulerAngles;
			globalRot = EditorGUILayout.Vector3Field("Global Rotation", Vector3E4Round(globalRot));

			// 90 ~ 270 각도 건너뛰기
			if (90f < globalRot.x && globalRot.x < 180f) globalRot.x += 180f;
			else if (180 < globalRot.x && globalRot.x < 270f) globalRot.x -= 180f;

			_transform.eulerAngles = globalRot; //Vector3E4Round(globalRot);

			// Refresh Button
			DrawRefreshButton(Color.blue, () => _transform.eulerAngles = Vector3.zero);

			EditorGUILayout.EndHorizontal();
			// ==================================== Global Scale =========================================
			EditorGUILayout.BeginHorizontal();

			Undo.RecordObject(_transform, "Transform Global Scale Changed");
			var changedGlobalScale =
				EditorGUILayout.Vector3Field("Global Scale", Vector3E4Round(_transform.lossyScale));
			ChangeGlobalScale(changedGlobalScale);

			// Refresh Button
			DrawRefreshButton(Color.blue, () => ChangeGlobalScale(Vector3.one));

			EditorGUILayout.EndHorizontal();
		}

		#endregion

		/***********************************************************************
		*                               Private Methods
		***********************************************************************/

		#region .

		/// <summary> 버튼을 그립니당 </summary>
		private void DrawRefreshButton(in Color color, Action action)
		{
			var oldBgColor = GUI.backgroundColor;
			GUI.backgroundColor = color;
			if (GUILayout.Button(_refreshTexture, GUILayout.Width(20f), GUILayout.Height(18f))) action();
			GUI.backgroundColor = oldBgColor;
		}

		private readonly Queue<Vector3> _scaleHierarchy = new();

		private void ChangeGlobalScale(Vector3 globalScale)
		{
			_scaleHierarchy.Clear();
			var parentTr = _transform.parent;
			while (parentTr != null)
			{
				_scaleHierarchy.Enqueue(parentTr.localScale);
				parentTr = parentTr.parent;
			}

			while (_scaleHierarchy.Count > 0)
			{
				var current = _scaleHierarchy.Dequeue();
				var x = globalScale.x / current.x;
				var y = globalScale.y / current.y;
				var z = globalScale.z / current.z;
				globalScale.Set(x, y, z);
			}

			_transform.localScale = globalScale;
		}

		/// <summary> 소수 4번째 자리까지 반올림 </summary>
		private static Vector3 Vector3E4Round(Vector3 vector)
		{
			vector.x = Mathf.Round(vector.x * 10000f) * 0.0001f;
			vector.y = Mathf.Round(vector.y * 10000f) * 0.0001f;
			vector.z = Mathf.Round(vector.z * 10000f) * 0.0001f;
			return vector;
		}

		#endregion
	}
}
#endif