using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gustorvo.SnakeVR
{
    public class Rotator : MonoBehaviour
    {
        [SerializeField] private bool rotateOnX = true;
        [SerializeField] private bool rotateOnY = true;
        [SerializeField] private bool rotateOnZ = true;

        [SerializeField] private float rotateSpeed = 10f;
        
        private void Update()
        {
            if (rotateOnX)
                transform.Rotate(Vector3.right, rotateSpeed * Time.deltaTime);
            if (rotateOnY)
                transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
            if (rotateOnZ)
                transform.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime);
                
        }
    }
}
