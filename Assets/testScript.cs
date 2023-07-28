using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testScript : MonoBehaviour
{
    public string toApply;
    public JavaScriptIO jsio; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            jsio.SetTaskList(toApply); 
        }
    }

    public void TestPrint(string message)
    {
        print(message); 
    }
}
