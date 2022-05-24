using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameDev_Ankush
{
    public class PlayerController : MonoBehaviour
    {
        #region PlayerInput Variables
        private float vertical = 0;
        private float horizontal = 0;
        private float lookX = 0;
        private float lookY = 0;

        private bool crouch = false;
        private bool sprint = false;
        private bool shoot = false;
        private bool jump = false;
        private bool aim = false;
        private bool walk = false;
        #endregion

        #region Camera Variables
        private bool lockMouse = true;
        private bool debug = false;
        public float height = 1.8f;
        public float distance = 4f;
        public float aimDistance = 1f;
        public float offset = 0.5f;
        public float followSpeed = 5f;
        public float rotateSpeedY = 5f;
        public float rotateSpeedX = 100f;
        public float minAngle = -45f;
        public float maxAngle = 45f;
        public LayerMask collisionMask;

        private Transform pivot = null;
        private Transform root = null;
        private Transform main = null;
        public float currentAngle = 0;
        private float dis = 0;
        private Vector3 velocity = Vector3.zero;
        private bool active = false;

        //public bool tempAming = false;

        public float Distance {
            get {
                if (aiming) {
                    return aimDistance;
                }
                else{
                        return distance;
                    }
                }
        }

        #endregion

        public float walkSpeed = 4f;
        public float runSpeed = 7f;
        public float sprintSpeed = 10f;
        public float aimedWalkSpeed = 4f;
        public float crouchWalkSpeed = 3f;
        public float rotateSpeed = 10f;
        public float aimedRotateSpeed = 20f;

        public bool grounded = true;
        public float movement = 0f;
        public int moveType = 0;
        public bool crouched = false;
        public bool aiming = false;

        public int MoveType {
            get {
                if (movement < 0.1f)
                {
                    return 0;
                }
                else {
                    return moveType;
                }
            }
        }

        public Vector3 movementPlayer = Vector3.zero;
        private float range = 1f;
        private float pointCheck = 0.1f;
        private Transform[] points = null;
        private float passedPointCheck = 0;

        public Vector3 MovementPlayer {
            get { return movementPlayer; }
        }

        public float Range {
            set { range = value; }
        }



        //Animator
        public LayerMask aimMask;
        private Animator playerAnimator;
        public bool readToMove = true;
        private Transform chestBone = null;
        public Vector3 targetPoint = Vector3.zero;
        private void Start()
        {
            playerAnimator = GetComponent<Animator>();
            Initialize();
            CreatePoints();
        }

        private void Update()
        {
            PC_Input();
            CameraMovement();
            PlayerMovement();
            targetPoint = GetAimTarget(50);

            playerAnimator.SetBool("crouch",crouched);
            playerAnimator.SetBool("aimed",aiming);
            playerAnimator.SetBool("grounded",grounded);
            playerAnimator.SetFloat("moveX",MovementPlayer.x);
            playerAnimator.SetFloat("moveY",MovementPlayer.z);

            switch (MoveType) {
                case 1:
                    playerAnimator.SetBool("Walk",true);
                    playerAnimator.SetBool("sprint",false);
                    playerAnimator.SetBool("run",false);
                    break;
                case 2:
                    playerAnimator.SetBool("Walk", false);
                    playerAnimator.SetBool("sprint", false);
                    playerAnimator.SetBool("run", true);
                    break;
                case 3:
                    playerAnimator.SetBool("Walk", false);
                    playerAnimator.SetBool("sprint", true);
                    playerAnimator.SetBool("run", false);
                    break;
                default:
                    playerAnimator.SetBool("Walk", false);
                    playerAnimator.SetBool("sprint", false);
                    playerAnimator.SetBool("run", false);
                    break;
            }
        }

        #region Camera variables Initialized
        private void Initialize()
        {
            dis = -distance;
            main = Camera.main.transform;
            pivot = new GameObject("Pivot").transform;
            root = new GameObject("PlayerCamera").transform;

            pivot.parent = root;
            main.parent = pivot;
            root.position = transform.position;
            pivot.localPosition = new Vector3(offset, height, 0);
            main.localPosition = new Vector3(0, 0, dis);
            main.rotation = Quaternion.identity;

            active = true;

            if (lockMouse)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
        #endregion

        #region PlayerInput Methods
        private void PC_Input() {
            vertical = Input.GetAxis("Vertical");
            horizontal = Input.GetAxis("Horizontal");
            lookX = Input.GetAxis("Mouse X");
            lookY = Input.GetAxis("Mouse Y");
            aim = Input.GetButton("Fire2");
            shoot = Input.GetButton("Fire1");
            crouch = Input.GetButtonDown("Crouch");
            sprint = Input.GetButton("Sprint");
            walk = Input.GetButton("Walk");
            jump = Input.GetButtonDown("Jump");
        }
        #endregion

        #region Camera Movement
        private void CameraCollision() {
            RaycastHit hit;
            if (Physics.Raycast(pivot.position, -pivot.forward, out hit, Distance, collisionMask))
            {
                dis = 0.01f - Vector3.Distance(hit.point, pivot.position);
            }
            else {
                if (dis != -Distance) {
                    dis = Mathf.Lerp(dis,-Distance, 10 * Time.deltaTime);
                }
            }
        }

        private void CameraMovement() {
            if (!active)
            {
                return;
            }

            float x = lookX;
            float y = lookY;
            if (debug)
            {
                x = 0;
                y = 0;
            }
            CameraCollision();

            main.localPosition = new Vector3(0, 0, dis);
            root.position = Vector3.SmoothDamp(root.position, transform.position, ref velocity, followSpeed * Time.deltaTime);
            currentAngle = currentAngle - y * rotateSpeedY;
            currentAngle = Mathf.Clamp(currentAngle, minAngle, maxAngle);
            pivot.localRotation = Quaternion.Euler(currentAngle, 0, 0);
            root.Rotate(Vector3.up, x * rotateSpeedX * Time.deltaTime, Space.Self);
        }
        #endregion

        private void PlayerMovement() {
            float ver = vertical;
            float hor = horizontal;
            if (crouch && grounded && sprint) {
                crouched = !crouched;
            }

            movement = Mathf.Clamp01(Mathf.Abs(hor) + Mathf.Abs(ver));
            if (grounded)
            {
                Vector3 cameraDirection = main.forward * ver;
                cameraDirection += main.right * hor;
                Vector3 dir = Vector3.zero;
                if (jump)
                {

                }
                else {
                    if (aiming)
                    {

                    }
                    else {
                        #region Rotation
                        cameraDirection.Normalize();

                        cameraDirection.y = 0;
                        if (cameraDirection == Vector3.zero) {
                            cameraDirection = transform.forward;
                        }

                        Quaternion quaternion = Quaternion.LookRotation(cameraDirection);
                        Quaternion targetRotation = Quaternion.Slerp(transform.rotation,quaternion,Time.deltaTime * movement * rotateSpeed);

                        transform.rotation = targetRotation;

                        #endregion

                        #region Movement
                        float speed = runSpeed;
                        moveType = 2;
                        if (sprint)
                        {
                            speed = sprintSpeed;
                            moveType = 3;
                        }
                        else if(walk){
                            speed = walkSpeed;
                            moveType = 1;

                        }

                        if (crouched) {
                            speed = crouchWalkSpeed;
                        }
                        Vector3 targetPosition = transform.forward * movement * speed / 100;
                        targetPosition.y = 0;
                        transform.Translate(targetPosition,Space.World);
                        #endregion
                    }
                }
            }
            else {
            
            }
        }

        private void CreatePoints() {
            points = new Transform[3];

            points[0] = new GameObject(gameObject.name + "Point1").transform;
            points[0].parent = transform;
            points[0].position = transform.position;
            points[0].rotation = transform.rotation;

            points[1] = new GameObject(gameObject.name + "Point2").transform;
            points[1].parent = transform;
            points[1].position = transform.position;
            points[1].rotation = transform.rotation;

            points[2] = new GameObject(gameObject.name + "Point3").transform;
            //points[2].parent = transform;
            points[2].position = transform.position;
            points[2].rotation = transform.rotation;

            pointCheck = 0;
        }

        private void UpdatePoints() {
            if (passedPointCheck >= pointCheck)
            {
                points[1].position = points[2].position;
                points[1].rotation = points[2].rotation;
                points[2].position = transform.position;
                points[2].rotation = transform.rotation;

                passedPointCheck = 0;
            }
            else {
                passedPointCheck += Time.deltaTime;
            }
        }

        private void Calculate() {
            if (points[0].localPosition == points[1].localPosition)
            {
                movementPlayer.z = 0;
                movementPlayer.x = 0;

            }
            else {
                Vector3 direction = points[0].localPosition - points[1].localPosition;
                movementPlayer.z = direction.z;
                movementPlayer.x = direction.x;

                float max = Mathf.Max(Mathf.Abs(movementPlayer.z),Mathf.Abs(movementPlayer.x));
                float min = Mathf.Min(Mathf.Abs(movementPlayer.z),Mathf.Abs(movementPlayer.x));

                float b = 0;

                if (max != 0) {
                    float p = min / max;
                    b = range * p;
                }
                if (Mathf.Abs(movementPlayer.z) > Mathf.Abs(movementPlayer.x))
                {
                    if (movementPlayer.z != 0)
                    {

                        movementPlayer.z = range * (movementPlayer.z / Mathf.Abs(movementPlayer.z));
                    }
                    if (movementPlayer.x != 0)
                    {
                        movementPlayer.x = b * (movementPlayer.x / Mathf.Abs(movementPlayer.x));
                    }
                }
                else if (Mathf.Abs(movementPlayer.z) < Mathf.Abs(movementPlayer.x))
                {
                    if (movementPlayer.z != 0)
                    {

                        movementPlayer.z = b * (movementPlayer.z / Mathf.Abs(movementPlayer.z));
                    }
                    if (movementPlayer.x != 0)
                    {
                        movementPlayer.x = range * (movementPlayer.x / Mathf.Abs(movementPlayer.x));
                    }
                }
                else {
                    movementPlayer.z = range;
                    movementPlayer.x = range;
                }
            }
        }

        private Vector3 GetAimTarget(float range) {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            ray.origin = pivot.position;

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, range, aimMask))
            {
                return hit.point;
            }
            else {
                return pivot.position + main.forward.normalized * range;
            }
        }

    }
}