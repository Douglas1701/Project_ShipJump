﻿/** 
* Author: Hisham Ata, Matthew Douglas
* Purpose: To Handle the Game Service logic utilizing the Voxel Native Plugin
**/

using System.Linq;
using UnityEngine;
using VoxelBusters.NativePlugins;
using VoxelBusters.Utility;

public class GameService : MonoBehaviour
{
    public UIDelgate ui;
    public MessageBox prompt;

    bool _isServiceAvailable;
    bool _isAuthenticated;

    private bool _isOffline = false;

    public eLeaderboardTimeScope myBoardScope;

    User[] myFriends;
    // Start is called before the first frame update
    void Awake()
    {
        _isServiceAvailable = NPBinding.GameServices.IsAvailable();
        SignIn();
    }

    public void SignIn()
    {
        if (_isServiceAvailable)
        {
            _isAuthenticated = NPBinding.GameServices.LocalUser.IsAuthenticated;
            if (!_isAuthenticated)
            {
                // Authenticate Local User
                NPBinding.GameServices.LocalUser.Authenticate((bool _success, string _error) => {

                    if (_success)
                    {
                        Debug.Log("Sign-In Successfully");
                        Debug.Log("Local User Details : " + NPBinding.GameServices.LocalUser.ToString());
                        _isOffline = false;

                        CloudSaving.instance.InitializeCloud();
                    }
                    else
                    {
                        Debug.Log("Sign-In Failed with error " + _error);

                        if (!_isOffline)
                        {
                            //SaveManager.instance.DefaultLoad();
                            CloudSaving.instance.DefaultLoad();
                            ui.HasAuthenitcated();
                            OfflineMode();
                        }
                        else
                        {
                            prompt.SetPrompt("Could Not sign In", "Authentication has failed.");
                        }
                    }
                });
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    //Call this function if we want to use the friends list features of gamecenter or google play games
    void LoadFriends()
    {
        NPBinding.GameServices.LocalUser.LoadFriends((User[] _friendsList, string _error) => {
            if (_friendsList != null)
            {
                Debug.Log("Succesfully loaded user friends.");
                myFriends = _friendsList;
            }
            else
            {
                Debug.Log("Failed to load user friends with error " + _error);
            }

        });
    }

    //Link to signout button - per google documentation - this is recommended
    public void SignOut()
    {
        NPBinding.GameServices.LocalUser.SignOut((bool _success, string _error) => {

            if (_success)
            {
                Debug.Log("Local user is signed out successfully!");
                SignIn();
            }
            else
            {
                Debug.Log("Request to signout local user failed.");
                Debug.Log(string.Format("Error= {0}.", _error.GetPrintableString()));
            }
        });
    }

    private void OfflineMode()
    {
        // custom code here for when your application is offline
        if (!_isOffline)
        {
            _isOffline = true;
            // disable all online buttons
            ui.toggleOnlineButtons(false);

            // disable ads
            AdService.instance.ToggleTracking(false);

            // display message
            prompt.SetPrompt("Could Not sign In", "All progress will not be saved.\n You can attemp to sign in again at the settings screen.");
        }

    }

    //show leaderboard - should be linked to a button.
    public void ShowLeaderboard(string leaderboardName)
    {
        NPBinding.GameServices.ShowLeaderboardUIWithGlobalID(leaderboardName, myBoardScope, (string _error) => {
            Debug.Log("Leaderboard view dismissed.");
            Debug.Log(string.Format("Error= {0}.", _error.GetPrintableString()));
        });
    }

    //Report the score to the leaderboard.
    public static void ReportScore(string leaderboardName, long leaderboardValue)
    {
        NPBinding.GameServices.ReportScoreWithGlobalID(leaderboardName, leaderboardValue, (bool _success, string _error) => {

            if (_success)
            {
                Debug.Log(string.Format("New score= {0}.", leaderboardValue));
            }
            else
            {
                Debug.Log(string.Format("Error= {0}.", _error.GetPrintableString()));
            }
        });
    }

}
