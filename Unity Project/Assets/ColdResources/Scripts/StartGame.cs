using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public UnityEvent OnStart;
    bool alreadyLoading;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.JoystickButton2) && !alreadyLoading)
        {
            alreadyLoading = true;
            OnStart.Invoke();

        }
    }

    public void Starting() { 
    
    }
}
