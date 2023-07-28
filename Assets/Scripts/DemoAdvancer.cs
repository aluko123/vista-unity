using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DemoAdvancer : MonoBehaviour, IPointerClickHandler
{
    public UnityEngine.UI.Text textToUpdate;
    public Swapper swapper; 

    private Animator thisAnimatior; 


    // Start is called before the first frame update
    void Start()
    {
        thisAnimatior = this.GetComponent<Animator>(); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Advance()
    {
        thisAnimatior.SetTrigger("Advance"); 
    }

    public void AssetAdvance()
    {
        swapper.ProgressSwap(); 
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Advance(); 
    }

    public void AdvanceText()
    {

        var currentNum = int.Parse(textToUpdate.text);
        var newNum = (currentNum == 4) ? 1 : currentNum + 1;

        textToUpdate.text = newNum.ToString() ;
    }
}
