using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swapper : MonoBehaviour
{
    public GameObject item1, item2;
    public float swapDelay;
    public DemoAdvancer advancer; 

    private GameObject activeItem; 

    // Start is called before the first frame update
    void Start()
    {
        activeItem = item1; 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActivateSwap() {
        //StartCoroutine(Swap()); 
        ProgressSwap(); 
    }

    public GameObject nextItem; 
    public void ProgressSwap()
    {
        print("Progressing...");

        //If object is active, deactivate asset and invoke asset swapper
        if (activeItem != null)
        {
            activeItem.GetComponent<Animator>().SetTrigger("Swap");
            nextItem = (activeItem == item1) ? item2 : item1;
            activeItem = null;

            advancer.Advance();
        }

        //If no object is active, activate least recently used
        else if (activeItem == null)
        {
            nextItem.GetComponent<Animator>().SetTrigger("Swap"); 
            activeItem = nextItem;
            nextItem = null; 
        }
    }

    IEnumerator Swap() {
        item1.GetComponent<Animator>().SetTrigger("Swap");

        yield return new WaitForSeconds(swapDelay);

        item2.GetComponent<Animator>().SetTrigger("Swap");
    }

}
