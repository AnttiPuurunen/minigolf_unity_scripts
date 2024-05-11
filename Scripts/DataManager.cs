using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public record CourseRecord(int CourseId, int Par, int Result)
{
    // A simple record to store data from played courses
    public int CourseId { get; set; } = CourseId;
    public int Par { get; set; } = Par;
    public int Result { get; set; } = Result;
};
public class DataManager : MonoBehaviour
{
    // A singleton instance of the DataManager-gameobject for transferring data from scene to scene
    public static DataManager Instance;

    public List<CourseRecord> courseRecords = new List<CourseRecord>();

    private void Awake()
    {
        // Make sure there is only ever one instance of the DataManager
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}