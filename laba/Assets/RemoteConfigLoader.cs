using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class RemoteConfigLoader : MonoBehaviour
{
    [Header("Config URL")]
    [SerializeField] private string configUrl = "https://shimailudoed.github.io/Build/config/weapons.json";
    
    [Header("Default values")]
    [SerializeField] private Weapon[] defaultWeapons = new Weapon[]
    {
        new Weapon(1, 5f, 1f),
        new Weapon(2, 10f, 0.8f)
    };

    // Массив загруженных оружий
    public List<Weapon> loadedWeapons { get; private set; }
    
    private string localFilePath;

    void Awake()
    {
        localFilePath = Path.Combine(Application.persistentDataPath, "weapon_config.json");
        loadedWeapons = new List<Weapon>();
    }

    void Start()
    {
        StartCoroutine(LoadConfig());
    }

    IEnumerator LoadConfig()
    {
        yield return StartCoroutine(TryLoadFromRemote());
        
        if (loadedWeapons.Count == 0)
        {
            LoadFromLocal();
        }
        
        if (loadedWeapons.Count == 0)
        {
            loadedWeapons.AddRange(defaultWeapons);
        }
        else
        {
            defaultWeapons = loadedWeapons.ToArray();
        }
        
        PrintWeaponsArray();
    }

    void PrintWeaponsArray()
    {
        Weapon[] weaponsArray = loadedWeapons.ToArray();
        
        for (int i = 0; i < weaponsArray.Length; i++)
        {
            Weapon w = weaponsArray[i];
            Debug.Log($"[{i}] Weapon ID: {w.id}, Damage: {w.damage}, Cooldown: {w.cooldown}");
        }
        
        Debug.Log($"Total weapons: {weaponsArray.Length}");
    }

    IEnumerator TryLoadFromRemote()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(configUrl))
        {
            yield return request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                string rawData = request.downloadHandler.text;
                
                if (configUrl.Contains(".json"))
                    ParseJson(rawData);
                else
                    ParseCSV(rawData);
                
                if (loadedWeapons.Count > 0)
                    SaveToLocal();
            }
        }
    }

    void ParseJson(string json)
    {
        try
        {
            WeaponDataWrapper wrapper = JsonUtility.FromJson<WeaponDataWrapper>(json);
            if (wrapper?.weapons != null)
            {
                loadedWeapons.Clear();
                foreach (Weapon w in wrapper.weapons)
                {
                    if (w.damage >= 0 && w.cooldown > 0)
                    {
                        loadedWeapons.Add(w);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"JSON parsing error: {e.Message}");
        }
    }

    void ParseCSV(string csv)
    {
        try
        {
            string[] lines = csv.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            loadedWeapons.Clear();
            
            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (string.IsNullOrEmpty(line)) continue;
                
                string[] values = line.Split(',');
                if (values.Length >= 3)
                {
                    if (int.TryParse(values[0].Trim(), out int id) &&
                        float.TryParse(values[1].Trim(), out float damage) &&
                        float.TryParse(values[2].Trim(), out float cooldown))
                    {
                        if (damage >= 0 && cooldown > 0)
                        {
                            loadedWeapons.Add(new Weapon(id, damage, cooldown));
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"CSV parsing error: {e.Message}");
        }
    }

    void SaveToLocal()
    {
        try
        {
            WeaponDataWrapper wrapper = new WeaponDataWrapper();
            wrapper.weapons = loadedWeapons.ToArray();
            string json = JsonUtility.ToJson(wrapper, true);
            File.WriteAllText(localFilePath, json);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save cache: {e.Message}");
        }
    }

    void LoadFromLocal()
    {
        try
        {
            if (File.Exists(localFilePath))
            {
                string json = File.ReadAllText(localFilePath);
                WeaponDataWrapper wrapper = JsonUtility.FromJson<WeaponDataWrapper>(json);
                if (wrapper?.weapons != null)
                {
                    loadedWeapons.Clear();
                    loadedWeapons.AddRange(wrapper.weapons);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load cache: {e.Message}");
        }
    }
}

[Serializable]
public class WeaponDataWrapper
{
    public Weapon[] weapons;
}