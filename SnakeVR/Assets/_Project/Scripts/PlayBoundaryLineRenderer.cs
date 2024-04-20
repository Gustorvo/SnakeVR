using System;
using NaughtyAttributes;
using UnityEngine;

namespace Gustorvo.SnakeVR
{
    public interface IBoundsVisualizer
    {
        Bounds bounds { get; }
        void VisualizeBounds(Bounds bounds);
    }

    public class PlayBoundaryLineRenderer : MonoBehaviour, IBoundsVisualizer
    {
        [SerializeField] LineRenderer lineRendererPrefab;
        private PlayBoundary playBoundary;
        private GameObject wiredCube;

        public Bounds bounds => playBoundary != null ? playBoundary.bounds : default;


        private void Awake()
        {
            playBoundary = FindObjectOfType<PlayBoundary>();
        }

        private void Start()
        {
            VisualizeBounds(bounds);
        }


        private void Update()
        {
            if (wiredCube)
            {
                wiredCube.transform.SetPositionAndRotation(transform.position, transform.rotation);
            }
        }

        public void VisualizeBounds(Bounds bounds)
        {
            Vector3[] corners = GetBoundsCorners(bounds);
            int i = 4;

            Vector3[] wireframePoints = new Vector3[16];
            wireframePoints[0] = corners[0];
            wireframePoints[1] = corners[1];
            wireframePoints[2] = corners[i + 1];
            wireframePoints[3] = corners[i + 2]; 
            wireframePoints[4] = corners[2];
            wireframePoints[5] = corners[1];
            wireframePoints[6] = corners[i + 1];
            wireframePoints[7] = corners[i + 0];
            wireframePoints[8] = corners[0];
            wireframePoints[9] = corners[3];
            wireframePoints[10] = corners[i + 3]; 
            wireframePoints[11] = corners[i + 2];
            wireframePoints[12] = corners[2];
            wireframePoints[13] = corners[3];
            wireframePoints[14] = corners[i + 3];
            wireframePoints[15] = corners[i + 0];
            
            wiredCube = new GameObject("wiredCube");
            LineRenderer lineRenderer = Instantiate(lineRendererPrefab, wiredCube.transform);

            lineRenderer.positionCount = 16;
            lineRenderer.SetPositions(wireframePoints);

            // instantiate a sphere of scale 0.1 at each corner
            for (int j = 0; j < corners.Length; j++)
            {
                Vector3 position = corners[j];
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.position = position;
                sphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                sphere.transform.parent = transform;
                sphere.transform.SetParent(wiredCube.transform);
            }
        }

        private Vector3[] GetBoundsCorners(Bounds bounds)
        {
            Vector3[] corners = new Vector3[8];
            Vector3 center = bounds.center;
            float x = bounds.extents.x;
            float y = bounds.extents.y;
            float z = bounds.extents.z;

            corners[0] = center + new Vector3(x, y, z);
            corners[1] = center + new Vector3(x, y, -z);
            corners[2] = center + new Vector3(-x, y, -z);
            corners[3] = center + new Vector3(-x, y, z);
            corners[4] = center + new Vector3(x, -y, z);
            corners[5] = center + new Vector3(x, -y, -z);
            corners[6] = center + new Vector3(-x, -y, -z);
            corners[7] = center + new Vector3(-x, -y, z);

            return corners;
        }

        [Button("Visualize Bounds")]
        public void VisualizeBounds() => VisualizeBounds(bounds);
    }
}