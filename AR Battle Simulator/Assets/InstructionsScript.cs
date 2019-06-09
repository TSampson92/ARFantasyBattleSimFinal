using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GoogleARCore.Examples.CloudAnchors;

public class InstructionsScript : MonoBehaviour
{
    private Button ToggleInstructionsButton;
    private bool isTextOn = true;
    public Text mInstructions;
    string mInstructionsText =  "Tap your units to select them.\n" +
                                "Selected units have actions on the bottom right (move, attack, and halt).\n\n" +

                                "Press 'move' and then tap a location to send units to that location.\n" +
                                "Press 'attack' and then tap a location to send units to that location (they will attack enemies along the way).\n" +
                                "Press 'halt' to stop units; they will defend their current location.\n\n" +

                                "The game ends when one team's forces have been eliminated.";

    void Start()
    {
        ToggleInstructionsButton = GameObject.Find("ToggleInstrButton").GetComponent<Button>();
        ToggleInstructionsButton.onClick.AddListener(delegate { TurnOffText(); });
        mInstructions.text = mInstructionsText;
        TurnOffText();
    }

    void TurnOffText()
    {
        if (isTextOn == true)
        {
            mInstructions.gameObject.SetActive(false);
            isTextOn = false;
        }
        else
        {
            mInstructions.gameObject.SetActive(true);
            isTextOn = true;
        }
    }
}