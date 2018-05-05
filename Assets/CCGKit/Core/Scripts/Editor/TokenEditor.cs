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
    /// CCG Kit editor's 'Token' tab.
    /// CCGキットエディタの[カードコレクション]タブ。
    /// </summary>
    public class TokenEditor : EditorTab
    {
        private ReorderableList TokenSetsList;
        private CardSet currentTokenSet;

        private ReorderableList currentTokenList;
        private Card currentToken;

        private ReorderableList currentTokenCostsList;
        private Cost currentTokenCost;

        private ReorderableList currentTokenKeywordsList;
        private RuntimeKeyword currentTokenKeyword;

        private ReorderableList currentTokenAbilitiesList;
        private Ability currentTokenAbility;

        private ReorderableList currentEffectCostsList;
        private Cost currentEffectCost;

        private Cost currentTokenEffectCost;

        private ReorderableList currentPlayerTargetConditionsList;
        private PlayerTargetBase currentPlayerTarget;
        private PlayerCondition currentPlayerTargetCondition;
        private ReorderableList currentTokenTargetConditionsList;
        private CardTargetBase currentTokenTarget;
        private CardCondition currentTokenTargetCondition;

        private ReorderableList currentCardTargetConditionsList;
        private CardTargetBase currentCardTarget;
        private CardCondition currentCardTargetCondition;

        private List<Type> triggerTypes;
        private List<string> triggerTypeNames;
        private List<Type> effectTypes;
        private List<string> effectTypeNames;
        private List<Type> humanTargetTypes;
        private List<string> humanTargetTypeNames;
        private List<Type> TokenTargetTypes;
        private List<string> TokenTargetTypeNames;


        /// <summary>
        /// コンストラクタ
        /// エディタのCard setsの項目(紅魔館等)を選択したら呼び出される
        /// </summary>
        /// <param name="config"></param>
        public TokenEditor(GameConfiguration config) : base(config)
        {
            TokenSetsList = EditorUtils.SetupReorderableList("陣営", gameConfig.tokenSets, ref currentTokenSet, (rect, x) =>
            {
                EditorGUI.LabelField(new Rect(rect.x, rect.y, 200, EditorGUIUtility.singleLineHeight), x.name);
            },
            (x) =>
            {
                currentTokenSet = x;
                currentToken = null;
                currentTokenCost = null;
                currentTokenKeyword = null;
                currentTokenAbility = null;
                CreateCurrentTokenSetTokensList();
            },
            () =>
            {
                gameConfig.tokenSets.Add(new CardSet());
            },
            (x) =>
            {
                currentTokenSet = null;
                currentToken = null;
                currentTokenCost = null;
                currentTokenKeyword = null;
                currentTokenAbility = null;
            });
        }
        /// <summary>
        /// トークンの一覧表示
        /// </summary>
        private void CreateCurrentTokenSetTokensList()
        {
            currentTokenList = EditorUtils.SetupReorderableList("トークン", currentTokenSet.cards, ref currentToken, (rect, x) =>
            {
                EditorGUI.LabelField(new Rect(rect.x, rect.y, 200, EditorGUIUtility.singleLineHeight), x.name);
            },
            (x) =>
            {
                currentToken = x;
                currentTokenCost = null;
                currentTokenKeyword = null;
                currentTokenAbility = null;
                CreateCurrentTokenCostsList();
                CreateCurrentTokenKeywordsList();
                CreateCurrentTokenAbilitiesList();
            },
            () =>
            {
                //+を押された時、カードタイプ(ミニオンかスペルか)の中から追加するタイプを読み込む
                var menu = new GenericMenu();
                foreach (var tokenType in gameConfig.cardTypes)
                {
                    menu.AddItem(new GUIContent(tokenType.name), false, CreateTokenCallback, tokenType);
                }
                menu.ShowAsContext();
            },
            (x) =>
            {
                currentToken = null;
                currentTokenCost = null;
                currentTokenKeyword = null;
                currentTokenAbility = null;
            });
        }
        /// <summary>
        /// コスト表示
        /// </summary>
        private void CreateCurrentTokenCostsList()
        {
            currentTokenCostsList = EditorUtils.SetupReorderableList("Costs", currentToken.costs, ref currentTokenCost, (rect, x) =>
            {
                EditorGUI.LabelField(new Rect(rect.x, rect.y, 200, EditorGUIUtility.singleLineHeight), x.GetReadableString(gameConfig));
            },
            (x) =>
            {
                currentTokenCost = x;
            },
            () =>
            {
                var menu = new GenericMenu();
                var costTypes = AppDomain.CurrentDomain.GetAllDerivedTypes(typeof(Cost));
                foreach (var type in costTypes)
                {
                    menu.AddItem(new GUIContent(StringUtils.DisplayCamelCaseString(type.Name)), false, CreateTokenCostCallback, type);
                }
                menu.ShowAsContext();
            },
            (x) =>
            {
                currentTokenCost = null;
            });
        }
        /// <summary>
        /// キーワード表示
        /// </summary>
        private void CreateCurrentTokenKeywordsList()
        {
            currentTokenKeywordsList = EditorUtils.SetupReorderableList("Keywords", currentToken.keywords, ref currentTokenKeyword, (rect, x) =>
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
                currentTokenKeyword = x;
            },
            () =>
            {
                var menu = new GenericMenu();
                for (var i = 0; i < gameConfig.keywords.Count; i++)
                {
                    menu.AddItem(new GUIContent(gameConfig.keywords[i].name), false, CreateTokenKeywordCallback, i);
                }
                menu.ShowAsContext();
            },
            (x) =>
            {
                currentTokenKeyword = null;
            });
        }
        /// <summary>
        /// 能力表示
        /// </summary>
        private void CreateCurrentTokenAbilitiesList()
        {
            
            currentTokenAbilitiesList = EditorUtils.SetupReorderableList("Abilities", currentToken.abilities, ref currentTokenAbility, (rect, x) =>
            {
                EditorGUI.LabelField(new Rect(rect.x, rect.y, 200, EditorGUIUtility.singleLineHeight), x.name);
            },
            (x) =>
            {
                currentTokenAbility = x;
                currentPlayerTargetConditionsList = null;
                currentPlayerTarget = null;
                currentPlayerTargetCondition = null;
                currentTokenTargetConditionsList = null;
                currentTokenTarget = null;
                currentTokenTargetCondition = null;
                if (currentTokenAbility is ActivatedAbility)
                {
                    CreateCurrentEffectCostsList();
                }
            },
            () =>
            {
                //ユーザの選択によってアビリティ表示の準備をする
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent("Triggered ability"), false, CreateTokenAbilityCallback, 0);
                menu.AddItem(new GUIContent("Activated ability"), false, CreateTokenAbilityCallback, 1);
                menu.ShowAsContext();
            },
            (x) =>
            {
                currentTokenAbility = null;
                currentPlayerTargetConditionsList = null;
                currentPlayerTarget = null;
                currentPlayerTargetCondition = null;
                currentTokenTargetConditionsList = null;
                currentTokenTarget = null;
                currentTokenTargetCondition = null;
            });
            
        }
        /// <summary>
        /// 能力表示内の、ActivatedAbilityを選択した時に表示するコスト
        /// 自傷シナジーなんかもここ
        /// </summary>
        private void CreateCurrentEffectCostsList()
        {
            currentEffectCostsList = EditorUtils.SetupReorderableList("Costs", (currentTokenAbility as ActivatedAbility).costs, ref currentEffectCost, (rect, x) =>
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
        /// <summary>
        /// 能力内の項目表示
        /// 行動を起こすプレイヤーを指定する
        /// </summary>
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
        /// <summary>
        /// 能力表示内の、ActivatedAbilityを選択した時に表示する
        /// 能力発動の条件を表示する
        /// </summary>
        private void CreateCurrentTokenTargetConditionsList()
        {
            

           
            currentTokenTargetConditionsList = EditorUtils.SetupReorderableList("Target Token conditions", currentTokenTarget.conditions, ref currentTokenTargetCondition, (rect, x) =>
            {
                EditorGUI.LabelField(new Rect(rect.x, rect.y, 200, EditorGUIUtility.singleLineHeight), x.GetReadableString(gameConfig));
            },
            (x) =>
            {
                currentTokenTargetCondition = x;
            },
            () =>
            {
                var menu = new GenericMenu();
                var conditionTypes = AppDomain.CurrentDomain.GetAllDerivedTypes(typeof(CardCondition));
                foreach (var type in conditionTypes)
                {
                    menu.AddItem(new GUIContent(StringUtils.DisplayCamelCaseString(type.Name)), false, CreateTokenTargetConditionCallback, type);
                }
                menu.ShowAsContext();
            },
            (x) =>
            {
                currentTokenTargetCondition = null;
            });
           
        }
        /// <summary>
        /// tokenリストにコストを追加する
        /// </summary>
        /// <param name="obj"></param>
        private void CreateTokenCostCallback(object obj)
        {
            var cost = Activator.CreateInstance((Type)obj);
            currentToken.costs.Add(cost as Cost);
        }
        /// <summary>
        /// tokenリストにキーワードを追加する
        /// </summary>
        /// <param name="obj"></param>
        private void CreateTokenKeywordCallback(object obj)
        {
            var keyword = new RuntimeKeyword();
            keyword.keywordId = (int)obj;
            currentToken.keywords.Add(keyword);
        }
        /// <summary>
        /// tokenリストに能力を追加する
        /// </summary>
        /// <param name="obj"></param>
        private void CreateTokenAbilityCallback(object obj)
        {
            Ability ability = null;
            switch ((int)obj)
            {
                case 0:
                    // Ability.typeに0を代入
                    ability = new TriggeredAbility();
                    break;

                case 1:
                    // Ability.typeに1を代入
                    ability = new ActivatedAbility();
                    break;
            }

            currentToken.abilities.Add(ability);

//            Debug.Log(currentToken.abilities[2]);
        }
        /// <summary>
        /// エディタの「カード設定」の頁で設定する値をCardクラスのリストに保存する。
        /// 新規にカードが作られる際呼ばれるコールバック関数
        /// ※コールバック関数は、非同期処理(この場合、新規カード作成)が完了した時に呼ばれる関数
        /// </summary>
        /// <param name="obj":cardTypeクラス></param>
        private void CreateTokenCallback(object obj)
        {
            var Token = new Card();
            var TokenType = obj as CardType;
            Token.cardTypeId = TokenType.id;
            if (TokenType != null)
            {
                foreach (var property in TokenType.properties)
                {
                    if (property is IntProperty)
                    {
                        var propertyCopy = new IntProperty();
                        propertyCopy.name = property.name;
                        propertyCopy.value = (property as IntProperty).value;
                        Token.properties.Add(propertyCopy);
                    }
                    else if (property is StringProperty)
                    {
                        var propertyCopy = new StringProperty();
                        propertyCopy.name = property.name;
                        propertyCopy.value = (property as StringProperty).value;
                        Token.properties.Add(propertyCopy);
                    }
                }

                foreach (var stat in TokenType.stats)
                {
                    var statCopy = new Stat();
                    statCopy.statId = stat.id;
                    statCopy.name = stat.name;
                    statCopy.baseValue = stat.baseValue;
                    statCopy.originalValue = stat.originalValue;
                    statCopy.minValue = stat.minValue;
                    statCopy.maxValue = stat.maxValue;
                    Token.stats.Add(statCopy);
                }
            }
            currentTokenSet.cards.Add(Token);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        private void CreateEffectCostCallback(object obj)
        {
            var cost = Activator.CreateInstance((Type)obj);
            (currentTokenAbility as ActivatedAbility).costs.Add(cost as Cost);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        private void CreatePlayerTargetConditionCallback(object obj)
        {
            var condition = Activator.CreateInstance((Type)obj);
            currentPlayerTarget.conditions.Add(condition as PlayerCondition);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        private void CreateTokenTargetConditionCallback(object obj)
        {
            var condition = Activator.CreateInstance((Type)obj);
            currentTokenTarget.conditions.Add(condition as CardCondition);
        }
        /// <summary>
        /// /タブ選択された時の挙動？
        /// </summary>
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

            TokenTargetTypes = new List<Type>(AppDomain.CurrentDomain.GetAllDerivedTypes(typeof(CardTargetBase)));
            TokenTargetTypes.RemoveAll(x => x.IsAbstract);
            TokenTargetTypeNames = new List<string>(TokenTargetTypes.Count);
            foreach (var type in TokenTargetTypes)
            {
                TokenTargetTypeNames.Add(StringUtils.DisplayCamelCaseString(type.Name));
            }
        }
        /// <summary>
        /// エディタのカード設定画面描画処理
        /// </summary>
        public override void Draw()
        {
            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical(GUILayout.MaxWidth(250));
            if (TokenSetsList != null)
            {
                TokenSetsList.DoLayoutList();
            }
            GUILayout.EndVertical();

            if (currentTokenSet != null)
            {
                DrawTokenSet(currentTokenSet);
            }

            GUILayout.EndHorizontal();
        }
        /// <summary>
        /// 陣営一覧描画処理
        /// </summary>
        /// <param name="set"></param>
        private void DrawTokenSet(CardSet set)
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
            if (currentTokenList != null)
            {
                currentTokenList.DoLayoutList();
            }
            GUILayout.EndVertical();

            if (currentToken != null)
            {
                DrawToken(currentToken);
            }

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            EditorGUIUtility.labelWidth = oldLabelWidth;
        }
        /// <summary>
        /// トークンの一覧描画処理
        /// </summary>
        /// <param name="Token"></param>
        private void DrawToken(Card Token)
        {
            var oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 100;

            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Name");
            Token.name = EditorGUILayout.TextField(Token.name, GUILayout.MaxWidth(EditorConfig.LargeTextFieldWidth));
            GUILayout.EndHorizontal();

            foreach (var stat in Token.stats)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(stat.name);
                stat.baseValue = EditorGUILayout.IntField(stat.baseValue, GUILayout.MaxWidth(EditorConfig.RegularIntFieldWidth));
                stat.originalValue = stat.baseValue;
                GUILayout.EndHorizontal();
            }

            foreach (var property in Token.properties)
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
            if (currentTokenCostsList != null)
            {
                currentTokenCostsList.DoLayoutList();
            }
            GUILayout.EndVertical();

            if (currentTokenCost != null)
            {
                DrawCost(currentTokenCost);
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical(GUILayout.MaxWidth(250));
            if (currentTokenKeywordsList != null)
            {
                currentTokenKeywordsList.DoLayoutList();
            }
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical(GUILayout.MaxWidth(250));
            if (currentTokenAbilitiesList != null)
            {
                currentTokenAbilitiesList.DoLayoutList();
            }
            GUILayout.EndVertical();

            if (currentTokenAbility != null)
            {
                DrawAbility(currentTokenAbility);
            }

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            EditorGUIUtility.labelWidth = oldLabelWidth;
        }
        /// <summary>
        /// カード内のコスト描画処理
        /// </summary>
        /// <param name="cost"></param>
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
        /// <summary>
        /// カード内の能力描画処理
        /// </summary>
        /// <param name="ability"></param>
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
                // 参照渡しをする場合には、引数にref修飾子をつけます。
                // http://albatrus.com/main/unity/7611
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="effect"></param>
        /// <param name="target"></param>
        private void DrawEffect(ref Effect effect, ref Target target)
        {
            /*
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
                        targetTypes = TokenTargetTypes;
                        targetTypeNames = TokenTargetTypeNames;
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
                        //Targetの値をセットする？
                        else if (target is CardTargetBase && currentTokenTargetConditionsList == null)
                        {
                            currentTokenTarget = target as CardTargetBase;
                            CreateCurrentTokenTargetConditionsList();
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
                    else if (currentTokenTarget != null)
                    {
                        GUILayout.BeginHorizontal();

                        GUILayout.BeginVertical(GUILayout.MaxWidth(250));
                        if (currentTokenTargetConditionsList != null)
                        {
                            currentTokenTargetConditionsList.DoLayoutList();
                        }
                        GUILayout.EndVertical();

                        if (currentTokenTargetCondition != null)
                        {
                            DrawTargetCondition(currentTokenTargetCondition);
                        }

                        GUILayout.EndHorizontal();
                    }

                }
            }
            */
        }
        /// <summary>
        /// TargetConditionの描画処理
        /// </summary>
        /// <param name="condition"></param>
        private void DrawTargetCondition(Condition condition)
        {
            var oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = EditorConfig.RegularLabelWidth;

            GUILayout.BeginVertical();
            //BindingFlagsはint型を返すEnum
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
