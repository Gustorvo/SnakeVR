using System;
using System.Collections.ObjectModel;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gustorvo.SnakeVR
{
    public class PlayBoundary : MonoBehaviour
    {
        [SerializeField] bool drawGizmos;
        [SerializeField] private Transform debugCube;


        private ReadOnlyCollection<Vector3> readOnlyCellArray;
        public Vector3 Forward => transform.forward;
        public Vector3 Right => transform.right;
        public Vector3 Up => transform.up;
        public Vector3 Position => transform.position;
        public Quaternion Rotation => transform.rotation;


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
            BuildGridOfCellsWithinBounds();
        }

        [SerializeField, Range(0.2f, 2)] private float boxSize = 1f;
        [ShowNativeProperty] Bounds gameBounds => new Bounds(Vector3.zero, Vector3.one * boxSize);
        [ShowNativeProperty] int itemsInRow => Mathf.CeilToInt(gameBounds.size.x / cellSize);

        [ShowNativeProperty] private float cellSize => Core.CellSize;
        public Bounds bounds => gameBounds;

        private ReadOnlyCollection<Vector3> BuildGridOfCellsWithinBounds()
        {
            float halfCellSize = cellSize / 2;
            Vector3 offset = new Vector3(halfCellSize, halfCellSize, halfCellSize);
            int totalItemsInBoundingBox = itemsInRow * itemsInRow * itemsInRow;
            Vector3[] cellArray = new Vector3[totalItemsInBoundingBox];
            for (int i = 0; i < totalItemsInBoundingBox; i++)
            {
                int j = i % itemsInRow;
                int k = (i / itemsInRow) % itemsInRow;
                int l = i / (itemsInRow * itemsInRow);

                float x = gameBounds.min.x + cellSize * j;
                float y = gameBounds.min.y + cellSize * k;
                float z = gameBounds.min.z + cellSize * l;

                cellArray[i] = new Vector3(x, y, z) + offset;
            }

            return Array.AsReadOnly(cellArray);
        }

        [Button]
        void InstantiateCubesInCellPositions()
        {
            GameObject debugCubeParent = new GameObject("DebugCubesParent");
            for (int i = 0; i < CellPositions.Count; i++)
            {
                Instantiate(debugCube, position: CellPositions[i], rotation: Quaternion.identity,
                    parent: debugCubeParent.transform).localScale = Vector3.one * cellSize;
            }

            var pos = Core.PlayBoundary.transform.position;
            var rot = Core.PlayBoundary.transform.rotation;
            debugCubeParent.transform.SetPositionAndRotation(pos, rot);
        }

        public bool TryGetRandomPositionExcluding(Vector3[] excludePositions, out Vector3 randomPosition)
        {
            randomPosition = Vector3.zero;
            Vector3 tempPosition = Vector3.zero;

            if (excludePositions.Length >= CellPositions.Count)
            {
                Debug.LogError("Too many positions to exclude");
                return false;
            }

            int i = 0;
            do
            {
                i++;

                int randomIndex = Random.Range(0, CellPositions.Count());
                tempPosition = CellPositions[randomIndex];
                tempPosition = transform.TransformPoint(tempPosition);
            } while (excludePositions.Any(x => x.AlmostEquals(tempPosition, 0.0001f)) && i < 100);


            if (i >= 100)
            {
                Debug.LogError("Failed to get random position");
                return false;
            }

            randomPosition = tempPosition;
            // PrintSomeDebug();


            return true;

            // void PrintSomeDebug()
            // {
            //     // find the number of all items in cell array that are almost equal to excludePositions array
            //     int count = CellPositions.Count(cell =>
            //         excludePositions.Any(excludePosition => excludePosition.AlmostEquals(cell, 0.0001f)));
            //     Debug.Log($"Number of items in cell array that are almost equal to excludePositions array: {count}");
            // }
        }

        public bool IsPositionInBounds(Vector3 position)
        {
            position = transform.InverseTransformPoint(position);
            return gameBounds.Contains(position);
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

            // draw the bounds
            Gizmos.DrawWireCube(gameBounds.center, gameBounds.size);

            // Revert back to the original matrix
            Gizmos.matrix = originalMatrix;
        }
    }
}