using UnityEngine;

namespace Gustorvo.SnakeVR.Input
{
    public class SnakeMoveDirection : MonoBehaviour
    {
        public static Vector3 Direction {get; set; } = Vector3.forward;

        private static SnakeMoveDirection instance;
        public static SnakeMoveDirection Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<SnakeMoveDirection>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("SnakeMoveDirection");
                        go.AddComponent<SnakeMoveDirection>();
                    }
                }

                return instance;
            }
        }
        
        public void SetDirection(int newDirectionIndex)
        {
            Vector3 newVector = Vector3.zero;
            switch (newDirectionIndex)
            {
                case 0: newVector = Vector3.forward; break;
                case 1: newVector = Vector3.back; break;
                case 2: newVector = Vector3.left; break;
                case 3: newVector = Vector3.right; break;
                case 4: newVector = Vector3.up; break;
                case 5: newVector = Vector3.down; break;
            }

            Direction = newVector;
        }
    }
}