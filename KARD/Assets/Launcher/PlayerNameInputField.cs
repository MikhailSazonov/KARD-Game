using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

using System.Collections;

namespace KARD {

[RequireComponent(typeof(InputField))]
public class PlayerNameInputField : MonoBehaviour
{

    #region Private Constants

    const string playerNamePrefKey = "PlayerName";

    #endregion

    #region MonoBehaviour CallBacks

    // Start is called before the first frame update
    void Start()
    {
        string defaultName = string.Empty;
        InputField _inputField = this.GetComponent<InputField>();
        if (_inputField != null)
        {
            if (PlayerPrefs.HasKey(playerNamePrefKey))
            {
                defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                _inputField.text = defaultName;
            }

            PhotonNetwork.NickName = defaultName;
        }        
    }

    #endregion

    #region Public Methods

    public void SetPlayerName(string value)
    {
        if (value == null)
        {
            Debug.LogError("Player Name is null");
            return;
        }
        PhotonNetwork.NickName = value;

        PlayerPrefs.SetString(playerNamePrefKey, value);
    }

    #endregion

}

}
