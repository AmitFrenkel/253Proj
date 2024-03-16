using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class SimulatorDatabase
{
    public List<SystemVersion> systemVersions;
    public List<Threat> threats;
    public List<MapCircle> mapCircles;
    public List<UserResponse> userResponses;
    public List<EducationalScreen> educationalScreens;
    public List<Scenario> scenarios;
    public List<Train> trains;

    public List<FloatHyperParameter> floatHyperParameters;

    // ========================= Fucntions =========================

    public SimulatorElement getSimulatorElementByIndex(List<SimulatorElement> simulatorElements, int index)
    {
        foreach (SimulatorElement loopSimulatorElement in simulatorElements)
            if (loopSimulatorElement.getIndex() == index)
                return loopSimulatorElement;
        return null;
    }

    private int getNextFreeIndex(List<SimulatorElement> list)
    {
        if (list.Count == 0)
            return 0;
        int maxIndex = list[0].getIndex();
        foreach (SimulatorElement loopElement in list)
            if (loopElement.getIndex() > maxIndex)
                maxIndex = loopElement.getIndex();
        return maxIndex+1;
    }

    public List<int> getListOfExistIndices(List<SimulatorElement> simulatorElements)
    {
        List<int> existIndices = new List<int>();
        foreach (SimulatorElement loopSimulatorElement in simulatorElements)
            existIndices.Add(loopSimulatorElement.getIndex());
        return existIndices;
    }


    // ========================= Get elements by index =========================

    public SystemVersion getSystemVersionByIndex(int index)
    {
        return getSimulatorElementByIndex(systemVersions.ConvertAll(x => (SimulatorElement)x), index) as SystemVersion;
    }

    public Threat getThreatByIndex(int index)
    {
        return getSimulatorElementByIndex(threats.ConvertAll(x => (SimulatorElement)x), index) as Threat;
    }

    public MapCircle getMapCircleByIndex(int index)
    {
        return getSimulatorElementByIndex(mapCircles.ConvertAll(x => (SimulatorElement)x), index) as MapCircle;
    }

    public UserResponse getUserResponseByIndex(int index)
    {
        return getSimulatorElementByIndex(userResponses.ConvertAll(x => (SimulatorElement)x), index) as UserResponse;
    }

    public EducationalScreen getEducationalScreenByIndex(int index)
    {
        return getSimulatorElementByIndex(educationalScreens.ConvertAll(x => (SimulatorElement)x), index) as EducationalScreen;
    }

    public Scenario getScenarioByIndex(int index)
    {
        return getSimulatorElementByIndex(scenarios.ConvertAll(x => (SimulatorElement)x), index) as Scenario;
    }

    public Train getTrainByIndex(int index)
    {
        return getSimulatorElementByIndex(trains.ConvertAll(x => (SimulatorElement)x), index) as Train;
    }

    // ========================= Get next free index =========================

    public int getNextSystemVersionIndex()
    {
        return getNextFreeIndex(systemVersions.ConvertAll(x => (SimulatorElement)x));
    }

    public int getNextThreatIndex()
    {
        return getNextFreeIndex(threats.ConvertAll(x => (SimulatorElement)x));
    }

    public int getNextMapCircleIndex()
    {
        return getNextFreeIndex(mapCircles.ConvertAll(x => (SimulatorElement)x));
    }

    public int getNextUserResponseIndex()
    {
        return getNextFreeIndex(userResponses.ConvertAll(x => (SimulatorElement)x));
    }

    public int getNextEducationalScreenIndex()
    {
        return getNextFreeIndex(educationalScreens.ConvertAll(x => (SimulatorElement)x));
    }

    public int getNextScenarioIndex()
    {
        return getNextFreeIndex(scenarios.ConvertAll(x => (SimulatorElement)x));
    }

    public int getNextTrainIndex()
    {
        return getNextFreeIndex(trains.ConvertAll(x => (SimulatorElement)x));
    }

}

[System.Serializable]
public abstract class SimulatorClass
{

}

[System.Serializable]
public abstract class SimulatorElement : SimulatorClass
{
    public abstract int getIndex();
    public abstract string getName();
}

[System.Serializable]
public class RGBColor : SimulatorClass
{
    public float R;
    public float G;
    public float B;

    public RGBColor()
    {
        R = 0f;
        G = 0f;
        B = 0f;
    }

    public RGBColor(float r, float g, float b)
    {
        R = r;
        G = g;
        B = b;
    }
}

[System.Serializable]
public class SystemVersion : SimulatorElement
{
    public int versionIndex;
    public string versionName;
    public int[] threatIndexesInSystemVersion;

