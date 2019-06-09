// <copyright file="CloudAnchorsExampleController.cs" company="Google">
//
// Copyright 2018 Google Inc. All Rights Reserved.


namespace GoogleARCore.Examples.CloudAnchors
{
    using GoogleARCore;
    using UnityEngine;
    using UnityEngine.Networking;
    using UnityEngine.UI;

    #pragma warning disable 618
    public class CloudAnchorsExampleController : NetworkBehaviour
    #pragma warning restore 618
    {
        [Header("ARCore")]
        public NetworkManagerUIController UIController;               // The UI Controller.
        public GameObject ARCoreRoot;                                 // The root for ARCore-specific GameObjects in the scene.
        public ARCoreWorldOriginHelper ARCoreWorldOriginHelper;       // The helper that will calculate the World Origin offset when performing a raycast or generating planes.

        [Header("ARKit")]
        public GameObject ARKitRoot;                                  // The root for ARKit-specific GameObjects in the scene.
        public Camera ARKitFirstPersonCamera;                         // The first-person camera used to render the AR background texture for ARKit.
        private ARKitHelper m_ARKit = new ARKitHelper();              // A helper object to ARKit functionality.

        [Header("WorldUI")]
        public GameObject WorldUI;


        private bool m_IsOriginPlaced = false;                        // Indicates whether the Origin of the new World Coordinate System, i.e. the Cloud Anchor was placed.
        private bool m_AnchorAlreadyInstantiated = false;             // Indicates whether the Anchor was already instantiated.
        private bool m_AnchorFinishedHosting = false;                 // Indicates whether the Cloud Anchor finished hosting.
        private bool m_IsQuitting = false;                            // True if the app is in the process of quitting due to an ARCore connection error otherwise false.
        private Component m_WorldOriginAnchor = null;                 // The anchor component that defines the shared world origin.
        private Pose? m_LastHitPose = null;                           // The last pose of the hit point from AR hit test.
        private ApplicationMode m_CurrentMode = ApplicationMode.Ready;// The current cloud anchor mode.


        public bool isGameStarted = false;
        public bool isWorldPlaced = false;

        /// <summary>
        /// The Network Manager.
        /// </summary>
#pragma warning disable 618
        private NetworkManager m_NetworkManager;
#pragma warning restore 618

        private bool m_MatchStarted = false;       
        public enum ApplicationMode
        {
            Ready,
            Hosting,
            Resolving,
        }


        public void Start()
        {
#pragma warning disable 618
            m_NetworkManager = UIController.GetComponent<NetworkManager>();
#pragma warning restore 618


            gameObject.name = "CloudAnchorsExampleController";
            ARCoreRoot.SetActive(false);
            ARKitRoot.SetActive(false);
            _ResetStatus();

        }

        public void SetGameStarted(bool mode)
        {
            isGameStarted = mode;
        }

        public void SetWorld(bool mode)
        {
            isWorldPlaced = mode;
        }

        public void Update()
        {
            _UpdateApplicationLifecycle();

  
            //If Game Started, Rest of this script is idle
            if (isGameStarted)
                return;

            //If the world is spawned, we dont need another raycast onto a plane
            if (isWorldPlaced)
            {
                return;
            }
                

            // If we are neither in hosting nor resolving mode then the update is complete.
            if (m_CurrentMode != ApplicationMode.Hosting && m_CurrentMode != ApplicationMode.Resolving){
                return;
            }

            // If the origin anchor has not been placed yet, then update in resolving mode is complete.
            if (m_CurrentMode == ApplicationMode.Resolving && !m_IsOriginPlaced)
            {
                return;
            }

            // If the player has not touched the screen then the update is complete.
            Touch touch;
            if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
            {
                return;
            }

            TrackableHit arcoreHitResult = new TrackableHit();
            m_LastHitPose = null;

            // Raycast against the location the player touched to search for planes.
            if (Application.platform != RuntimePlatform.IPhonePlayer)
            {
                if (ARCoreWorldOriginHelper.Raycast(touch.position.x, touch.position.y,
                        TrackableHitFlags.PlaneWithinPolygon, out arcoreHitResult))
                {
                    m_LastHitPose = arcoreHitResult.Pose;
                }
            }
            else
            {
                Pose hitPose;
                if (m_ARKit.RaycastPlane(
                    ARKitFirstPersonCamera, touch.position.x, touch.position.y, out hitPose))
                {
                    m_LastHitPose = hitPose;
                }
            }

            // If there was an anchor placed, then instantiate the corresponding object.
            if (m_LastHitPose != null)
            {
                // The first touch on the Hosting mode will instantiate the origin anchor. Any
                // subsequent touch will instantiate a star, both in Hosting and Resolving modes.
                if (_CanPlaceStars())
                {
                    _InstantiateStar();
                    SetWorld(true);
                }
                else if (!m_IsOriginPlaced && m_CurrentMode == ApplicationMode.Hosting)
                {
                    if (Application.platform != RuntimePlatform.IPhonePlayer)
                    {
                        m_WorldOriginAnchor =
                            arcoreHitResult.Trackable.CreateAnchor(arcoreHitResult.Pose);
                    }
                    else
                    {
                        m_WorldOriginAnchor = m_ARKit.CreateAnchor(m_LastHitPose.Value);
                    }

                    SetWorldOrigin(m_WorldOriginAnchor.transform);
                    _InstantiateAnchor();
                    OnAnchorInstantiated(true);
                }
            }
        }

        public void SetWorldOrigin(Transform anchorTransform)
        {
            if (m_IsOriginPlaced)
            {
                Debug.LogWarning("The World Origin can be set only once.");
                return;
            }

            m_IsOriginPlaced = true;

            if (Application.platform != RuntimePlatform.IPhonePlayer)
            {
                ARCoreWorldOriginHelper.SetWorldOrigin(anchorTransform);
            }
            else
            {
                m_ARKit.SetWorldOrigin(anchorTransform);
            }
        }

        public void OnEnterHostingModeClick()
        {
            if (m_CurrentMode == ApplicationMode.Hosting)
            {
                m_CurrentMode = ApplicationMode.Ready;
                _ResetStatus();
                return;
            }

            m_CurrentMode = ApplicationMode.Hosting;
            _SetPlatformActive();
        }

        public void OnEnterResolvingModeClick()
        {
            if (m_CurrentMode == ApplicationMode.Resolving)
            {
                m_CurrentMode = ApplicationMode.Ready;
                _ResetStatus();
                return;
            }

            m_CurrentMode = ApplicationMode.Resolving;
            _SetPlatformActive();
        }

        public void OnAnchorInstantiated(bool isHost)
        {
            if (m_AnchorAlreadyInstantiated)
            {
                return;
            }

            m_AnchorAlreadyInstantiated = true;
            UIController.OnAnchorInstantiated(isHost);
        }

        public void OnAnchorHosted(bool success, string response)
        {
            m_AnchorFinishedHosting = success;
            UIController.OnAnchorHosted(success, response);
        }

        public void OnAnchorResolved(bool success, string response)
        {
            UIController.OnAnchorResolved(success, response);
        }

        private void _InstantiateAnchor()
        {
            // The anchor will be spawned by the host, so no networking Command is needed.
            GameObject.Find("LocalPlayer").GetComponent<LocalPlayerController>()
                .SpawnAnchor(Vector3.zero, Quaternion.identity, m_WorldOriginAnchor);

        }

        private void _InstantiateStar()
        {
            //This will return the first local player found, typically the host
            //This is a dirty way to returning only host object, to prevent client from spawning
            GameObject.Find("LocalPlayer").GetComponent<LocalPlayerController>()
                .CmdSpawnWorld(m_LastHitPose.Value.position, m_LastHitPose.Value.rotation);

            WorldUI.SetActive(true);
            UIController.ShowErrorMessage("Stage Placed - Adjust as needed.");

            //GameObject[] mPlayers = GameObject.FindGameObjectsWithTag("Player");
            //LocalPlayerController pc;
            //foreach (GameObject p in mPlayers)
            // {
            //    pc = p.GetComponent<LocalPlayerController>();
            //    if (!pc.isLocalPlayer)
            //        continue;
            //    pc.CmdSpawnWorld(m_LastHitPose.Value.position, m_LastHitPose.Value.rotation);
            //}
        }

        private void _SetPlatformActive()
        {
            if (Application.platform != RuntimePlatform.IPhonePlayer)
            {
                ARCoreRoot.SetActive(true);
                ARKitRoot.SetActive(false);
            }
            else
            {
                ARCoreRoot.SetActive(false);
                ARKitRoot.SetActive(true);
            }
        }

        private bool _CanPlaceStars()
        {
            if (m_CurrentMode == ApplicationMode.Resolving)
            {
                return m_IsOriginPlaced;
            }

            if (m_CurrentMode == ApplicationMode.Hosting)
            {
                return m_IsOriginPlaced && m_AnchorFinishedHosting;
            }

            return false;
        }

        private void _ResetStatus()
        {
            // Reset internal status.
            m_CurrentMode = ApplicationMode.Ready;
            if (m_WorldOriginAnchor != null)
            {
                Destroy(m_WorldOriginAnchor.gameObject);
            }

            m_WorldOriginAnchor = null;
        }

        private void _UpdateApplicationLifecycle()
        {
            if (!m_MatchStarted && m_NetworkManager.IsClientConnected())
            {
                m_MatchStarted = true;
            }

            // Exit the app when the 'back' button is pressed.
            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
            }

            var sleepTimeout = SleepTimeout.NeverSleep;

#if !UNITY_IOS
            // Only allow the screen to sleep when not tracking.
            if (Session.Status != SessionStatus.Tracking)
            {
                const int lostTrackingSleepTimeout = 15;
                sleepTimeout = lostTrackingSleepTimeout;
            }
#endif

            Screen.sleepTimeout = sleepTimeout;

            if (m_IsQuitting)
            {
                return;
            }

            // Quit if ARCore was unable to connect and give Unity some time for the toast to
            // appear.
            if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
            {
                UIController.ShowErrorMessage(
                    "Camera permission is needed to run this application.");
                m_IsQuitting = true;
                Invoke("_DoQuit", 5.0f);
            }
            else if (Session.Status.IsError())
            {
                UIController.ShowErrorMessage(
                    "ARCore encountered a problem connecting.  Please start the app again.");
                m_IsQuitting = true;
                Invoke("_DoQuit", 5.0f);
            }
            else if (m_MatchStarted && !m_NetworkManager.IsClientConnected())
            {
                UIController.ShowErrorMessage(
                    "Network session disconnected!  Please start the app again.");
                m_IsQuitting = true;
                Invoke("_DoQuit", 5.0f);
            }
        }

        /// <summary>
        /// Actually quit the application.
        /// </summary>
        private void _DoQuit()
        {
            Application.Quit();
        }
    }
}
