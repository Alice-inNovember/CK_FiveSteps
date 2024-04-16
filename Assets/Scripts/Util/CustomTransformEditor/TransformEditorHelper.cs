#if UNITY_EDITOR

using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace Util.CustomTransformEditor
{
	[InitializeOnLoad]
	public static class TransformEditorHelper
	{
		static TransformEditorHelper()
		{
			InitFolderPath();
			// Load Adv Foldout Value
			TransformEditor.LoadGlobalFoldOutValue(EditorPrefs.GetBool(GlobalPrefName, false));
		}

		public static string FolderPath { get; private set; }

		private static void InitFolderPath([CallerFilePath] string sourceFilePath = "")
		{
			FolderPath = Path.GetDirectoryName(sourceFilePath);
			if (FolderPath == null)
				return;
			var rootIndex = FolderPath.IndexOf(@"Assets\", StringComparison.Ordinal);
			if (rootIndex > -1) FolderPath = FolderPath.Substring(rootIndex, FolderPath.Length - rootIndex);
		}

		public static (object rotationGUI, MethodInfo OnEnable, MethodInfo RotationField) GetTransformRotationGUI()
		{
			var targetType = Type.GetType("UnityEditor.TransformRotationGUI, UnityEditor");
			var target = Activator.CreateInstance(targetType);

			var onEnableMethod = targetType.GetMethod(
				"OnEnable",
				new[] { typeof(SerializedProperty), typeof(GUIContent) }
			);
			var rotationFieldMethod = targetType.GetMethod("RotationField", new Type[] { });

			return (target, onEnableMethod, rotationFieldMethod);
		}

		/***********************************************************************
		*                               EditorPrefs
		***********************************************************************/

		#region .

		private const string GlobalPrefName = "TE_GlobalFoldOut";

		public static void SaveGlobalFoldOutPref(bool value)
		{
			EditorPrefs.SetBool(GlobalPrefName, value);
		}

		#endregion
	}
}

#endif