using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;

// © 2018 TheFlyingKeyboard and released under MIT License 
// theflyingkeyboard.net 
public class GameManager : MonoBehaviour
{
    #region Private Variables
    [DllImport("user32.dll")]
    static extern bool SetCursorPos(int X, int Y);

    [Header("Game Data")]
    [SerializeField] private float previousMousePositionX;
    [SerializeField] private float previousMousePositionY;

    [SerializeField] private float previousDeltaX;
    [SerializeField] private float previousDeltaY;

    [SerializeField] private float deltaX;
    [SerializeField] private float deltaY;

    [SerializeField] private bool buttonPressed = false;
    [SerializeField] private bool TestRunning;

    [SerializeField] private int pressedTime;
    [SerializeField] private int startTime;
    [SerializeField] private int waitTimer;
    [SerializeField] private int testTimer;

    [SerializeField] private int frameCounter;
    [SerializeField] private float screenWidthCenter = (float) Screen.width/2;
    [SerializeField] private float screenHeightCenter = (float) Screen.height/2;

    #region Measurements
    [Header("Measurements")]
    [SerializeField] private int ShakeCounterX;
    [SerializeField] private int ShakeCounterY;
    [SerializeField] private float minDeltaX = float.MaxValue;
    [SerializeField] private float minDeltaY = float.MaxValue;
    [SerializeField] private float maxDeltaX;
    [SerializeField] private float maxDeltaY;
    [SerializeField] private float TotalAbsoluteShakeX;
    [SerializeField] private float TotalAbsoluteShakeY;
    [SerializeField] private double AvgDeltaXpShake;
    [SerializeField] private double AvgDeltaYpShake;
    [SerializeField] private double AvgDeltaXpSec;
    [SerializeField] private double AvgDeltaYpSec;
    [SerializeField] private double ShakesXPerSecond;
    [SerializeField] private double ShakesYPerSecond;
    [SerializeField] private float offset1Time = 0;
    [SerializeField] private float offset2Time = 0;
    [SerializeField] private float offsetXTime1 = 0;
    [SerializeField] private float offsetYTime1 = 0;
    [SerializeField] private float offsetXTime2 = 0;
    [SerializeField] private float offsetYTime2 = 0;
    #endregion

    #endregion

    #region Public Variales

    #region User Info

    public string userID;
    public string parentFolderPath;
    public string folderPath;

    #endregion

    #region Parameters
    [Header("Parameters")]
    //public float measurePeriod = 1f;
    //public float testDuration = 150f;
    //public float waitTime = 5f;

    [SerializeField] private float shakeDeltaThreshold = 0.25f;
    [SerializeField] private int measurePeriod = 1;
    private float shakeDeltaThresholdQuantum = 1f;
    [SerializeField] private int testDuration = 300;
    [SerializeField] private int waitTime = 0;

    #endregion

    #region UI
    [Header("Settings Panel")]

    public InputField userInput;
    public Text userMessage;

    public Slider ShakeDeltaSlider;
    public Text ShakeDeltaValue;

    public Slider TestDurationSlider;
    public Text TestDurationValue;

    //public Slider MeasurePeriodSlider;
    //public Text MeasurePeriodValue;

    //public Slider WaitTimeSlider;
    //public Text WaitTimeValue;

    [Header("Results Panel")]
    public Text ShakeCounterXValue;
    public Text ShakeCounterYValue;
    public Text minDeltaXValue;
    public Text minDeltaYValue;
    public Text maxDeltaXValue;
    public Text maxDeltaYValue;
    public Text TotalAbsoluteShakeXValue;
    public Text TotalAbsoluteShakeYValue;
    public Text AvgDeltaXpShakeValue;
    public Text AvgDeltaYpShakeValue;
    public Text AvgDeltaXpSecValue;
    public Text AvgDeltaYpSecValue;
    public Text ShakesXPerSecondValue;
    public Text ShakesYPerSecondValue;
    public Text offset1TimeValue;
    public Text offset2TimeValue;
    public Text offsetXTime1Value;
    public Text offsetYTime1Value;
    public Text offsetXTime2Value;
    public Text offsetYTime2Value;
    
