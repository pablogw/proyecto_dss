using Fire_Emblem.Models;
using Fire_Emblem_View;
using System.Text.Json;

namespace Fire_Emblem.Services
{
    
    public class FileLoaderService
    {
        private readonly string _teamsFolder;
        private readonly View _view;

        public FileLoaderService(string teamsFolder, View view)
        {
            _teamsFolder = teamsFolder;
            _view = view;

        }
        public string[] GetTeamFiles()
        {
            return Directory.GetFiles(_teamsFolder, "*.txt")
                .Select(Path.GetFileName)
                .OrderBy(f => f)
                .ToArray();
        }
        public (Team Player1Team, Team Player2Team) LoadTeams(string filename)
        {
            string filePath = Path.Combine(_teamsFolder, filename);
            string[] lines = File.ReadAllLines(filePath);

            var characterData = LoadCharactersFromJson();

            Team player1Team = new Team();
            Team player2Team = new Team();
            Team currentTeam = null;

            foreach (string line in lines)
            {
                if (line.StartsWith("Player 1 Team"))
                {
                    currentTeam = player1Team;
                }
                else if (line.StartsWith("Player 2 Team"))
                {
                    currentTeam = player2Team;
                }
                else if (!string.IsNullOrWhiteSpace(line) && currentTeam != null)
                {
                    Unit unit = ParseUnitFromLine(line, characterData);
                    currentTeam.AddUnit(unit);
                }
            }

            return (player1Team, player2Team);
        }

        private Unit ParseUnitFromLine(string line, Dictionary<string, Unit> characterData)
        {
            string[] parts = line.Split('(');
            string name = parts[0].Trim();

            if (!characterData.TryGetValue(name, out Unit characterTemplate))
            {
                throw new InvalidOperationException($"Character {name} not found in character data.");
            }

            // Create a new Unit object based on the template
            Unit unit = new Unit(
                characterTemplate.Name,
                characterTemplate.Weapon,
                characterTemplate.Gender,
                characterTemplate.MaxHP,
                characterTemplate.Atk,
                characterTemplate.Spd,
                characterTemplate.Def,
                characterTemplate.Res
            );

            if (parts.Length > 1)
            {
                string skillsPart = parts[1].Trim(')');
                string[] skillNames = skillsPart.Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (string skillName in skillNames)
                {
                    Skill skill = CreateSkill(skillName.Trim());
                    if (skill != null)
                    {
                        unit.AddSkill(skill);
                    }
                    else
                    {
                        Console.WriteLine($"Warning: Skill '{skillName.Trim()}' not recognized for unit {name}.");
                    }
                }
            }

            return unit;
        }

        private Skill CreateSkill(string skillName)
{
    return skillName switch
    {
        "HP +15" => SkillFactory.CreateHPPlus15(),
        "Fair Fight" => SkillFactory.CreateFairFight(),
        "Will to win" => SkillFactory.CreateWillToWin(),
        "Single-Minded" => SkillFactory.CreateSingleMinded(),
        "Ignis" => SkillFactory.CreateIgnis(),
        "Perceptive" => SkillFactory.CreatePerceptive(),
        "Tome Precision" => SkillFactory.CreateTomePrecision(),
        "Attack +6" => SkillFactory.CreateAttackPlus6(),
        "Speed +5" => SkillFactory.CreateSpeedPlus5(),
        "Defense +5" => SkillFactory.CreateDefensePlus5(),
        "Wrath" => SkillFactory.CreateWrath(),
        "Resolve" => SkillFactory.CreateResolve(),
        "Resistance +5" => SkillFactory.CreateResistancePlus5(),
        "Atk/Def +5" => SkillFactory.CreateAtkDefPlus5(),
        "Atk/Res +5" => SkillFactory.CreateAtkResPlus5(),
        "Spd/Res +5" => SkillFactory.CreateSpdResPlus5(),
        "Deadly Blade" => SkillFactory.CreateDeadlyBlade(),
        "Death Blow" => SkillFactory.CreateDeathBlow(),
        "Armored Blow" => SkillFactory.CreateArmoredBlow(),
        "Darting Blow" => SkillFactory.CreateDartingBlow(),
        "Warding Blow" => SkillFactory.CreateWardingBlow(),
        "Swift Sparrow" => SkillFactory.CreateSwiftSparrow(),
        "Sturdy Blow" => SkillFactory.CreateSturdyBlow(),
        "Mirror Strike" => SkillFactory.CreateMirrorStrike(),
        "Steady Blow" => SkillFactory.CreateSteadyBlow(),
        "Swift Strike" => SkillFactory.CreateSwiftStrike(),
        "Bracing Blow" => SkillFactory.CreateBracingBlow(),
        "Brazen Atk/Spd" => SkillFactory.CreateBrazenAtkSpd(),
        "Brazen Atk/Def" => SkillFactory.CreateBrazenAtkDef(),
        "Brazen Atk/Res" => SkillFactory.CreateBrazenAtkRes(),
        "Brazen Spd/Def" => SkillFactory.CreateBrazenSpdDef(),
        "Brazen Spd/Res" => SkillFactory.CreateBrazenSpdRes(),
        "Brazen Def/Res" => SkillFactory.CreateBrazenDefRes(),
        "Fire Boost" => SkillFactory.CreateFireBoost(),
        "Wind Boost" => SkillFactory.CreateWindBoost(),
        "Earth Boost" => SkillFactory.CreateEarthBoost(),
        "Water Boost" => SkillFactory.CreateWaterBoost(),
        "Chaos Style" => SkillFactory.CreateChaosStyle(),
        // ... (other existing skills)
        _ => SkillFactory.OtherSkill(skillName) // Default case for unrecognized skills
    };
}

