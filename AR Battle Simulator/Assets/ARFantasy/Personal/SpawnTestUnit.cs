using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnTestUnit : MonoBehaviour
{
    public Button mSpawnButton;

    // Start is called before the first frame update
    void Start()
    {
        mSpawnButton.onClick.AddListener(delegate { SpawnRandomUnit(); });
    }

    void SpawnRandomUnit()
    {
        GameObject[] mPlayers = GameObject.FindGameObjectsWithTag("Player");
        LocalPlayerControlScript pc;
        Vector3 Pos = new Vector3(Random.Range(-.7f, .7f), 0, Random.Range(-0.7f, 0.7f));

        foreach (GameObject p in mPlayers)
        {
            pc = p.GetComponent<LocalPlayerControlScript>();
            if (!pc.isLocalPlayer)
                continue;
            pc.CmdSpawnUnit2(Pos, Quaternion.identity);
        }
    }
}
