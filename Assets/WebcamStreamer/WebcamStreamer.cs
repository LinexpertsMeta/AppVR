using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Author: Webcam Streamer written by WITS, www.walterit.se
/// 
/// Instructions: Put this script on the GameObject where you want the stream from webcamera to appear. 
/// </summary>

namespace MetaverseSample
{
    public class WebcamStreamer : MonoBehaviour
    {

        #region Variables
        WebCamTexture _webCamTexture;
        Texture _defaultTexture;
        Texture2D texture;
        [SerializeField] private MetaverseSample.PlayerManager player;
        private bool isPlaying;
        [SerializeField] private Material material;
        Color32[] colors;
        byte[] bytesArray;
        byte[] bytesArray2;
        Texture2D newTexture;

        public MeshRenderer meshRenderer;

        [SerializeField]
        [Tooltip("If several cameras is connected, choose which one to render to this texture.\n0 = ID of first connected camera.")]
        int _webCamIdToRender = 0;

        [SerializeField]
        [Tooltip("Frames Per Second to render.\n0 = use default camera settings.")]
        int _webCamFPS = 0;

        [SerializeField]
        [Tooltip("Desired height of video in pixels.\n0 = use default camera resolution.")]
        int _videoHeight = 0;

        [SerializeField]
        [Tooltip("Desired width of video in pixels.\n0 = use default camera resolution.")]
        int _videoWidth = 0;

        [SerializeField]
        [Tooltip("Start stream automatically at startup if checked.")]
        bool autoPlay = true;

        [SerializeField]
        [Tooltip("If the stream get toggled off, this checkbox will reset the renderer to the default texture, instead of showing the last frame of stream.")]
        bool resetTextureAtEndOfStream = true;

        [SerializeField]
        [Tooltip("Print all connected camera devices to console at startup.")]
        bool debugPrintDevices = true;

        [SerializeField]
        [Tooltip("If checked, key T will toggle the stream on and off.")]
        bool debugShortcutToggleT = true;
        #endregion

        void Start()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            if (isStartupError())
            {
                Debug.Log("ERROR at startup of " + transform.name + ". Aborting.");
            }
            else
            {
                Initialize();
            }
            texture = new Texture2D(_webCamTexture.width, _webCamTexture.height, TextureFormat.RGBA32, false);
            newTexture = new Texture2D(_webCamTexture.width, _webCamTexture.height, TextureFormat.RGBA32, false);
        }

        private void Update()
        {
            if (debugShortcutToggleT && Input.GetKeyDown(KeyCode.T) && player.isLocalPlayer)
            {
                ToggleWebCamStream();
            }
        }

        private void Initialize()
        {
            // If chosen webcamera is outside allowed range, then set default value
            if (_webCamIdToRender < 0 || _webCamIdToRender > WebCamTexture.devices.Length)
            {
                _webCamIdToRender = 0;
            }

            // Save default material for resetting later
            _defaultTexture = GetComponent<Renderer>().material.mainTexture;

            // Modify texture according to inspector settings
            SetUserSettings();

            // Apply new webcam texture to current GameObjects texture
            GetComponent<Renderer>().material.mainTexture = _webCamTexture;

            if (autoPlay)
            {
                ToggleWebCamStream();
            }

            //Subscribe to change scene event to turn off camera
            SceneManager.activeSceneChanged += ToggleWebCamStream;
        }

        void SetUserSettings()
        {
            #region Debug settings
            // Print connected camera devices if checked in inspector
            if (debugPrintDevices)
            {
                for (int i = 0; i < WebCamTexture.devices.Length; i++)
                {
                    Debug.Log("DEBUG: Connected camera id & name: " + i + " - " + WebCamTexture.devices[i].name);
                }
            }
            #endregion

            #region Video settings
            /*if (_webCamFPS > 0)
            {
                _webCamTexture.requestedFPS = _webCamFPS;
            }

            if (_videoHeight > 0)
            {
                _webCamTexture.requestedHeight = _videoHeight;
            }

            if (_videoWidth > 0)
            {
                _webCamTexture.requestedWidth = _videoWidth;
            }*/
            // Initialize rendering to texture with user settings
            _webCamTexture = new WebCamTexture(WebCamTexture.devices[_webCamIdToRender].name, _videoWidth, _videoHeight, _webCamFPS);
            #endregion
        }

        public void ToggleWebCamStream()
        {
            if (_webCamTexture.isPlaying)
            {
                _webCamTexture.Stop();
                isPlaying = false;
                StopCoroutine(SendTexture());
                ResetCamera();
                //NetworkManager.instance.EmitTurnOffCamera();
            }
            else
            {
                GetComponent<Renderer>().material.mainTexture = _webCamTexture;
                _webCamTexture.Play();
                isPlaying = true;
                UpdateStateCamera();
            }
        }

        public void UpdateStateCamera()
        {
            StartCoroutine(SendTexture());

        }

        IEnumerator SendTexture()
        {
            // Crear una única instancia de Texture2D fuera del bucle y ajustar sus dimensiones una vez.


            while (isPlaying)
            {
                // Verificar si las dimensiones han cambiado y ajustar la textura si es necesario.
                if (texture.width != _webCamTexture.width || texture.height != _webCamTexture.height)
                {
                    texture.Reinitialize(_webCamTexture.width, _webCamTexture.height, TextureFormat.RGBA32, false);
                }

                colors = _webCamTexture.GetPixels32();
                texture.SetPixels32(colors);
                bytesArray = texture.EncodeToJPG();
                //MetaverseSample.NetworkManager.instance.EmitUpdateCamera(bytesArray);

                yield return new WaitForSeconds(0.1f);
            }

            yield return null;
        }


        public void OnsUpdateStateCamera(string texture)
        {
            bytesArray2 = Convert.FromBase64String(texture);

            newTexture.LoadImage(bytesArray2);


            GetComponent<Renderer>().material.mainTexture = newTexture;
        }

        public void ToggleWebCamStream(Scene scene1, Scene scene2)
        {
            //This overloaded method is needed to match event subscription when switching scenes.
            _webCamTexture.Stop();
        }

        bool isStartupError()
        {
            bool isError = false;

            if (WebCamTexture.devices.Length <= 0 || WebCamTexture.devices == null)
            {
                Debug.Log("ERROR: No connected cameras found.");
                isError = true;
            }

            if (GetComponent<Renderer>() == null)
            {
                Debug.Log("ERROR: " + transform.name + " is missing required component Renderer.");
                isError = true;
            }

            return isError;
        }
        public void ResetCamera()
        {
            _webCamTexture.Stop();
            GetComponent<Renderer>().material.mainTexture = material.mainTexture;
            player.TurnOffCameraMesh();
        }
    }

}
