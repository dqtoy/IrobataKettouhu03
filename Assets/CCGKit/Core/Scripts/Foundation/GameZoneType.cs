namespace CCGKit
{
    /// <summary>
    /// The available zone owners.
    /// 使用可能なゾーンの所有者。
    /// </summary>
    public enum ZoneOwner
    {
        Player,
        Shared
    }

    /// <summary>
    /// The available zone types.
    /// 利用可能なゾーンの種類。
    /// </summary>
    public enum ZoneType
    {
        Static,
        Dynamic
    }

    /// <summary>
    /// The available zone owner visibilities.
    /// 使用可能なゾーン所有者の可視性。
    /// </summary>
    public enum ZoneOwnerVisibility
    {
        Visible,
        Hidden,
    }

    /// <summary>
    /// The available zone opponent visibilities.
    /// 使用可能なゾーンの相手の視認性。
    /// </summary>
    public enum ZoneOpponentVisibility
    {
        Visible,
        Hidden
    }

    /// <summary>
    /// This class represents a game zone type available in a game.
    /// このクラスは、ゲームで使用可能なゲームゾーンタイプを表します。
    /// </summary>
    public class GameZoneType : Resource
    {
        /// <summary>
        /// The current resource identifier.
        /// 現在のリソース識別子。
        /// </summary>
        public static int currentId;

        /// <summary>
        /// The name of this zone.
        /// このゾーンの名前。
        /// </summary>
        public string name;

        /// <summary>
        /// The owner of this zone.
        /// このゾーンの所有者。
        /// </summary>
        public ZoneOwner owner;

        /// <summary>
        /// The type of this zone.
        /// このゾーンのタイプ。
        /// </summary>
        public ZoneType type;

        /// <summary>
        /// The visibility of this zone for the owner player.
        /// 所有者プレーヤーのためのこのゾーンの可視性。
        /// </summary>
        public ZoneOwnerVisibility ownerVisibility;

        /// <summary>
        /// The visibility of this zone for the opponent player.
        /// 相手プレーヤーのためのこのゾーンの可視性。
        /// </summary>
        public ZoneOpponentVisibility opponentVisibility;

        /// <summary>
        /// True if this zone has a maximum size.
        /// このゾーンが最大サイズを持つ場合はTrueです。
        /// </summary>
        public bool hasMaxSize;

        /// <summary>
        /// The maximum size of this number.
        /// このゾーンが最小サイズを持つ場合はTrueです。
        /// </summary>
        public int maxSize;

        /// <summary>
        /// Constructor.
        /// </summary>
        public GameZoneType() : base(currentId++)
        {
        }
    }
}
