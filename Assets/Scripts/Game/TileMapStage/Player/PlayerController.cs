﻿using System;
using DG.Tweening;
using Game.TileMapStage.Tile;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using Util.EventSystem;
using EventType = Util.EventSystem.EventType;

namespace Game.TileMapStage.Player
{
	public class PlayerController : MonoBehaviour, IEventListener
	{
		private bool _isHighlighted;

		public void Start()
		{
			EventManager.Instance.AddListener(EventType.TileClicked, this);
			_isHighlighted = false; 
			//마우스 입력 감지 등록
			var eventTrigger = gameObject.GetOrAddComponent<EventTrigger>();
			var onPointerClickEntry = new EventTrigger.Entry
			{
				eventID = EventTriggerType.PointerClick
			};
			onPointerClickEntry.callback.AddListener(OnPointerClick);
			eventTrigger.triggers.Add(onPointerClickEntry);
		}

		public void OnEvent(EventType eventType, Component sender, object param = null)
		{
			switch (eventType)
			{
				case EventType.StageLoad:
					break;
				case EventType.StageInit:
					break;
				case EventType.GameStart:
					break;
				case EventType.GameOver:
					break;
				case EventType.TileClicked:
					TileClicked((Vector3Int)param);
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
			_isHighlighted = true;
			Debug.Log("PlayerClicked");
			var position = Vector3Int.FloorToInt(transform.position);
			TileManager.Instance.GetTileOnPosition(position + new Vector3Int(1, -1, 0))?.HighlightTile();
			TileManager.Instance.GetTileOnPosition(position + new Vector3Int(-1, -1, 0))?.HighlightTile();
			TileManager.Instance.GetTileOnPosition(position + new Vector3Int(0, -1, 1))?.HighlightTile();
			TileManager.Instance.GetTileOnPosition(position + new Vector3Int(0, -1, -1))?.HighlightTile();
		}

		private void TileClicked(Vector3Int pos)
		{
			if (_isHighlighted)
			{
				var selectTile = TileManager.Instance.GetTileOnPosition(pos);
				if (selectTile == null)
					return;
				var dist = Vector3Int.Distance(Vector3Int.FloorToInt(transform.position), pos + new Vector3Int(0,1,0));
				if (selectTile.Status == TileStatus.Walkable && dist <= 1)
					Move(selectTile.TargetPosition);
			}
			_isHighlighted = false;
		}

		private void Move(Vector3Int pos)
		{
			var dist = Vector3Int.Distance(Vector3Int.FloorToInt(transform.position), pos);
			transform.DOPause();
			transform.DOMove(pos, dist * 0.2f).SetEase(Ease.Linear);
		}
	}
}