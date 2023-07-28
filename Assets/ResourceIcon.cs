using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceIcon : MonoBehaviour
{
    public ArrowPair arrowPair;
    private bool isHidden;
    public Animator animator;
    public bool statusLocked; 

    private Collider collider;


    // Start is called before the first frame update
    void Start()
    {
        this.collider = this.gameObject.GetComponent<Collider>();
        this.animator = this.gameObject.GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        collider.enabled = arrowPair.IsVisible() && !isHidden;
        this.animator.SetBool("Hidden", this.isHidden);
    }

    public void Hide()
    {
        this.isHidden = true; 
    }

    public void Show()
    {
        this.isHidden = false; 
    }

    public void SetVisible(bool newVisible)
    {
        if (this.statusLocked)
            return; 

        var oldVisible = !this.isHidden;

        if (newVisible == oldVisible)
            return;

        if (newVisible == true)
            Show();

        if (newVisible == false)
            Hide();

        this.isHidden = !newVisible; 
    }
}
