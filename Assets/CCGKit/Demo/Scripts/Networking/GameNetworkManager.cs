using UnityEngine;
using UnityEngine.Networking;

using CCGKit;

/// <summary>
/// Create a NetworkManager subclass with an automatically-managed lifetime. Having a subclass will also
/// come in handy if we need to extend the capabilities of the vanilla NetworkManager in the future.
/// 自動的に管理されるライフタイムを持つNetworkManagerサブクラスを作成します。 将来、バニラNetworkManagerの機能を拡張する必要がある場合は、サブクラスを用意することも便利です。
/// </summary>
public class GameNetworkManager : NetworkManager
{
    private static GameNetworkManager instance;

    public static GameNetworkManager Instance
    {
        get
        {
            return instance ?? new GameObject("GameNetworkManager").AddComponent<GameNetworkManager>();
        }
    }

    /// <summary>
    /// Indicates if the current match is single-player against the AI or multiplayer between humans.
    /// 現在の試合が、人間同士のAIまたはマルチプレイヤーに対するシングルプレイヤーかどうかを示します。
    /// </summary>
    public bool isSinglePlayer;

    /// <summary>
    /// Prefab for the AI-controlled player.
    /// AI制御プレーヤのプレハブ。
    /// </summary>
    public GameObject aiPlayerPrefab;

    //GameNetworkManagerの初期化
    public void Initialize()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        // UNET currently crashes on iOS if the runInBackground property is set to true.
        //runInBackgroundプロパティがtrueに設定されていると、UNETは現在iOSでクラッシュします。
        //なので、iOSの場合ここでfalseに設定する
        ///UNETはネットワーク通信するための仕組み。
        if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.tvOS)
        {
            runInBackground = false;
        }
    }

    private void OnEnable()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    //サーバ側のユーザ名表示
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        GameObject player = null;
        if (isSinglePlayer && playerControllerId == 1)
        {
            player = Instantiate(aiPlayerPrefab);
            player.name = "マエリベリー・ハーン";
        }
        else
        {
            player = Instantiate(playerPrefab);
            player.name = "宇佐見蓮子";
        }
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);

        var server = GameObject.Find("Server");
        if (server != null)
        {
            server.GetComponent<Server>().OnPlayerConnected(conn.connectionId);
        }
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);

        var server = GameObject.Find("Server");
        if (server != null)
        {
            server.GetComponent<Server>().OnPlayerDisconnected(conn.connectionId);
        }
    }
}
