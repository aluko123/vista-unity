using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProgressBar : MonoBehaviour
{
    public int level;
    [Range(0, 100)]
    public float percentage, unappliedPercentage;
    public API.Key key;

    public SpriteRenderer colorFill, unappliedColorFill;
    public float minFillX, maxFillX; 
    public TextMeshPro percentageText, keyText, levelText;
    public Animator animator; 

    // Start is called before the first frame update
    void Start()
    {
        //Blank
    }

    // Update is called once per frame
    void Update()
    {
        UpdateFill();
        UpdateUnappliedFill(); 

        if (percentage < 0.001f)
        {
            percentageText.text = "";
        } else
        {
            percentageText.text = percentage.ToString("00") + "%";
        }

        keyText.text = API.keyToString(key);
        levelText.text = "Level " + level.ToString(); 
    }

    private void UpdateFill()
    {
        var lerpArgument = percentage / 100f;
        var newXCoordinate = Mathf.Lerp(minFillX, maxFillX, lerpArgument);

        var oldPositionVector = colorFill.transform.localPosition;
        var newPositionVector = oldPositionVector;
        newPositionVector.x = newXCoordinate;

        colorFill.transform.localPosition = newPositionVector; 

    }

    private void UpdateUnappliedFill()
    {
        var lerpArgument = (percentage + unappliedPercentage) / 100f;
        lerpArgument = Mathf.Clamp(lerpArgument, 0, 1); 
        var newXCoordinate = Mathf.Lerp(minFillX, maxFillX, lerpArgument);

        var oldPositionVector = colorFill.transform.localPosition;
        var newPositionVector = oldPositionVector;
        newPositionVector.x = newXCoordinate;

        unappliedColorFill.transform.localPosition = newPositionVector;
    }
}
