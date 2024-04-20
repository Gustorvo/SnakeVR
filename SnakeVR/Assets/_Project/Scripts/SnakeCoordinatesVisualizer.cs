using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gustorvo.SnakeVR
{
    public class SnakeCoordinatesVisualizer : MonoBehaviour
    {
        [SerializeField] private Transform xPoint0, xPoint1, yPoint0, yPoint1, zPoint0, zPoint1;
        [SerializeField] private Transform xLine, zLine, yLine;

        private PlayBoundary boundary => Core.PlayBoundary;
        private Vector3 snakePosition => Core.Snake.Head.PositionLocal;

        private void Start()
        {
            xLine.localPosition = yLine.localPosition = zLine.localPosition = Vector3.zero;
            xLine.Rotate(Vector3.forward, 90);
            yLine.Rotate(Vector3.up, 90);
            zLine.Rotate(Vector3.right, 90);
        }

        private void Update()
        {
            //x = yz Plane
            //y = xz plane
            //z = xy plane

            xLine.localPosition = new Vector3(0, snakePosition.y, snakePosition.z);
            yLine.localPosition = new Vector3(snakePosition.x, 0, snakePosition.z);
            zLine.localPosition = new Vector3(snakePosition.x, snakePosition.y, 0);

           // xPoint0.localPosition = new Vector3(0, snakePosition.y, snakePosition.z);
        }

        private void OnDrawGizmos()
        {
            // draw a point 
        }
    }
}