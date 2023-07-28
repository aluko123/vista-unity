using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using System.Net;
using UnityEngine.Networking;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

public class API : MonoBehaviour
{
    private int pointsPerLevel = 50;

    private const int minLevel = 1;
    private const int maxLevel = 4; 

    private string jwtToken = "NOT SET";

    private const string baseURL = "https://nij-aisms.nicholasdwest.com/api/nij/ai-sms/";
    private const string authURL = baseURL + "authentication";
    private const string taskListURL = baseURL + "task/user-task-list"; 

    public Dictionary<Key, int> GetFiveKeyLevels()
    {

        //var fivekeyLevels = new Dictionary<Key, int>();

        ////Step 1: Get all tasks
        //var taskList = GetTaskList();

        ////Step 2: Tally up scores in each five key category
        //Dictionary<Key, int> pointTotals = new Dictionary<Key, int>()
        //{
        //    [Key.HealthyThinkingPatterns] = 0,
        //    [Key.EffectiveCopingStrategies] = 0,
        //    [Key.MeaningfulWorkTrajectories] = 0,
        //    [Key.PositiveRelationships] = 0,
        //    [Key.PositiveSocialEngagement] = 0,
        //};
        //Dictionary<string, Key> stringToKey = new Dictionary<string, Key>()
        //{
        //    ["HealthyThinkingPatterns"] = Key.HealthyThinkingPatterns,
        //    ["EffectiveCopingStrategies"] = Key.EffectiveCopingStrategies,
        //    ["MeaningfulWorkTrajectories"] = Key.MeaningfulWorkTrajectories,
        //    ["MeaningfulWorkTrajectories"] = Key.PositiveRelationships,
        //    ["MeaningfulWorkTrajectories"] = Key.PositiveSocialEngagement
        //};

        //foreach (var task in taskList)
        //{
        //    Key thisKey;

        //    try
        //    {
        //        thisKey = stringToKey[task.fivekeyCategory];
        //    } catch (KeyNotFoundException)
        //    {
        //        //Debug.Log($"Category {task.fivekeyCategory} not found from task {task.title}!");
        //        continue;
        //    }

        //    pointTotals[thisKey] = GetPointsEarned(task);
        //}

        ////Step 3: Divide tallies by points-per-level

        //foreach (var entry in pointTotals)
        //{
        //    var level = 1 + Mathf.FloorToInt(entry.Value / pointsPerLevel);
        //    level = Math.Max(minLevel, level);
        //    level = Math.Min(maxLevel, level);

        //    fivekeyLevels[entry.Key] = level;
        //}

        ////Debug:
        ////if (presentationFlag)
        ////{
        ////    return new Dictionary<Key, int>()   
        ////    {
        ////        [Key.EffectiveCopingStrategies] = 4,
        ////        [Key.HealthyThinkingPatterns] = 3,
        ////        [Key.PositiveRelationships] = 3,
        ////        [Key.PositiveSocialEngagement] = 3
        ////    };
        ////}

        return new Dictionary<Key, int>()
        {
            [Key.EffectiveCopingStrategies] = 4,
            [Key.HealthyThinkingPatterns] = 2,
            [Key.PositiveRelationships] = 3,
            [Key.PositiveSocialEngagement] = 3
        };

        //return fivekeyLevels; 
    }

    public Dictionary<Key, int> fiveKeyLevels; 
    private IEnumerator GetFiveKeyLevelsCoroutine()
    {
        fiveKeyLevels = new Dictionary<Key, int>()
        {
            [Key.EffectiveCopingStrategies] = 4,
            [Key.HealthyThinkingPatterns] = 2,
            [Key.PositiveRelationships] = 3,
            [Key.PositiveSocialEngagement] = 3
        };

        yield break; 
    }

    //private IEnumerator PresentationUpdate()
    //{
    //    yield return new WaitForSeconds(15);
    //    presentationFlag = true;
    //}

    // Start is called before the first frame update
    void Start()
    {
        System.IO.File.WriteAllLines("FINDME.txt", new string[] { "bababooe" });
        fiveKeyLevels = new Dictionary<Key, int>();
        StartCoroutine(Initialize());
        //SignIn();
        //GetFiveKeyLevels();
        //StartCoroutine(PresentationUpdate()); 

    }

    private IEnumerator Initialize()
    {

        //Sign in
        yield return SignInCoroutine();

        //Get five key levels
        print("B"); 
        yield return GetTaskListCoroutine();
        yield return GetFiveKeyLevelsCoroutine(); 

        //Distribute results
        //TODO
    }

    private IEnumerator SignInCoroutine()
    {
        print("Signing in...");

        //Set credentials
        Credentials credentials = new Credentials();
        credentials.username = "Nick Diliberti";
        credentials.password = "1111";

        var content = "{" +
            "\"password\":\"" +
            "1111" +
            "\", \"username\":\"" +
            "Nick Diliberti" +
            "\"}";

        //Send web request
        //Wait for response
        yield return GetWebResponseCoRoutine("POST", authURL, content, null);

        //Set JWT
        print("Applying " + webResult); 
        var parsedResponse = JsonUtility.FromJson<AuthorizationResponse>(webResult);
        this.jwtToken = parsedResponse.jwtToken;

        yield break; 
    }