    public SystemVersion(int versionIndex)
    {
        this.versionIndex = versionIndex;
        this.versionName = "";
        this.threatIndexesInSystemVersion = new int[] {};
    }

    public SystemVersion(int versionIndex, string versionName, int[] threatIndexesInSystemVersion)
    {
        this.versionIndex = versionIndex;
        this.versionName = versionName;
        this.threatIndexesInSystemVersion = threatIndexesInSystemVersion;
    }

    public override int getIndex()
    {
        return versionIndex;
    }
    public override string getName()
    {
        return versionName;
    }

}

[System.Serializable]
public class Threat : SimulatorElement
{
    [System.Serializable]
    public class ThreatLock : SimulatorClass
    {
        public string threatLockName;
        public string[] threatLockPathsToSymbols;
        public string threatLockPathToSound;
        public RGBColor threatLockColor;

        public ThreatLock(string threatLockName, string[] threatLockPathsToSymbols, string threatLockPathToSound, RGBColor threatLockColor)
        {
            this.threatLockName = threatLockName;
            this.threatLockPathsToSymbols = threatLockPathsToSymbols;
            this.threatLockPathToSound = threatLockPathToSound;
            this.threatLockColor = threatLockColor;
        }

        public ThreatLock()
        {
            this.threatLockName = "";
            this.threatLockPathsToSymbols = new string[] { };
            this.threatLockPathToSound = "";
            this.threatLockColor = new RGBColor();
        }
    }

    public int threatIndex;
    public string threatName;
    public float threatRadius;
    public ThreatLock[] threatLocks;
    public float timeBetweenSymbolChanging;
    public float defaultUncertaintyRange;
    public float defaultUncertaintyTime;

    public Threat(int threatIndex)
    {
        this.threatIndex = threatIndex;
        this.threatName = "";
        this.timeBetweenSymbolChanging = 0.5f;
        this.threatLocks = new ThreatLock[] { };
        this.defaultUncertaintyRange = 0f;
        this.defaultUncertaintyTime = 0f;
    }

    public Threat(int threatIndex, string threatName, float timeBetweenSymbolChanging, ThreatLock[] threatLocks, float defaultUncertaintyRange, float defaultUncertaintyTime)
    {
        this.threatIndex = threatIndex;
        this.threatName = threatName;
        this.timeBetweenSymbolChanging = timeBetweenSymbolChanging;
        this.threatLocks = threatLocks;
        this.defaultUncertaintyRange = defaultUncertaintyRange;
        this.defaultUncertaintyTime = defaultUncertaintyTime;
    }

    public override int getIndex()
    {
        return threatIndex;
    }
    public override string getName()
    {
        return threatName;
    }
}

[System.Serializable]
public class MapCircle : SimulatorElement
{
    public int circleIndex;
    public string circleName;
    public float circleRadius;
    
    public string circlePathToSymbol;
    public RGBColor circleColor;

    public MapCircle(int circleIndex)
    {
        this.circleIndex = circleIndex;
        this.circleName = "";
        this.circleRadius = 0f;
        this.circlePathToSymbol = "";
    }

    public MapCircle(int circleIndex, string circleName, float circleRadius, string circlePathToSymbol, RGBColor circleColor) : this(circleIndex)
    {
        this.circleName = circleName;
        this.circleRadius = circleRadius;
        this.circlePathToSymbol = circlePathToSymbol;
        this.circleColor = circleColor;
    }

    public override int getIndex()
    {
        return circleIndex;
    }
    public override string getName()
    {
        return circleName;
    }
}

[System.Serializable]
public class UserResponse : SimulatorElement
{
    public int responseIndex;
    public string responseName;
    public bool isResponseCausesToEndOfScenario;

    public override int getIndex()
    {
        return responseIndex;
    }
    public override string getName()
    {
        return responseName;
    }

    public UserResponse(int index)
    {
        responseIndex = index;
        responseName = "";
        isResponseCausesToEndOfScenario = false;
    }

    public UserResponse(int responseIndex, string responseName, bool isResponseCausesToEndOfScenario)
    {
        this.responseIndex = responseIndex;
        this.responseName = responseName;
        this.isResponseCausesToEndOfScenario = isResponseCausesToEndOfScenario;
    }
}

[System.Serializable]
public class EducationalScreen : SimulatorElement
{
    public int educationalScreenIndex;
    public string educationalScreenName;
    public bool isEducationalScreenBasedOnImage;
    public string educationalScreenPathToImage;
    public string educationalScreenText;

    public override int getIndex()
    {
        return educationalScreenIndex;
    }
    public override string getName()
    {
        return educationalScreenName;
    }

