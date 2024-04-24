using System;
using System.Collections.ObjectModel;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gustorvo.SnakeVR
{
    public class RoomBoundary : MonoBehaviour
    {
        [SerializeField] bool drawGizmos;
        [SerializeField] private Transform debugCube;
        [SerializeField] LayerMask wallLayer;
        [SerializeField] LayerMask furnitureLayer;

        public event Action OnBoundaryReady;


        private ReadOnlyCollection<Vector3> readOnlyCellArray;


        public ReadOnlyCollection<Vector3> CellPositions
        {
            get
            {
                if (readOnlyCellArray == null)
                {
                    readOnlyCellArray = BuildGridOfCellsWithinBounds();
                }

                return readOnlyCellArray;
            }
        }


        private void Awake()
        {
            RoomManager.OnRoomBoundsSet += Init;
        }

        private void OnDestroy()
        {
            RoomManager.OnRoomBoundsSet -= Init;
        }

        private void Init(Bounds bounds)
        {
            this.bounds = bounds;
            BuildGridOfCellsWithinBounds();
            OnBoundaryReady?.Invoke();
        }

        [ShowNativeProperty] int itemsInRow => Mathf.CeilToInt(bounds.size.x / cellSize);

        [ShowNativeProperty] private float cellSize => SnakeCore.CellSize;
        public Bounds bounds { get; private set; }

        private ReadOnlyCollection<Vector3> BuildGridOfCellsWithinBounds()
        {
            float halfCellSize = cellSize / 2f;
            Vector3 offset = new Vector3(halfCellSize, halfCellSize, halfCellSize);

            int itemsInRow = Mathf.CeilToInt(bounds.size.x / cellSize);
            int itemsInColumn = Mathf.CeilToInt(bounds.size.y / cellSize);
            int itemsInDepth = Mathf.CeilToInt(bounds.size.z / cellSize);

            int totalItemsInBoundingBox = itemsInRow * itemsInColumn * itemsInDepth;
            Vector3[] cellArray = new Vector3[totalItemsInBoundingBox];

            for (int i = 0; i < totalItemsInBoundingBox; i++)
            {
                int xIndex = i % itemsInRow;
                int yIndex = (i / itemsInRow) % itemsInColumn;
                int zIndex = i / (itemsInRow * itemsInColumn);

                float x = bounds.min.x + cellSize * xIndex;
                float y = bounds.min.y + cellSize * yIndex;
                float z = bounds.min.z + cellSize * zIndex;

                cellArray[i] = new Vector3(x, y, z) + offset;
            }

            return Array.AsReadOnly(cellArray);
        }

        public bool TryGetRandomPositionExcluding(Vector3[] excludePositions, out Vector3 randomPosition)
        {
            var random = GetRandomPosition();
            ;
            randomPosition = random;

            if (excludePositions.Length >= CellPositions.Count)
            {
                Debug.LogError("Too many positions to exclude");
                return false;
            }

            int i = 0;

            while (excludePositions.Any(x => x.AlmostEquals(random, 0.0001f)) && i++ < 1000)
                random = transform.TransformPoint(GetRandomPosition());
            randomPosition = random;


            if (i >= 1000)
            {
                Debug.LogError("Failed to get random position");
                return false;
            }

            return true;
        }

        public bool IsPositionInBounds(Vector3 position)
        {
            position = transform.InverseTransformPoint(position);
            return bounds.Contains(position) && !IsInsideFurniture(position);
        }

        public bool IsInsideFurniture(Vector3 position)
        {
            bool isInFurniture = Physics.CheckBox(position, Vector3.one * (SnakeCore.CellSize * 05f),
                transform.rotation, furnitureLayer);
            bool isBelowFurniture = Physics.Raycast(position, Vector3.up, Mathf.Infinity, furnitureLayer);
            return isInFurniture || isBelowFurniture;
        }

        public Vector3 GetNearestPositionInGrid(Vector3 position)
        {
            return CellPositions.OrderBy(cell => (cell - position).sqrMagnitude).First();
        }

        public Vector3[] GetMiddlePositionsOfEachSide(bool world = true)
        {
            Vector3 center = bounds.center;
            Vector3 extents = bounds.extents;

            Vector3 worldCenter = world ? transform.TransformPoint(center) : center;
            Vector3[] midPoints = new Vector3[6];

            // Transform each extents offset by the localToWorldMatrix to include rotation
            Vector3 topOffset = transform.localToWorldMatrix.MultiplyVector(new Vector3(0, extents.y, 0));
            Vector3 rightOffset = transform.localToWorldMatrix.MultiplyVector(new Vector3(extents.x, 0, 0));
            Vector3 forwardOffset = transform.localToWorldMatrix.MultiplyVector(new Vector3(0, 0, extents.z));

            // Top
            midPoints[0] = worldCenter + topOffset;
            // Bottom
            midPoints[1] = worldCenter - topOffset;
            // Left
            midPoints[2] = worldCenter - rightOffset;
            // Right
            midPoints[3] = worldCenter + rightOffset;
            // Front
            midPoints[4] = worldCenter + forwardOffset;
            // Back
            midPoints[5] = worldCenter - forwardOffset;

            return midPoints;
        }

        private void OnDrawGizmos()
        {
            if (!drawGizmos) return;
            Gizmos.color = Color.green;
            // Store the current matrix to revert back later
            Matrix4x4 originalMatrix = Gizmos.matrix;

            // Apply the object's rotation and position to the Gizmos.matrix
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);

            foreach (var cell in CellPositions)
            {
                Gizmos.DrawWireCube(cell, cellSize * Vector3.one);
            }

            // draw the bounds
            Gizmos.DrawWireCube(bounds.center, bounds.size);

            // Revert back to the original matrix
            Gizmos.matrix = originalMatrix;
        }

        public Vector3 GetRandomPosition()
        {
            int maxIter = 0;
            int randomIndex = Random.Range(0, CellPositions.Count());
            Vector3 newPosition = transform.TransformPoint(CellPositions[randomIndex]);
            while (IsInsideFurniture(newPosition) && maxIter++ < 1000)
            {
                randomIndex = Random.Range(0, CellPositions.Count());
                newPosition = transform.TransformPoint(CellPositions[randomIndex]);
            }

            if (IsInsideFurniture(newPosition))
            {
                Debug.LogError("Failed to get random position");
                return Vector3.zero;
            }

            return newPosition;
        }
    }
}