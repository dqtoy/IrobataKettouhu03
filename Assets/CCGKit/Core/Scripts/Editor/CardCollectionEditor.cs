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
    /// CCG Kit editor's 'Card collection' tab.
    /// CCGキットエディタの[カードコレクション]タブ。
    /// </summary>
    public class CardCollectionEditor : EditorTab
    {
        //陣営毎に分けたカードの一覧
        private ReorderableList cardSetsList;
        private CardSet currentCardSet;

        //トークンの一覧
        private TokenSet currentTokenSet;

        //カード
        private ReorderableList currentCardList;
        private Card currentCard;

        private Token currentToken;

        //カードのコスト
        private ReorderableList currentCardCostsList;
        private Cost currentCardCost;

        private Cost currentTokenCost;

        //カードのキーワード
        private ReorderableList currentCardKeywordsList;
        private RuntimeKeyword currentCardKeyword;

        private RuntimeKeyword currentTokenKeyword;

        //カードのアビリティ
        private ReorderableList currentCardAbilitiesList;
        private Ability currentCardAbility;

        private Ability currentTokenAbility;

        //効果で使用するコスト
        private ReorderableList currentEffectCostsList;
        private Cost currentEffectCost;

        private Cost currentTokenEffectCost;

        private ReorderableList currentPlayerTargetConditionsList;
        private PlayerTargetBase currentPlayerTarget;
        private PlayerCondition currentPlayerTargetCondition;
        private ReorderableList currentCardTargetConditionsList;
        private CardTargetBase currentCardTarget;
        private CardCondition currentCardTargetCondition;

        private List<Type> triggerTypes;
        private List<string> triggerTypeNames;
        private List<Type> effectTypes;
        private List<string> effectTypeNames;
        private List<Type> humanTargetTypes;
        private List<string> humanTargetTypeNames;
        private List<Type> cardTargetTypes;
        private List<string> cardTargetTypeNames;

        /// <summary>
        /// コンストラクタ
        /// エディタのCard setsの項目(紅魔館等)を選択したら呼び出される
        /// </summary>
        /// <param name="config"></param>
        public CardCollectionEditor(GameConfiguration config) : base(config)
        {
            cardSetsList = EditorUtils.SetupReorderableList("card sets", gameConfig.cardSets, ref currentCardSet, (rect, x) =>
            {
                EditorGUI.LabelField(new Rect(rect.x, rect.y, 200, EditorGUIUtility.singleLineHeight), x.name);
            },
            (x) =>
            {
                //選択されたCardSstが入る
                currentCardSet = x;

//                Debug.Log(currentCardSet.cards);
//                Debug.Log(currentCardSet.Id);
//                Debug.Log(currentCardSet.name);

                currentCard = null;
                currentCardCost = null;
                currentCardKeyword = null;
                currentCardAbility = null;
                CreateCurrentCardSetCardsList();
            },
            () =>
            {
                gameConfig.cardSets.Add(new CardSet());

//                Debug.Log(gameConfig.cardSets);
            },
            (x) =>
            {
                currentCardSet = null;
                currentCard = null;
                currentCardCost = null;
                currentCardKeyword = null;
                currentCardAbility = null;
            });
        }

        /// <summary>
        /// カードの一覧表示
        /// </summary>
        private void CreateCurrentCardSetCardsList()
        {
            currentCardList = EditorUtils.SetupReorderableList("cards", currentCardSet.cards, ref currentCard, (rect, x) =>
            {
                EditorGUI.LabelField(new Rect(rect.x, rect.y, 200, EditorGUIUtility.singleLineHeight), x.name);
            },
            (x) =>
            {
                //currentCardは選択したカード
                currentCard = x;
                currentCardCost = null;
                currentCardKeyword = null;
                currentCardAbility = null;
                CreateCurrentCardCostsList();
                CreateCurrentCardKeywordsList();
                CreateCurrentCardAbilitiesList();
            },
            () =>
            {
                var menu = new GenericMenu();
                foreach (var cardType in gameConfig.cardTypes)
                {
                    menu.AddItem(new GUIContent(cardType.name), false, CreateCardCallback, cardType);
                }
                menu.ShowAsContext();
            },
            (x) =>
            {
                currentCard = null;
                currentCardCost = null;
                currentCardKeyword = null;
                currentCardAbility = null;
            });
        }

        /// <summary>
        /// コスト表示
        /// </summary>
        private void CreateCurrentCardCostsList()
        {
            currentCardCostsList = EditorUtils.SetupReorderableList("Costs", currentCard.costs, ref currentCardCost, (rect, x) =>
            {
                EditorGUI.LabelField(new Rect(rect.x, rect.y, 200, EditorGUIUtility.singleLineHeight), x.GetReadableString(gameConfig));
            },
            (x) =>
            {
                currentCardCost = x;
            },
            () =>
            {
                var menu = new GenericMenu();
                var costTypes = AppDomain.CurrentDomain.GetAllDerivedTypes(typeof(Cost));
                foreach (var type in costTypes)
                {
                    menu.AddItem(new GUIContent(StringUtils.DisplayCamelCaseString(type.Name)), false, CreateCardCostCallback, type);
                }
                menu.ShowAsContext();
            },
            (x) =>
            {
                currentCardCost = null;
            });
        }

        /// <summary>
        /// キーワード表示
        /// </summary>
        private void CreateCurrentCardKeywordsList()
        {
            currentCardKeywordsList = EditorUtils.SetupReorderableList("Keywords", currentCard.keywords, ref currentCardKeyword, (rect, x) =>
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
                currentCardKeyword = x;
            },
            () =>
            {
                var menu = new GenericMenu();
                for (var i = 0; i < gameConfig.keywords.Count; i++)
                {
                    menu.AddItem(new GUIContent(gameConfig.keywords[i].name), false, CreateCardKeywordCallback, i);
                }
                menu.ShowAsContext();
            },
            (x) =>
            {
                currentCardKeyword = null;
            });
        }

        /// <summary>
        /// 能力表示
        /// </summary>
        private void CreateCurrentCardAbilitiesList()
        {
            currentCardAbilitiesList = EditorUtils.SetupReorderableList("Abilities", currentCard.abilities, ref currentCardAbility, (rect, x) =>
            {
                EditorGUI.LabelField(new Rect(rect.x, rect.y, 200, EditorGUIUtility.singleLineHeight), x.name);
            },
            (x) =>
            {
                currentCardAbility = x;
                currentPlayerTargetConditionsList = null;
                currentPlayerTarget = null;
                currentPlayerTargetCondition = null;
                currentCardTargetConditionsList = null;
                currentCardTarget = null;
                currentCardTargetCondition = null;
                if (currentCardAbility is ActivatedAbility)
                {
                    CreateCurrentEffectCostsList();
                }
            },
            () =>
            {
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent("Triggered ability"), false, CreateCardAbilityCallback, 0);
                menu.AddItem(new GUIContent("Activated ability"), false, CreateCardAbilityCallback, 1);
                menu.ShowAsContext();
            },
            (x) =>
            {
                currentCardAbility = null;
                currentPlayerTargetConditionsList = null;
                currentPlayerTarget = null;
                currentPlayerTargetCondition = null;
                currentCardTargetConditionsList = null;
                currentCardTarget = null;
                currentCardTargetCondition = null;
            });
        }

        /// <summary>
        /// 能力表示内の、ActivatedAbilityを選択した時に表示するコスト
        /// 自傷シナジーなんかもここ
        /// </summary>
        private void CreateCurrentEffectCostsList()
        {
            currentEffectCostsList = EditorUtils.SetupReorderableList("Costs", (currentCardAbility as ActivatedAbility).costs, ref currentEffectCost, (rect, x) =>
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
        /// </summary>
        private void CreateCurrentCardTargetConditionsList()
        {
            currentCardTargetConditionsList = EditorUtils.SetupReorderableList("Target card conditions", currentCardTarget.conditions, ref currentCardTargetCondition, (rect, x) =>

           {
               EditorGUI.LabelField(new Rect(rect.x, rect.y, 200, EditorGUIUtility.singleLineHeight), x.GetReadableString(gameConfig));
           },
            (x) =>
            {
                currentCardTargetCondition = x;
            },
            () =>
            {
                var menu = new GenericMenu();
                var conditionTypes = AppDomain.CurrentDomain.GetAllDerivedTypes(typeof(CardCondition));
                foreach (var type in conditionTypes)
                {
                    menu.AddItem(new GUIContent(StringUtils.DisplayCamelCaseString(type.Name)), false, CreateCardTargetConditionCallback, type);
                }
                menu.ShowAsContext();
            },
            (x) =>
            {
                currentCardTargetCondition = null;
            });
        }

        /// <summary>
        /// Cardリストにコストを追加する
        /// </summary>
        /// <param name="obj"></param>
        private void CreateCardCostCallback(object obj)
        {
            var cost = Activator.CreateInstance((Type)obj);

            //            Debug.Log(currentCardSet.name);

            if (currentCardSet.name == "トークン")
            {
                currentToken.costs.Add(cost as Cost);
            }
            else {
                currentCard.costs.Add(cost as Cost);
            }


        }

        /// <summary>
        /// Cardリストにキーワードを追加する
        /// </summary>
        /// <param name="obj"></param>
        private void CreateCardKeywordCallback(object obj)
        {
            var keyword = new RuntimeKeyword();
            keyword.keywordId = (int)obj;
            currentCard.keywords.Add(keyword);
            
        }
        /// <summary>
        /// Cardリストに能力を追加する
        /// </summary>
        /// <param name="obj"></param>
        private void CreateCardAbilityCallback(object obj)
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
            if (currentCardSet.name == "トークン")
            {
                currentToken.abilities.Add(ability);
            }
            else
            {
                currentCard.abilities.Add(ability);
            }
        }




        /// <summary>
        /// エディタの「カード設定」の頁で設定する値をCardクラスのリストに保存する
        /// 新規にカードが作られる際呼ばれるコールバック関数
        /// ※コールバック関数は、非同期処理(この場合、新規カード作成)が完了した時に呼ばれる関数
        /// </summary>
        /// <param name="obj":cardTypeクラス></param>
        private void CreateCardCallback(object obj)
        {
            var card = new Card();
            var token = new Token();
            var cardType = obj as CardType;
            var tokenType = obj as CardType;
            //            var tokenType = obj as TokenType;
            //CardクラスのcardTypeIdにCardTypeクラス(ミニオンorスペル)のIDを代入
            //内部的には各スタッツ、最大コピー数、イラスト、テキスト、破壊の定義を含む
            card.cardTypeId = cardType.id;
            token.tokenTypeId = tokenType.id;

            //            token.tokenTypeId = tokenType.id;
            /*
                        if (tokenType != null && currentCardSet.name == "トークン")
                        {

                        }
            */



            if (cardType != null)
            {
                Debug.Log(currentCardSet.name);

                foreach (var property in cardType.properties)
                {
                    //スタッツや最大コピー数
                    if (property is IntProperty)
                    {
                        var propertyCopy = new IntProperty();
                        propertyCopy.name = property.name;
                        propertyCopy.value = (property as IntProperty).value;

                        if (currentCardSet.name == "トークン")
                        {
                            token.properties.Add(propertyCopy);
                        }
                        else
                        {
                            card.properties.Add(propertyCopy);
                        }
                         

                        
                    }
                    //イラストのファイル名やテキストなど
                    else if (property is StringProperty)
                    {
                        var propertyCopy = new StringProperty();
                        propertyCopy.name = property.name;
                        propertyCopy.value = (property as StringProperty).value;

                        if (currentCardSet.name == "トークン")
                        {
                            token.properties.Add(propertyCopy);
                        }
                        else
                        {
                            card.properties.Add(propertyCopy);
                        }

                    }
                }

                foreach (var stat in cardType.stats)
                {
                    var statCopy = new Stat();
                    //StatIDの0がアタック、1がタフネス
                    statCopy.statId = stat.id;
                    statCopy.name = stat.name;
                    statCopy.baseValue = stat.baseValue;
                    statCopy.originalValue = stat.originalValue;
                    statCopy.minValue = stat.minValue;
                    statCopy.maxValue = stat.maxValue;
                    if (currentCardSet.name == "トークン")
                    {
                        token.stats.Add(statCopy);
                    }
                    else
                    {
                        card.stats.Add(statCopy);
                    }

                }
            }
            //           Debug.Log(currentCardSet.name);

            /*
            if (currentCardSet.name=="トークン") {
                currentTokenSet.tokens.Add(card);
            }
            */
            if (currentCardSet.name == "トークン")
            {
                currentTokenSet.tokens.Add(token);
            }
            else
            {
                currentCardSet.cards.Add(card);
            }


        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        private void CreateEffectCostCallback(object obj)
        {
            var cost = Activator.CreateInstance((Type)obj);
            (currentCardAbility as ActivatedAbility).costs.Add(cost as Cost);
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
        private void CreateCardTargetConditionCallback(object obj)
        {
            var condition = Activator.CreateInstance((Type)obj);
            currentCardTarget.conditions.Add(condition as CardCondition);

            //            Debug.Log(currentCardTargetConditionsList);
//            Debug.Log(currentCardTarget);


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

            cardTargetTypes = new List<Type>(AppDomain.CurrentDomain.GetAllDerivedTypes(typeof(CardTargetBase)));
            cardTargetTypes.RemoveAll(x => x.IsAbstract);
            cardTargetTypeNames = new List<string>(cardTargetTypes.Count);
            foreach (var type in cardTargetTypes)
            {
                cardTargetTypeNames.Add(StringUtils.DisplayCamelCaseString(type.Name));
            }
        }

        /// <summary>
        /// エディタのカード設定画面描画処理
        /// </summary>
        public override void Draw()
        {
            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical(GUILayout.MaxWidth(250));
            if (cardSetsList != null)
            {
                cardSetsList.DoLayoutList();
            }
            GUILayout.EndVertical();

            if (currentCardSet != null)
            {
                DrawCardSet(currentCardSet);
            }

            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// 陣営一覧描画処理
        /// </summary>
        /// <param name="set"></param>
        private void DrawCardSet(CardSet set)
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
            if (currentCardList != null)
            {
                currentCardList.DoLayoutList();
            }
            GUILayout.EndVertical();

            if (currentCard != null)
            {
                DrawCard(currentCard);
            }

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            EditorGUIUtility.labelWidth = oldLabelWidth;
        }

        /// <summary>
        /// カード一覧描画処理
        /// </summary>
        /// <param name="card"></param>
        private void DrawCard(Card card)
        {
            var oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 100;

            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Name");
            card.name = EditorGUILayout.TextField(card.name, GUILayout.MaxWidth(EditorConfig.LargeTextFieldWidth));
            GUILayout.EndHorizontal();

            foreach (var stat in card.stats)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(stat.name);
                stat.baseValue = EditorGUILayout.IntField(stat.baseValue, GUILayout.MaxWidth(EditorConfig.RegularIntFieldWidth));
                stat.originalValue = stat.baseValue;
                GUILayout.EndHorizontal();
            }

            foreach (var property in card.properties)
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
            if (currentCardCostsList != null)
            {
                currentCardCostsList.DoLayoutList();
            }
            GUILayout.EndVertical();

            if (currentCardCost != null)
            {
                DrawCost(currentCardCost);
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical(GUILayout.MaxWidth(250));
            if (currentCardKeywordsList != null)
            {
                currentCardKeywordsList.DoLayoutList();
            }
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical(GUILayout.MaxWidth(250));
            if (currentCardAbilitiesList != null)
            {
                currentCardAbilitiesList.DoLayoutList();
            }
            GUILayout.EndVertical();

            if (currentCardAbility != null)
            {
                DrawAbility(currentCardAbility);
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
                        targetTypes = cardTargetTypes;
                        targetTypeNames = cardTargetTypeNames;
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
                    //既に指定してあるターゲットを表示
                    targetTypeId = EditorGUILayout.Popup(prevTargetTypeId, targetTypeNames.ToArray(), GUILayout.MaxWidth(EditorConfig.RegularComboBoxWidth));

//                    Debug.Log(targetTypeId);

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
                        else if (target is CardTargetBase && currentCardTargetConditionsList == null)
                        {
                            currentCardTarget = target as CardTargetBase;
                            CreateCurrentCardTargetConditionsList();
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
                    else if (currentCardTarget != null)
                    {
                        GUILayout.BeginHorizontal();

                        GUILayout.BeginVertical(GUILayout.MaxWidth(250));
                        if (currentCardTargetConditionsList != null)
                        {
                            currentCardTargetConditionsList.DoLayoutList();
                        }
                        GUILayout.EndVertical();

                        if (currentCardTargetCondition != null)
                        {
                            DrawTargetCondition(currentCardTargetCondition);
                        }

                        GUILayout.EndHorizontal();
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="condition"></param>
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
