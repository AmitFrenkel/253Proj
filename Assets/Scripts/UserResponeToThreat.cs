public partial class Scenario
{
    public partial class ActiveThreat
    {
        [System.Serializable]
        public class UserResponeToThreat : SimulatorClass
        {
            public int userResponeListIndex;
            public float responeToThreatScore;
            public string responseToThreatExplanation;
            public bool isOverridingEndOfScenarioEducationalScreen;
            public int overrideEducationalScreenLinkIndex;

            public UserResponeToThreat()
            {
                userResponeListIndex = 0;
                responeToThreatScore = 0f;
                responseToThreatExplanation = "";
                isOverridingEndOfScenarioEducationalScreen = false;
                overrideEducationalScreenLinkIndex = 0;
            }

            public UserResponeToThreat(int userResponeListIndex, float responeToThreatScore, string responseToThreatExplanation, bool isOverridingEndOfScenarioEducationalScreen, int overrideEducationalScreenLinkIndex)
            {
                this.userResponeListIndex = userResponeListIndex;
                this.responeToThreatScore = responeToThreatScore;
                this.responseToThreatExplanation = responseToThreatExplanation;
                this.isOverridingEndOfScenarioEducationalScreen = isOverridingEndOfScenarioEducationalScreen;
                this.overrideEducationalScreenLinkIndex = overrideEducationalScreenLinkIndex;
            }
        }
    }
}


