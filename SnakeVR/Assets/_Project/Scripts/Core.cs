using System.Collections.Generic;
using UnityEngine;

namespace Gustorvo.SnakeVR
{
    public class Core : MonoBehaviour
    {
        [SerializeField] int randomSeed = 0;
        [SerializeField, Range(0.05f, 0.2f)] private float cellSize = 0.1f;
        [field: SerializeField] public PlayBoundary playBoundary { get; private set; }
        [field: SerializeField] public SnakeBehaviour snake { get; private set; }


        public static PlayBoundary PlayBoundary => Instance.playBoundary;
        public static SnakeBehaviour Snake => Instance.snake;
        public static float CellSize => Instance.cellSize;
        public static float DistanceTolerance { get; private set; }

        public static List<Vector3> MoveDirections;

        #region Singleton

        private static Core instance { get; set; }

        public static Core Instance
        {
            get
            {
                if (instance == null)
                    instance = FindObjectOfType<Core>();
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
        }

        private static float CellDiagonalDistance()
        {
            // calculate the diagonal distance between cells
            return CellSize * Mathf.Sqrt(2);
        }
    }
}