    private void SignIn()
    {


        //var login = GetCredentialsFromFile(credentialsFileName);
        //var content = "{" +
        //    "\"password\":\"" +
        //    login.password +
        //    "\", \"username\":\"" +
        //    login.username +
        //    "\"}";

        var content = "{" +
            "\"password\":\"" +
            "1111" +
            "\", \"username\":\"" +
            "Nick Diliberti" +
            "\"}";

        print("Signing in...");
        var result = GetWebResponse("POST", authURL, content, null);
        print("Sign in complete.  Results:");
        print(result);
        var resultObject = JsonUtility.FromJson<AuthorizationResponse>(result);
        this.jwtToken = resultObject.jwtToken;

        if (this.jwtToken != "NOT SET")
            print("API sign-in successful");
        else
            print("API sign-in failed");
    }

    private string webResult; 
    private IEnumerator GetWebResponseCoRoutine(string method, string url, string content, List<(string, string)> headers)
    {

        var contentsAsBytes = Encoding.ASCII.GetBytes(content);

        //Create request
        UnityWebRequest request = new UnityWebRequest();
        request.certificateHandler = new FixedHandler();
        request.method = method;
        request.uploadHandler = new UploadHandlerRaw(contentsAsBytes);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.url = url;
        request.SetRequestHeader("Content-Type", "application/json");

        //Add any additional headers
        if (headers != null)
            foreach (var header in headers)
                request.SetRequestHeader(header.Item1, header.Item2);

        //Send request
        request.SendWebRequest();
        while (!request.isDone) 
        {
            yield return null; 
        } 

        //Return result
        webResult = request.downloadHandler.text;

        print("Result gotten: " + webResult); 

        yield break; 
    }


    private string GetWebResponse(string method, string url, string content, List<(string, string)> headers)
    {

        var contentsAsBytes = Encoding.ASCII.GetBytes(content);

        //Create request
        UnityWebRequest request = new UnityWebRequest();
        request.certificateHandler = new FixedHandler();
        request.method = method; 
        request.uploadHandler = new UploadHandlerRaw(contentsAsBytes);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.url = url;
        request.SetRequestHeader("Content-Type", "application/json");

        //Add any additional headers
        if (headers != null)
            foreach (var header in headers)
                request.SetRequestHeader(header.Item1, header.Item2); 

        //Send request
        request.SendWebRequest();
        while (!request.isDone) {} //Hack to get coroutine result in synchronous function

        //Return result
        var result = request.downloadHandler.text;
        return result; 
    }

    private Credentials GetCredentialsFromFile(string filename)
    {
        //var fullFilePath = Path.Combine(baseFilePath, filename); 
        //var jsonString = File.ReadAllText(fullFilePath);
        //var toReturn = JsonUtility.FromJson<Credentials>(jsonString);

        var toReturn = new Credentials();
        toReturn.username = "Nick Diliberti";
        toReturn.password = "1111"; 

        return toReturn; 
    }

    private List<TaskResponse> taskList; 
    private List<TaskResponse> GetTaskList()
    {

        if(taskList == null)
        {
            var headers = new List<(string, string)>() { ("Authorization", "Bearer " + this.jwtToken) };
            var jsonString = GetWebResponse("GET", taskListURL, "test", headers);
            jsonString = "{\"tasks\": " + jsonString + "}";

            TaskListResponse tasks = JsonUtility.FromJson<TaskListResponse>(jsonString);

            taskList = new List<TaskResponse>(tasks.tasks);
        }

        return taskList;
    }

    private IEnumerator GetTaskListCoroutine()
    {
        print("A"); 

        if (taskList == null)
        {
            var headers = new List<(string, string)>() { ("Authorization", "Bearer " + this.jwtToken) };
            yield return GetWebResponseCoRoutine("GET", taskListURL, "test", headers);
            var jsonString = "{\"tasks\": " + webResult + "}";

            print("Task list:"); 
            print(webResult);
            TaskListResponse tasks = JsonUtility.FromJson<TaskListResponse>(jsonString);

            taskList = new List<TaskResponse>(tasks.tasks);
        }

        yield break;
    }

    private int GetPointsEarned(TaskResponse task)
    {
        int total = 0;

        if (task.progress >= 0.2f)
            total += task.rewardAmt1;
        if (task.progress >= 0.4f)
            total += task.rewardAmt2;
        if (task.progress >= 0.6f)
            total += task.rewardAmt3;
        if (task.progress >= 0.8f)
            total += task.rewardAmt4;
        if (task.progress >= 1f)
            total += task.rewardAmt5;

        return total; 
    }


    class FixedHandler : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            return true; 
        }
    }

    class AuthorizationResponse
    {
        public string message, jwtToken, username;
        public int userId;
        
    }

    [Serializable]
    public class TaskResponse
    {
        public string fivekeyCategory;
        public int rewardAmt1, rewardAmt2, rewardAmt3, rewardAmt4, rewardAmt5;
        public float progress;
        public string title; 
    }

    [Serializable]
    public class TaskListResponse
    {
        public TaskResponse[] tasks; 
    }

    class Credentials
    {
        public string username, password; 
    }

    public enum Key
    {
        HealthyThinkingPatterns,
        PositiveRelationships,
        PositiveSocialEngagement,
        MeaningfulWorkTrajectories,
        EffectiveCopingStrategies
    }

    public static string keyToString(Key key)
    {
        var keyNames = new Dictionary<API.Key, string>()
        {
            [API.Key.EffectiveCopingStrategies] = "Effective Coping Strategies",
            [API.Key.HealthyThinkingPatterns] = "Healthy Thinking Patterns",
            [API.Key.MeaningfulWorkTrajectories] = "Meaningful Work Trajectories",
            [API.Key.PositiveRelationships] = "Positive Relationships",
            [API.Key.PositiveSocialEngagement] = "Positive Social Engagement"
        };

        return keyNames[key]; 
    }
}
