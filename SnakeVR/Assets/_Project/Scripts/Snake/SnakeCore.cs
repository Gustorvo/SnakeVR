using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gustorvo.SnakeVR
{
    public class SnakeCore : MonoBehaviour
    {
        [SerializeField] int randomSeed = 0;
        [SerializeField, Range(0.05f, 0.2f)] private float cellSize = 0.1f;
        [field: SerializeField] public RoomBoundary grid { get; private set; }
        [field: SerializeField] public SnakeBehaviour snake { get; private set; }


        public static RoomBoundary Grid => Instance.grid;
        public static SnakeBehaviour Snake => Instance.snake;
        public static float CellSize => Instance.cellSize;
        public static float DistanceTolerance { get; private set; }

        public static List<Vector3> MoveDirections;

        public static event Action OmRoomBoundsReady;

        #region Singleton

        private static SnakeCore instance { get; set; }

        public static SnakeCore Instance
        {
            get
            {
                if (instance == null)
                    instance = FindObjectOfType<SnakeCore>();
                return instance;
            }
            set => instance = value;
        }

        #endregion


        private void Awake()
        {
            Instance = this;
            Random.InitState(randomSeed);

            // calculate the distance tolerance between cells, useful when trying to compare cell positions
            DistanceTolerance =  CellDiagonalDistance() - CellSize - 0.01f;
            Debug.Log("DistanceTolerance: " + DistanceTolerance);
            
            MoveDirections = new List<Vector3>()
                { Vector3.forward, Vector3.back, Vector3.right, Vector3.left, Vector3.up, Vector3.down };
            
          grid.OnBoundaryReady += Init;
        }

        private void OnDestroy()
        {
            grid.OnBoundaryReady -= Init;
        }

        private void Init()
        {
           Snake.Init();
        }

        private static float CellDiagonalDistance()
        {
            // calculate the diagonal distance between cells
            return CellSize * Mathf.Sqrt(2);
        }
    }
}