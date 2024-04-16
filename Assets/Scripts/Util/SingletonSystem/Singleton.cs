namespace Util.SingletonSystem
{
	public class Singleton<T> where T : Singleton<T>, new()
	{
		private static T _instance;

		public static T Instance
		{
			get
			{
				if (_instance != null)
					return _instance;
				_instance = new T();
				_instance.Init();
				return _instance;
			}
		}

		protected virtual void Init()
		{
		}
	}
}