using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using Util;
using Util.EventSystem;
using EventType = Util.EventSystem.EventType;

namespace Game.TileMapStage.Tile
{
	public class Tile : MonoBehaviour, IEventListener
	{
		[SerializeField] protected TileType type;
		[SerializeField] protected Vector3Int targetPosition;
		[SerializeField] protected Tile topTile;
		[SerializeField] protected Tile bottomTile;
		private bool _isHighlighted;
		private Vector3Int _position;
		public TileType Type => type;
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

		private void OnPointerClick(BaseEventData data)
		{
			EventManager.Instance.PostNotification(EventType.TileClicked, this, _position);
		}

		private void InitTile()
		{
			_position = Vector3Int.FloorToInt(transform.position);
			TileManager.Instance.RegisterTile(_position, this);

			_isHighlighted = false;
			targetPosition = _position + new Vector3Int(0, 1, 0);

			var eventTrigger = gameObject.GetOrAddComponent<EventTrigger>();
			var onPointerClickEntry = new EventTrigger.Entry
			{
				eventID = EventTriggerType.PointerClick
			};
			onPointerClickEntry.callback.AddListener(OnPointerClick);
			eventTrigger.triggers.Add(onPointerClickEntry);
		}
		
		public static bool IsTileWalkable(TileType tileType)
		{
			switch(tileType)
			{
				case TileType.Walkable:
				case TileType.Stair:
				case TileType.StairFloor:
					return true;
				case TileType.Blocked:
				default:
					return false;
			}
		}

		public Vector3[] GetStairPath(bool isUpward)
		{
			var path = new Vector3[3];
			var position = this.transform.position;

			if (isUpward)
			{
				path[0] = (position + bottomTile.TargetPosition) / 2;
				path[1] = (position + new Vector3(0, 1, 0) + topTile.TargetPosition) / 2;
				path[2] = topTile.TargetPosition;
			}
			else
			{
				path[0] = (position + new Vector3(0, 1, 0) + topTile.TargetPosition) / 2;
				path[1] = (position + bottomTile.TargetPosition) / 2;
				path[2] = bottomTile.TargetPosition;
			}
			return path;
		}

		public void HighlightTile()
		{
			if (type == TileType.StairFloor)
			{
				TileManager.Instance.GetTileOnPosition(_position + new Vector3Int(0,1,0)).HighlightTile();
				return;
			}
			if (_isHighlighted || !IsTileWalkable(type))
				return;
			_isHighlighted = true;
			GetComponent<Renderer>().material
				.SetColor(Shader.PropertyToID("_EmissionColor"), new Color(1f, 1f, 1f));
		}

		public void HighlightReset()
		{
			if (!_isHighlighted)
				return;
			_isHighlighted = false;
			GetComponent<Renderer>().material.SetColor(Shader.PropertyToID("_EmissionColor"), Color.black);
		}
	}
}