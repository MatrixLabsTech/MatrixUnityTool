using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUIManager : MonoBehaviour
{

    public GameObject mainScreenGO;
    public GameObject scInteractGO;

    // Start is called before the first frame update
    void Start()
    {
        GoToScreen("main");

    }

    // Update is called once per frame
    void Update()
    {

    }



    public void GoToScreen(string screenName)
    {
        if (screenName == "main")
        {
            scInteractGO.SetActive(false);
            mainScreenGO.SetActive(true);
        }
        else
        {
            scInteractGO.SetActive(true);
            mainScreenGO.SetActive(false);
        }
    }
}
