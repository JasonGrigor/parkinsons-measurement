using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using UnityEngine.UI;

public class LogHandler : MonoBehaviour {

    //public GameObject setupCanvas;
    public GameObject gameManager;

    public string userID;
    public string parentFolder;
    public string logPath;
    public float logTime;

    public float startTime;

    private StreamWriter _writer;

    //public string logFileName = "game_log.txt";

    public string output = "";
    public string stack = "";

    public void OnEnable()
    {
        //startTime = Time.time;
        //userID = setupCanvas.GetComponent<SetupGame>().userID;
        userID = gameManager.GetComponent<GameManager>().userID;
        parentFolder = gameManager.GetComponent<GameManager>().folderPath;

        if (userID != null && parentFolder != null)
        {
            string logname = string.Format(@"Test_Log_{0}.txt", userID);
            logPath = Path.Combine(parentFolder, logname);
            //Debug.Log("LogHandler: " + userID + folderPath);

            _writer = File.AppendText(@logPath);
            _writer.Write("=============== Test initialized ================" + Environment.NewLine + Environment.NewLine);
            //DontDestroyOnLoad(gameObject);

            Application.logMessageReceived += HandleLog;
        }
    }

    public void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        output = logString;
        stack = stackTrace;
        logTime = Time.time - startTime;
        _writer.Write(logTime.ToString("0.000") + " - " + logString + Environment.NewLine);
    }

    public void OnDestroy()
    {
        if (_writer != null)
        {
            _writer.Close();
        }
        else Debug.Log("Null Log Handler exception.");
    }
}
