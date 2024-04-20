using System;
using System.Collections.Generic;
using System.Linq;
using Gustorvo.SnakeVR.Input;
using UnityEngine;

namespace Gustorvo.SnakeVR
{
    public interface IPositioner
    {
        public bool TryGetMovePosition(out Vector3 movePosition);
        public IEnumerable<Vector3> GetMovePositions();
        public Vector3 GetPositionInDirection(Vector3 direction);
        bool IsPositionValid(Vector3 newPos);
    }

    public class Positioner : IPositioner
    {
        private PlayBoundary boundary => Core.PlayBoundary;
        private SnakeBehaviour snake => Core.Snake;
        private float moveStep => Core.CellSize;


        public bool TryGetMovePosition(out Vector3 movePosition)
        {
            Vector3 direction = SnakeMoveDirection.Direction;
            movePosition = GetPositionInDirection(direction);
            return IsPositionValid(movePosition);
        }


        public Vector3 GetPositionInDirection(Vector3 direction)
        {
            return snake.Head.Position + direction * moveStep;
        }

        public bool IsPositionValid(Vector3 newPos)
        {
            return
                boundary.IsPositionInBounds(newPos)
                && !snake.IsSnakePosition(newPos);
        }

        #region Not implemented

        public IEnumerable<Vector3> GetMovePositions()
        {
            throw new NotImplementedException();
        }

        #endregion
    }


    public class AIPositioner : IPositioner
    {
        private IPositioner positioner = new Positioner();
        PlayBoundary boundary => Core.PlayBoundary;
        ISnake snake => Core.Snake;


        public Vector3 GetPositionInDirection(Vector3 direction) =>
            positioner.GetPositionInDirection(direction);

        public bool IsPositionValid(Vector3 newPos) =>
            positioner.IsPositionValid(newPos);

        public bool TryGetMovePosition(out Vector3 movePosition)
        {
            movePosition = GetMovePositions().FirstOrDefault();
            return IsPositionValid(movePosition);
        }


        public IEnumerable<Vector3> GetMovePositions()
        {
            // Apply world rotation of Play boundary
            var rotation = Core.PlayBoundary.transform.rotation;
            var moveDirections = Core.MoveDirections.Select(direction => rotation * direction);

            // Get positions for each direction
            var positions = moveDirections.Select(direction => GetPositionInDirection(direction));

            // Remove positions that are out of bounds
            positions = positions.Where(p => boundary.IsPositionInBounds(p));

            // Remove positions that are occupied by the snake
            positions = positions.Where(p => !snake.IsSnakePosition(p));

            //sort position by distance to target
            positions = positions.OrderBy(p => Vector3.Distance(snake.Target.Position, p));
            if (positions.Count() == 0)
            {
                Debug.LogError("No possible positions");
            }

            return positions;
        }
    }
}