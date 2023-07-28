using UnityEngine;

public class VistaElementManager : MonoBehaviour
{
    public VistaElementConfig houseConfig, boatConfig, carConfig;
    public VistaAccessConfig gardenConfig, friendsConfig; 

    public int targetLevel { get; set; }
    public int targetConfigIdx {
        set
        {
            targetConfig = possibleTargets[value]; 
        }
    }

    private VistaElementConfig targetConfig;
    private VistaElementConfig[] possibleTargets; 

    // Start is called before the first frame update
    void Start()
    {
        possibleTargets = new VistaElementConfig[] { houseConfig, boatConfig, carConfig };
        targetConfig = possibleTargets[0];
        targetLevel = 1; 
    }

    // Update is called once per frame
    void Update()
    {
        //Debug config selector
        if (Input.GetKeyDown(KeyCode.H))
            targetConfig = houseConfig;
        else if (Input.GetKeyDown(KeyCode.C))
            targetConfig = carConfig;
        else if (Input.GetKeyDown(KeyCode.B))
            targetConfig = boatConfig; 
            
        //Debug target level selector
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            targetLevel = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            targetLevel = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            targetLevel = 3;
        }
        else if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            targetLevel = 4;
        }

        else if (Input.GetKeyDown(KeyCode.Space))
        {
            StartSwap();
        }
    }

    public void StartSwap()
    {
        //Signal animator to start animation on main asset
        targetConfig.animator.SetTrigger("continue");

    }

    public void ExchangeModels()
    {
        //Disable all models
        foreach (GameObject g in targetConfig.levels)
            g.SetActive(false);

        //Enable target asset & accent
        var targetIdx = targetLevel - 1;
        targetConfig.levels[targetIdx].SetActive(true);

        //Signal animator to continue animation
        targetConfig.animator.SetTrigger("continue");
    }

    [System.Serializable]
    public struct VistaElementConfig
    {
        public GameObject[] levels;
        public Animator animator;
    }

    public struct VistaAccessConfig
    {
        public GameObject[][] levels;
        public Animator animator; 
    }
}
