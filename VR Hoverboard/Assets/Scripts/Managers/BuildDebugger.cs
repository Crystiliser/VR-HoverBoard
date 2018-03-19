public class BuildDebugger : UnityEngine.MonoBehaviour
{
    public static void InitDebugger()
    {
#if DEBUGGER
        if (!stcBoolDebuggerInited)
        {
            System.Globalization.CultureInfo.CurrentCulture.ClearCachedData();
            if (null == stcStrlistLines)
                stcStrlistLines = new System.Collections.Generic.List<string>();
            string lStrTimeStamp = Xander.Debugging.Helper.DozenalTimeStamp;
            string logpath = LogFilePath(lStrTimeStamp);
            string logError = "Unknown Error";
            try { stcSwWriter = new System.IO.StreamWriter(logpath, true); } catch (System.Exception e) { stcSwWriter = null; logError = e.Message; }
            if (null != stcSwWriter)
            {
                stcBoolFileOpen = true;
                stcSwWriter.Write("<BEGIN>\n\n");
            }
            else
                UnityEngine.Debug.LogWarning($"Failed to open log file: ({logpath}) [{logError}]");
            stcBoolDebuggerInited = true;
            WriteLine("Log Startup: " + lStrTimeStamp);
            WriteToErrorLog("INIT LOG", logpath + "\n", UnityEngine.LogType.Log, lStrTimeStamp);
            UnityEngine.Application.logMessageReceived += GetLog;
        }
#endif
    }
#if DEBUGGER
    public static bool WASD => stcBoolWasd;
    private const int cstIntMaxNumLines = 20;
    private const int cstIntMaxLineLength = 45;
    private static System.Collections.Generic.List<string> stcStrlistLines = null;
    private static System.IO.StreamWriter stcSwWriter;
    private static UnityEngine.GameObject stcGobjTextObject = null;
    private static TMPro.TextMeshProUGUI stcCompTextmesh = null;
    private static UnityEngine.UI.Image stcImageWarningIcon = null;
    private static ulong stcUlongLineCounter = 0ul, stcUlongFrameCounter = 0ul;
    private static bool stcBoolDebuggerInited = false;
    private static bool stcBoolFileOpen = false;
    private static float stcFloatWarningTimer = 0.0f;
    private static bool stcBoolWasd = false;
    private static string LogFilePath(string pStrTimeStamp) => UnityEngine.Application.persistentDataPath + "/log" + pStrTimeStamp.Substring(0, 7) + ".txt";
    private static void GetLog(string pStrLogMessage, string pStrStackTrace, UnityEngine.LogType pEnmLogType)
    {
        string lStrTimeStamp = Xander.Debugging.Helper.DozenalTimeStamp;
        WriteLine(pEnmLogType.ToString() + " << " + pStrLogMessage);
        if (UnityEngine.LogType.Log != pEnmLogType)
        {
            if (null != stcImageWarningIcon)
            {
                stcFloatWarningTimer = 1.0f;
                switch (pEnmLogType)
                {
                    case UnityEngine.LogType.Error:
                        stcImageWarningIcon.color = new UnityEngine.Color(0.8f, 0.2f, 0.2f, 1.0f);
                        break;
                    case UnityEngine.LogType.Assert:
                        stcImageWarningIcon.color = new UnityEngine.Color(0.9f, 0.9f, 0.1f, 1.0f);
                        break;
                    case UnityEngine.LogType.Warning:
                        stcImageWarningIcon.color = new UnityEngine.Color(0.8f, 0.8f, 0.2f, 1.0f);
                        break;
                    case UnityEngine.LogType.Exception:
                        stcImageWarningIcon.color = new UnityEngine.Color(0.9f, 0.1f, 0.1f, 1.0f);
                        break;
                }
            }
            WriteToErrorLog(pStrLogMessage, pStrStackTrace, pEnmLogType, lStrTimeStamp);
        }
    }
    private static void WriteToErrorLog(string pStrLogMessage, string pStrStackTrace, UnityEngine.LogType pEnmLogType, string pStrTimeStamp)
    {
        if (stcBoolFileOpen)
        {
            stcSwWriter.Write("[!!" + pEnmLogType.ToString().ToUpper() +
                $"!!]\nTime: {pStrTimeStamp} @" +
                Xander.Dozenal.DozenalStrings.Dozenal(stcUlongFrameCounter) +
                $"\nLog Message: \"{pStrLogMessage}\"\n");
            if (null == pStrStackTrace || "" == pStrStackTrace)
                stcSwWriter.Write("NO STACK TRACE\n\n");
            else
                stcSwWriter.Write("Stack Trace:\n" + pStrStackTrace + "\n");
        }
    }
    private static bool WriteLine(string pStrLine)
    {
        if (null != pStrLine && pStrLine.Length > 0)
        {
            string[] lStrarrSplitLines = pStrLine.Split('\n');
            if (lStrarrSplitLines.Length > 1)
            {
                foreach (string lStrSplitLine in lStrarrSplitLines)
                    if (WriteLine(lStrSplitLine))
                        --stcUlongLineCounter;
                ++stcUlongLineCounter;
                return true;
            }
        }
        else
            return false;
        if (pStrLine.Length > cstIntMaxLineLength)
        {
            WriteLine(pStrLine.Substring(0, cstIntMaxLineLength));
            --stcUlongLineCounter;
            return WriteLine(pStrLine.Substring(cstIntMaxLineLength));
        }
        if (null == stcStrlistLines)
            stcStrlistLines = new System.Collections.Generic.List<string>();
        pStrLine = Xander.Dozenal.DozenalStrings.Dozenal(stcUlongLineCounter) + ": " + pStrLine;
        ++stcUlongLineCounter;
        stcStrlistLines.Add(pStrLine);
        if (stcStrlistLines.Count > cstIntMaxNumLines)
            stcStrlistLines.RemoveAt(0);
        return true;
    }
    private void Awake()
    {
        InitDebugger();
    }
    private void Start()
    {
        stcCompTextmesh = GetComponentInChildren<TMPro.TextMeshProUGUI>();
        stcImageWarningIcon = GetComponentInChildren<UnityEngine.UI.Image>();
        if (null != stcCompTextmesh)
        {
            stcGobjTextObject = stcCompTextmesh.gameObject;
            stcGobjTextObject.SetActive(false);
            Xander.NullConversion.NullConverter.ConvertNull(stcImageWarningIcon)?.gameObject.SetActive(false);
        }
        else enabled = false;
    }
    private void OnApplicationQuit()
    {
        if (stcBoolDebuggerInited)
        {
            UnityEngine.Application.logMessageReceived -= GetLog;
            if (stcBoolFileOpen)
            {
                stcSwWriter.Write("<END>\n\n\n");
                stcSwWriter.Close();
            }
        }
    }
    private void Update()
    {
        if (null != stcImageWarningIcon)
        {
            UnityEngine.Color lColorTmp = stcImageWarningIcon.color;
            lColorTmp.a = UnityEngine.Mathf.Clamp01(stcFloatWarningTimer);
            stcImageWarningIcon.color = lColorTmp;
            stcImageWarningIcon.gameObject.SetActive(stcFloatWarningTimer > 0.0f);
            stcFloatWarningTimer -= UnityEngine.Time.deltaTime;
        }
        ++stcUlongFrameCounter;
        string lStrTemp = "";
        foreach (string lStrLine in stcStrlistLines)
            lStrTemp += lStrLine + "\n";
        stcCompTextmesh.SetText(lStrTemp);
        if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.BackQuote) ||
            (UnityEngine.Input.GetKey(KeyInputManager.XBOX_X) &&
            UnityEngine.Input.GetKey(KeyInputManager.XBOX_A) &&
            UnityEngine.Input.GetKeyDown(KeyInputManager.XBOX_LB)))
            stcGobjTextObject.SetActive(!stcGobjTextObject.activeSelf);
        if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.F10))
            UnityEngine.Debug.Log("F10: WASD controls are turned " + ((stcBoolWasd = !stcBoolWasd) ? "on." : "off."));
        if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.F) ||
            (UnityEngine.Input.GetKey(UnityEngine.KeyCode.F) &&
            (UnityEngine.Input.GetKey(UnityEngine.KeyCode.LeftShift) ||
            UnityEngine.Input.GetKey(UnityEngine.KeyCode.RightShift))))
            UnityEngine.Debug.Log("FPS: " + (1.0f / UnityEngine.Time.deltaTime));
    }
#else
    private void Awake() => Destroy(gameObject);
#endif
}