using System;
using UnityEngine;
using Util.EventSystem;
using EventType = Util.EventSystem.EventType;

namespace TileMapStage.Tile
{
	public class Tile : MonoBehaviour, IEventListener
	{
		[SerializeField] private TileStatus status;
		[SerializeField] private Vector3Int position;

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

		private void InitTile()
		{
			var worldPos = transform.position;
			position = new Vector3Int((int)worldPos.x, (int)worldPos.y, (int)worldPos.z);
			TileManager.Instance.RegisterTile(position, this);
		}
	}
}