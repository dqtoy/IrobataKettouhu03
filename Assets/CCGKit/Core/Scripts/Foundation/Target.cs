using System.Collections.Generic;

namespace CCGKit
{
    /// <summary>
    /// The available effect targets.
    /// 使用可能なエフェクトターゲット。
    /// </summary>
    public enum EffectTarget
    {
        Player,
        Opponent,
        TargetPlayer,
        RandomPlayer,
        AllPlayers,
        ThisCard,
        PlayerCard,
        OpponentCard,
        TargetCard,
        RandomPlayerCard,
        RandomOpponentCard,
        RandomCard,
        AllPlayerCards,
        AllOpponentCards,
        AllCards,
        PlayerOrPlayerCreature,
        OpponentOrOpponentCreature,
        AnyPlayerOrCreature
    }

    /// <summary>
    /// The base class for targets.
    /// ターゲットのベースとなるクラス
    /// </summary>
    public abstract class Target
    {
        public virtual EffectTarget GetTarget()
        {
            return EffectTarget.Player;
        }
    }

    public interface IPlayerTarget
    {
    }

    public interface ICardTarget
    {
    }

    public interface IHeroPowerTarget
    {
    }

    public interface ITokenTarget
    {
    }

    public interface IUserTarget
    {
    }

    public interface IComputedTarget
    {
    }

    public abstract class PlayerTargetBase : Target, IPlayerTarget
    {
        public List<PlayerCondition> conditions = new List<PlayerCondition>();
    }

    public abstract class CardTargetBase : Target, ICardTarget
    {
        public List<CardCondition> conditions = new List<CardCondition>();
    }

    public abstract class HeroPowerTargetBase : Target, IHeroPowerTarget
    {
        public List<HeroPowerCondition> conditions = new List<HeroPowerCondition>();
    }

    public abstract class TokenTargetBase : Target, ITokenTarget
    {
        public List<TokenCondition> conditions = new List<TokenCondition>();
    }


    public class PlayerTarget : PlayerTargetBase, IComputedTarget
    {
        public override EffectTarget GetTarget()
        {
            return EffectTarget.Player;
        }
    }

    public class OpponentTarget : PlayerTargetBase, IComputedTarget
    {
        public override EffectTarget GetTarget()
        {
            return EffectTarget.Opponent;
        }
    }

    public class TargetPlayer : PlayerTargetBase, IUserTarget
    {
        public override EffectTarget GetTarget()
        {
            return EffectTarget.TargetPlayer;
        }
    }

    public class RandomPlayer : PlayerTargetBase, IComputedTarget
    {
        public override EffectTarget GetTarget()
        {
            return EffectTarget.RandomPlayer;
        }
    }

    public class AllPlayers : PlayerTargetBase, IComputedTarget
    {
        public override EffectTarget GetTarget()
        {
            return EffectTarget.AllPlayers;
        }
    }

    public class ThisCard : CardTargetBase, IComputedTarget
    {
        public override EffectTarget GetTarget()
        {
            return EffectTarget.ThisCard;
        }
    }

    public class PlayerCard : CardTargetBase, IUserTarget
    {
        public override EffectTarget GetTarget()
        {
            return EffectTarget.PlayerCard;
        }
    }

    public class OpponentCard : CardTargetBase, IUserTarget
    {
        public override EffectTarget GetTarget()
        {
            return EffectTarget.OpponentCard;
        }
    }

    public class TargetCard : CardTargetBase, IUserTarget
    {
        public override EffectTarget GetTarget()
        {
            return EffectTarget.TargetCard;
        }
    }

    public class RandomPlayerCard : CardTargetBase, IComputedTarget
    {
        public override EffectTarget GetTarget()
        {
            return EffectTarget.RandomPlayerCard;
        }
    }

    public class RandomOpponentCard : CardTargetBase, IComputedTarget
    {
        public override EffectTarget GetTarget()
        {
            return EffectTarget.RandomOpponentCard;
        }
    }

    public class RandomCard : CardTargetBase, IComputedTarget
    {
        public override EffectTarget GetTarget()
        {
            return EffectTarget.RandomCard;
        }
    }

    public class AllPlayerCards : CardTargetBase, IComputedTarget
    {
        public override EffectTarget GetTarget()
        {
            return EffectTarget.AllPlayerCards;
        }
    }

    public class AllOpponentCards : CardTargetBase, IComputedTarget
    {
        public override EffectTarget GetTarget()
        {
            return EffectTarget.AllOpponentCards;
        }
    }

    public class AllCards : CardTargetBase, IComputedTarget
    {
        public override EffectTarget GetTarget()
        {
            return EffectTarget.AllCards;
        }
    }
}
