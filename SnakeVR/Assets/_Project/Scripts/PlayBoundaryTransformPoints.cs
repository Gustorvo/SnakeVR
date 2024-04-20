using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Gustorvo.SnakeVR
{
    public class PlayBoundaryTransformPoints : MonoBehaviour
    {
        [SerializeField]
        private Transform leftTransform, rightTransform, upTransform, downTransform, forwardTransform, backTransform;

        [SerializeField] bool worldSpace;

        private Transform[] transformPoints = new Transform[6];
        private PlayBoundary playBoundary;
        private Vector3[] playBoundarySidesPositions;


        private void Awake()
        {
            transformPoints = new[]
                { leftTransform, rightTransform, upTransform, downTransform, forwardTransform, backTransform };
            playBoundary = FindObjectOfType<PlayBoundary>();
            Assert.IsNotNull(playBoundary);
        }

        private void Update()
        {
            UpdatePointsPositions();
        }

        private void UpdatePointsPositions()
        {
            playBoundarySidesPositions = playBoundary.GetMiddlePositionsOfEachSide(worldSpace);

            for (int i = 0; i < transformPoints.Length; i++)
            {
                transformPoints[i].position = playBoundarySidesPositions[i];
                if (!worldSpace)
                    transformPoints[i].position += transform.position;

            }
        }

        void OnDrawGizmosSelected()
        {
            if (playBoundarySidesPositions == null) return;
            for (int i = 0; i < playBoundarySidesPositions.Length; i++)
            {
                Vector3 world = worldSpace ? Vector3.zero : transform.position;
                Vector3 pos = playBoundarySidesPositions[i] + world;
                Gizmos.DrawCube(pos, 0.05f * Vector3.one);
            }
        }

        private void OnDisable()
        {
            playBoundarySidesPositions = null;
        }
    }
}