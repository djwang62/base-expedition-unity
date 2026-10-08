using System.Collections.Generic;
using UnityEngine;

namespace BaseExpedition
{
    public static class ArtSpriteLibrary
    {
        static readonly Dictionary<string, Sprite> Cache = new();

        public static Sprite Get(string path, bool crop = true)
        {
            if (string.IsNullOrEmpty(path)) return null;
            var cacheKey = path + (crop ? "#crop" : "#full");
            if (Cache.TryGetValue(cacheKey, out var sprite)) return sprite;
            var texture = Resources.Load<Texture2D>("Art/" + path);
            if (texture == null) return null;
            texture.filterMode = FilterMode.Point;
            var rect = crop ? OpaqueBounds(texture) : new Rect(0, 0, texture.width, texture.height);
            sprite = Sprite.Create(texture, rect, new Vector2(.5f, .5f), 100f);
            Cache[cacheKey] = sprite;
            return sprite;
        }

        static Rect OpaqueBounds(Texture2D texture)
        {
            try
            {
                var pixels = texture.GetPixels32(); var minX = texture.width; var minY = texture.height; var maxX = -1; var maxY = -1;
                for (var y = 0; y < texture.height; y++) for (var x = 0; x < texture.width; x++) if (pixels[y * texture.width + x].a > 8) { minX = Mathf.Min(minX, x); minY = Mathf.Min(minY, y); maxX = Mathf.Max(maxX, x); maxY = Mathf.Max(maxY, y); }
                if (maxX >= minX && maxY >= minY) return new Rect(minX, minY, maxX - minX + 1, maxY - minY + 1);
            }
            catch (UnityException) { }
            return new Rect(0, 0, texture.width, texture.height);
        }

        public static string Character(EnemyKind kind) => kind switch
        {
            EnemyKind.TreeSprite => "characters/tree-sprite/runtime/character_tree-sprite_move_01",
            EnemyKind.Seedling => "characters/seedling/runtime/character_seedling_move_01",
            EnemyKind.ForestGuardian => "characters/forest-guardian/runtime/character_forest-guardian_move_01",
            EnemyKind.StoneGolem => "characters/stone-golem/runtime/character_stone-golem_move_01",
            EnemyKind.RockThrower => "characters/rock-thrower/runtime/character_rock-thrower_move_01",
            EnemyKind.MountainGuardian => "characters/mountain-guardian/runtime/character_mountain-guardian_move_01",
            EnemyKind.IronBrute => "characters/iron-brute/runtime/character_iron-brute_move_01",
            EnemyKind.OreShooter => "characters/ore-shooter/runtime/character_ore-shooter_move_01",
            EnemyKind.MineGuardian => "characters/mine-guardian/runtime/character_mine-guardian_move_01",
            EnemyKind.OuterBoss => "characters/outer-boss/runtime/character_outer-boss_move_01",
            _ => null
        };

        public static string Drop(string id) => id switch
        {
            "wood" => "drops/wood", "stone" => "drops/stone", "metal" => "drops/metal", "magicShard" => "drops/magic-shard",
            "forestCore" => "drops/forest-core", "mountainCore" => "drops/mountain-core", "mineCore" => "drops/mine-core", _ => null
        };
    }
}
