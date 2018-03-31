using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace CCGKit
{ 
    /// <summary>
    /// HeroPowerCollectionEditorクラスのパクリ
    /// </summary>
    public class HeroPowerEditor : EditorTab
    {
        private ReorderableList HeroPowerSetsList;
        private HeroPowerSet currentHeroPowerSet;

        private ReorderableList currentHeroPowerList;
        private HeroPower currentHeroPower;

        private ReorderableList currentHeroPowerCostsList;
        private Cost currentHeroPowerCost;

        private ReorderableList currentHeroPowerKeywordsList;
        private RuntimeKeyword currentHeroPowerKeyword;

        private ReorderableList currentHeroPowerAbilitiesList;
        private Ability currentHeroPowerAbility;

        private ReorderableList currentEffectCostsList;
        private Cost currentEffectCost;

        private ReorderableList currentPlayerTargetConditionsList;
        private PlayerTargetBase currentPlayerTarget;
        private PlayerCondition currentPlayerTargetCondition;
        private ReorderableList currentHeroPowerTargetConditionsList;
        private HeroPowerTargetBase currentHeroPowerTarget;
        private HeroPowerCondition currentHeroPowerTargetCondition;

        private List<Type> triggerTypes;
        private List<string> triggerTypeNames;
        private List<Type> effectTypes;
        private List<string> effectTypeNames;
        private List<Type> humanTargetTypes;
        private List<string> humanTargetTypeNames;
        private List<Type> HeroPowerTargetTypes;
        private List<string> HeroPowerTargetTypeNames;

        public HeroPowerEditor(GameConfiguration config) : base(config)
        {
            HeroPowerSetsList = EditorUtils.SetupReorderableList("HeroPower sets", gameConfig.HeroPowerSets, ref currentHeroPowerSet, (rect, x) =>
            {
                EditorGUI.LabelField(new Rect(rect.x, rect.y, 200, EditorGUIUtility.singleLineHeight), x.name);
            },
            (x) =>
            {
                currentHeroPowerSet = x;
                currentHeroPower = null;
                currentHeroPowerCost = null;
                currentHeroPowerKeyword = null;
                currentHeroPowerAbility = null;
                CreateCurrentHeroPowerSetHeroPowersList();
            },
            () =>
            {
                gameConfig.HeroPowerSets.Add(new HeroPowerSet());
            },
            (x) =>
            {
                currentHeroPowerSet = null;
                currentHeroPower = null;
                currentHeroPowerCost = null;
                currentHeroPowerKeyword = null;
                currentHeroPowerAbility = null;
            });
        }

        private void CreateCurrentHeroPowerSetHeroPowersList()
        {
            currentHeroPowerList = EditorUtils.SetupReorderableList("HeroPowers", currentHeroPowerSet.HeroPowers, ref currentHeroPower, (rect, x) =>
            {
                EditorGUI.LabelField(new Rect(rect.x, rect.y, 200, EditorGUIUtility.singleLineHeight), x.name);
            },
            (x) =>
            {
                currentHeroPower = x;
                currentHeroPowerCost = null;
                currentHeroPowerKeyword = null;
                currentHeroPowerAbility = null;
                CreateCurrentHeroPowerCostsList();
                CreateCurrentHeroPowerKeywordsList();
                CreateCurrentHeroPowerAbilitiesList();
            },
            () =>
            {
                var menu = new GenericMenu();
                foreach (var HeroPowerType in gameConfig.HeroPowerTypes)
                {
                    menu.AddItem(new GUIContent(HeroPowerType.name), false, CreateHeroPowerCallback, HeroPowerType);
                }
                menu.ShowAsContext();
            },
            (x) =>
            {
                currentHeroPower = null;
                currentHeroPowerCost = null;
                currentHeroPowerKeyword = null;
                currentHeroPowerAbility = null;
            });
        }

        private void CreateCurrentHeroPowerCostsList()
        {
            currentHeroPowerCostsList = EditorUtils.SetupReorderableList("Costs", currentHeroPower.costs, ref currentHeroPowerCost, (rect, x) =>
            {
                EditorGUI.LabelField(new Rect(rect.x, rect.y, 200, EditorGUIUtility.singleLineHeight), x.GetReadableString(gameConfig));
            },
            (x) =>
            {
                currentHeroPowerCost = x;
            },
            () =>
            {
                var menu = new GenericMenu();
                var costTypes = AppDomain.CurrentDomain.GetAllDerivedTypes(typeof(Cost));
                foreach (var type in costTypes)
                {
                    menu.AddItem(new GUIContent(StringUtils.DisplayCamelCaseString(type.Name)), false, CreateHeroPowerCostCallback, type);
                }
                menu.ShowAsContext();
            },
            (x) =>
            {
                currentHeroPowerCost = null;
            });
        }

        private void CreateCurrentHeroPowerKeywordsList()
        {
            currentHeroPowerKeywordsList = EditorUtils.SetupReorderableList("Keywords", currentHeroPower.keywords, ref currentHeroPowerKeyword, (rect, x) =>
            {
                var currentKeyword = gameConfig.keywords.Find(k => k.id == x.keywordId);
                var options = new List<string>();
                foreach (var value in currentKeyword.values)
                {
                    options.Add(value.value);
                }
                x.valueId = EditorGUI.Popup(new Rect(rect.x, rect.y, 150, EditorGUIUtility.singleLineHeight), x.valueId, options.ToArray());
            },
            (x) =>
            {
                currentHeroPowerKeyword = x;
            },
            () =>
            {
                var menu = new GenericMenu();
                for (var i = 0; i < gameConfig.keywords.Count; i++)
                {
                    menu.AddItem(new GUIContent(gameConfig.keywords[i].name), false, CreateHeroPowerKeywordCallback, i);
                }
                menu.ShowAsContext();
            },
            (x) =>
            {
                currentHeroPowerKeyword = null;
            });
        }

        private void CreateCurrentHeroPowerAbilitiesList()
        {
            currentHeroPowerAbilitiesList = EditorUtils.SetupReorderableList("Abilities", currentHeroPower.abilities, ref currentHeroPowerAbility, (rect, x) =>
            {
                EditorGUI.LabelField(new Rect(rect.x, rect.y, 200, EditorGUIUtility.singleLineHeight), x.name);
            },
            (x) =>
            {
                currentHeroPowerAbility = x;
                currentPlayerTargetConditionsList = null;
                currentPlayerTarget = null;
                currentPlayerTargetCondition = null;
                currentHeroPowerTargetConditionsList = null;
                currentHeroPowerTarget = null;
                currentHeroPowerTargetCondition = null;
                if (currentHeroPowerAbility is ActivatedAbility)
                {
                    CreateCurrentEffectCostsList();
                }
            },
            () =>
            {
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent("Triggered ability"), false, CreateHeroPowerAbilityCallback, 0);
                menu.AddItem(new GUIContent("Activated ability"), false, CreateHeroPowerAbilityCallback, 1);
                menu.ShowAsContext();
            },
            (x) =>
            {
                currentHeroPowerAbility = null;
                currentPlayerTargetConditionsList = null;
                currentPlayerTarget = null;
                currentPlayerTargetCondition = null;
                currentHeroPowerTargetConditionsList = null;
                currentHeroPowerTarget = null;
                currentHeroPowerTargetCondition = null;
            });
        }

        private void CreateCurrentEffectCostsList()
        {
            currentEffectCostsList = EditorUtils.SetupReorderableList("Costs", (currentHeroPowerAbility as ActivatedAbility).costs, ref currentEffectCost, (rect, x) =>
            {
                EditorGUI.LabelField(new Rect(rect.x, rect.y, 200, EditorGUIUtility.singleLineHeight), x.GetReadableString(gameConfig));
            },
            (x) =>
            {
                currentEffectCost = x;
            },
            () =>
            {
                var menu = new GenericMenu();
                var costTypes = AppDomain.CurrentDomain.GetAllDerivedTypes(typeof(Cost));
                foreach (var type in costTypes)
                {
                    menu.AddItem(new GUIContent(StringUtils.DisplayCamelCaseString(type.Name)), false, CreateEffectCostCallback, type);
                }
                menu.ShowAsContext();
            },
            (x) =>
            {
                currentEffectCost = null;
            });
        }

        private void CreateCurrentPlayerTargetConditionsList()
        {
            currentPlayerTargetConditionsList = EditorUtils.SetupReorderableList("Target player conditions", currentPlayerTarget.conditions, ref currentPlayerTargetCondition, (rect, x) =>
            {
                EditorGUI.LabelField(new Rect(rect.x, rect.y, 200, EditorGUIUtility.singleLineHeight), x.GetReadableString(gameConfig));
            },
            (x) =>
            {
                currentPlayerTargetCondition = x;
            },
            () =>
            {
                var menu = new GenericMenu();
                var conditionTypes = AppDomain.CurrentDomain.GetAllDerivedTypes(typeof(PlayerCondition));
                foreach (var type in conditionTypes)
                {
                    menu.AddItem(new GUIContent(StringUtils.DisplayCamelCaseString(type.Name)), false, CreatePlayerTargetConditionCallback, type);
                }
                menu.ShowAsContext();
            },
            (x) =>
            {
                currentPlayerTargetCondition = null;
            });
        }

        private void CreateCurrentHeroPowerTargetConditionsList()
        {
            currentHeroPowerTargetConditionsList = EditorUtils.SetupReorderableList("Target HeroPower conditions", currentHeroPowerTarget.conditions, ref currentHeroPowerTargetCondition, (rect, x) =>
            {
                EditorGUI.LabelField(new Rect(rect.x, rect.y, 200, EditorGUIUtility.singleLineHeight), x.GetReadableString(gameConfig));
            },
            (x) =>
            {
                currentHeroPowerTargetCondition = x;
            },
            () =>
            {
                var menu = new GenericMenu();
                var conditionTypes = AppDomain.CurrentDomain.GetAllDerivedTypes(typeof(HeroPowerCondition));
                foreach (var type in conditionTypes)
                {
                    menu.AddItem(new GUIContent(StringUtils.DisplayCamelCaseString(type.Name)), false, CreateHeroPowerTargetConditionCallback, type);
                }
                menu.ShowAsContext();
            },
            (x) =>
            {
                currentHeroPowerTargetCondition = null;
            });
        }

        private void CreateHeroPowerCostCallback(object obj)
        {
            var cost = Activator.CreateInstance((Type)obj);
            currentHeroPower.costs.Add(cost as Cost);
        }

        private void CreateHeroPowerKeywordCallback(object obj)
        {
            var keyword = new RuntimeKeyword();
            keyword.keywordId = (int)obj;
            currentHeroPower.keywords.Add(keyword);
        }

        private void CreateHeroPowerAbilityCallback(object obj)
        {
            Ability ability = null;
            switch ((int)obj)
            {
                case 0:
                    ability = new TriggeredAbility();
                    break;

                case 1:
                    ability = new ActivatedAbility();
                    break;
            }
            currentHeroPower.abilities.Add(ability);
        }

        private void CreateHeroPowerCallback(object obj)
        {
            var HeroPower = new HeroPower();
            var HeroPowerType = obj as HeroPowerType;
            HeroPower.HeroPowerTypeId = HeroPowerType.id;
            if (HeroPowerType != null)
            {
                foreach (var property in HeroPowerType.properties)
                {
                    if (property is IntProperty)
                    {
                        var propertyCopy = new IntProperty();
                        propertyCopy.name = property.name;
                        propertyCopy.value = (property as IntProperty).value;
                        HeroPower.properties.Add(propertyCopy);
                    }
                    else if (property is StringProperty)
                    {
                        var propertyCopy = new StringProperty();
                        propertyCopy.name = property.name;
                        propertyCopy.value = (property as StringProperty).value;
                        HeroPower.properties.Add(propertyCopy);
                    }
                }

                foreach (var stat in HeroPowerType.stats)
                {
                    var statCopy = new Stat();
                    statCopy.statId = stat.id;
                    statCopy.name = stat.name;
                    statCopy.baseValue = stat.baseValue;
                    statCopy.originalValue = stat.originalValue;
                    statCopy.minValue = stat.minValue;
                    statCopy.maxValue = stat.maxValue;
                    HeroPower.stats.Add(statCopy);
                }
            }
            currentHeroPowerSet.HeroPowers.Add(HeroPower);
        }

        private void CreateEffectCostCallback(object obj)
        {
            var cost = Activator.CreateInstance((Type)obj);
            (currentHeroPowerAbility as ActivatedAbility).costs.Add(cost as Cost);
        }

        private void CreatePlayerTargetConditionCallback(object obj)
        {
            var condition = Activator.CreateInstance((Type)obj);
            currentPlayerTarget.conditions.Add(condition as PlayerCondition);
        }

        private void CreateHeroPowerTargetConditionCallback(object obj)
        {
            var condition = Activator.CreateInstance((Type)obj);
            currentHeroPowerTarget.conditions.Add(condition as HeroPowerCondition);
        }

        public override void OnTabSelected()
        {
            triggerTypes = new List<Type>(AppDomain.CurrentDomain.GetAllDerivedTypes(typeof(Trigger)));
            triggerTypes.RemoveAll(x => x.IsAbstract);
            triggerTypeNames = new List<string>(triggerTypes.Count);
            foreach (var type in triggerTypes)
            {
                triggerTypeNames.Add(StringUtils.DisplayCamelCaseString(type.Name));
            }

            effectTypes = new List<Type>(AppDomain.CurrentDomain.GetAllDerivedTypes(typeof(Effect)));
            effectTypes.RemoveAll(x => x.IsAbstract);
            effectTypeNames = new List<string>(effectTypes.Count);
            foreach (var type in effectTypes)
            {
                effectTypeNames.Add(StringUtils.DisplayCamelCaseString(type.Name));
            }

            humanTargetTypes = new List<Type>(AppDomain.CurrentDomain.GetAllDerivedTypes(typeof(PlayerTargetBase)));
            humanTargetTypes.RemoveAll(x => x.IsAbstract);
            humanTargetTypeNames = new List<string>(humanTargetTypes.Count);
            foreach (var type in humanTargetTypes)
            {
                humanTargetTypeNames.Add(StringUtils.DisplayCamelCaseString(type.Name));
            }

            HeroPowerTargetTypes = new List<Type>(AppDomain.CurrentDomain.GetAllDerivedTypes(typeof(HeroPowerTargetBase)));
            HeroPowerTargetTypes.RemoveAll(x => x.IsAbstract);
            HeroPowerTargetTypeNames = new List<string>(HeroPowerTargetTypes.Count);
            foreach (var type in HeroPowerTargetTypes)
            {
                HeroPowerTargetTypeNames.Add(StringUtils.DisplayCamelCaseString(type.Name));
            }
        }

        public override void Draw()
        {
            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical(GUILayout.MaxWidth(250));
            if (HeroPowerSetsList != null)
            {
                HeroPowerSetsList.DoLayoutList();
            }
            GUILayout.EndVertical();

            if (currentHeroPowerSet != null)
            {
                DrawHeroPowerSet(currentHeroPowerSet);
            }

            GUILayout.EndHorizontal();
        }

        private void DrawHeroPowerSet(HeroPowerSet set)
        {
            var oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 50;

            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Name");
            set.name = EditorGUILayout.TextField(set.name, GUILayout.MaxWidth(EditorConfig.LargeTextFieldWidth));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical(GUILayout.MaxWidth(250));
            if (currentHeroPowerList != null)
            {
                currentHeroPowerList.DoLayoutList();
            }
            GUILayout.EndVertical();

            if (currentHeroPower != null)
            {
                DrawHeroPower(currentHeroPower);
            }

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            EditorGUIUtility.labelWidth = oldLabelWidth;
        }

        private void DrawHeroPower(HeroPower HeroPower)
        {
            var oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 100;

            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Name");
            HeroPower.name = EditorGUILayout.TextField(HeroPower.name, GUILayout.MaxWidth(EditorConfig.LargeTextFieldWidth));
            GUILayout.EndHorizontal();

            foreach (var stat in HeroPower.stats)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(stat.name);
                stat.baseValue = EditorGUILayout.IntField(stat.baseValue, GUILayout.MaxWidth(EditorConfig.RegularIntFieldWidth));
                stat.originalValue = stat.baseValue;
                GUILayout.EndHorizontal();
            }

            foreach (var property in HeroPower.properties)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(property.name);
                if (property is IntProperty)
                {
                    var intProperty = property as IntProperty;
                    intProperty.value = EditorGUILayout.IntField(intProperty.value, GUILayout.MaxWidth(EditorConfig.RegularIntFieldWidth));
                }
                else if (property is StringProperty)
                {
                    var stringProperty = property as StringProperty;
                    stringProperty.value = EditorGUILayout.TextField(stringProperty.value, GUILayout.MaxWidth(EditorConfig.RegularTextFieldWidth));
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical(GUILayout.MaxWidth(250));
            if (currentHeroPowerCostsList != null)
            {
                currentHeroPowerCostsList.DoLayoutList();
            }
            GUILayout.EndVertical();

            if (currentHeroPowerCost != null)
            {
                DrawCost(currentHeroPowerCost);
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical(GUILayout.MaxWidth(250));
            if (currentHeroPowerKeywordsList != null)
            {
                currentHeroPowerKeywordsList.DoLayoutList();
            }
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical(GUILayout.MaxWidth(250));
            if (currentHeroPowerAbilitiesList != null)
            {
                currentHeroPowerAbilitiesList.DoLayoutList();
            }
            GUILayout.EndVertical();

            if (currentHeroPowerAbility != null)
            {
                DrawAbility(currentHeroPowerAbility);
            }

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            EditorGUIUtility.labelWidth = oldLabelWidth;
        }

        private void DrawCost(Cost cost)
        {
            var oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 80;

            GUILayout.BeginVertical();

            var fields = cost.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
            for (var i = 0; i < fields.Length; i++)
            {
                var attribute = Attribute.GetCustomAttribute(fields[i], typeof(FieldAttribute)) as FieldAttribute;
                if (attribute != null)
                {
                    GUILayout.BeginHorizontal();
                    attribute.Draw(gameConfig, cost, ref fields[i]);
                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.EndVertical();

            EditorGUIUtility.labelWidth = oldLabelWidth;
        }

        private void DrawAbility(Ability ability)
        {
            var oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 70;

            GUILayout.BeginVertical();

            if (ability is TriggeredAbility)
            {
                EditorGUILayout.LabelField("Triggered ability", EditorStyles.boldLabel);

                GUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Name");
                ability.name = EditorGUILayout.TextField(ability.name, GUILayout.MaxWidth(EditorConfig.LargeTextFieldWidth));
                GUILayout.EndHorizontal();

                var triggeredAbility = ability as TriggeredAbility;

                var triggerTypeId = 0;
                if (triggeredAbility.trigger != null)
                {
                    triggerTypeId = triggerTypes.FindIndex(x => x == triggeredAbility.trigger.GetType());
                }

                var prevTriggerTypeId = triggerTypeId;

                GUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Trigger");
                triggerTypeId = EditorGUILayout.Popup(prevTriggerTypeId, triggerTypeNames.ToArray(), GUILayout.MaxWidth(EditorConfig.LargeComboBoxWidth));
                GUILayout.EndHorizontal();

                if (triggeredAbility.trigger == null || triggerTypeId != prevTriggerTypeId)
                {
                    var type = triggerTypes[triggerTypeId];
                    triggeredAbility.trigger = Activator.CreateInstance(type) as Trigger;
                }

                var fields = triggeredAbility.trigger.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
                for (var i = 0; i < fields.Length; i++)
                {
                    var attribute = Attribute.GetCustomAttribute(fields[i], typeof(FieldAttribute)) as FieldAttribute;
                    if (attribute != null)
                    {
                        GUILayout.BeginHorizontal();
                        attribute.Draw(gameConfig, triggeredAbility.trigger, ref fields[i]);
                        GUILayout.EndHorizontal();
                    }
                }

                DrawEffect(ref triggeredAbility.effect, ref triggeredAbility.target);
            }
            else if (ability is ActivatedAbility)
            {
                EditorGUILayout.LabelField("Activated ability", EditorStyles.boldLabel);

                GUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Name");
                ability.name = EditorGUILayout.TextField(ability.name, GUILayout.MaxWidth(EditorConfig.LargeTextFieldWidth));
                GUILayout.EndHorizontal();

                var activatedAbility = ability as ActivatedAbility;

                var fields = activatedAbility.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
                for (var i = 0; i < fields.Length; i++)
                {
                    var attribute = Attribute.GetCustomAttribute(fields[i], typeof(FieldAttribute)) as FieldAttribute;
                    if (attribute != null)
                    {
                        GUILayout.BeginHorizontal();
                        attribute.Draw(gameConfig, activatedAbility, ref fields[i]);
                        GUILayout.EndHorizontal();
                    }
                }

                GUILayout.BeginHorizontal();

                GUILayout.BeginVertical(GUILayout.MaxWidth(250));
                if (currentEffectCostsList != null)
                {
                    currentEffectCostsList.DoLayoutList();
                }
                GUILayout.EndVertical();

                if (currentEffectCost != null)
                {
                    DrawCost(currentEffectCost);
                }

                GUILayout.EndHorizontal();

                DrawEffect(ref activatedAbility.effect, ref activatedAbility.target);
            }

            GUILayout.EndVertical();

            EditorGUIUtility.labelWidth = oldLabelWidth;
        }

        private void DrawEffect(ref Effect effect, ref Target target)
        {
            var effectTypeId = 0;
            if (effect != null)
            {
                var effectCopy = effect;
                effectTypeId = effectTypes.FindIndex(x => x == effectCopy.GetType());
            }

            var prevEffectTypeId = effectTypeId;

            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Effect");
            effectTypeId = EditorGUILayout.Popup(prevEffectTypeId, effectTypeNames.ToArray(), GUILayout.MaxWidth(EditorConfig.RegularComboBoxWidth));
            GUILayout.EndHorizontal();

            if (effect == null || effectTypeId != prevEffectTypeId)
            {
                var type = effectTypes[effectTypeId];
                effect = Activator.CreateInstance(type) as Effect;
            }

            var fields = effect.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public).OrderBy(x => ((OrderAttribute)x.GetCustomAttributes(typeof(OrderAttribute), false)[0]).Order).ToArray();
            for (var i = 0; i < fields.Length; i++)
            {
                var attribute = Attribute.GetCustomAttribute(fields[i], typeof(FieldAttribute)) as FieldAttribute;
                if (attribute != null)
                {
                    GUILayout.BeginHorizontal();
                    attribute.Draw(gameConfig, effect, ref fields[i]);
                    GUILayout.EndHorizontal();
                }
            }

            if (effect != null)
            {
                var targetAttributes = effectTypes[effectTypeId].GetCustomAttributes(typeof(System.Attribute), true);
                if (targetAttributes != null && targetAttributes.Length > 0)
                {
                    List<Type> targetTypes;
                    List<string> targetTypeNames;
                    if (targetAttributes[0] is PlayerTargetAttribute)
                    {
                        targetTypes = humanTargetTypes;
                        targetTypeNames = humanTargetTypeNames;
                    }
                    else
                    {
                        targetTypes = HeroPowerTargetTypes;
                        targetTypeNames = HeroPowerTargetTypeNames;
                    }

                    var targetTypeId = 0;
                    if (target != null)
                    {
                        var targetCopy = target;
                        targetTypeId = targetTypes.FindIndex(x => x == targetCopy.GetType());
                    }

                    var prevTargetTypeId = targetTypeId;

                    GUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel("Target");
                    targetTypeId = EditorGUILayout.Popup(prevTargetTypeId, targetTypeNames.ToArray(), GUILayout.MaxWidth(EditorConfig.RegularComboBoxWidth));
                    GUILayout.EndHorizontal();

                    if (target == null || targetTypeId != prevTargetTypeId)
                    {
                        var type = targetTypes[targetTypeId];
                        target = Activator.CreateInstance(type) as Target;
                    }

                    fields = target.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
                    for (var i = 0; i < fields.Length; i++)
                    {
                        var attribute = Attribute.GetCustomAttribute(fields[i], typeof(FieldAttribute)) as FieldAttribute;
                        if (attribute != null)
                        {
                            GUILayout.BeginHorizontal();
                            attribute.Draw(gameConfig, target, ref fields[i]);
                            GUILayout.EndHorizontal();
                        }
                    }

                    if (target != null)
                    {
                        if (target is PlayerTargetBase && currentPlayerTargetConditionsList == null)
                        {
                            currentPlayerTarget = target as PlayerTargetBase;
                            CreateCurrentPlayerTargetConditionsList();
                        }
                        else if (target is HeroPowerTargetBase && currentHeroPowerTargetConditionsList == null)
                        {
                            currentHeroPowerTarget = target as HeroPowerTargetBase;
                            CreateCurrentHeroPowerTargetConditionsList();
                        }
                    }

                    if (currentPlayerTarget != null)
                    {
                        GUILayout.BeginHorizontal();

                        GUILayout.BeginVertical(GUILayout.MaxWidth(250));
                        if (currentPlayerTargetConditionsList != null)
                        {
                            currentPlayerTargetConditionsList.DoLayoutList();
                        }
                        GUILayout.EndVertical();

                        if (currentPlayerTargetCondition != null)
                        {
                            DrawTargetCondition(currentPlayerTargetCondition);
                        }

                        GUILayout.EndHorizontal();
                    }
                    else if (currentHeroPowerTarget != null)
                    {
                        GUILayout.BeginHorizontal();

                        GUILayout.BeginVertical(GUILayout.MaxWidth(250));
                        if (currentHeroPowerTargetConditionsList != null)
                        {
                            currentHeroPowerTargetConditionsList.DoLayoutList();
                        }
                        GUILayout.EndVertical();

                        if (currentHeroPowerTargetCondition != null)
                        {
                            DrawTargetCondition(currentHeroPowerTargetCondition);
                        }

                        GUILayout.EndHorizontal();
                    }
                }
            }
        }

        private void DrawTargetCondition(Condition condition)
        {
            var oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = EditorConfig.RegularLabelWidth;

            GUILayout.BeginVertical();

            var fields = condition.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public).OrderBy(x => ((OrderAttribute)x.GetCustomAttributes(typeof(OrderAttribute), false)[0]).Order).ToArray();
            for (var i = 0; i < fields.Length; i++)
            {
                var attribute = Attribute.GetCustomAttribute(fields[i], typeof(FieldAttribute)) as FieldAttribute;
                if (attribute != null)
                {
                    GUILayout.BeginHorizontal();
                    attribute.Draw(gameConfig, condition, ref fields[i]);
                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.EndVertical();

            EditorGUIUtility.labelWidth = oldLabelWidth;
        }
    }

}

