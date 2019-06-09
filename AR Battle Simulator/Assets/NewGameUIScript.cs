using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using GoogleARCore.Examples.CloudAnchors;

#pragma warning disable 618
public class NewGameUIScript : NetworkBehaviour
{
    public Button mNewGameButton;
    public Text mNewGameText;
    public GameController mGameController;
    public BattleControlScript mBattleController;

    // Start is called before the first frame update
    void Start()
    {
        mNewGameButton.onClick.AddListener(delegate { NewGameButtonPressed(); });
    }

    void NewGameButtonPressed()
    {
        //Reset World
        GameObject[] mPlayers = GameObject.FindGameObjectsWithTag("Player");
        LocalPlayerController pc;
        foreach (GameObject p in mPlayers)
        {
            pc = p.GetComponent<LocalPlayerController>();
            if (!pc.isLocalPlayer)
                continue;
            //Reset world
            GameObject world = GameObject.Find("World");
            Vector3 mWorldPos = world.transform.position;
            Quaternion mWorldRot = world.transform.rotation;


            pc.CmdDespawnWorld(true);
            pc.CmdSpawnWorld(mWorldPos, mWorldRot);
        }

            GameObject[] mPlayers2 = GameObject.FindGameObjectsWithTag("Player");
            LocalPlayerControlScript pc2;
            foreach (GameObject p in mPlayers2)
            {
                pc2 = p.GetComponent<LocalPlayerControlScript>();
                if (!pc2.isLocalPlayer)
                    continue;
                pc2.CmdResetGame();
            }
    }

    public void SetWinnerText(string text)
    {
        mNewGameText.text = text;
    }
}
