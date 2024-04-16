using System;
using System.Collections.Generic;
using UnityEngine;
using Util.EventSystem;
using Util.SingletonSystem;
using EventType = Util.EventSystem.EventType;

namespace Game.TileMapStage.Tile
{
	public class TileManager : MonoBehaviourSingleton<TileManager>, IEventListener
	{
		private Dictionary<Vector3Int, Tile> _tiles;

		public void Start()
		{
			_tiles = new Dictionary<Vector3Int, Tile>();
			EventManager.Instance.AddListener(EventType.StageLoad, this);
		}

		public void OnEvent(EventType eventType, Component sender, object param = null)
		{
			switch (eventType)
			{
				case EventType.StageLoad:
					ResetTileDictionary();
					break;
				case EventType.StageInit:
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

		private void ResetTileDictionary()
		{
			_tiles = null;
			_tiles = new Dictionary<Vector3Int, Tile>();
		}

		public void RegisterTile(Vector3Int pos, Tile tile)
		{
			_tiles.Add(pos, tile);
		}

		public Tile GetTileOnPosition(Vector3Int pos)
		{
			return _tiles.TryGetValue(pos, out var tile) ? tile : null;
		}
	}
}