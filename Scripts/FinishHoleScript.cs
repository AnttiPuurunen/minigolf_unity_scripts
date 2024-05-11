using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FinishHoleScript : MonoBehaviour
{
    [SerializeField] GameObject resultsPanel;
    [SerializeField] TextMeshProUGUI resultsTxt;
    [SerializeField] GameObject cMFreelookCamera;
    [SerializeField] private Slider _slider;
    [SerializeField] private TextMeshProUGUI _parText;
    [SerializeField] GameObject summaryPanel;
    [SerializeField] TextMeshProUGUI courseSummary;
    [SerializeField] GameObject endPanel;
    [SerializeField] GameObject endSummary;
    [SerializeField] TextMeshProUGUI endTotalShots;

    public int par;
    private VerticalLayoutGroup summaryPanelLayout;
    private HorizontalLayoutGroup endSummaryPanelLayout;
    private int endTotal;

    // Start is called before the first frame update
    void Start()
    {
        endTotal = 0;
        PlayerInput.strokes = 0;
        _parText.text = "Par: " + par;
        summaryPanelLayout = summaryPanel.GetComponent<VerticalLayoutGroup>();
        endSummaryPanelLayout = endSummary.GetComponent<HorizontalLayoutGroup>();

        // Clear the course records data if the game is started again from the beginning, to avoid multiple entries for a course
        if (DataManager.Instance.courseRecords.Count > 0 & SceneManager.GetActiveScene().buildIndex == 1 )
        {
            DataManager.Instance.courseRecords.Clear();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // When the ball goes in the hole
        if (other.gameObject.CompareTag("Ball"))
        {
            // Show the results panel, hide other UI elements/disable free look camera
            _slider.gameObject.SetActive(false);
            cMFreelookCamera.SetActive(false);

            if (SceneManager.GetActiveScene().buildIndex == 3)
            {
                endPanel.SetActive(true);
            } else
            {
                resultsPanel.SetActive(true);
            }
            
            if (PlayerInput.strokes == 1)
            {
                resultsTxt.text = "Hole in One!";
            }
            else if (PlayerInput.strokes == par - 1)
            {
                resultsTxt.text = "Birdie!";
            }
            else if (PlayerInput.strokes == par - 2)
            {
                resultsTxt.text = "Eagle!";
            }
            else if (par == 5 & PlayerInput.strokes == par - 3)
            {
                resultsTxt.text = "Albatross!";
            }
            else if (PlayerInput.strokes == par)
            {
                resultsTxt.text = "Par!";
            }
            else if (PlayerInput.strokes == par + 1)
            {
                resultsTxt.text = "Bogey!";
            }
            else if (PlayerInput.strokes == par + 2)
            {
                resultsTxt.text = "Double Bogey!";
            }
            else if (PlayerInput.strokes == par + 3)
            {
                resultsTxt.text = "Triple Bogey!";
            }
            else
            {
                resultsTxt.text = "Too Bad!";
            }

            // Save course details in the DataManager
            // Check if the course data exists, if it does, replace it with the new data
            if (DataManager.Instance.courseRecords.Exists(x => x.CourseId == SceneManager.GetActiveScene().buildIndex)) 
            {
                DataManager.Instance.courseRecords[SceneManager.GetActiveScene().buildIndex-1] = new CourseRecord(SceneManager.GetActiveScene().buildIndex, par, PlayerInput.strokes);
            } else
            {
                DataManager.Instance.courseRecords.Add(new CourseRecord(SceneManager.GetActiveScene().buildIndex, par, PlayerInput.strokes));
            }

            foreach (CourseRecord cr in DataManager.Instance.courseRecords)
            {
                // Show a summary of each course, add a text element from a prefab for each course's data
                TextMeshProUGUI summaryText = Instantiate(courseSummary);

                if (SceneManager.GetActiveScene().buildIndex == 3)
                {
                    summaryText.transform.SetParent(endSummaryPanelLayout.transform, false);
                } else
                {
                    summaryText.transform.SetParent(summaryPanelLayout.transform, false);
                }
                
                summaryText.text = "Hole " + cr.CourseId + " | " + "Par " + cr.Par + " | " + "Result: " + (cr.Result - cr.Par > 0 ? "+" + (cr.Result - cr.Par) : cr.Result - cr.Par);
                endTotal += cr.Result - cr.Par;
            }
            
            endTotalShots.text = "End total: " + (endTotal > 0 ? "+" + endTotal : endTotal);
        }
    }
}