    [Header("Mouse Info")]
    public Text XText;
    public Text YText;
    public Text deltaXText;
    public Text deltaYText;

    #endregion

    #endregion

    #region Object References
    [Header("Object References")]
    //public GameObject lineDrawer;
    private LineRenderer line;
    private Vector2 mousePosition;

    public GameObject timer;
    public GameObject Background;

    public GameObject Car;
    public GameObject Aeroplane;
    public GameObject SettingsPanel;
    public GameObject EndPanel;
    public GameObject ResultsPanel;
    public GameObject StartButton;
    public GameObject OKButton;
    public GameObject ExitButton;
    #endregion

    //[SerializeField] private bool simplifyLine = false;
    //[SerializeField] private float simplifyTolerance = 0.02f;
    //public int frameCounter = 0;


    #region Main Methods

    private void Start()
    {
        GetComponent<LogHandler>().enabled = false;

        line = GetComponent<LineRenderer>();
        Application.targetFrameRate = 60;
        OKButton.GetComponent<Button>().interactable = false;
        EndPanel.SetActive(false);
        ResultsPanel.SetActive(false);
    }

    public void EnableSettings()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void DisableSettings()
    {
        SettingsPanel.SetActive(false);
    }

    public void StartCountdown() //called when button is pressed
    {
        buttonPressed = true;
        ExitButton.SetActive(false);
        pressedTime = frameCounter; //get frame count when button is pressed
        StartButton.SetActive(false);
        GetComponent<LogHandler>().enabled = true;
    }

