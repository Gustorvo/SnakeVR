using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using NaughtyAttributes;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Gustorvo.SnakeVR
{
    public class MoveHandler : MonoBehaviour
    {
        [SerializeField, Range(0, maxMovesPerSecond)]
        int movesPerSecond = 1;

        [SerializeField] private SnakeBehaviour snakeBehaviour;
        [SerializeField] private SnakeTarget snakeTargetBehaviour;
        [SerializeField] Coroutine snakeMoveCoroutine;

        private Vector3 snakeCurrentPosition;
        private const int maxMovesPerSecond = 90;

        public ISnake Snake { get; private set; }

        private void Awake()
        {
            Init();
        }


        private void Start()
        {
            Assert.IsNotNull(snakeBehaviour, "SnakeMoverBehaviour is not set");
            snakeMoveCoroutine = StartCoroutine(MoveSnake());
        }

        void Init()
        {
            Snake = snakeBehaviour;
            Snake.Target = snakeTargetBehaviour;
        }

        void InitAllEditor()
        {
            Init();
            Snake.Init();
        }


        [Button]
        public bool TryMove()
        {
            Snake.Move();


#if UNITY_EDITOR
            if (EditorApplication.isPaused && !Snake.CanMove)
            {
                Debug.LogError("Snake can't move");
            }
#endif

            return Snake.CanMove;
        }


        IEnumerator MoveSnake()
        {
            bool canMove = true;
            while (canMove)
            {
                canMove = TryMove();
                if (maxMovesPerSecond == movesPerSecond)
                    yield return null;
                else if (movesPerSecond == 0)
                    yield return new WaitForSeconds(float.MaxValue);
                else
                    yield return new WaitForSeconds(1f / movesPerSecond);
            }

            Debug.Log($"Snake died. Total snake length: {Snake.Length}");
        }
    }
}