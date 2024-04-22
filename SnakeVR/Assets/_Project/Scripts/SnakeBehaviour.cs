using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

namespace Gustorvo.SnakeVR
{
    public interface ISnake
    {
        public Vector3[] Positions { get; }
        public SnakeBody Head { get; }
        public SnakeBody Tail { get; }

        IPositioner Positioner { get; }
        bool CanMove { get; }
        ITarget Target { get; set; }
        int Length => Positions.Length;
        void Move();
        void Init();
        public bool IsSnakePosition(Vector3 newPos);
    }

    public class SnakeBehaviour : MonoBehaviour, ISnake
    {
        [SerializeField] private bool drawGizmos = false;
        [SerializeField] private SnakeBody snakeBodyPrefab;
        [SerializeField] SnakeBody headPointer;
        [SerializeField] SnakeBody tailPointer;
        public List<SnakeBody> snakeParts = new();
        public IPositioner Positioner { get; private set; } = new AIPositioner();

        public bool CanMove { get; private set; } = true;
        public ITarget Target { get; set; }

        private float distanceToTarget => Vector3.Distance(Head.Position, Target.Position);
        private bool targetValid => Target != null && Target.Transform != null;

        public bool HasReachedTarget =>
            targetValid && (distanceToTarget - Core.CellSize) <= Core.DistanceTolerance;


        private List<Vector3> nextPositions = new List<Vector3>();
        private static SnakeBehaviour instance { get; set; }


        private void Awake()
        {
            Init();
            // AlignToGrid();
        }

        private void Update()
        {
            nextPositions = Positioner.GetMovePositions().ToList();
        }


        public void Init()
        {
            instance = this;

            snakeParts.Clear();
            foreach (Transform child in transform)
            {
                if (!child.TryGetComponent(out SnakeBody body))
                {
                    body = child.gameObject.AddComponent<SnakeBody>();
                }

                snakeParts.Add(body);
            }

            Head = headPointer;
            Tail = tailPointer;
        }

        public bool IsSnakePosition(Vector3 newPos) => Positions.Any(p => p.AlmostEquals(newPos, 0.0001f));


        private void TakeTarget()
        {
            SnakeBody newHead =
                Instantiate(snakeBodyPrefab, parent: transform, position: Target.Position,
                    rotation: Quaternion.identity);
            Target.Reposition();
            snakeParts.Insert(headIndex, newHead);

            Head.ApplyBodyMaterial();
            Head = newHead;
            Head.ApplyHeadMaterial();
        }

        public void Move()
        {
            if (HasReachedTarget)
            {
                TakeTarget();
                return;
            }

            CanMove = false;
            if (Positioner.TryGetMovePosition(out Vector3 newPos))
            {
                CanMove = true;
                Tail.MoveTo(newPos);
                Tail.ApplyHeadMaterial();
                Head.ApplyBodyMaterial();
                Head = Tail;
                Tail = snakeParts[GetPreviousIndex(tailIndex)];
            }
        }


        public int GetPreviousIndex(int index)
        {
            return (index - 1 + snakeParts.Count) % snakeParts.Count;
        }

        public int GetNextIndex(int index)
        {
            return (index + 1 + snakeParts.Count) % snakeParts.Count;
        }


        private int tailIndex => snakeParts.IndexOf(Tail);
        private int headIndex => snakeParts.IndexOf(Head);

        public SnakeBody GetTailPart(out int tailIndex)
        {
            tailIndex = snakeParts.IndexOf(Tail);
            return Tail;
        }


        public Vector3[] Positions => snakeParts.Select(x => x.Position).ToArray();
        public Vector3 HeadDirection => Head.Transform.forward;
        public Vector3 DirectionLocal => Head.Transform.InverseTransformDirection(HeadDirection);

        private SnakeBody tail;

        public SnakeBody Tail
        {
            get => tail;
            set { tail = value; }
        }

        private SnakeBody head;

        public SnakeBody Head
        {
            get => instance?.head;
            set => instance.head = value;
        }


        [Button]
        private void AlignToGrid()
        {
            Init();
            for (int i = 0; i < snakeParts.Count; i++)
            {
                Vector3 nearestPositionInGrid = Core.PlayBoundary.GetNearestPositionInGrid(snakeParts[i].PositionLocal);
                snakeParts[i].MoveToLocal(nearestPositionInGrid);
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (!drawGizmos) return;
            // draw possible next positions
            for (int i = 0; i < nextPositions.Count; i++)
            {
                Vector3 pos = nextPositions[i];
                if (Positioner.IsPositionValid(pos))
                {
                    Gizmos.color = Color.green;
                }
                else if (IsSnakePosition(pos))
                {
                    Gizmos.color = Color.magenta;
                    Debug.LogError("Snake position: " + pos);
                }
                else if (!Core.PlayBoundary.IsPositionInBounds(pos))
                {
                    Gizmos.color = Color.red;
                    Debug.LogError("Position out of bounds: " + pos);
                }
                else
                {
                    Debug.LogError("Position not valid: " + pos);
                    Gizmos.color = Color.yellow;
                }

                // draw a green sphere at the position
                Gizmos.DrawWireSphere(pos, Core.CellSize * 0.5f);
            }


            // // draw head direction
            // if (Head != null)
            // {
            //     var headPossibleDirections = new List<Vector3>
            //     {
            //         Vector3.forward, Vector3.back, Vector3.right, Vector3.left, Vector3.up, Vector3.down
            //     };
            //
            //     Gizmos.color = Color.blue;
            //     foreach (var dir in headPossibleDirections)
            //     {
            //         Gizmos.DrawLine(Head.Position, Head.Position + dir);
            //     }
            //     // Vector3 headDirectionLocal = transform.InverseTransformDirection(Head.Transform.forward);
            //     // Gizmos.DrawLine(Head.Position, Head.Position + headDirectionLocal);
            // }
        }
    }
}