    private void Update()
    {
        #region Get Parameters

        if (Screen.width != screenWidthCenter * 2)
        {
            screenWidthCenter = (float) Screen.width / 2;
            Debug.Log("ScreenWidthCenter: " + screenWidthCenter);
        }
        if (Screen.height != screenHeightCenter * 2)
        {
            screenHeightCenter = (float) Screen.height / 2;
            Debug.Log("Screen Height Center: " + screenHeightCenter);
        }
        #endregion

        #region Wait For Countdown

        if (buttonPressed) //If Start Button is pressed
        {
            frameCounter++;
            if (waitTimer < waitTime)
            {
                waitTimer = frameCounter - pressedTime;
                timer.GetComponent<Text>().text = "Test starts in " + (waitTime - waitTimer)/ Application.targetFrameRate;
            }

            if (waitTimer == waitTime )
            {
                SetCursorPos( (int) screenWidthCenter, (int) screenHeightCenter);

                startTime = frameCounter;
                waitTimer++;

                Debug.Log("ΠΑΡΑΜΕΤΡΟΙ");
                Debug.Log("Ανοχή μετατόπισης: " + shakeDeltaThreshold + " pixels");
                Debug.Log("Περίοδος δειγματοληψίας: " + measurePeriod + " frames");
                Debug.Log("Διάρκεια τεστ: " + testDuration/Application.targetFrameRate + " seconds" + Environment.NewLine);
            }

            if (waitTimer == waitTime+1 && testTimer < testDuration)
            {
                if ((int)Math.Ceiling(Input.mousePosition.x) - (int)screenWidthCenter < 2 && (int)Math.Ceiling(Input.mousePosition.y) - (int)screenHeightCenter < 2)
                {
                    TestRunning = true;
                }

                //Debug.Log(Input.mousePosition.x);
                //Debug.Log(Input.mousePosition.y);
                timer.SetActive(false);


                XText.text = "X: " + Input.mousePosition.x.ToString();
                YText.text = "Y: " + Input.mousePosition.y.ToString();
            }
        }
        #endregion

        #region Run Test

        if (TestRunning && testTimer < testDuration) //while test is running
        {
            #region Draw Line

            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            line.positionCount++;
            line.SetPosition(line.positionCount - 1, mousePosition);

            #endregion

            #region Offset Timers

            if ((Input.mousePosition.x > screenWidthCenter + 10) || (Input.mousePosition.x < screenWidthCenter - 10) || (Input.mousePosition.y > screenHeightCenter + 10) || (Input.mousePosition.y < screenHeightCenter - 10))
            {
                offset1Time++;
            }
            if ((Input.mousePosition.x > screenWidthCenter + 50) || (Input.mousePosition.x < screenWidthCenter - 50) || (Input.mousePosition.y > screenHeightCenter + 50) || (Input.mousePosition.y < screenHeightCenter - 50))
            {
                offset2Time++;
            }
            if ((Input.mousePosition.x > screenWidthCenter + 10) || (Input.mousePosition.x < screenWidthCenter - 10))
            {
                offsetXTime1++;
                //Debug.Log("mousePosition.x: " + Input.mousePosition.x);
                //Debug.Log("X offset: " + (Input.mousePosition.x - screenWidthCenter));
            }
            if ((Input.mousePosition.y > screenHeightCenter + 10) || (Input.mousePosition.y < screenHeightCenter - 10))
            {
                offsetYTime1++;
                //Debug.Log("mousePosition.y: " + Input.mousePosition.y);
                //Debug.Log("Y offset: " + (Input.mousePosition.y - screenHeightCenter));
            }

            if ((Input.mousePosition.x > screenWidthCenter + 50) || (Input.mousePosition.x < screenWidthCenter - 50))
            {
                offsetXTime2++;
                //Debug.Log("mousePosition.x: " + Input.mousePosition.x);
                //Debug.Log("X offset: " + (Input.mousePosition.x - screenWidthCenter));
            }
            if ((Input.mousePosition.y > screenHeightCenter + 50) || (Input.mousePosition.y < screenHeightCenter - 50))
            {
                offsetYTime2++;
                //Debug.Log("mousePosition.y: " + Input.mousePosition.y);
                //Debug.Log("Y offset: " + (Input.mousePosition.y - screenHeightCenter));
            }
            #endregion

            #region Vehicles
            //if (Input.mousePosition.y < screenHeightCenter)
            //{
            //    Car.transform.position = Input.mousePosition;
            //    float scaler = (screenHeightCenter - Input.mousePosition.y) / screenHeightCenter;
            //    Car.transform.localScale = new Vector3( 1.5f* scaler + 0.05f, 1.5f * scaler + 0.05f, 1.5f * scaler + 0.05f);
            //}

            //if(Input.mousePosition.y > screenHeightCenter)
            //{
            //    Aeroplane.transform.position = Input.mousePosition;
            //    float scaler = (Input.mousePosition.y - screenHeightCenter) / screenHeightCenter;
            //    Aeroplane.transform.localScale = new Vector3(1.5f * scaler + 0.05f, 1.5f * scaler + 0.05f, 1.5f * scaler + 0.05f);
            //}
            #endregion

            #region Shake Detection

            testTimer = frameCounter - startTime; //increase testTimer
            XText.text = "X: " + Input.mousePosition.x.ToString();
            YText.text = "Y: " + Input.mousePosition.y.ToString();

            if (testTimer % measurePeriod == 0) //check for shake every "measurePeriod" frames
            {
                //deltaX = Mathf.Abs(Input.mousePosition.x - previousMousePositionX);
                //deltaY = Mathf.Abs(Input.mousePosition.y - previousMousePositionY);

                deltaX = Input.mousePosition.x - previousMousePositionX;
                deltaY = Input.mousePosition.y - previousMousePositionY;

                if (previousDeltaX * deltaX < 0 && (Mathf.Abs(deltaX) > shakeDeltaThreshold)) //if change in direction and movement are detected - detect shake in X axis
                {
                    Debug.Log("Οριζόντιο τρέμουλο X: " + deltaX);
                    deltaXText.text = "Shake X: " + deltaX.ToString();
                    TotalAbsoluteShakeX = Mathf.Abs(deltaX) + TotalAbsoluteShakeX;
                    ShakeCounterX++;

                    #region Min/Max X
                    if (Mathf.Abs(deltaX) > maxDeltaX)
                    {
                        maxDeltaX = Mathf.Abs(deltaX);
                    }

                    if (Mathf.Abs(deltaX) < minDeltaX)
                    {
                        minDeltaX = Mathf.Abs(deltaX);
                    }
                    #endregion
                }
                //else
                //{
                //    deltaXText.text = "Shake X: 0";
                //}

                if (previousDeltaY * deltaY < 0 && (Mathf.Abs(deltaY) > shakeDeltaThreshold)) //if change in direction and movement are detected - detect shake in Y axis
                {
                    Debug.Log("Κατακόρυφο τρέμουλο Y: " + deltaY);
                    deltaYText.text = "Shake Y: " + deltaY.ToString();
                    TotalAbsoluteShakeY = Mathf.Abs(deltaY) + TotalAbsoluteShakeY;
                    ShakeCounterY++;

                    #region Min/Max Y
                    if (Mathf.Abs(deltaY) > maxDeltaY)
                    {
                        maxDeltaY = Mathf.Abs(deltaY);
                    }

                    if (Mathf.Abs(deltaY) < minDeltaY)
                    {
                        minDeltaY = Mathf.Abs(deltaY);
                    }
                    #endregion
                }
                //else
                //{
                //    deltaYText.text = "Shake Y: 0";
                //}

                previousDeltaX = deltaX;
                previousDeltaY = deltaY;

                previousMousePositionX = Input.mousePosition.x;
                previousMousePositionY = Input.mousePosition.y;
            }

            #endregion
        }

        if (TestRunning && testTimer == testDuration  ) //check for time-out
        {
            TestRunning = false;
            buttonPressed = false;
            Debug.Log("Τέλος μέτρησης." + Environment.NewLine);
            Debug.Log("ΑΠΟΤΕΛΕΣΜΑΤΑ");
            Calculate();
        }

        #endregion

    }
    
