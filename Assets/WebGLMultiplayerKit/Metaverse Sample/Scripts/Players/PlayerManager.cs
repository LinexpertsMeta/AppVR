using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
using TMPro;

/// <summary>
///Manage Network player if isLocalPlayer variable is false
/// or Local player if isLocalPlayer variable is true.
/// </summary>
namespace MetaverseSample
{
    public class PlayerManager : MonoBehaviour
    {

        public static PlayerManager instance;

        public string id;

        public string name;

        public TMP_Text txtName;

        public bool isOnline;

        public bool isLocalPlayer;

        public bool move;

        public bool mute;

        public bool hasToUpdateAnimation;

        public Rigidbody myRigidbody;

        //interact variables
        public Transform pointToInteract;
        public bool onSitChair;
        public Collider normalCollider, sittingCollider;
        public CharacterController characterController;
        //public ChairController chair;

        //distances low to arrive close to the player
        [Range(1f, 200f)] [SerializeField] float minDistanceToPlayer = 10f;


        public int current_model;


        public Animator _animator;

        float h;

        float v;

        public ThirdPersonController thirdPersonController;

        StarterAssetsInputs _input;

        // animation IDs
        private int _animIDSpeed;
        private int _animIDMotionSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDSitting;
        private float _animationBlend;

        private bool _hasAnimator;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;


        //for web camera
        [SerializeField] private WebcamStreamer webcam;
        [SerializeField] private GameObject webcamGameObject;

        private bool cameraIsOn;

        [SerializeField] private List<GameObject> modelsHeads, modelsHairs;
        [SerializeField] private List<Material> materialsClothes;
        [SerializeField] private SkinnedMeshRenderer skinnedMesh;
        [SerializeField] private List<Material> textureSkin1, textureSkin2, textureSkin3;
        [SerializeField] private List<List<Material>> listas;
        [SerializeField] private Material material;
        public bool isSharing;
        public string currentZone;

        // Use this for initialization
        void Awake()
        {

            listas = new List<List<Material>>();
            listas.Add(textureSkin1);
            listas.Add(textureSkin2);
            listas.Add(textureSkin3);
            if (GetComponent<StarterAssetsInputs>())
            {
                _input = GetComponent<StarterAssetsInputs>();
            }

            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDSitting = Animator.StringToHash("Sitting");

            // reset our timeouts on start
            //_jumpTimeoutDelta = thirdPersonController.JumpTimeout;
            //_fallTimeoutDelta = thirdPersonController.FallTimeout;


        }

        private void Start()
        {
            if (isLocalPlayer)
            {
                if (instance == null)
                {
                    instance = this;
                }
                else
                {
                    Destroy(this);
                }
            }
        }



        // Use this for initialization
        public void Set3DName(string name)
        {
            if (!isLocalPlayer)
            {
                txtName.text = name;

            }
            else
            {
                txtName.text = string.Empty;

            }


        }

        public void SetModel(int head, int hair)
        {
            modelsHairs[hair].SetActive(true);
            modelsHeads[head].SetActive(true);
        }

        public void SetModelColor(int i, int j, int body)
        {
            int number = i;
            if (i > 2)
            {
                number -= 3;
            }
            if (body < 3)
            {
                skinnedMesh.material = listas[number][j];
            }
            else if (body >= 3)
            {
                Material[] materials = new Material[] { material, listas[number][j] };
                skinnedMesh.materials = materials;
            }
        }

