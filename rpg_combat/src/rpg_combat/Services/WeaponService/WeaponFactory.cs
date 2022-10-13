using rpg_combat.Dtos.Weapon;
using rpg_combat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rpg_combat.Services.WeaponService
{
    public static class WeaponFactory
    {
        private static Dictionary<CharacterClass, List<string>> weaponNames = new Dictionary<CharacterClass, List<string>>
        {
            {
                CharacterClass.Cleric, new List<string>
                {
                    "Preyer Wheel",
                    "Divine Baton",
                    "Mallet",
                    "Staff",
                    "Rod",
                    "Ancient Tome"
                }
            },
            {
                CharacterClass.Wizard, new List<string>
                {
                    "Staff",
                    "Rod",
                    "Wand",
                    "Cane",
                    "knife"
                }
            },
            {
                CharacterClass.Fighter, new List<string>
                {
                    "Short Sword",
                    "Long Sword",
                    "Bastard Sword",
                    "Spear",
                    "Hand-Axe",
                    "Battle-Axe"
                }
            }

        };
        private static Dictionary<string, int> adjectivesAndDamage = new Dictionary<string, int>
        {
            {"Heavenly constructed", 10 },
            {"Heavy built", 11 },
            {"Crude made", 4 },
            {"Weird", 3 },
            {"Prime", 6 },
            {"Well-crafted", 5 },
            {"Bruised", 3 },
            {"Peculiar", 4 },
            {"Strange", 4 },
            {"Curious", 4 },
            {"Awkward", 4 },
            {"Odd", 4 },
            {"Uncanny", 5 },
        };

        public static AddWeaponDto GetWeaponForClass(CharacterClass @class, int characterId)
        {
            var weapon = GetWeaponForClass(@class);
            weapon.CharacterId = characterId;
            return weapon;
        }
        public static AddWeaponDto GetWeaponForClass(CharacterClass @class)
        {
            var random = new Random();
            var names = weaponNames[@class];
            var adjectiveKeyPair = adjectivesAndDamage.ElementAt(random.Next(adjectivesAndDamage.Count));
            return new AddWeaponDto
            {
                Name = $"{adjectiveKeyPair.Key} {names[random.Next(names.Count)]}",
                Damage = GetDamageForClass(@class) + adjectiveKeyPair.Value
            };
        }

        private static int GetDamageForClass(CharacterClass @class)
        {
            int damage = 10;
            var random = new Random();
            switch(@class)
            {
                case CharacterClass.Cleric:
                    damage = random.Next(13);
                    break;
                case CharacterClass.Wizard:
                    damage = random.Next(9);
                    break;
                case CharacterClass.Fighter:
                    damage = random.Next(18);
                    break;
            }
            return damage;
        }
    }
}