    public EducationalScreen(int index)
    {
        educationalScreenIndex = index;
        educationalScreenName = "";
        isEducationalScreenBasedOnImage = false;
        educationalScreenPathToImage = "";
        educationalScreenText = "";

    }

    public EducationalScreen(int educationalScreenIndex, string educationalScreenName, bool isEducationalScreenBasedOnImage, string educationalScreenPathToImage, string educationalScreenText) : this(educationalScreenIndex)
    {
        this.educationalScreenName = educationalScreenName;
        this.isEducationalScreenBasedOnImage = isEducationalScreenBasedOnImage;
        this.educationalScreenPathToImage = educationalScreenPathToImage;
        this.educationalScreenText = educationalScreenText;
    }
}

[System.Serializable]
public partial class Scenario : SimulatorElement
{
    [System.Serializable]
    public class SteerPoint : SimulatorClass
    {
        public string N;
        public string E;

        public SteerPoint()
        {
            this.N = "";
            this.E = "";
        }

        public SteerPoint(string n, string e)
        {
            N = n;
            E = e;
        }
    }

    [System.Serializable]
    public partial class ActiveThreat : SimulatorClass
    {

        [System.Serializable]
        public class ActiveThreatEvent : SimulatorClass
        {
            public int threatLockListIndex;
            public float threatEventDistance;
            public bool isPlayingThreatLockSound;

            public ActiveThreatEvent()
            {
                threatLockListIndex = 0;
                threatEventDistance = 0f;
                isPlayingThreatLockSound = false;
            }

            public ActiveThreatEvent(int threatLockListIndex, float threatEventDistance, bool isPlayingThreatLockSound)
            {
                this.threatLockListIndex = threatLockListIndex;
                this.threatEventDistance = threatEventDistance;
                this.isPlayingThreatLockSound = isPlayingThreatLockSound;
            }
        }


        public int activeThreatLinkIndex;
        public SteerPoint threatPosition;
        public ActiveThreatEvent[] activeThreatEvents;
        public float threatDifficultyFactor;
        public float minResponseTime;
        public float maxReasonableResponeTime;
        public UserResponeToThreat[] userResponesToThreats;

        public ActiveThreat()
        {
            activeThreatLinkIndex = 0;
            threatPosition = new SteerPoint();
            activeThreatEvents = new ActiveThreatEvent[] { };
            threatDifficultyFactor = 0f;
            minResponseTime = 0f;
            maxReasonableResponeTime = 0f;
            userResponesToThreats = new UserResponeToThreat[] { };
        }

        public ActiveThreat(int activeThreatLinkIndex, SteerPoint threatPosition, ActiveThreatEvent[] activeThreatEvents, float threatDifficultyFactor, float minResponseTime, float maxReasonableResponeTime, UserResponeToThreat[] userResponesToThreats)
        {
            this.activeThreatLinkIndex = activeThreatLinkIndex;
            this.threatPosition = threatPosition;
            this.activeThreatEvents = activeThreatEvents;
            this.threatDifficultyFactor = threatDifficultyFactor;
            this.minResponseTime = minResponseTime;
            this.maxReasonableResponeTime = maxReasonableResponeTime;
            this.userResponesToThreats = userResponesToThreats;
        }
    }

    [System.Serializable]
    public class ActiveMapCircle : SimulatorClass
    {
        public int mapCircleIndexLinkIndex;
        public SteerPoint mapCenterPosition;


        public ActiveMapCircle()
        {
            mapCircleIndexLinkIndex = 0;
            mapCenterPosition = new SteerPoint();
        }

        public ActiveMapCircle(int mapCircleIndexLinkIndex, SteerPoint mapCenterPosition)
        {
            this.mapCircleIndexLinkIndex = mapCircleIndexLinkIndex;
            this.mapCenterPosition = mapCenterPosition;
        }
    }

    public int scenarioIndex;
    public string scenarioName;
    public SteerPoint[] airplaneSteerPoints;
    public int airPlaneSteerPointOfRelease;
    public float airplaneHeight;
    public float airplaneVelocity;
    public ActiveThreat[] activeThreats;
    public ActiveMapCircle[] activeMapCircles;
    public int[] includedUserResponsesIndexes;
    public bool enableEndOfScenarioEducationalScreen;
    public bool hasDefaultEndOfScenarioEducationalScreen;
    public int endOfScenarioEducationalScreenLinkIndex;

    public override int getIndex()
    {
        return scenarioIndex;
    }
    public override string getName()
    {
        return scenarioName;
    }

