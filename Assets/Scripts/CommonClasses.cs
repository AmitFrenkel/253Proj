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
public class SystemVersion : SimulatorElement
{
    public int versionIndex;
    public string versionName;
    public int[] threatIndexesInSystemVersion;

    public SystemVersion(int versionIndex)
    {
        this.versionIndex = versionIndex;
        this.versionName = "";
        this.threatIndexesInSystemVersion = new int[] { 0 };
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

        public ThreatLock(string threatLockName, string[] threatLockPathsToSymbols, string threatLockPathToSound)
        {
            this.threatLockName = threatLockName;
            this.threatLockPathsToSymbols = threatLockPathsToSymbols;
            this.threatLockPathToSound = threatLockPathToSound;
        }

        public ThreatLock() { }
    }

    public int threatIndex;
    public string threatName;
    public float timeBetweenSymbolChanging;
    public ThreatLock[] threatLocks;
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

    public MapCircle(int circleIndex)
    {
        this.circleIndex = circleIndex;
        this.circleName = "";
        this.circleRadius = 0f;
        this.circlePathToSymbol = "";
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
}

[System.Serializable]
public class Scenario : SimulatorElement
{
    [System.Serializable]
    public class SteerPoint : SimulatorClass
    {
        public int steerPointIndex;
        public string N;
        public string E;
    }

    [System.Serializable]
    public class ActiveThreat : SimulatorClass
    {
        [System.Serializable]
        public class UserResponeToThreat : SimulatorClass
        {
            public int userResponeIndex;
            public float responeToThreatScore;
            public bool responseToThreatExplanation;
            public bool isOverridingEndOfScenarioEducationalScreen;
            public int overrideEducationalScreenIndex;
        }

        [System.Serializable]
        public class ActiveThreatEvent : SimulatorClass
        {
            public string threatLock;
            public float threatEventTime;
            public bool isPlayingThreatLockSound;
        }

        public int activeThreatIndex;
        public SteerPoint threatPosition;
        public float threatApperanceTime;
        public List<ActiveThreatEvent> activeThreatEvents;
        public float threatDifficultyFactor;
        public float minResponseTime;
        public float maxReasonableResponeTime;
        public List<UserResponeToThreat> userResponesToThreats;
    }

    public int scenarioIndex;
    public string scenarioName;
    public List<SteerPoint> airplaneSteerPoints;
    public float airplaneHeight;
    public float airplaneVelocity;
    public List<int> userResponesInScenario;
    public bool hasEndOfScenarioEducationalScreen;
    public int endOfScenarioEducationalScreenIndex;

    public override int getIndex()
    {
        return scenarioIndex;
    }
    public override string getName()
    {
        return scenarioName;
    }

}

[System.Serializable]
public class Train : SimulatorElement
{
    public int trainIndex;
    public string trainName;
    public List<int> trainScenarioIndex;

    public override int getIndex()
    {
        return trainIndex;
    }
    public override string getName()
    {
        return trainName;
    }
}
