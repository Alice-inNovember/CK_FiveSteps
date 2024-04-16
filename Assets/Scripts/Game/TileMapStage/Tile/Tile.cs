using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Util;
using Util.EventSystem;
using EventType = Util.EventSystem.EventType;

namespace Game.TileMapStage.Tile
{
	public class Tile : MonoBehaviour, IEventListener
	{
		[SerializeField] private TileStatus status;
		[SerializeField] private Vector3Int targetPosition;
		private Vector3Int _position;
		public TileStatus Status => status;
		public Vector3Int TargetPosition => targetPosition;

		private void Start()
		{
			EventManager.Instance.AddListener(EventType.StageInit, this);
		}

		public void OnEvent(EventType eventType, Component sender, object param = null)
		{
			switch (eventType)
			{
				case EventType.StageLoad:
					break;
				case EventType.StageInit:
					InitTile();
					break;
				case EventType.GameStart:
					break;
				case EventType.GameOver:
					break;
				case EventType.DataSave:
					break;
				case EventType.DataLoad:
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(eventType), eventType, null);
			}
		}

		public void OnPointerClick(BaseEventData data)
		{
			GameManager.Instance.TileClicked(_position, this);
		}

		private void InitTile()
		{
			var worldPos = transform.position;
			_position = new Vector3Int((int)worldPos.x, (int)worldPos.y, (int)worldPos.z);
			TileManager.Instance.RegisterTile(_position, this);

			targetPosition = _position + new Vector3Int(0, 1, 0);
			
			var eventTrigger = gameObject.GetOrAddComponent<EventTrigger>();
			var onPointerClickEntry = new EventTrigger.Entry
			{
				eventID = EventTriggerType.PointerClick
			};
			onPointerClickEntry.callback.AddListener(OnPointerClick);
			eventTrigger.triggers.Add(onPointerClickEntry);
		}
	}
}