        void Update()
        {
            if (isLocalPlayer)
            {

                _hasAnimator = TryGetComponent(out _animator);

                // set target speed based on move speed, sprint speed and if sprint is pressed
                float targetSpeed = 0;
                if (!hasToUpdateAnimation)
                {
                    targetSpeed = _input.sprint ? thirdPersonController.SprintSpeed : thirdPersonController.MoveSpeed;
                }

                if (_input.move == Vector2.zero) targetSpeed = 0.0f;

                float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

                _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * thirdPersonController.SpeedChangeRate);
                if (_animationBlend < 0.01f) _animationBlend = 0f;

                // update animator if using character
                if (_hasAnimator && !onSitChair)
                {


                    NetworkManager.instance.EmitAnimation(_animIDSpeed, _animationBlend.ToString(), "float");
                    NetworkManager.instance.EmitAnimation(_animIDMotionSpeed, inputMagnitude.ToString(), "float");



                }

                //Interact();
                //ShareScreen();
                //ShareScreenOFF();
                EmitJumpToServer();
                //ToggleInputCamera();

                if (_input.move != Vector2.zero)
                {
                    UpdateStatusToServer();
                }

            }

        }

        public void ToggleCamera()
        {
            if (cameraIsOn)
            {
                TurnOffCamera();
            }
            else
            {
                TurnOnCamera();
            }

        }

        public void ToggleCameraOnline()
        {
            if (cameraIsOn)
            {
                TurnOffCamera();
            }
            else
            {
                TurnOnCamera();
            }
        }

        public void TurnOnCamera()
        {
            webcam.gameObject.SetActive(true);
            webcam.ToggleWebCamStream();
        }

        public void TurnOffCamera()
        {
            Debug.Log("reset camera");
            webcam.ResetCamera();
            Debug.Log("turn off camera");
        }

        public void TurnOffCameraMesh()
        {
            webcamGameObject.SetActive(false);
        }      


        public void UpdateCamera(string texture)
        {
            if (webcamGameObject.activeSelf == false)
            {
                webcamGameObject.SetActive(true);
            }
            webcam.OnsUpdateStateCamera(texture);
        }


        public void UpdateStatusToServer()
        {


            //hash table <key, value>
            Dictionary<string, string> data = new Dictionary<string, string>();

            data["local_player_id"] = id;

            data["posX"] = transform.position.x.ToString();
            data["posY"] = transform.position.y.ToString();
            data["posZ"] = transform.position.z.ToString();

            data["rotation"] = transform.rotation.x + ";" + transform.rotation.y + ";" + transform.rotation.z + ";" + transform.rotation.w;


            NetworkManager.instance.EmitMoveAndRotate(data);



        }

        public void EmitSittinToServer(bool state)
        {
            string textState = state == true ? "true" : "false";
            if (thirdPersonController.Grounded)
            {
                //if (_input.sitting)
                //{
                    NetworkManager.instance.EmitAnimation(_animIDSitting, textState, "bool");
                //}
            }
        }

        void EmitJumpToServer()
        {
            if (thirdPersonController.Grounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = thirdPersonController.FallTimeout;

                // update animator if using character
                if (_hasAnimator)
                {
                    NetworkManager.instance.EmitAnimation(_animIDJump, "false", "bool");
                    NetworkManager.instance.EmitAnimation(_animIDFreeFall, "false", "bool");
                }


                // Jump
                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                {

                    // update animator if using character
                    if (_hasAnimator &&!hasToUpdateAnimation)
                    {

                        NetworkManager.instance.EmitAnimation(_animIDJump, "true", "bool");
                    }
                }
                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }

            }//END_IF

            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = thirdPersonController.JumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    // update animator if using character
                    if (_hasAnimator)
                    {
                        NetworkManager.instance.EmitAnimation(_animIDFreeFall, "true", "bool");

                    }
                }


            }//END_ELSE

            // update animator if using character
            if (_hasAnimator)
            {
                NetworkManager.instance.EmitAnimation(_animIDGrounded, thirdPersonController.Grounded.ToString(), "bool");
            }
        }




        public void UpdatePosition(Vector3 position)
        {

            transform.position = new Vector3(position.x, position.y, position.z);

        }

        public void UpdateRotation(Quaternion _rotation)
        {
            transform.rotation = _rotation;

        }



    }//END_CLASS
}//END_NAMESPACE