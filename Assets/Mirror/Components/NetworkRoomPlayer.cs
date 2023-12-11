using UnityEngine;

namespace Mirror
{
    /// <summary>
    /// This component works in conjunction with the NetworkRoomManager to make up the multiplayer room system.
    /// <para>The RoomPrefab object of the NetworkRoomManager must have this component on it. This component holds basic room player data required for the room to function. Game specific data for room players can be put in other components on the RoomPrefab or in scripts derived from NetworkRoomPlayer.</para>
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Network/Network Room Player")]
    [HelpURL("https://mirror-networking.gitbook.io/docs/components/network-room-player")]
    public class NetworkRoomPlayer : NetworkBehaviour
    {
        /// <summary>
        /// This flag controls whether the default UI is shown for the room player.
        /// <para>As this UI is rendered using the old GUI system, it is only recommended for testing purposes.</para>
        /// </summary>
        [Tooltip("This flag controls whether the default UI is shown for the room player")]
        public bool showRoomGUI = true;

        [Header("Diagnostics")]

        /// <summary>
        /// Diagnostic flag indicating whether this player is ready for the game to begin.
        /// <para>Invoke CmdChangeReadyState method on the client to set this flag.</para>
        /// <para>When all players are ready to begin, the game will start. This should not be set directly, CmdChangeReadyState should be called on the client to set it on the server.</para>
        /// </summary>
        [Tooltip("Diagnostic flag indicating whether this player is ready for the game to begin")]
        [SyncVar(hook = nameof(ReadyStateChanged))]
        public bool readyToBegin;

        /// <summary>
        /// Diagnostic index of the player, e.g. Player1, Player2, etc.
        /// </summary>
        [Tooltip("Diagnostic index of the player, e.g. Player1, Player2, etc.")]
        [SyncVar(hook = nameof(IndexChanged))]
        public int index;

        #region Unity Callbacks

        /// <summary>
        /// Do not use Start - Override OnStartHost / OnStartClient instead!
        /// </summary>
        public virtual void Start()
        {
            if (NetworkManager.singleton is NetworkRoomManager room)
            {
                // NetworkRoomPlayer object must be set to DontDestroyOnLoad along with NetworkRoomManager
                // in server and all clients, otherwise it will be respawned in the game scene which would
                // have undesirable effects.
                if (room.dontDestroyOnLoad)
                    DontDestroyOnLoad(gameObject);

                room.roomSlots.Add(this);

                if (NetworkServer.active)
                    room.RecalculateRoomPlayerIndices();

                if (NetworkClient.active)
                    room.CallOnClientEnterRoom();
            }
            else Debug.LogError("RoomPlayer could not find a NetworkRoomManager. The RoomPlayer requires a NetworkRoomManager object to function. Make sure that there is one in the scene.");
        }

        public virtual void OnDisable()
        {
            if (NetworkClient.active && NetworkManager.singleton is NetworkRoomManager room)
            {
                // only need to call this on client as server removes it before object is destroyed
                room.roomSlots.Remove(this);

                room.CallOnClientExitRoom();
            }
        }

        #endregion

        #region Commands

        [Command]
        public void CmdChangeReadyState(bool readyState)
        {
            readyToBegin = readyState;
            NetworkRoomManager room = NetworkManager.singleton as NetworkRoomManager;
            if (room != null)
            {
                room.ReadyStatusChanged();
            }
        }

        #endregion

        #region SyncVar Hooks

        /// <summary>
        /// This is a hook that is invoked on clients when the index changes.
        /// </summary>
        /// <param name="oldIndex">The old index value</param>
        /// <param name="newIndex">The new index value</param>
        public virtual void IndexChanged(int oldIndex, int newIndex) {}

        /// <summary>
        /// This is a hook that is invoked on clients when a RoomPlayer switches between ready or not ready.
        /// <para>This function is called when the a client player calls CmdChangeReadyState.</para>
        /// </summary>
        /// <param name="newReadyState">New Ready State</param>
        public virtual void ReadyStateChanged(bool oldReadyState, bool newReadyState) {}

        #endregion

        #region Room Client Virtuals

        /// <summary>
        /// This is a hook that is invoked on clients for all room player objects when entering the room.
        /// <para>Note: isLocalPlayer is not guaranteed to be set until OnStartLocalPlayer is called.</para>
        /// </summary>
        public virtual void OnClientEnterRoom() {}

        /// <summary>
        /// This is a hook that is invoked on clients for all room player objects when exiting the room.
        /// </summary>
        public virtual void OnClientExitRoom() {}

        #endregion

        #region Optional UI

        /// <summary>
        /// Render a UI for the room. Override to provide your own UI
        /// </summary>
        public virtual void OnGUI()
        {
            if (!showRoomGUI)
                return;

            NetworkRoomManager room = NetworkManager.singleton as NetworkRoomManager;
            if (room)
            {
                if (!room.showRoomGUI)
                    return;

                if (!Utils.IsSceneActive(room.RoomScene))
                    return;

                DrawPlayerReadyState();
                DrawPlayerReadyButton();
            }
        }

        void DrawPlayerReadyState()
        {
            GUIStyle labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 40,
                fontStyle = FontStyle.Bold
            };

            GUIStyle outStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 30,
                fontStyle = FontStyle.Bold
            };


            GUILayout.BeginArea(new Rect(60f + (index * 300), 150f, 300f, 250f));
            labelStyle.normal.textColor = Color.green;
            GUILayout.Label($"Player [{index + 1}]", labelStyle);

            if (readyToBegin)
            {
                labelStyle.normal.textColor = Color.yellow;
                GUILayout.Label("준비", labelStyle);
            }
            else
            {
                labelStyle.normal.textColor = Color.red;
                GUILayout.Label("대기", labelStyle);
            }

            if (((isServer && index > 0) || isServerOnly) && GUILayout.Button("강퇴", outStyle, GUILayout.Width(200), GUILayout.Height(70)))
            {
                GetComponent<NetworkIdentity>().connectionToClient.Disconnect();
            }

            GUILayout.EndArea();
        }


        void DrawPlayerReadyButton()
        {
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 30, // 여기서 원하는 크기로 설정합니다.
                fontStyle = FontStyle.Bold
            };

            if (NetworkClient.active && isLocalPlayer)
            {
                GUILayout.BeginArea(new Rect(20f + (index * 300), 400f, 200f, 50f));

                if (readyToBegin)
                {
                    if (GUILayout.Button("취소", buttonStyle)) // 여기서 스타일을 적용합니다.
                        CmdChangeReadyState(false);
                }
                else
                {
                    if (GUILayout.Button("준비", buttonStyle)) // 여기서 스타일을 적용합니다.
                        CmdChangeReadyState(true);
                }

                GUILayout.EndArea();
            }
        }
        #endregion
    }
}
