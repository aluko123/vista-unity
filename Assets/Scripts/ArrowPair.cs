using System.Collections;
using UnityEngine;

public class ArrowPair : MonoBehaviour
{
    public AnimationCurve showCurve;
    public Vector3 showDisplacement;
    public float transitionTime;
    public float maxScale = 1; 

    private bool isHidden, isBusy;
    private float animationStartTime;
    private Vector3 startPosition;

    public Camera cameraToMove;
    public GameObject cameraZoomGo;
    private Vector3 oldCameraPosition;
    private Quaternion oldCameraRotation;

    private ProgressBar barGUI;
    private HouseSwapper associatedSwapper;


    [Header("Hammer effects")]
    public int numHammers;
    public int numWaves; 
    public float hammerWaveDelay;
    public float hammerDestroyDelay; 
    public GameObject hammerPrefab;
    public Vector2 hammerHeightRange;
    public float hammerRadius;
    public GameObject hammerSubject;
    public float hammerScale;
    public float interHammerDelay;

    [Header("Icon effects")]
    public GameObject icon; 
    public Transform iconCloseupTarget;
    private Vector3 iconStartLocalPos;
    private Quaternion iconStartRot;

    private static bool _isZoomed;

    void Start()
    {
        isBusy = false;
        isHidden = true;
        startPosition = transform.position;

        oldCameraPosition = cameraToMove.gameObject.transform.position;
        oldCameraRotation = cameraToMove.gameObject.transform.rotation;

        barGUI = GameObject.FindObjectOfType<ProgressBar>();
        associatedSwapper = this.transform.parent.gameObject.GetComponentInChildren<HouseSwapper>();

        if(icon != null)
        {
            iconStartLocalPos = icon.transform.localPosition;
            iconStartRot = icon.transform.rotation;
        }

    }

    private void Update()
    {
        //If asset is at current max level, hide top arrow
        //if (Input.GetKeyDown(KeyCode.S))
        //    StartCoroutine(SpawnHammers());

        if (Input.GetMouseButtonDown(0) && _isZoomed && !isBusy)
        {
            Toggle();
        }

        if (!this.isHidden)
            UpdateGUI(); 

    }

    public void Toggle()
    {
        if (!isBusy)
        {

            if (isHidden && !ArrowPair._isZoomed)
            {
                StartCoroutine(Show());
                StartCoroutine(ZoomIn());

                ArrowPair._isZoomed = true;
                isBusy = true;

                UpdateGUI();
                barGUI.animator.SetTrigger("Toggle");
            }

            if (!isHidden && ArrowPair._isZoomed)
            {

                StartCoroutine(Hide());
                StartCoroutine(ZoomOut());

                ArrowPair._isZoomed = false;
                isBusy = true;

                UpdateGUI();
                barGUI.animator.SetTrigger("Toggle");
            }

        }
    }

    private IEnumerator Hide()
    {
        animationStartTime = Time.time;

        for (; ; )
        {
            var timeSinceStart = Time.time - animationStartTime;
            var percentAlong = (timeSinceStart / transitionTime);
            var displacementPercent = 1 - showCurve.Evaluate(percentAlong);

            var thisDisplacement = showDisplacement * displacementPercent;
            var newPosition = startPosition + thisDisplacement;
            this.transform.position = newPosition;
            this.transform.localScale = maxScale * Vector3.one * displacementPercent;

            if (percentAlong < 1)
            {
                yield return null;
            }
            else
            {
                isHidden = true;
                isBusy = false;
                yield break;
            }

        }

    }

    private IEnumerator Show()
    {
        animationStartTime = Time.time;

        for (; ; )
        {
            var timeSinceStart = Time.time - animationStartTime;
            var percentAlong = (timeSinceStart / transitionTime);
            var displacementPercent = showCurve.Evaluate(percentAlong);

            var thisDisplacement = showDisplacement * displacementPercent;
            var newPosition = startPosition + thisDisplacement;
            this.transform.position = newPosition;
            this.transform.localScale = maxScale * Vector3.one * displacementPercent;

            if (percentAlong < 1)
            {
                yield return null;
            }
            else
            {
                isHidden = false;
                isBusy = false;
                yield break;
            }

        }
    }

