using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 618
public class WorldScalerUIScript : MonoBehaviour
{

    public Slider mScaleWorld;
    public Slider mRotateWorld;
    public Button mStart;
    private GameObject mWorld;    //Parent world object
   // private Transform mMap;
    private WorldController mWorldControl;

    // Start is called before the first frame update
    void Start()
    {
        mScaleWorld.onValueChanged.AddListener(delegate { ScaleValueChanged(mScaleWorld.value); });
        mRotateWorld.onValueChanged.AddListener(delegate { RotateValueChanged(mRotateWorld.value); });
        mStart.onClick.AddListener(delegate { StartPlacement(); });
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (GameObject.FindGameObjectWithTag("World") == null)
            return;
         mWorld = GameObject.FindGameObjectWithTag("World");
         mWorldControl = mWorld.GetComponent<WorldController>();
    }

    void StartPlacement()
    {
        if (GameObject.FindGameObjectWithTag("World") == null)
        {
            return;
        }
        else
            gameObject.SetActive(false);
    }

    void ScaleValueChanged(float value)
    {
        mScaleWorld.value = value;
        Vector3 newScale = new Vector3(1, 2, 1);
        newScale *= 1 + (value / 20);
        mWorldControl.CmdSetScale(newScale);
    }

    void RotateValueChanged(float value)
    {
        mRotateWorld.value = value;
        var rot = mWorld.transform.localRotation.eulerAngles;
        rot.Set(rot.x, value, rot.z);
        mWorld.transform.localRotation = Quaternion.Euler(rot);
    }
}
