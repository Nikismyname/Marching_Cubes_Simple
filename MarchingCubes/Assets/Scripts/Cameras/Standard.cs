namespace Assets.Cameras
{
    using UnityEngine;

    public class Standard : MonoBehaviour
    {
        public Transform target;
        private Vector3 targetOffset = Vector3.zero;
        private float distance = 20.0f;
        private float maxDistance = 50;
        private float minDistance = 0f;
        private float xSpeed = 200.0f;
        private float ySpeed = 200.0f;
        private int zoomRate = 40;
        private float zoomDampening = 5.0f;

        private float xDeg = 0.0f;
        private float yDeg = 0.0f;
        private float currentDistance;
        private float desiredDistance;
        private Quaternion currentRotation;
        private Quaternion desiredRotation;
        private Quaternion rotation;
        private Vector3 position;
        private bool rotate;
        private bool doDistance;

        public float yMinLimit = -360f;
        public float yMaxLimit = 360f;
        void Start()
        {
            var thing = Cursor.lockState;
            this.rotate = false;
            this.doDistance = true;

            var main = GameObject.Find("Main");

            Init();
        }

        void OnEnable()
        {
            Init();
        }

        public void Init()
        {
            if (!target)
            {
                GameObject go = new GameObject("Cam Target");
                go.transform.position = transform.position + (transform.forward * distance);
                target = go.transform;
            }

            distance = Vector3.Distance(transform.position, target.position);
            currentDistance = distance;
            desiredDistance = distance;

            //be sure to grab the current rotations as starting points.
            position = transform.position;
            rotation = transform.rotation;
            currentRotation = transform.rotation;
            desiredRotation = transform.rotation;

            xDeg = Vector3.Angle(Vector3.right, transform.right);
            yDeg = Vector3.Angle(Vector3.up, transform.up);
        }

        public void SetDistance(bool distance)
        {
            this.doDistance = distance;
        }

        void LateUpdate()
        {
            if (Input.GetMouseButtonDown(1) /*&& (this.inputFocusManager == null || this.inputFocusManager.SafeToTrigger())*/)
            {
                if (this.rotate == false)
                {
                    this.rotate = true;
                    Cursor.lockState = CursorLockMode.Locked;
                    return;
                }
            }

            if (Input.GetMouseButtonUp(1) /*&& (this.inputFocusManager == null || this.inputFocusManager.SafeToTrigger())*/)
            {
                if (this.rotate == true)
                {
                    this.rotate = false;
                    Cursor.lockState = CursorLockMode.None;
                    return;
                }
            }

            if (this.rotate)
            {
                xDeg += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
                yDeg -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

                //yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);

                currentRotation = transform.rotation;
                desiredRotation = Quaternion.Euler(yDeg, xDeg, 0);
                //rotation = Quaternion.Lerp(currentRotation, desiredRotation, Time.deltaTime * zoomDampening)
                this.rotation = Quaternion.Lerp(currentRotation, desiredRotation, 1);
                this.gameObject.transform.rotation = rotation;
            }

            if (this.doDistance)
            {
                float msw = Input.GetAxis("Mouse ScrollWheel");
                
                // affect the desired Zoom distance if we roll the scrollwheel
                desiredDistance -= msw * Time.deltaTime * zoomRate *
                                   Mathf.Abs(desiredDistance);
                //clamp the zoom min/max
                desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);
                // For smoothing of the zoom, lerp distance
                currentDistance = Mathf.Lerp(currentDistance, desiredDistance, Time.deltaTime * zoomDampening);
            }

            // calculate position based on the new currentDistance 
            position = target.position - (rotation * Vector3.forward * currentDistance + targetOffset);

            transform.position = position;
        }

        //private static float ClampAngle(float angle, float min, float max)
        //{
        //    if (angle < -360)
        //        angle += 360;
        //    if (angle > 360)
        //        angle -= 360;
        //    return Mathf.Clamp(angle, min, max);
        //}
    }
}
