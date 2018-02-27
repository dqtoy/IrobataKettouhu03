namespace CCGKit
{
    /// <summary>
    /// The base stat class used in player and card definitions.
    /// プレーヤとカードの定義で使用される基本statクラス。
    /// </summary>
    public class DefinitionStat : Resource
    {
        /// <summary>
        /// The name of this stat.
        /// </summary>
        public string name;

        /// <summary>
        /// The base value of this stat.
        /// </summary>
        public int baseValue;

        /// <summary>
        /// The original value of this stat.
        /// </summary>
        public int originalValue;

        /// <summary>
        /// The minimum value of this stat.
        /// </summary>
        public int minValue;

        /// <summary>
        /// The maximum value of this stat.
        /// </summary>
        public int maxValue;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="id">The identifier of this resource.</param>
        public DefinitionStat(int id) : base(id)
        {
        }
    }

    /// <summary>
    /// The stat of a player definition.
    /// </summary>
    public class PlayerStat : DefinitionStat
    {
        /// <summary>
        /// The current resource identifier.
        /// 複数のプレイヤーをIDで管理する
        /// </summary>
        public static int currentId;

        /// <summary>
        /// Constructor.
        /// 呼び出されるたびにIDを1ずつ増やす
        /// </summary>
        /// 派生クラスから基底クラスの共通処理を呼び出すときは「base.関数名」で呼び出すことができる
        public PlayerStat() : base(currentId++)
        {
        }
    }

    /// <summary>
    /// The stat of a card definition.
    /// </summary>
    public class CardStat : DefinitionStat
    {
        /// <summary>
        /// The current resource identifier.
        /// </summary>
        public static int currentId;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// 呼び出されるたびにIDを1ずつ増やす
        /// 派生クラスから基底クラスの共通処理を呼び出すときは「base.関数名」で呼び出すことができる
        public CardStat() : base(currentId++)
        {
        }
    }
}
