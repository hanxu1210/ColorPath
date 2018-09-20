/// <summary>
/// Ads manager class. This is lite version supporting UnityAds.
/// </summary>
using UnityEngine;
using System.Collections;

#if UNITY_ADS
using UnityEngine.Advertisements;

// only compile Ads code on supported platforms
#endif

namespace SgLib
{
    public enum AdType
    {
        Interstitial,
        Rewarded
    }

    public enum AdEvent
    {
        ShouldReward,
        Displayed,
        Skipped,
        Failed
    }

    public class AdEventArgs
    {
        public AdEvent adEvent;
    }

    public class AdsManager : MonoBehaviour
    {
        public static AdsManager Instance;

        public static event System.Action<AdEventArgs> NewInterstitialAdEvent = delegate {};
        public static event System.Action<AdEventArgs> NewRewardedAdEvent = delegate {};

        [Header("Check if you want to show ads manually")]
        public bool disableAutoShowAds = false;

        [Header("Show ad every [how many] games")]
        public int gamesPerAd = 3;
        // number of games to be played before ad is shown

        [Header("How many seconds after game over that ad is shown")]
        public float showAdDelay = 3f;
        // number of seconds between game over event and ad showing

        static int gameCount = 0;

        void Awake()
        {
            if (Instance)
            {
                DestroyImmediate(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        void OnEnable()
        {
            PlayerController.NewGameEvent += OnNewGameEvent;
        }

        void OnDisable()
        {
            PlayerController.NewGameEvent -= OnNewGameEvent;
        }

        void OnNewGameEvent(GameEvent newEvent)
        {
            if (!disableAutoShowAds)
            {
                if (newEvent == GameEvent.GameOver)
                {
                    gameCount++;

                    if (gameCount >= gamesPerAd)
                    {
                        // Show default ad after some delay
                        Invoke("ShowAd", showAdDelay);

                        // Reset game count
                        gameCount = 0;
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether ad is ready to be shown.
        /// </summary>
        /// <returns><c>true</c> if ad is ready; otherwise, <c>false</c>.</returns>
        /// <param name="zoneId">Optional zone identifier, if not specified default zone is used.</param>
        public bool IsAdReady(string zoneId = null)
        {
            #if UNITY_ADS

            if (zoneId == null)
            {
                return Advertisement.IsReady();
            }
            else
            {
                return Advertisement.IsReady(zoneId);
            }

            #else

            return false;

            #endif
        }

        /// <summary>
        /// Shows the default ad. No ad events associated with this ad will be fired.
        /// </summary>
        public void ShowAd()
        {
            #if UNITY_ADS

            // Show default ad
            if (!Advertisement.IsReady())
            {
                Debug.Log("Ads not ready for default zone");
                return;
            }

            Advertisement.Show();

            #endif
        }

        /// <summary>
        /// Shows the ad at the specified zone. A NewAdEvent event will be fired whenever this ad is finished, skipped or failed.
        /// </summary>
        /// <param name="zoneId">Zone identifier.</param>
        public void ShowAd(string zoneId)
        {
            #if UNITY_ADS

            // Show ad at the specified zone
            if (!Advertisement.IsReady(zoneId))
            {
                Debug.Log(string.Format("Ads not ready for zone '{0}'", zoneId));
                return;
            }
                
            var showOptions = new ShowOptions { resultCallback = InterstitialAdShowResult };
            Advertisement.Show(zoneId, showOptions);

            #endif
        }

        /// <summary>
        /// Shows the rewarded ad. A NewAdEvent event will be fired whenever this rewarded ad is finished or failed.
        /// </summary>
        public void ShowRewardedAd()
        {
            const string RewardedZoneId = "rewardedVideo";

            #if UNITY_ADS

            if (!Advertisement.IsReady(RewardedZoneId))
            {
                Debug.Log(string.Format("Ads not ready for zone '{0}'", RewardedZoneId));
                return;
            }

            var showOptions = new ShowOptions { resultCallback = RewardedAdShowResult };
            Advertisement.Show(RewardedZoneId, showOptions);

            #endif
        }

        #if UNITY_ADS
        
        private void InterstitialAdShowResult(ShowResult result)
        {
            AdEventArgs eventArgs = new AdEventArgs();

            switch (result)
            {
                case ShowResult.Finished:
                    Debug.Log("The ad was successfully shown.");
                    eventArgs.adEvent = AdEvent.Displayed;
                    NewInterstitialAdEvent(eventArgs);
                    break;
                case ShowResult.Skipped:
                    Debug.Log("The ad was skipped before reaching the end.");
                    eventArgs.adEvent = AdEvent.Skipped;
                    NewInterstitialAdEvent(eventArgs);
                    break;
                case ShowResult.Failed:
                    Debug.LogError("The ad failed to be shown.");
                    eventArgs.adEvent = AdEvent.Failed;
                    NewInterstitialAdEvent(eventArgs);
                    break;
            }
        }

        private void RewardedAdShowResult(ShowResult result)
        {
            AdEventArgs eventArgs = new AdEventArgs();

            switch (result)
            {
                case ShowResult.Finished:
                    Debug.Log("The ad was successfully shown.");
                    eventArgs.adEvent = AdEvent.ShouldReward;
                    NewRewardedAdEvent(eventArgs);
                    break;
                case ShowResult.Skipped:
                    Debug.Log("The ad was skipped before reaching the end.");
                    eventArgs.adEvent = AdEvent.Skipped;
                    NewRewardedAdEvent(eventArgs);
                    break;
                case ShowResult.Failed:
                    Debug.LogError("The ad failed to be shown.");
                    eventArgs.adEvent = AdEvent.Failed;
                    NewRewardedAdEvent(eventArgs);
                    break;
            }
        }

        #endif
    }
}













