using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VistaElementSelector : MonoBehaviour
{
    public Animator animator;
    public GameObject[] subSelectors; 
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Activate(string elementToSelect)
    {
        //Obtain target index
        Dictionary<string, int> indexMap = new Dictionary<string, int>()
        {
            { "house", 0 }
        };
        int targetIdx = indexMap[elementToSelect];

        //Only enable target index
        foreach (GameObject g in subSelectors)
            g.SetActive(false);

        subSelectors[targetIdx].SetActive(true);

        //Start activation animation
        animator.SetTrigger("goUp"); 
    }

    public void Deactivate()
    {
        animator.SetTrigger("goDown"); 
    }
}
