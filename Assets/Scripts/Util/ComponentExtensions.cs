using UnityEngine;

namespace Util
{
	public static class ComponentExtensions
	{
		public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
		{
			var component = gameObject.GetComponent<T>();

			if (component == null)
				component = gameObject.AddComponent<T>();

			return component;
		}
	}
}