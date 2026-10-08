using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BaseExpedition
{
    public sealed class BaseExpeditionHud : MonoBehaviour
    {
        GameBootstrap game;
        UnityEngine.UI.Text vitalText;
        UnityEngine.UI.Text statusText;
        UnityEngine.UI.Text bossText;
        UnityEngine.UI.Text baseHintText;
        GameObject basePanel;
        GameObject bossPanel;

        static Sprite whiteSprite;

        public static BaseExpeditionHud Create(GameBootstrap game)
        {
            var root = new GameObject("GameHud", typeof(Canvas), typeof(UnityEngine.UI.CanvasScaler), typeof(UnityEngine.UI.GraphicRaycaster));
            var canvas = root.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = root.GetComponent<UnityEngine.UI.CanvasScaler>();
            scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = .5f;
            if (FindFirstObjectByType<EventSystem>() == null)
            {
                var eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
                DontDestroyOnLoad(eventSystem);
            }
            var hud = root.AddComponent<BaseExpeditionHud>();
            hud.game = game;
            hud.Build();
            return hud;
        }

        void Build()
        {
            var root = transform as RectTransform;
            var vitalPanel = Panel("VitalPanel", root, new Color(.025f, .05f, .07f, .88f));
            SetAnchor(vitalPanel, new Vector2(0, 1), new Vector2(0, 1), new Vector2(26, -26), new Vector2(540, 148), new Vector2(0, 1));
            vitalText = Text("VitalText", vitalPanel, 26, TextAnchor.UpperLeft, new Color(.94f, .97f, .92f));
            Stretch(vitalText.rectTransform, 20, 16, 20, 16);

            var statusPanel = Panel("StatusPanel", root, new Color(.025f, .05f, .07f, .86f));
            SetAnchor(statusPanel, new Vector2(.5f, 1), new Vector2(.5f, 1), new Vector2(0, -28), new Vector2(760, 54), new Vector2(.5f, 1));
            statusText = Text("StatusText", statusPanel, 24, TextAnchor.MiddleCenter, new Color(1f, .87f, .45f));
            Stretch(statusText.rectTransform, 12, 6, 12, 6);

            bossPanel = Panel("BossPanel", root, new Color(.26f, .04f, .04f, .92f)).gameObject;
            var bossRect = bossPanel.transform as RectTransform;
            SetAnchor(bossRect, new Vector2(.5f, 1), new Vector2(.5f, 1), new Vector2(0, -96), new Vector2(560, 38), new Vector2(.5f, 1));
            bossText = Text("BossText", bossRect, 21, TextAnchor.MiddleCenter, Color.white);
            Stretch(bossText.rectTransform, 8, 2, 8, 2);

            basePanel = Panel("BasePanel", root, new Color(.025f, .05f, .07f, .92f)).gameObject;
            var baseRect = basePanel.transform as RectTransform;
            SetAnchor(baseRect, new Vector2(1, 1), new Vector2(1, 1), new Vector2(-26, -26), new Vector2(370, 432), new Vector2(1, 1));
            BuildBasePanel(baseRect);
            BuildMobileControls(root);
        }

        void BuildBasePanel(RectTransform panel)
        {
            var title = Text("Title", panel, 26, TextAnchor.MiddleLeft, new Color(1f, .87f, .45f));
            SetAnchor(title.rectTransform, new Vector2(0, 1), new Vector2(1, 1), new Vector2(18, -14), new Vector2(-18, 34), new Vector2(.5f, 1));
            title.text = "基地設施";
            baseHintText = Text("BaseHint", panel, 18, TextAnchor.UpperLeft, new Color(.86f, .91f, .87f));
            SetAnchor(baseHintText.rectTransform, new Vector2(0, 0), new Vector2(1, 0), new Vector2(18, 14), new Vector2(-18, 80), new Vector2(.5f, 0));

            var actions = new (string label, Action action)[]
            {
                ("存入資源", game.DepositResources), ("升級背包", game.UpgradeBackpack),
                ("製作彈弓", () => game.CraftWeapon(WeaponId.Slingshot)), ("製作長槍", () => game.CraftWeapon(WeaponId.Spear)),
                ("製作弓箭", () => game.CraftWeapon(WeaponId.Bow)), ("升級武器", game.UpgradeWeapon),
                ("升級護甲", game.UpgradeArmor), ("升級靴子", game.UpgradeBoots),
                ("火焰打擊", () => game.AddModule("fireStrike", new() { { "wood", 5 }, { "magicShard", 5 } })),
                ("多重散射", () => game.AddModule("multishot", new() { { "stone", 5 }, { "magicShard", 5 } })),
                ("衝擊波", () => game.AddModule("shockwave", new() { { "wood", 5 }, { "magicShard", 5 } })),
                ("穿透", () => game.AddModule("piercing", new() { { "stone", 5 }, { "magicShard", 5 } }))
            };
            for (var i = 0; i < actions.Length; i++)
            {
                var column = i % 2;
                var row = i / 2;
                var button = Button(actions[i].label, panel, actions[i].action);
                SetAnchor(button.transform as RectTransform, new Vector2(0, 1), new Vector2(0, 1), new Vector2(18 + column * 170, -64 - row * 48), new Vector2(154, 38), new Vector2(0, 1));
            }
        }

        void BuildMobileControls(RectTransform root)
        {
            var directions = new (string label, Vector2 direction, Vector2 position)[]
            {
                ("▲", Vector2.up, new Vector2(142, 222)), ("◀", Vector2.left, new Vector2(58, 138)),
                ("▶", Vector2.right, new Vector2(226, 138)), ("▼", Vector2.down, new Vector2(142, 54))
            };
            foreach (var input in directions)
            {
                var button = Button(input.label, root, null);
                SetAnchor(button.transform as RectTransform, Vector2.zero, Vector2.zero, input.position, new Vector2(76, 76), Vector2.zero);
                var mover = button.gameObject.AddComponent<MobileMoveButton>();
                mover.Setup(game, input.direction);
                button.gameObject.SetActive(Application.isMobilePlatform);
            }
            var dash = Button("衝刺", root, game.TryDash);
            SetAnchor(dash.transform as RectTransform, Vector2.one, Vector2.one, new Vector2(-50, 56), new Vector2(128, 70), Vector2.one);
            dash.gameObject.SetActive(Application.isMobilePlatform);
            var interact = Button("互動", root, game.Interact);
            SetAnchor(interact.transform as RectTransform, Vector2.one, Vector2.one, new Vector2(-50, 142), new Vector2(128, 70), Vector2.one);
            interact.gameObject.SetActive(Application.isMobilePlatform);
        }

        public void Refresh()
        {
            if (game == null || game.Player == null) return;
            var player = game.Player;
            vitalText.text = $"基地探索\n生命 {Mathf.CeilToInt(player.Health)} / {player.MaxHealth}   護甲 {player.Armor}\n武器 {player.Definition.Label} Lv.{player.WeaponLevel}   區域 {game.CurrentRegion}\n背包 {player.Inventory.UsedSlots}/{player.Inventory.SlotCount} 格   已解鎖 {game.UnlockedRegion}\nWASD 移動  Shift 衝刺  F 互動  1–4 武器";
            statusText.text = game.Message;
            var boss = game.Boss;
            bossPanel.SetActive(boss != null && boss.Alive);
            if (boss != null) bossText.text = $"外圈守衛者  {Mathf.CeilToInt(boss.Health)} / {boss.Definition.MaxHealth}";
            basePanel.SetActive(game.AtBase);
            baseHintText.text = "F：依序建造倉庫、工作台、注魔台\n模組依武器相容性自動作用。\n武器 1–4 切換。";
        }

        RectTransform Panel(string name, RectTransform parent, Color color)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(UnityEngine.UI.Image));
            go.transform.SetParent(parent, false);
            var image = go.GetComponent<UnityEngine.UI.Image>();
            image.sprite = WhiteSprite;
            image.color = color;
            image.raycastTarget = false;
            return go.transform as RectTransform;
        }

        UnityEngine.UI.Text Text(string name, RectTransform parent, int size, TextAnchor alignment, Color color)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(UnityEngine.UI.Text));
            go.transform.SetParent(parent, false);
            var text = go.GetComponent<UnityEngine.UI.Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = size;
            text.alignment = alignment;
            text.color = color;
            text.supportRichText = false;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.raycastTarget = false;
            return text;
        }

        UnityEngine.UI.Button Button(string label, RectTransform parent, Action action)
        {
            var go = new GameObject(label, typeof(RectTransform), typeof(UnityEngine.UI.Image), typeof(UnityEngine.UI.Button));
            go.transform.SetParent(parent, false);
            var image = go.GetComponent<UnityEngine.UI.Image>();
            image.sprite = WhiteSprite;
            image.color = new Color(.16f, .34f, .30f, .96f);
            var button = go.GetComponent<UnityEngine.UI.Button>();
            var colors = button.colors;
            colors.highlightedColor = new Color(.28f, .52f, .43f, 1);
            colors.pressedColor = new Color(.08f, .2f, .18f, 1);
            button.colors = colors;
            if (action != null) button.onClick.AddListener(() => action());
            var text = Text("Label", go.transform as RectTransform, 19, TextAnchor.MiddleCenter, Color.white);
            text.text = label;
            Stretch(text.rectTransform, 4, 2, 4, 2);
            return button;
        }

        static void SetAnchor(RectTransform rect, Vector2 min, Vector2 max, Vector2 position, Vector2 size, Vector2 pivot)
        {
            rect.anchorMin = min; rect.anchorMax = max; rect.pivot = pivot; rect.anchoredPosition = position; rect.sizeDelta = size;
        }

        static void Stretch(RectTransform rect, float left, float bottom, float right, float top)
        {
            rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one; rect.offsetMin = new Vector2(left, bottom); rect.offsetMax = new Vector2(-right, -top);
        }

        static Sprite WhiteSprite
        {
            get
            {
                if (whiteSprite != null) return whiteSprite;
                var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                texture.SetPixels(new[] { Color.white, Color.white, Color.white, Color.white });
                texture.Apply();
                whiteSprite = Sprite.Create(texture, new Rect(0, 0, 2, 2), new Vector2(.5f, .5f));
                return whiteSprite;
            }
        }
    }

    public sealed class MobileMoveButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        GameBootstrap game;
        Vector2 direction;
        public void Setup(GameBootstrap source, Vector2 value) { game = source; direction = value; }
        public void OnPointerDown(PointerEventData eventData) { game.SetTouchMove(direction); }
        public void OnPointerUp(PointerEventData eventData) { game.SetTouchMove(Vector2.zero); }
    }
}
