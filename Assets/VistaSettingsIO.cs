using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class VistaSettingsIO : MonoBehaviour
{
    public HouseSwapper houseSwapper, carSwapper, boatSwapper, treeSwapper, friendSwapper; 

    private VistaSettings settings;
    private List<(string, HouseSwapper)> levelAssocs;
    private List<(string, string, HouseSwapper)> pointAssocs; 

    void Start()
    {
        levelAssocs = GetLevelAssocs();
        pointAssocs = GetPointAssocs(); 

        LoadLevels();
        LoadClaimedPoints();
        LoadTotalPoints(); 
        _instance = this; 
    }

    //Singleton pattern
    private static VistaSettingsIO _instance; 
    public static VistaSettingsIO GetInstance()
    {
        return _instance; 
    }

    void Update()
    {
        //If current settings are different to saved settings, save new settings
        foreach(var tuple in levelAssocs)
            if (tuple.Item2.GetCurrentAssetLevel() != PlayerPrefs.GetInt(tuple.Item1))
                SaveSettings();
    }

    private void LoadLevels()
    {
        foreach(var tuple in levelAssocs)
        {
            tuple.Item2.startIdx = PlayerPrefs.GetInt(tuple.Item1, 1) - 1; 
        }
    }

    private void LoadClaimedPoints()
    {
        foreach(var tuple in pointAssocs)
        {
            var claimedPoints = PlayerPrefs.GetInt(tuple.Item1, 0);
            var swapper = tuple.Item3;

            swapper.InitializeClaimedPoints(claimedPoints);
            print("Initialized " + swapper.gameObject.name + " to " + claimedPoints + " claimed points.");
        }
    }

    private void LoadTotalPoints()
    {
        foreach(var tuple in pointAssocs)
        {
            var totalPoints = PlayerPrefs.GetInt(tuple.Item2, 0);
            var swapper = tuple.Item3;

            swapper.SetTotalPoints(totalPoints);
            print("Initialized " + swapper.gameObject.name + " to " + totalPoints + " total points.");
        }
    }

    public void SaveTotalPoints()
    {
        print("Saving total points...");
        foreach(var tuple in pointAssocs)
        {
            var key = tuple.Item2;
            var value = tuple.Item3.totalPoints;

            PlayerPrefs.SetInt(key, value); 
        }
    }

    public void SaveClaimedPoints()
    {
        print("Saving claimed points..."); 
        foreach (var tuple in pointAssocs)
        {
            var playerPrefsKey = tuple.Item1;
            var claimedPoints = tuple.Item3.claimedPoints;

            PlayerPrefs.SetInt(playerPrefsKey, claimedPoints);
        }
    }

    public void SetTotalPoints(HouseSwapper swapper, int totalPoints)
    {
        swapper.SetTotalPoints(totalPoints); 
    }

    public void SaveSettings()
    {
        print("Saving vista settings..."); 
        foreach( var tuple in levelAssocs)
        {
            PlayerPrefs.SetInt(tuple.Item1, tuple.Item2.GetCurrentAssetLevel());
        }
    }

    public List<(string, HouseSwapper)> GetLevelAssocs()
    {
        return new List<(string, HouseSwapper)>
        {
            ("House level", houseSwapper),
            ("Car level", carSwapper),
            ("Boat level", boatSwapper),
            ("Tree level", treeSwapper),
            ("Friend level", friendSwapper)
        };
    }

    public List<(string, string, HouseSwapper)> GetPointAssocs()
    {
        List<string> prefixes = new List<string> { "House", "Car", "Boat", "Tree", "Friend" };
        List<string> suffixes = new List<string> { "claimed", "total" };
        List<HouseSwapper> swappers = new List<HouseSwapper> { houseSwapper, carSwapper, boatSwapper, treeSwapper, friendSwapper };

        var toReturn = new List<(string, string, HouseSwapper)>(); 

        for(int i = 0; i < prefixes.Count; i++)
        {
            var item1 = prefixes[i] + " " + suffixes[0];
            var item2 = prefixes[i] + " " + suffixes[1];
            var item3 = swappers[i];
            toReturn.Add((item1, item2, item3)); 
        }

        return toReturn;

    }




    private class VistaSettings 
    {
        [Range(1, 4)]
        public int houseLevel, carLevel, boatLevel, treeLevel, friendLevel;

        public VistaSettings(int houseLevel, int carLevel, int boatLevel, int treeLevel, int friendLevel)
        {
            this.houseLevel = houseLevel;
            this.carLevel = carLevel;
            this.boatLevel = boatLevel;
            this.treeLevel = treeLevel;
            this.friendLevel = friendLevel; 
        }

        public VistaSettings(HouseSwapper house, HouseSwapper car, HouseSwapper boat, HouseSwapper tree)
        {
            this.houseLevel = house.GetCurrentAssetLevel();
            this.carLevel = car.GetCurrentAssetLevel();
            this.boatLevel = boat.GetCurrentAssetLevel();
            this.treeLevel = tree.GetCurrentAssetLevel(); 
        }

        public bool Equals(VistaSettings other)
        {
            bool houseEqual = this.houseLevel == other.houseLevel;
            bool carEqual = this.carLevel == other.carLevel;
            bool boatEqual = this.boatLevel == other.boatLevel;
            bool treeEqual = this.treeLevel == other.treeLevel;

            return (houseEqual && carEqual  && boatEqual && treeEqual); 
        }

        public bool EqualsCurrentSettings(HouseSwapper house, HouseSwapper car, HouseSwapper boat, HouseSwapper tree)
        {
            bool houseEqual = this.houseLevel == house.GetCurrentAssetLevel();
            bool carEqual = this.carLevel == car.GetCurrentAssetLevel();
            bool boatEqual = this.boatLevel == boat.GetCurrentAssetLevel();
            bool treeEqual = this.treeLevel == tree.GetCurrentAssetLevel(); 

            return (houseEqual && carEqual && boatEqual && treeEqual); 
        }
    }
}