    public Scenario(int index)
    {
        scenarioIndex = index;
        scenarioName = "";
        airplaneSteerPoints = new SteerPoint[] { };
        airPlaneSteerPointOfRelease = 1;
        airplaneHeight = 0f;
        airplaneVelocity = 0f;
        activeThreats = new ActiveThreat[] { };
        activeMapCircles = new ActiveMapCircle[] { };
        includedUserResponsesIndexes = new int[] { };
        enableEndOfScenarioEducationalScreen = false;
        hasDefaultEndOfScenarioEducationalScreen = false;
        endOfScenarioEducationalScreenLinkIndex = 0;
    }

    public Scenario(int scenarioIndex, string scenarioName, SteerPoint[] airplaneSteerPoints, float airplaneHeight, float airplaneVelocity, ActiveThreat[] activeThreats, ActiveMapCircle[] activeMapCircles, int[] includedUserResponsesIndexes, bool enableEndOfScenarioEducationalScreen, bool hasDefaultEndOfScenarioEducationalScreen, int endOfScenarioEducationalScreenLinkIndex) : this(scenarioIndex)
    {
        this.scenarioName = scenarioName;
        this.airplaneSteerPoints = airplaneSteerPoints;
        this.airplaneHeight = airplaneHeight;
        this.airplaneVelocity = airplaneVelocity;
        this.activeThreats = activeThreats;
        this.activeMapCircles = activeMapCircles;
        this.includedUserResponsesIndexes = includedUserResponsesIndexes;
        this.enableEndOfScenarioEducationalScreen = enableEndOfScenarioEducationalScreen;
        this.hasDefaultEndOfScenarioEducationalScreen = hasDefaultEndOfScenarioEducationalScreen;
        this.endOfScenarioEducationalScreenLinkIndex = endOfScenarioEducationalScreenLinkIndex;
    }
}

[System.Serializable]
public class Train : SimulatorElement
{
    public int trainIndex;
    public string trainName;
    public int[] trainScenarioIndex;

    public override int getIndex()
    {
        return trainIndex;
    }
    public override string getName()
    {
        return trainName;
    }

    public Train(int index)
    {
        trainIndex = index;
        trainName = "";
        trainScenarioIndex = new int[] { };
    }
}

[System.Serializable]
public class HyperParameter : SimulatorClass
{
    public string parameterName;
}

[System.Serializable]
public class FloatHyperParameter : HyperParameter
{
    
    public float parameterValue;
}

// ==================== Editor default values =====================

[System.Serializable]
public class EditorDatabase
{
    public List<FloatEditorDefualtValue> floatEditorDefaultValue;
    public List<StringEditorDefualtValue> stringEditorDefaultValue;
    public List<BoolEditorDefualtValue> boolEditorDefaultValue;

    public bool isDefaultValueExist(string searchValueName)
    {
        foreach (FloatEditorDefualtValue loopDefaultValue in floatEditorDefaultValue)
            if (loopDefaultValue.fieldName == searchValueName)
                return true;

        foreach (StringEditorDefualtValue loopDefaultValue in stringEditorDefaultValue)
            if (loopDefaultValue.fieldName == searchValueName)
                return true;

        foreach (BoolEditorDefualtValue loopDefaultValue in boolEditorDefaultValue)
            if (loopDefaultValue.fieldName == searchValueName)
                return true;

        return false;
    }

    public float getFloatDefaultValue(string searchValueName)
    {
        foreach (FloatEditorDefualtValue loopDefaultValue in floatEditorDefaultValue)
            if (loopDefaultValue.fieldName == searchValueName)
                return loopDefaultValue.fieldValue;
        return -1f;
    }

    public string getStringDefaultValue(string searchValueName)
    {
        foreach (StringEditorDefualtValue loopDefaultValue in stringEditorDefaultValue)
            if (loopDefaultValue.fieldName == searchValueName)
                return loopDefaultValue.fieldValue;
        return "";
    }

    public bool getBoolDefaultValue(string searchValueName)
    {
        foreach (BoolEditorDefualtValue loopDefaultValue in boolEditorDefaultValue)
            if (loopDefaultValue.fieldName == searchValueName)
                return loopDefaultValue.fieldValue;
        return false;
    }
}

[System.Serializable]
public class EditorDefualtValue
{
    public string fieldName;
}

[System.Serializable]
public class FloatEditorDefualtValue : EditorDefualtValue
{
    public float fieldValue;
}

[System.Serializable]
public class StringEditorDefualtValue : EditorDefualtValue
{
    public string fieldValue;
}

[System.Serializable]
public class BoolEditorDefualtValue : EditorDefualtValue
{
    public bool fieldValue;
}