    private IEnumerator ZoomIn()
    {
        var startPos = oldCameraPosition;
        var startRot = oldCameraRotation;
        var endPos = cameraZoomGo.transform.position;
        var endRot = cameraZoomGo.transform.rotation;
        var startScale = 0f;
        var endScale = maxScale; 

        for (;;)
        {
            var displacementPercent = showCurve.Evaluate((Time.time - animationStartTime / transitionTime));

            cameraToMove.transform.position = Vector3.Lerp(startPos, endPos, displacementPercent);
            cameraToMove.transform.rotation = Quaternion.Lerp(startRot, endRot, displacementPercent);

            icon.transform.localPosition = Vector3.Lerp(iconStartLocalPos, iconCloseupTarget.transform.localPosition, displacementPercent);
            icon.transform.rotation = Quaternion.Lerp(iconStartRot, iconCloseupTarget.transform.rotation, displacementPercent);

            if (displacementPercent < 1)
            {
                yield return null;
            }
            else
            {
                yield break;
            }

        }
    }

    private IEnumerator ZoomOut()
    {
        var startPos = cameraToMove.transform.position;
        var startRot = cameraToMove.transform.rotation;
        var endPos = oldCameraPosition;
        var endRot = oldCameraRotation;

        for (; ; )
        {
            var displacementPercent = showCurve.Evaluate((Time.time - animationStartTime / transitionTime));

            cameraToMove.transform.position = Vector3.Lerp(startPos, endPos, displacementPercent);
            cameraToMove.transform.rotation = Quaternion.Lerp(startRot, endRot, displacementPercent);

            icon.transform.localPosition = Vector3.Lerp(iconStartLocalPos, iconCloseupTarget.transform.localPosition, 1-displacementPercent);
            icon.transform.rotation = Quaternion.Lerp(iconStartRot, iconCloseupTarget.transform.rotation, 1-displacementPercent);

            if (displacementPercent < 1)
            {
                yield return null;
            }
            else
            {
                yield break;
            }

        }


    }

    private void UpdateGUI()
    {
        barGUI.key = associatedSwapper.associatedKey;
        barGUI.level = associatedSwapper.maxLevel;

        var GUIClaimedPercentage = (float)(associatedSwapper.claimedPoints % 100) / (float)associatedSwapper.pointsPerLevel * 100f;

        barGUI.percentage = GUIClaimedPercentage;
        var unclaimedPoints = associatedSwapper.totalPoints - associatedSwapper.claimedPoints; 
        barGUI.unappliedPercentage = (float)unclaimedPoints / (float)associatedSwapper.pointsPerLevel * 100f;
    }

    public IEnumerator SpawnHammers()
    {
        for (int i = 0; i < numWaves; i++)
        {
            StartCoroutine(SpawnHammerWave());
            yield return new WaitForSeconds(hammerWaveDelay);
        }

    }

    private IEnumerator SpawnHammerWave()
    {
        var hammersPerWave = numHammers / numWaves;

        for (int i = 0; i < hammersPerWave; i++)
        {
            var hammer  = SpawnHammer();
            yield return new WaitForSeconds(Random.Range(0, interHammerDelay));
            GameObject.Destroy(hammer, hammerDestroyDelay);
        }
    }

    private GameObject SpawnHammer()
    {
        //Create gameobject
        var hammer = GameObject.Instantiate(hammerPrefab);

        //Choose height
        var verticalOffset = Random.Range(hammerHeightRange.x, hammerHeightRange.y);

        //Get x, y offset from random angle
        var angle = Random.Range(0, 360) * Mathf.Deg2Rad;
        var horizontalOffset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * hammerRadius;

        //Place and rotate gameobject
        var offset = new Vector3(horizontalOffset.x, verticalOffset, horizontalOffset.y);
        hammer.transform.position = hammerSubject.transform.position + offset;
        hammer.transform.LookAt(hammerSubject.transform);
        hammer.transform.Rotate(Vector3.up * 180f); 
        hammer.transform.localScale = Vector3.one * hammerScale;

        return hammer; 
    }

    public bool IsVisible()
    {
        return !isHidden; 
    }

}