    public void ExitApp()
    {
        Application.Quit();
    }

    #endregion

    #region Secondary Methods

    public void GetPath()
    {
        parentFolderPath = Application.dataPath + "/Users/";
        if (!Directory.Exists(parentFolderPath))
        {
            Directory.CreateDirectory(parentFolderPath);
        }
        //parentFolderPath = Application.dataPath; /*pathInput.text;*/
    }

    public void GetUser()
    {
        userID = userInput.text;
        folderPath = Path.Combine(parentFolderPath, userID);

        userMessage.enabled = false;

        if (folderPath != parentFolderPath)
        {
            userMessage.enabled = true;
            if (Directory.Exists(@folderPath))
            {
                userMessage.text = "This userID already exists! Please enter a different userID.";
                Debug.Log("UserID already exists!");
                return;
            }
            else
            {
                userMessage.text = "Διαδρομή αποθήκευσης αρχείου: " + Environment.NewLine + folderPath;
            }
        }
    }

    public void SaveUser()
    {
        if (Directory.Exists(@folderPath))
        {
            return;
        }
        else
        {
            userID = userInput.text.ToString();
            Directory.CreateDirectory(folderPath);

            Debug.Log("Got userID: " + userID + Environment.NewLine);
            userInput.enabled = false;
            userInput.GetComponent<Image>().color = Color.grey;

            OKButton.GetComponent<Button>().interactable = true;
        }
    }

    public void SetShakeDeltaThreshold()
    {
        shakeDeltaThreshold = ShakeDeltaSlider.value * shakeDeltaThresholdQuantum;
        ShakeDeltaValue.text = shakeDeltaThreshold  + " pixels".ToString();
        measurePeriod = (int)(shakeDeltaThreshold * 4);
    }

    public void SetTestDuration()
    {
        testDuration = (int)TestDurationSlider.value * 300;
        TestDurationValue.text = ((float)testDuration / Application.targetFrameRate).ToString("0.00") + " seconds / " + testDuration + " frames";
    }

