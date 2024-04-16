using Game.TileMapStage.Player;
using Game.TileMapStage.Tile;
using UnityEngine;
using Util.EventSystem;
using Util.SingletonSystem;
using EventType = Util.EventSystem.EventType;

namespace Game
{
	public class GameManager : MonoBehaviourSingleton<GameManager>
	{
		[SerializeField] private PlayerController playerController;
		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Space))
			{
				StageTest();
			}
		}

		private void StageTest()
		{
			EventManager.Instance.PostNotification(EventType.StageLoad, this);
			EventManager.Instance.PostNotification(EventType.StageInit, this);
			Debug.Log("StageTestInit Complete!");
		}

		public void TileClicked(Vector3Int pos, Tile tile = null)
		{
			Debug.Log("Tile Clicked At : " + pos);
			if (tile == null)
				return;
			if (tile.Status == TileStatus.Walkable)
				playerController.Move(tile.TargetPosition);
		}
	}
}