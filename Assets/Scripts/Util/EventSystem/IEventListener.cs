using UnityEngine;

namespace Util.EventSystem
{
	public interface IEventListener
	{
		void OnEvent(EventType eventType, Component sender, object param = null);
	}
}