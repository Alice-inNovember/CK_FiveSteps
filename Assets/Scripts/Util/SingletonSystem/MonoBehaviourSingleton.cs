using UnityEngine;

namespace Util.SingletonSystem
{
	public class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviourSingleton<T>
	{
		private static T _instance;
		public static T Instance => _instance == null ? null : _instance;

		protected virtual void Awake()
		{
			if (_instance == null)
			{
				_instance = (T)this;
				DontDestroyOnLoad(gameObject);
			}
			else
			{
				Destroy(gameObject);
			}
		}
	}
}