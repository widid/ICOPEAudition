using ICOPE.Immersive.Progress;
using ICOPE.Immersive.Rewards;
using ICOPE.Immersive.Tracking;

namespace ICOPE.Immersive.Bootstrap
{
    public class ImmersiveServices
    {
        public IClinicalCaseTracker Tracker { get; }
        public IPatientProgressProvider Progress { get; }
        public ICoinWallet Wallet { get; }

        public ImmersiveServices(IClinicalCaseTracker tracker, IPatientProgressProvider progress, ICoinWallet wallet)
        {
            Tracker = tracker;
            Progress = progress;
            Wallet = wallet;
        }
    }
}
