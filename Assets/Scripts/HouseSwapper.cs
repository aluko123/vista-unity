using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseSwapper : MonoBehaviour
{
    public AnimationCurve risingCurve;
    public float riseTime, riseDistance;

    public AnimationCurve fallCurve;
    public float fallTime; 

    public List<GameObject> assets;
    public int startIdx; 

    private float startTime;
    private Vector3 basePositionCurrent, basePositionTarget; 
    private GameObject currentAsset, targetAsset;
    private Vector3 animationStartPosition; 

    private Dictionary<GameObject, Vector3> basePositions; 

    private bool busy = false;

    public GameObject topArrow;
    private GameObject bottomArrow; 
    public int maxLevel;
    public API.Key associatedKey;

    [Header("Resource gain animation")]
    public AnimationCurve applicationCurve;
    public ArrowPair hammerSpawner;
    public ResourceIcon gainIcon;

    [Header("Points")]
    public int pointsPerLevel; 
    public int claimedPoints {  get; private set; }
    public int totalPoints {  get; private set; }

    [Header("Points")]
    public int debugStartClaimedPoints;
    public int debugStartUnclaimedPoints; 


    private void Start()
    {
        //Initialize data
        currentAsset = assets[startIdx];
        bottomArrow = topArrow.transform.parent.Find("Down Arrow").gameObject;

        basePositions = new Dictionary<GameObject, Vector3>();
        foreach (var asset in assets)
        {
            basePositions.Add(asset, asset.transform.position); 
        }

        //Initalize scene
        Transition(startIdx);

        foreach (var asset in assets)
            HideAsset(asset);

        //DEBUG: Initailize points
        if(this.debugStartClaimedPoints != 0)
            this.claimedPoints = debugStartClaimedPoints;

        if (this.debugStartUnclaimedPoints != 0)
            this.SetTotalPoints(this.claimedPoints + this.debugStartUnclaimedPoints);

        this.maxLevel = 1 + Mathf.FloorToInt((float)this.claimedPoints / (float)this.pointsPerLevel);
    }

    private void Update()
    {
        var maxLevel = this.maxLevel;
        var currentLevel = this.GetCurrentAssetLevel();

        var topArrowEnabled = currentLevel < maxLevel;
        var bottomArrowEnabled = currentLevel > 1;

        topArrow.SetActive(topArrowEnabled);
        bottomArrow.SetActive(bottomArrowEnabled); 

        UpdateIcon(); 
    }

    /// <summary>
    /// Returns the current asset level, 1-based.
    /// </summary>
    /// <returns></returns>
    public int GetCurrentAssetLevel()
    {
        int index = assets.IndexOf(currentAsset);

        return index + 1; 
    }

    private void HideAsset(GameObject asset)
    {
        asset.SetActive(false);
        asset.transform.position = basePositionCurrent;
    }

    public void GoForward()
    {
        var currentIdx = assets.IndexOf(currentAsset);
        var targetIdx = currentIdx + 1;
        var targetLevel = targetIdx + 1;
        var apiLevel = this.maxLevel;

        var inBounds = targetIdx < assets.Capacity;
        var inLevel = targetLevel <= apiLevel;

        if (inBounds && inLevel)
            Transition(targetIdx);
    }

    public void GoBack()
    {

        var currentIdx = assets.IndexOf(currentAsset);
        var targetIdx = currentIdx - 1;

        if (targetIdx >= 0)
            Transition(targetIdx);
    }

    public void Transition(int targetIdx)
    {

        if (!busy)
        {
            busy = true; 

            targetAsset = assets[targetIdx];
            startTime = Time.time;

            basePositionCurrent = currentAsset.transform.position;
            basePositionTarget = targetAsset.transform.position;

            StartCoroutine(Rise());
        }
    }

    private IEnumerator Rise()
    {

        var startPos = basePositions[currentAsset];
        var endPos = startPos + riseDistance * Vector3.up;

        for (; ; )
        {
            var timeSinceStart = Time.time - startTime;
            var percentAlong = (timeSinceStart) / riseTime;
            var percentHigh = risingCurve.Evaluate(percentAlong);

            currentAsset.transform.position = Vector3.Lerp(startPos, endPos, percentHigh);

            if (percentAlong < 1)
            {
                yield return null;
            }
            else
            {
                yield return StartCoroutine(SwapAssets());
                yield break;
            }

        }
    }

    private IEnumerator SwapAssets()
    {
        //Hide current
        currentAsset.SetActive(false);
        currentAsset.transform.position = basePositionCurrent;

        //Show & Set target to current
        currentAsset = targetAsset;
        basePositionCurrent = basePositionTarget;

        //Position current
        currentAsset.SetActive(true);
        currentAsset.transform.position = basePositionCurrent + riseDistance * Vector3.up;

        yield return StartCoroutine(Fall());
    }

    private IEnumerator Fall()
    {
        var endPos = basePositions[currentAsset];
        var startPos = endPos + riseDistance * Vector3.up;

        for (; ;)
        {
            var timeSinceCurveStart = Time.time - (startTime + riseTime);
            var percentAlong = (timeSinceCurveStart) / fallTime;
            var percentHigh = fallCurve.Evaluate(percentAlong);

            currentAsset.transform.position = Vector3.Lerp(endPos, startPos, percentHigh);

            if (percentAlong < 1)
                yield return null;
            else
            {
                busy = false;
                yield break;

            }
        }
    }

    public void ApplyUnclaimedPoints()
    {
        print("Applying points...");

        //Make icon disappear
        gainIcon.statusLocked = true; 
        gainIcon.Hide(); 

        //Make percentage go up
        StartCoroutine(GainPercentage()); 

        //Spawn hammers
        StartCoroutine(hammerSpawner.SpawnHammers()); 
    }

    private IEnumerator GainPercentage()
    {
        int startPoints = this.claimedPoints;
        float startTime = Time.time;
        float lerpPercent = 0;

        //Claim all points, up to the current level
        var unclaimedPoints = totalPoints - claimedPoints; 

        int pointsToClaim = 0;
        if (this.claimedPoints % 100 + unclaimedPoints > 100)
        {
            pointsToClaim = 100 - this.claimedPoints; 
        } else
        {
            pointsToClaim = unclaimedPoints; 
        }

        while (lerpPercent < 0.999f)
        {
            var elapsedTime = Time.time - startTime;
            lerpPercent = applicationCurve.Evaluate(elapsedTime);

            var targetPoints = startPoints + Mathf.FloorToInt(lerpPercent * (float)pointsToClaim);
            var pointsToClaimNow = targetPoints - this.claimedPoints; 

            ClaimPoints(pointsToClaimNow); 

            yield return new WaitForEndOfFrame(); 
        }

        ClaimPoints(totalPoints - claimedPoints);
        gainIcon.statusLocked = false;


        VistaSettingsIO.GetInstance().SaveClaimedPoints();
        yield return null; 
    }

    //public void AddPoints(int amount) {
    //    this.totalPoints += amount; 

    //    if (this.unclaimedPoints > 0)
    //    {
    //        this.gainIcon.SetVisible(true); 
    //    }
    //} 

    public void ClaimPoints(int amount)
    {
        if (this.totalPoints == this.claimedPoints)
            return; 

        var amountToClaim = Mathf.Clamp(amount, 0, this.pointsPerLevel); 

        this.claimedPoints += amountToClaim;

        this.maxLevel = 1 + Mathf.FloorToInt((float)this.claimedPoints / (float)this.pointsPerLevel);

        gainIcon.SetVisible(false);
    }

    private void UpdateIcon()
    {
        this.gainIcon = this.transform.parent.GetComponentInChildren<ResourceIcon>(); //Only returns the active icon

        if (gainIcon == null)
            return; 

        //Enable icon if necessary
        gainIcon.SetVisible(this.totalPoints > this.claimedPoints); 
    }

    public void InitializeClaimedPoints(int points)
    {
        this.claimedPoints = points; 
    }

    public void SetTotalPoints(int totalPoints)
    {
        this.totalPoints = totalPoints; 
    }
}