        public List<Unit> LoadCharacters()
        {
            string json = File.ReadAllText(Path.Combine(_teamsFolder, "characters.json"));
            return JsonSerializer.Deserialize<List<Unit>>(json);
        }

        public List<Skill> LoadSkills()
        {
            string json = File.ReadAllText(Path.Combine(_teamsFolder, "skills.json"));
            return JsonSerializer.Deserialize<List<Skill>>(json);
        }
        private Dictionary<string, Unit> LoadCharactersFromJson()
        {
            string dataFolder = Path.GetDirectoryName(Path.GetDirectoryName(_teamsFolder));
            string charactersPath = Path.Combine(dataFolder, "characters.json");
            string json = File.ReadAllText(charactersPath);
            var characters = JsonSerializer.Deserialize<List<CharacterData>>(json);
    
            return characters.ToDictionary(
                c => c.Name,
                c => new Unit(
                    c.Name,
                    ParseWeapon(c.Weapon),
                    c.Gender,
                    int.Parse(c.HP),
                    int.Parse(c.Atk),
                    int.Parse(c.Spd),
                    int.Parse(c.Def),
                    int.Parse(c.Res)
                )
            );
        }
        
        public void DisplayTeamSelectionPrompt()
        {
            _view.WriteLine("Elige un archivo para cargar los equipos");
        }

        public string[] GetAndDisplayTeamFiles()
        {
            string[] teamFiles = GetTeamFiles();
            DisplayTeamFiles(teamFiles);
            return teamFiles;
        }

        private void DisplayTeamFiles(string[] teamFiles)
        {
            for (int i = 0; i < teamFiles.Length; i++)
            {
                _view.WriteLine($"{i}: {teamFiles[i]}");
            }
        }

        public bool IsValidFileIndex(int index, int length)
        {
            return index >= 0 && index < length;
        }

        public void DisplayInvalidTeamFileMessage()
        {
            _view.WriteLine("Archivo de equipos no válido");
        }

        public bool AreValidTeams(Team player1Team, Team player2Team)
        {
            return IsValidTeam(player1Team) && IsValidTeam(player2Team);
        }

        public bool IsValidTeam(Team team)
        {
            return HasValidUnitCount(team) &&
                   HasUniqueUnitNames(team) &&
                   AllUnitsHaveValidSkills(team);
        }

        private bool HasValidUnitCount(Team team)
        {
            return team.Units.Count >= 1 && team.Units.Count <= 3;
        }

        private bool HasUniqueUnitNames(Team team)
        {
            return team.Units.Select(unit => unit.Name).Distinct().Count() == team.Units.Count;
        }

        private bool AllUnitsHaveValidSkills(Team team)
        {
            return team.Units.All(UnitHasValidSkills);
        }

        private bool UnitHasValidSkills(Unit unit)
        {
            return unit.Skills.Count <= 2 &&
                   unit.Skills.Select(skill => skill.Name).Distinct().Count() == unit.Skills.Count;
        }
        private class CharacterData
        {
            public string Name { get; set; }
            public string Weapon { get; set; }
            public string Gender { get; set; }
            public string DeathQuote { get; set; }
            public string HP { get; set; }
            public string Atk { get; set; }
            public string Spd { get; set; }
            public string Def { get; set; }
            public string Res { get; set; }
        }

        private Weapon ParseWeapon(string weaponString)
        {
            return Enum.Parse<Weapon>(weaponString, true);
        }
    }
}