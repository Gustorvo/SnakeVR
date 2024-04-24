using NaughtyAttributes;
using UnityEngine;

namespace Gustorvo.SnakeVR
{
    public interface ITarget
    {
        Vector3 Position { get; }
        Transform Transform { get; }

        void Reposition();
    }

    public class SnakeTarget : MonoBehaviour, ITarget
    {
        [SerializeField] bool repositionOnStart = true;
        public Vector3 Position => transform.position;
        public Transform Transform => transform;

        private void Start()
        {
            if (repositionOnStart)
                Reposition();
        }

        [Button]
        public void Reposition()
        {
            var snakePositions = SnakeCore.Snake.Positions;
            if (SnakeCore.Grid.TryGetRandomPositionExcluding(snakePositions, out var randomPosition))
            {
                transform.position = randomPosition;
            }
            else
            {
                Debug.LogError("Failed to get random position");
            }
        }
        [Button]
        private void AlignToGrid()
        {
            // get local position in grid
          //  var posLocal = transform.InverseTransformPoint(Position);
            transform.localPosition = SnakeCore.Grid.GetNearestPositionInGrid(transform.localPosition);
        }
    }
}