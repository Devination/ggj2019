using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    private Text shroomsRemaining;

    private void Start()
    {
        shroomsRemaining = transform.Find("ShroomsRemaining").GetComponent<Text>();
    }

    public void UpdateShroomsRemaining( int numShrooms )
    {
        shroomsRemaining.text = numShrooms.ToString();
    }
}
