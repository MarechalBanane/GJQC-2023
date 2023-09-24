using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour
{
    bool alreadyLoading = false;

    public void Update()
    {
        Input.GetKeyDown(KeyCode.JoystickButton2);

        if (Input.GetKeyDown(KeyCode.JoystickButton2) && !alreadyLoading)
        {
            alreadyLoading = true;
            SceneManager.LoadScene("MainMenu");
        }
    }

}
