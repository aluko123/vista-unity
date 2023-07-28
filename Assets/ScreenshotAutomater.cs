using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenshotAutomater : MonoBehaviour
{
    public int[] keyLevels;
    private int[] maxLevels; 

    public bool debugInput = true; 

    public string saveLocation;
    public List<HouseSwapper> swappers; 
    public float bufferTime = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        keyLevels = new int[] { 0, 0, 0, 0, 0 };
        //maxLevels = new int[] { 3, 3, 3, 3, 3 };
        maxLevels = new int[] { 0, 3, 3, 3, 3 };
        swappers = new List<HouseSwapper> (GameObject.FindObjectsOfType<HouseSwapper>()); 
    }

    // Update is called once per frame
    void Update()
    {
        if (debugInput)
        {
            if (Input.GetKeyDown(KeyCode.A))
                ApplyLevels();
            if (Input.GetKeyDown(KeyCode.S))
                TakeSreenshot();
            if (Input.GetKeyDown(KeyCode.T))
                StartCoroutine(TakeAllScreenshots()); 
        }
    }

    private IEnumerator TakeAllScreenshots()
    {
        for (int i = 0; i < keyLevels.Length; i++) keyLevels[i] = 0; 
        var isAtMax = false;
        var maxIndex = keyLevels.Length - 1;

        while (isAtMax == false)
        {
            TakeSreenshot();
            AdvanceLevels();
            ApplyLevels(); 

            if (keyLevels[maxIndex] >= maxLevels[maxIndex])
                isAtMax = true; 

            yield return new WaitForSeconds(bufferTime);  
        }

        yield return null; 
    }

    void AdvanceLevels()
    {
        keyLevels[0] += 1; 

        for (int i = 0; i < keyLevels.Length; i++) 
        {
            if (keyLevels[i] > maxLevels[i])
            {
                keyLevels[i] = 0;
                keyLevels[i + 1] += 1; 
            }
        }
    }

    void ApplyLevels()
    {
        //Apply each key level entry
        //If swapper does not exist, ignore that level 

        for (int index = 0; index < keyLevels.Length; index++)
        {
            if(swappers.Count > index)
            {
                var swapper = swappers[index];
                var targetLevel = keyLevels[index];
                swapper.Transition(targetLevel);
            }
        }

    }

    void TakeSreenshot()
    {
        var filename = saveLocation+"/";
        for (int i = 0; i < keyLevels.Length; i++)
            filename += keyLevels[i].ToString();
        filename += ".jpg"; 

        ScreenCapture.CaptureScreenshot(filename);
        print("Screenshot " + filename + " taken"); 
    }
}
