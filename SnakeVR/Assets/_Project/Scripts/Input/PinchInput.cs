using Oculus.Interaction;
using Oculus.Interaction.Input;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

namespace Gustorvo.SnakeVR.Input
{
    public enum Dimentions
    {
        axis1D,
        axis2D,
    }

    public class PinchInput : MonoBehaviour
    {
        /// <summary>
        /// The hand to check.
        /// </summary>
        [Tooltip("The hand to check.")] [SerializeField, Interface(typeof(IHand))]
        private UnityEngine.Object _hand;

        [SerializeField] Transform verticalSphere, horizontalSphere;
        [SerializeField] float sensivity = 2f;

        [SerializeField] private OneEuroFilterPropertyBlock _positionFilterProperties =
            new OneEuroFilterPropertyBlock(2f, 3f);

        [SerializeField] Dimentions dimentions = Dimentions.axis1D;
        [SerializeField] private UnityEvent OnLeft, OnRight, OnUp, OnDown;

        private OVRCameraRig rig;

        private bool isIndexFingerPinching;
        protected bool started = false;
        private Pose prevPpinchPose = default;
        private bool wasPinching;
        private Vector3 prevCameraSpace;
        private Matrix4x4 cameraInverseMatrix;
        private IOneEuroFilter<Vector3> verticalPositionFilter, horizontalPositionFilter;
        public IHand Hand { get; private set; }

        protected virtual void Awake()
        {
            rig = FindObjectOfType<OVRCameraRig>();
            verticalPositionFilter = OneEuroFilter.CreateVector3();
            horizontalPositionFilter = OneEuroFilter.CreateVector3();
            Hand = _hand as IHand;
        }

        private void Start()
        {
            this.BeginStart(ref started);
            this.AssertField(Hand, nameof(Hand));
            this.EndStart(ref started);
        }

        protected virtual void OnEnable()
        {
            if (started)
            {
                Hand.WhenHandUpdated += HandleHandUpdated;
            }
        }

        protected virtual void OnDisable()
        {
            if (started)
            {
                Hand.WhenHandUpdated -= HandleHandUpdated;
            }
        }

        private void HandleHandUpdated()
        {
            Pose pinchPose = default;
            var pinching = Hand.GetIndexFingerIsPinching();
            if (!wasPinching && pinching)
            {
                transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
                Hand.GetJointPose(HandJointId.HandIndexTip, out prevPpinchPose);
                cameraInverseMatrix = rig.centerEyeAnchor.worldToLocalMatrix;
                prevCameraSpace = cameraInverseMatrix.MultiplyPoint3x4(prevPpinchPose.position);
                transform.position = prevPpinchPose.position;

                transform.rotation = Quaternion.LookRotation(rig.centerEyeAnchor.transform.forward, Vector3.up);
            }
            else if (wasPinching && !pinching)
            {
                transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack);
                verticalSphere.localPosition = Vector3.zero;
                horizontalSphere.localPosition = Vector3.zero;
            }

            wasPinching = pinching;

            if (pinching)
            {
                Hand.GetJointPose(HandJointId.HandIndexTip, out pinchPose);
                Vector3 cameraSpace = cameraInverseMatrix.MultiplyPoint3x4(pinchPose.position);

                Vector3 delta = cameraSpace - prevCameraSpace;
                (float verticalSensitivity, float horizontalSensitivity) = GetSensivity(delta);
                var vertical = Mathf.Clamp(delta.y * verticalSensitivity, -0.1f, 0.1f);
                var horizontal = Mathf.Clamp(delta.x * horizontalSensitivity, -0.1f, 0.1f);


                // smooth using one-euro filter
                verticalPositionFilter.SetProperties(_positionFilterProperties);
                horizontalPositionFilter.SetProperties(_positionFilterProperties);
                
                verticalSphere.localPosition = verticalPositionFilter.Step(new Vector3(0, vertical, 0), Time.deltaTime);
                horizontalSphere.localPosition = horizontalPositionFilter.Step(new Vector3(horizontal, 0, 0), Time.deltaTime);
            }
        }

        (float, float) GetSensivity(Vector3 delta)
        {
            float vertcalSensitivity = sensivity;
            float horizontalSensivity = sensivity;

            float dampFactor = 0.01f;

            if (dimentions == Dimentions.axis1D)
            {
                // will damp the sensitivity of the opposite axis based on the theirs delta
                if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                {
                    vertcalSensitivity = Mathf.Lerp(sensivity * 0.5f, sensivity * dampFactor,
                        Mathf.InverseLerp(0f, 0.1f, Mathf.Abs(delta.y)));
                }
                else
                {
                    horizontalSensivity = Mathf.Lerp(sensivity * 0.5f, sensivity * dampFactor,
                        Mathf.InverseLerp(0f, 0.1f, Mathf.Abs(delta.x)));
                }
            }

            return (vertcalSensitivity, horizontalSensivity);
        }
    }
}