    //public void SetMeasurePeriod()
    //{
    //    measurePeriod = (int)MeasurePeriodSlider.value;
    //    MeasurePeriodValue.text = ((float)measurePeriod / Application.targetFrameRate).ToString("0.00") + " seconds / " + measurePeriod + " frames";
    //}
    
    //public void SetWaitTime()
    //{
    //    waitTime = (int)WaitTimeSlider.value * 60;
    //    WaitTimeValue.text = ((float)waitTime / Application.targetFrameRate).ToString("0.00") + " seconds / " + waitTime + " frames";
    //}

    void Calculate()
    {
        //Background.SetActive(false);

        StartCoroutine(Screenshot());
        
        AvgDeltaXpShake = (double) TotalAbsoluteShakeX / ShakeCounterX; // μέση μετατόπιση τρέμουλου Χ
        AvgDeltaYpShake = (double) TotalAbsoluteShakeY / ShakeCounterY;

        AvgDeltaXpSec = (double)TotalAbsoluteShakeX / (testDuration / Application.targetFrameRate); // μέση μετατόπιση ανά δευτερόλεπτο
        AvgDeltaYpSec = (double)TotalAbsoluteShakeY / (testDuration / Application.targetFrameRate);

        ShakesXPerSecond = (double) ShakeCounterX / (testDuration / Application.targetFrameRate); // μέσο πλήθος τρέμουλων ανά δευτερόλεπτο
        ShakesYPerSecond = (double) ShakeCounterY / (testDuration / Application.targetFrameRate);

        #region Parse results to UI

        ShakeCounterXValue.text = ShakeCounterX.ToString();
        ShakeCounterYValue.text = ShakeCounterY.ToString();
        if (minDeltaX != float.MaxValue)
            minDeltaXValue.text = minDeltaX.ToString("0") + " pixels";
        else minDeltaXValue.text = "_";

        if (minDeltaY != float.MaxValue)
            minDeltaYValue.text = minDeltaY.ToString("0") + " pixels";
        else minDeltaXValue.text = "_";
        maxDeltaXValue.text = maxDeltaX.ToString("0") + " pixels";
        maxDeltaYValue.text = maxDeltaY.ToString("0") + " pixels";

        TotalAbsoluteShakeXValue.text = TotalAbsoluteShakeX.ToString("0") + " pixels";
        TotalAbsoluteShakeYValue.text = TotalAbsoluteShakeY.ToString("0") + " pixels";

        AvgDeltaXpShakeValue.text = AvgDeltaXpShake.ToString("0.00") + " pixels";
        AvgDeltaYpShakeValue.text = AvgDeltaYpShake.ToString("0.00") + " pixels";
        AvgDeltaXpSecValue.text = AvgDeltaXpSec.ToString("0.00") + " pixels";
        AvgDeltaYpSecValue.text = AvgDeltaYpSec.ToString("0.00") + " pixels";
        ShakesXPerSecondValue.text = ShakesXPerSecond.ToString("0.00") + " pixels";
        ShakesYPerSecondValue.text = ShakesYPerSecond.ToString("0.00") + " pixels";

        offset1TimeValue.text = (offset1Time / Application.targetFrameRate).ToString("0.00") + " seconds";
        offset2TimeValue.text = (offset2Time / Application.targetFrameRate).ToString("0.00") + " seconds";
        offsetXTime1Value.text = (offsetXTime1 / Application.targetFrameRate).ToString("0.00") + " seconds";
        offsetYTime1Value.text = (offsetYTime1 / Application.targetFrameRate).ToString("0.00") + " seconds";
        offsetXTime2Value.text = (offsetXTime2 / Application.targetFrameRate).ToString("0.00") + " seconds";
        offsetYTime2Value.text = (offsetYTime2 / Application.targetFrameRate).ToString("0.00") + " seconds";

        #endregion

        Debug.Log("Πλήθος Τρέμουλων X: " + ShakeCounterXValue.text);
        Debug.Log("Πλήθος Τρέμουλων Υ: " + ShakeCounterYValue.text);

        Debug.Log("Ελάχιστη Μετατόπιση Χ: " + minDeltaXValue.text);
        Debug.Log("Ελάχιστη Μετατόπιση Y: " + minDeltaYValue.text);
        Debug.Log("Μέγιστη Μετατόπιση Χ: " + maxDeltaXValue.text);
        Debug.Log("Μέγιστη Μετατόπιση Y: " + maxDeltaYValue.text);

        Debug.Log("Αθροιστική Μετατόπιση Χ: " + TotalAbsoluteShakeXValue.text);
        Debug.Log("Αθροιστική Μετατόπιση Y: " + TotalAbsoluteShakeYValue.text);

        Debug.Log("Μέση Μετατόπιση Χ ανά τρέμουλο: " + AvgDeltaXpShakeValue.text);
        Debug.Log("Μέση Μετατόπιση Y ανά τρέμουλο: " + AvgDeltaYpSecValue.text);
        Debug.Log("Μέση Μετατόπιση Χ ανά δευτερόλεπτο: " + AvgDeltaXpSecValue.text);
        Debug.Log("Μέση Μετατόπιση Y ανά δευτερόλεπτο: " + AvgDeltaYpSecValue.text);

        Debug.Log("Μέσο Πλήθος Τρέμουλων Χ ανά δευτερόλεπτο: " + ShakesXPerSecondValue.text);
        Debug.Log("Μέσο Πλήθος Τρέμουλων Χ ανά δευτερόλεπτο: " + ShakesYPerSecondValue.text);

        Debug.Log("Χρόνος εκτός κέντρου 1: " + offset1TimeValue.text);
        Debug.Log("Χρόνος εκτός κέντρου 2: " + offset2TimeValue.text);
        Debug.Log("Χρόνος εντός ζώνης 1 Χ: " + offsetXTime1Value.text);
        Debug.Log("Χρόνος εντός ζώνης 1 Υ: " + offsetYTime1Value.text);
        Debug.Log("Χρόνος εντός ζώνης 2 X: " + offsetXTime2Value.text);
        Debug.Log("Χρόνος εντός ζώνης 2 Υ: " + offsetYTime2Value.text);

        GetComponent<LogHandler>().enabled = false;

        ExitButton.SetActive(true);
    }

