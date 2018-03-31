using System.Collections.Generic;

namespace CCGKit
{
    /// <summary>
    /// The ability types supported by the kit.
    /// �L�b�g�ŃT�|�[�g����Ă���\�̓^�C�v�B
    /// </summary>
    public enum AbilityType
    {
        Triggered,
        Activated
    }

    /// <summary>
    /// The base ability class.
    /// ��b�\�̓N���X�B
    /// </summary>
    public class Ability
    {
        /// <summary>
        /// The name of this ability.
        /// ���̔\�̖͂��O�B
        /// </summary>
        public string name;

        /// <summary>
        /// The type of this ability.
        /// ���̔\�͂̃^�C�v�B
        /// </summary>
        public AbilityType type;

        /// <summary>
        /// The effect of this ability.
        /// ���̔\�͂̌��ʁB
        /// </summary>
        public Effect effect;

        /// <summary>
        /// The target of this ability.
        /// ���̔\�̖͂ڕW�B
        /// </summary>
        public Target target;
    }

    /// <summary>
    /// Triggered abilities are abilities that get resolved when their
    /// associated trigger takes place.
    /// �g���K�[�\�͂Ƃ́A���̃g���K�[�����������Ƃ��ɉ��������\�͂ł��B
    /// </summary>
    public class TriggeredAbility : Ability
    {
        /// <summary>
        /// The trigger of this ability.
        /// ���̔\�͂̈������B
        /// </summary>
        public Trigger trigger;

        /// <summary>
        /// Constructor.
        /// </summary>
        public TriggeredAbility()
        {
            type = AbilityType.Triggered;
        }
    }

    /// <summary>
    /// Activated abilities are abilities that get resolved when the player
    /// pays a cost/s.
    /// �A�N�e�B�u�����ꂽ�\�͂́A�v���C���[���R�X�g���x�������Ƃ��ɉ��������\�͂ł��B
    /// </summary>
    public class ActivatedAbility : Ability
    {
        /// <summary>
        /// The zone of this ability.
        /// ���̔\�͂̃]�[���B
        /// </summary>
        [GameZoneField("Zone")]
        public int zoneId;

        /// <summary>
        /// The costs of this ability.
        /// ���̔\�͂̃R�X�g
        /// </summary>
        public List<Cost> costs = new List<Cost>();

        /// <summary>
        /// Constructor.
        /// </summary>
        public ActivatedAbility()
        {
            type = AbilityType.Activated;
        }
    }
}
