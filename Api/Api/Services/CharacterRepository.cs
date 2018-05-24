using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Api.Models;

namespace Api.Services
{
    public class CharacterRepository
    {
        private const string CacheKey = "CharacterStore";

        public Character[] GetAllCharacters()
        {
            var context = HttpContext.Current;

            if (context != null)
            {
                return (Character[])context.Cache[CacheKey];
            }

            return new Character[]
            {
                new Character
                {
                    Id = 0,
                    Name = "Placeholder"
                }
            };
        }

        public CharacterRepository()
        {
            var context = HttpContext.Current;

            if (context != null)
            {
                if (context.Cache[CacheKey] == null)
                {
                    var characters = new Character[]
                    {
                        new Character
                        {
                            Id = 1,
                            Name = "Cecil Harvey",
                            Age = "20",
                            Gender = "Male",
                            Race = "Half-Lunarian",
                            Job = "Dark Knight/Paladin",
                            Height = "1.78m",
                            Weight = "58kg",
                            Origin = "Final Fantasy 4",
                            Description = "Cecil Harvey (セシル・ハーヴィ Seshiru Hāvi) is the main protagonist of Final Fantasy IV, who also appears in the sequel Final Fantasy IV: The After Years, and the interquel Final Fantasy IV -Interlude- that bridges the gap between the two games. Cecil is one of the few characters in the series to change his job during the game, starting out as a Dark Knight, but after a trial and the battle with the first of the Four Elemental Archfiends, transforms into a Paladin.",
                            Picture = ""
                        },
                        new Character
                        {
                            Id = 2,
                            Name = "Rosa Joanna Farrell",
                            Age = "19",
                            Gender = "Female",
                            Race = "Human",
                            Job = "White Mage",
                            Height = "1.62m",
                            Weight = "47kg",
                            Origin = "Final Fantasy 4",
                            Description = "Rosa Joanna Farrell (ローザ・ファレル Rōza Fareru) is a playable character in Final Fantasy IV and its sequel, Final Fantasy IV: The After Years. She hails from Baron, and is a skilled Archer and White Mage. Rosa is Cecil's childhood friend, and harbors romantic feelings for him. Though Cecil is reluctant to let her follow him into danger at first, she stays by his side.",
                            Picture = ""
                        }
                    };

                    context.Cache[CacheKey] = characters;
                }
            }
        }

        public bool SaveCharacter(Character character)
        {
            var context = HttpContext.Current;

            if (context != null)
            {
                try
                {
                    var currentData = ((Character[])context.Cache[CacheKey]).ToList();
                    currentData.Add(character);
                    context.Cache[CacheKey] = currentData.ToArray();

                    return true;
                }

                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            }

            return false;
        }
    }
}