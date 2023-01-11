using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class CollissionDetect : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        string command = "Start-Process -FilePath “sketch_230110a.exe” -WorkingDirectory “C:/Users/Manos/Desktop/sketch_230110a/application.windows64/” ";

        if (other.gameObject.tag == "Enemy")
        {
            //Destroy(other.gameObject);
            Process process = Process.Start("powershell.exe",command);
            process.WaitForExit();
            process.Close();
        }
    }
}
