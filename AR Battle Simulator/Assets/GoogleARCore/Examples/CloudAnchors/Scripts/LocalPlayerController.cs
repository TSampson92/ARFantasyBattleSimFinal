// <copyright file="LocalPlayerController.cs" company="Google">
//
// Copyright 2018 Google Inc. All Rights Reserved.

namespace GoogleARCore.Examples.CloudAnchors
{
    using UnityEngine;
    using UnityEngine.Networking;
    using UnityEngine.UI;

    /// <summary>
    /// Local player controller. Handles the spawning of the networked Game Objects.
    /// </summary>
#pragma warning disable 618
    public class LocalPlayerController : NetworkBehaviour
    {
        public GameObject WorldPrefab;
        public GameObject AnchorPrefab;


        private CloudAnchorsExampleController CloudController;
        private NetworkInstanceId mNetID;

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            gameObject.name = "LocalPlayer";
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            gameObject.name = "LocalPlayer";
        }

        void Update()
        {
            if (CloudController == null)
                CloudController = GameObject.Find("CloudAnchorsExampleController").GetComponent<CloudAnchorsExampleController>();

            if (mNetID.ToString() == "0")
            {
                mNetID = gameObject.GetComponent<NetworkIdentity>().netId;
            }
        }

        public NetworkInstanceId GetNetID()
        {
            return mNetID;
        }

        public void SpawnAnchor(Vector3 position, Quaternion rotation, Component anchor)
        {
            var anchorObject = Instantiate(AnchorPrefab, position, rotation);
            anchorObject.GetComponent<AnchorController>().HostLastPlacedAnchor(anchor);
            NetworkServer.Spawn(anchorObject);
        }

        [Command]
        public void CmdSpawnWorld(Vector3 position, Quaternion rotation)
        {
            var rot = rotation.eulerAngles;
            rot.x = 90;
            var starObject = Instantiate(WorldPrefab, position, Quaternion.Euler(rot));
            NetworkServer.Spawn(starObject);
        }

        [Command]
        public void CmdDespawnWorld(bool mode)
        {
            if (mode == false)
                CloudController.SetWorld(mode);
            if (GameObject.Find("World") == null)
                return;

            GameObject world = GameObject.FindGameObjectWithTag("World");
            NetworkServer.Destroy(world);
        }
    }
}
