using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Results : MonoBehaviour
{
    public Text status;
    public Text numMushrooms;
    private float waitTimeToReset = 0.0f;

    void Start()
    {
        numMushrooms.text = "Number of Mushrooms: " + GameManager.numMushroomsCollected;
        
        int mushroomMeshIndex = GameManager.mushroomHouseIndex;
        string statusStr = "";
        switch ( mushroomMeshIndex )
        {
            case -1:
                statusStr = "You are...Inadequate!";
                break;
            case 0:
                statusStr = "You are...Decent!";
                break;
            case 1:
                statusStr = "You are...Amazing!";
                break;
        }
        status.text = statusStr;
    }

    private void Update()
    {
        waitTimeToReset += Time.deltaTime;
        if ( Input.anyKeyDown && waitTimeToReset > 1.0f )
        {
            GameManager.ResetStats();
            SceneManager.LoadScene("GameScene");
        }
    }

}
