using UnityEngine;

using Oculus.Interaction;
using UnityEngine.Events;

namespace Game.Interaction {

    /// <summary>
    /// Class to enable "Grabbing" the towers and horizontal rotation via stick
    /// </summary>
    public class GrabRotationController : MonoBehaviour, ITransformer
    {
        private OVRInput.Controller _Controller;

        private Transform _GrabbedObject;
        private Vector3 _RotationAxis;

        private bool _IsGrabbed;

        [SerializeField] public UnityEvent<GrabRotationController> ExtraEventStart;
        [SerializeField] public UnityEvent<GrabRotationController> ExtraEventUpdate;
        [SerializeField] public UnityEvent<GrabRotationController> ExtraEventEnd;


        [SerializeField] public PlayerTower Tower;

        public void BeginTransform()
        {
            _IsGrabbed = true;
            if (ExtraEventStart != null) ExtraEventStart.Invoke(this);
        }

        public void UpdateTransform()
        {
            RotateViaStick();
            if (ExtraEventUpdate != null) ExtraEventUpdate.Invoke(this);
        }

        public void EndTransform()
        {
            _IsGrabbed = false;
            if (ExtraEventEnd != null) ExtraEventEnd.Invoke(this);
        }

        public void Initialize(IGrabbable grabbable)
        {
            _Controller = OVRInput.GetActiveController();
            _GrabbedObject = grabbable.Transform;
            _RotationAxis = _GrabbedObject.up;
        }

        void Start()
        {
        }
        void Update()
        {
            if (_IsGrabbed)
                UpdateTransform();
        }

        private void RotateViaStick()
        {
            Vector2 stickAxis = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, _Controller);
            _GrabbedObject.RotateAround(_GrabbedObject.position, Vector3.up, stickAxis.x * 0.5f);
        }

    }

}