using System;
using UnityEngine;

namespace Gustorvo.SnakeVR
{
    [Serializable]
    public class SnakeBody : SnakeBodyBase
    {
        [SerializeField] Material headMaterial;
        [SerializeField] Material bodyMaterial;
        [SerializeField] Material tailMaterial;
        public Vector3 Position => transform.position;
        public Vector3 PositionLocal => transform.localPosition;
        public Transform Transform => transform;
        private Renderer renderer;

        private void Init()
        {
            renderer = transform.GetComponentInChildren<Renderer>();
            transform.localRotation = Quaternion.identity;
        }

        private void Awake()
        {
            Init();
        }

       
        public void MoveTo(Vector3 moveTo)
        {
            transform.position = moveTo;
        }
        
        public void MoveToLocal(Vector3 moveTo)
        {
            transform.localPosition = moveTo;
        }

        public void ApplyHeadMaterial()
        {
            renderer.material = headMaterial;
        }
        
        public void ApplyBodyMaterial() => renderer.material = bodyMaterial;
        
        public void ApplyTailMaterial() => renderer.material = tailMaterial;
    }
}