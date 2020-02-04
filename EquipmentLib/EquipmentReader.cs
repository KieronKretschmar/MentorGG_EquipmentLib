using EquipmentLibrary;
using EquipmentLibrary.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace EquipmentLibrary
{
    public class EquipmentReader
    {

        private static Dictionary<string, string> ReplacementTypeDictionary = new Dictionary<string, string>
        {
            {"deagle", "4"},
            {"revolver", "10"},
            {"elite", "6"},
            {"fiveseven", "5"},
            {"glock", "2"},
            {"hkp2000", "1"},
            {"usp_silencer", "9"},
            {"p250", "3"},
            {"cz75a", "8"},
            {"tec9", "7"},
            {"mag7", "203"},
            {"nova", "202"},
            {"sawedoff", "201"},
            {"xm1014", "204"},
            {"bizon", "103"},
            {"mac10", "104"},
            {"mp7", "101"},
            {"mp5sd", "107"},
            {"mp9", "102"},
            {"p90", "106"},
            {"ump45", "105"},
            {"ak47", "303"},
            {"aug", "308"},
            {"famas", "302"},
            {"galilar", "301"},
            {"m4a1", "304"},
            {"m4a1_silencer", "305"},
            {"sg556", "307"},
            {"m249", "205"},
            {"negev", "206"},
            {"awp", "309"},
            {"g3sg1", "311"},
            {"scar20", "310"},
            {"ssg08", "306"},
        };

        private static Dictionary<string, string> ReplacementNameDictionary = new Dictionary<string, string>
        {
            {"deagle", "Desert Eagle"},
            {"revolver", "R8 Revolver"},
            {"elite", "Dual Berettas"},
            {"fiveseven", "Five-SeveN"},
            {"glock", "Glock-18"},
            {"hkp2000", "P2000"},
            {"usp_silencer", "USP-S"},
            {"p250", "P250"},
            {"cz75a", "CZ75-Auto"},
            {"tec9", "Tec-9"},
            {"mag7", "MAG-7"},
            {"nova", "Nova"},
            {"sawedoff", "Sawed-Off"},
            {"xm1014", "XM1014"},
            {"bizon", "PP-Bizon"},
            {"mac10", "Mac-10"},
            {"mp7", "MP7"},
            {"mp5sd", "MP5SD"},
            {"mp9", "MP9"},
            {"p90", "P90"},
            {"ump45", "UMP-45"},
            {"ak47", "AK-47"},
            {"aug", "AUG"},
            {"famas", "FAMAS"},
            {"galilar", "Galil AR"},
            {"m4a1", "M4A4"},
            {"m4a1_silencer", "M4A1-S"},
            {"sg556", "SG 553"},
            {"m249", "M249"},
            {"negev", "Negev"},
            {"awp", "AWP"},
            {"g3sg1", "G3SG1"},
            {"scar20", "SCAR-20"},
            {"ssg08", "SSG 08"},
        };
        
        public List<EquipmentSet> LoadEquipmentsFromFiles(string csvDirectory)
        {
            var equipmentSets = new List<EquipmentSet>();
            foreach (var filePath in Directory.GetFiles(csvDirectory))
            {
                var equipmentSet = new EquipmentSet();

                //Set Source and ValidFrom
                var filename = Path.GetFileNameWithoutExtension(filePath);

                equipmentSet.Source = (Source) Enum.Parse(typeof(Source), filename.Split('#').First());

                var time = filename.Split('#').Last().Split('-');
                int year = int.Parse(time[0]);
                int month = int.Parse(time[1]);
                int day = int.Parse(time[2]);
                int hour = int.Parse(time[3]);
                int min = int.Parse(time[4]);
                int sec = int.Parse(time[5]);
                equipmentSet.ValidFrom = new DateTime(year, month, day, hour, min, sec);

                // Set EquipmentDict
                equipmentSet.EquipmentDict = ReadEquipmentDictFromCsv(filePath);

                equipmentSets.Add(equipmentSet);
            }

            return equipmentSets;
        }

        private Dictionary<short, EquipmentInfo> ReadEquipmentDictFromCsv(string filePath)
        {

            //Create new Values
            string[] csvStrings = File.ReadAllLines(filePath);

            //Remove $ and commas in numbers
            for (int i = 0; i < csvStrings.Length; i++)
            {
                //Remove $
                csvStrings[i] = csvStrings[i].Replace("$", "");

                //Remove commas in numbers
                MatchCollection matches = Regex.Matches(csvStrings[i], "\"[0-9]*(,)[0-9.]*\"");
                foreach (Match match in matches)
                {
                    string fixedMatch = match.Value.Replace(",", "");
                    csvStrings[i] = csvStrings[i].Replace(match.Value, fixedMatch);
                }

                //Remove extra "
                csvStrings[i] = csvStrings[i].Replace("\"", "");
            }

            //Calculate Values from CSV
            //Add new Values
            var equipmentDict = new Dictionary<short, EquipmentInfo>();
            //Work Values from CSV
            foreach (var csvString in csvStrings)
            {
                string[] fields = csvString.Split(',');

                EquipmentInfo eq = new EquipmentInfo();

                eq.Type = short.Parse(ReplacementTypeDictionary[fields[0]]);
                eq.InGameName = fields[0];
                eq.DisplayName = ReplacementNameDictionary[fields[0]];
                eq.WeaporArmorRatio = !string.IsNullOrEmpty(fields[01]) ? float.Parse(fields[01]) : 0;
                eq.Damage = !string.IsNullOrEmpty(fields[02]) ? int.Parse(fields[02]) : 0;
                eq.RangeModifier = !string.IsNullOrEmpty(fields[03]) ? float.Parse(fields[03]) : 0;
                eq.CycleTime = !string.IsNullOrEmpty(fields[04]) ? float.Parse(fields[04]) : 0;
                eq.Penetration = !string.IsNullOrEmpty(fields[05]) ? float.Parse(fields[05]) : 0;
                eq.KillAward = !string.IsNullOrEmpty(fields[06]) ? int.Parse(fields[06]) : 0;
                eq.MaxPlayerSpeed = !string.IsNullOrEmpty(fields[07]) ? int.Parse(fields[07]) : 0;
                eq.ClipSize = !string.IsNullOrEmpty(fields[08]) ? int.Parse(fields[08]) : 0;
                eq.Price = !string.IsNullOrEmpty(fields[09]) ? int.Parse(fields[09]) : 0;
                eq.Range = !string.IsNullOrEmpty(fields[10]) ? int.Parse(fields[10]) : 0;
                eq.WeaponClass = !string.IsNullOrEmpty(fields[11]) ? fields[11] : "";
                eq.FullAuto = !string.IsNullOrEmpty(fields[12]) ? float.Parse(fields[12]) : 0;
                eq.Bullets = !string.IsNullOrEmpty(fields[13]) ? float.Parse(fields[13]) : 0;
                eq.TracerFrequency = !string.IsNullOrEmpty(fields[14]) ? float.Parse(fields[14]) : 0;
                eq.FlinchVelocityModifierLarge = !string.IsNullOrEmpty(fields[15]) ? float.Parse(fields[15]) : 0;
                eq.FlinchVelocityModifierSmall = !string.IsNullOrEmpty(fields[16]) ? float.Parse(fields[16]) : 0;
                eq.Spread = !string.IsNullOrEmpty(fields[17]) ? float.Parse(fields[17]) : 0;
                eq.InaccuracyCrouch = !string.IsNullOrEmpty(fields[18]) ? float.Parse(fields[18]) : 0;
                eq.InaccuracyStand = !string.IsNullOrEmpty(fields[19]) ? float.Parse(fields[19]) : 0;
                eq.InaccuracyFire = !string.IsNullOrEmpty(fields[20]) ? float.Parse(fields[20]) : 0;
                eq.InaccuracyMove = !string.IsNullOrEmpty(fields[21]) ? float.Parse(fields[21]) : 0;
                eq.InaccuracyJump = !string.IsNullOrEmpty(fields[22]) ? float.Parse(fields[22]) : 0;
                eq.InaccuracyJumpIntial = !string.IsNullOrEmpty(fields[23]) ? float.Parse(fields[23]) : 0;
                eq.InaccuracyLand = !string.IsNullOrEmpty(fields[24]) ? float.Parse(fields[24]) : 0;
                eq.InaccuracyLadder = !string.IsNullOrEmpty(fields[25]) ? float.Parse(fields[25]) : 0;
                eq.RecoveryTimeCrouch = !string.IsNullOrEmpty(fields[26]) ? float.Parse(fields[26]) : 0;
                eq.RecoveryTimeCrouchFinal = !string.IsNullOrEmpty(fields[27]) ? float.Parse(fields[27]) : 0;
                eq.RecoveryTimeStand = !string.IsNullOrEmpty(fields[28]) ? float.Parse(fields[28]) : 0;
                eq.RecoveryTimeStandFinal = !string.IsNullOrEmpty(fields[29]) ? float.Parse(fields[29]) : 0;
                eq.RecoilAngleVariance = !string.IsNullOrEmpty(fields[30]) ? float.Parse(fields[30]) : 0;
                eq.RecoilMagnitude = !string.IsNullOrEmpty(fields[31]) ? float.Parse(fields[31]) : 0;
                eq.RecoilMagnitudeVariance = !string.IsNullOrEmpty(fields[32]) ? float.Parse(fields[32]) : 0;
                eq.SpreadAlt = !string.IsNullOrEmpty(fields[33]) ? float.Parse(fields[33]) : 0;
                eq.InaccuracyCrouchAlt = !string.IsNullOrEmpty(fields[34]) ? float.Parse(fields[34]) : 0;
                eq.InaccuracyStandAlt = !string.IsNullOrEmpty(fields[35]) ? float.Parse(fields[35]) : 0;
                eq.InaccuracyFireAlt = !string.IsNullOrEmpty(fields[36]) ? float.Parse(fields[36]) : 0;
                eq.InaccuracyMoveAlt = !string.IsNullOrEmpty(fields[37]) ? float.Parse(fields[37]) : 0;
                eq.InaccuracyJumpAlt = !string.IsNullOrEmpty(fields[38]) ? float.Parse(fields[38]) : 0;
                eq.InaccuracyLandAlt = !string.IsNullOrEmpty(fields[39]) ? float.Parse(fields[39]) : 0;
                eq.InaccuracyLadderAlt = !string.IsNullOrEmpty(fields[40]) ? float.Parse(fields[40]) : 0;
                eq.RecoilAngleVarianceAlt = !string.IsNullOrEmpty(fields[41]) ? float.Parse(fields[41]) : 0;
                eq.RecoilMagnitudeAlt = !string.IsNullOrEmpty(fields[42]) ? float.Parse(fields[42]) : 0;
                eq.RecoilMagnitudeVarianceAlt = !string.IsNullOrEmpty(fields[43]) ? float.Parse(fields[43]) : 0;
                eq.MaxPlayerSpeedAlt = !string.IsNullOrEmpty(fields[44]) ? float.Parse(fields[44]) : 0;
                eq.TracerFrequencyAlt = !string.IsNullOrEmpty(fields[45]) ? float.Parse(fields[45]) : 0;
                eq.ZoomFov = !string.IsNullOrEmpty(fields[46]) ? float.Parse(fields[46]) : 0;
                eq.ZoomFovAlt = !string.IsNullOrEmpty(fields[47]) ? float.Parse(fields[47]) : 0;
                eq.CycleTimeAlt = !string.IsNullOrEmpty(fields[48]) ? float.Parse(fields[48]) : 0;
                eq.CycletimeBurst = !string.IsNullOrEmpty(fields[49]) ? float.Parse(fields[49]) : 0;
                eq.TimeInbetweenBurstShots = !string.IsNullOrEmpty(fields[50]) ? float.Parse(fields[50]) : 0;

                equipmentDict.Add(eq.Type, eq);
            }

            //Calculate Extras
            short[] extras = { 401, 402, 403, 404, 405, 406, 501, 502, 503, 504, 505, 506 };

            //add additional equipments not mentioned in csv
            equipmentDict.Add(401, new EquipmentInfo()
            {
                Type = 401,
                InGameName = "taser",
                DisplayName = "Zeus",
                WeaponClass = "Equipment",
                KillAward = 300,
                Price = 200,
            });

            equipmentDict.Add(402, new EquipmentInfo()
            {
                Type = 402,
                InGameName = "kevlar",
                DisplayName = "Kevlar",
                WeaponClass = "Equipment",
                Price = 650,
            });

            equipmentDict.Add(403, new EquipmentInfo()
            {
                Type = 403,
                InGameName = "helmet",
                DisplayName = "Helmet",
                WeaponClass = "Equipment",
                Price = 350,
            });

            equipmentDict.Add(404, new EquipmentInfo()
            {
                Type = 404,
                InGameName = "c4",
                DisplayName = "C4",
                WeaponClass = "Equipment",
            });

            equipmentDict.Add(405, new EquipmentInfo()
            {
                Type = 405,
                InGameName = "knife",
                DisplayName = "Knife",
                WeaponClass = "Equipment",
                KillAward = 300,
            });

            equipmentDict.Add(406, new EquipmentInfo()
            {
                Type = 406,
                InGameName = "defuser",
                DisplayName = "Defuse kit",
                WeaponClass = "Equipment",
                Price = 400,
            });

            equipmentDict.Add(501, new EquipmentInfo()
            {
                Type = 501,
                InGameName = "decoy",
                DisplayName = "Decoy",
                WeaponClass = "Grenade",
                Price = 50,
            });

            equipmentDict.Add(502, new EquipmentInfo()
            {
                Type = 502,
                InGameName = "molotov",
                DisplayName = "Molotov",
                WeaponClass = "Grenade",
                Price = 400,
            });

            equipmentDict.Add(503, new EquipmentInfo()
            {
                Type = 503,
                InGameName = "incgrenade",
                DisplayName = "Incendiary",
                WeaponClass = "Grenade",
                Price = 600,
            });

            equipmentDict.Add(504, new EquipmentInfo()
            {
                Type = 504,
                InGameName = "flashbang",
                DisplayName = "Flashbang",
                WeaponClass = "Grenade",
                Price = 200,
            });

            equipmentDict.Add(505, new EquipmentInfo()
            {
                Type = 505,
                InGameName = "smokegrenade",
                DisplayName = "Smoke",
                WeaponClass = "Grenade",
                Price = 300,
            });

            equipmentDict.Add(506, new EquipmentInfo()
            {
                Type = 506,
                InGameName = "hegrenade",
                DisplayName = "HE",
                WeaponClass = "Grenade",
                Price = 300,
            });

            return equipmentDict;
        }
    }
}