    IEnumerator Screenshot()
    {
        // Disable unrequired cameras
        //mainCamera.SetActive(false);
        Background.SetActive(false);

        // Enable the camera to take a screenshot with 
        //photographer.SetActive(true);

        yield return new WaitForEndOfFrame();

        //int width = Screen.width;
        //int height = Screen.height;

        //Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
        //yield return new WaitForEndOfFrame();

        //tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        //tex.Apply();
        //byte[] bytes = tex.EncodeToPNG();

        //save texture in a file
        //string captureName = string.Format("/Trace_{0}.png", userID);
        //string filePath = Path.Combine(folderPath, captureName); //set capture location
        //File.WriteAllBytes(filePath + ".png", bytes); //write bytes to file

        //mainCamera.SetActive(true);
        //photographer.SetActive(false);

        Debug.Log("Screen captured at: " + folderPath);
        string imageName = string.Format("/Trace_{0}.png", userID);
        ScreenCapture.CaptureScreenshot(folderPath + imageName);

        yield return (0);

        EndPanel.SetActive(true);
        Background.SetActive(true);
    }

    public void ShowResults()
    {
        if (!ResultsPanel.activeSelf)
            ResultsPanel.SetActive(true);
        else ResultsPanel.SetActive(false);
    }

    public void ShowExplorer()
    {
        GetComponent<LogHandler>().OnDestroy();
        folderPath = folderPath.Replace(@"/", @"\");
        System.Diagnostics.Process.Start("explorer.exe", "/select," + folderPath);
    }
    #endregion
}