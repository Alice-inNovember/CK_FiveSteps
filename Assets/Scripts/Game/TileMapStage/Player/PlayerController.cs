
using DG.Tweening;
using UnityEngine;

namespace Game.TileMapStage.Player
{
	public class PlayerController : MonoBehaviour
	{
		public void Move(Vector3Int pos)
		{
			var dist = Vector3Int.Distance(Vector3Int.FloorToInt(transform.position), pos);
			transform.DOPause();
			transform.DOMove(pos, dist * 0.2f).SetEase(Ease.Linear);
		}
	}
}