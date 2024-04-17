using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Util;
using Util.EventSystem;
using EventType = Util.EventSystem.EventType;

namespace Game.TileMapStage.Tile
{
	public class Tile : MonoBehaviour, IEventListener
	{
		[SerializeField] protected TileStatus status;
		[SerializeField] protected Vector3Int targetPosition;
		[SerializeField] protected List<Vector3> stairPath;
		private bool _isHighlighted;
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

		public void HighlightTile()
		{
			Debug.Log("HighlightTile : " + _position);
			if (_isHighlighted || (status != TileStatus.Walkable))
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