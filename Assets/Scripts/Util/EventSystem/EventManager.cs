using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Util.SingletonSystem;

namespace Util.EventSystem
{
	public class EventManager : MonoBehaviourSingleton<EventManager>
	{
		//이벤트 리스너 리스트 관리
		private Dictionary<EventType, List<IEventListener>> _listeners = new();

		private void OnEnable()
		{
			SceneManager.sceneLoaded += SceneManager_sceneLoaded;
		}

		private void OnDisable()
		{
			SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
		}

		private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
		{
			RefreshListeners();
		}

		public void AddListener(EventType eventType, IEventListener listener)
		{
			// Debug.Log("AddListener : " + eventType);
			if (_listeners.TryGetValue(eventType, out var listenList))
			{
				listenList.Add(listener);
				return;
			}

			listenList = new List<IEventListener> { listener };
			_listeners.Add(eventType, listenList);
		}

		public void PostNotification(EventType eventType, Component sender, object param = null)
		{
			//이벤트 리스너(대기자)가 없으면 그냥 리턴.
			if (!_listeners.TryGetValue(eventType, out var listenList))
				return;

			//모든 이벤트 리스너(대기자)에게 이벤트 전송.
			foreach (var t in listenList.Where(t => !t.Equals(null)))
				t.OnEvent(eventType, sender, param);
		}

		public void RemoveEvent(EventType eventType)
		{
			_listeners.Remove(eventType);
		}

		private void RefreshListeners()
		{
			//임시 Dictionary 생성
			var tmpListeners = new Dictionary<EventType, List<IEventListener>>();

			//씬이 바뀜에 따라 리스너가 Null이 된 부분을 삭제해준다.
			foreach (var item in _listeners)
			{
				for (var i = item.Value.Count - 1; i >= 0; i--)
					if (item.Value[i].Equals(null))
						item.Value.RemoveAt(i);

				if (item.Value.Count > 0)
					tmpListeners.Add(item.Key, item.Value);
			}

			//살아있는 리스너는 다시 넣어준다.
			_listeners = tmpListeners;
		}
	}
}