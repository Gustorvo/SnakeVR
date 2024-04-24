using UnityEngine;

namespace Gustorvo.SnakeVR
{
    public class TransformFollower : MonoBehaviour
    {
        public Transform target;
        void Update()
        {
            if (target == null) return;
            transform.position = target.position;
            transform.rotation = target.rotation;
        }
    }
}
