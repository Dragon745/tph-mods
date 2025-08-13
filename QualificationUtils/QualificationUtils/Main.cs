﻿using Harmony12;
using QualificationUtils.Patches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TH20;
using UnityEngine;
using UnityModManagerNet;
using Object = UnityEngine.Object;

namespace QualificationUtils
{
#if DEBUG

    [EnableReloading]
#endif
    public static class Main
    {
        #region Properties

        public static bool Enabled { get; private set; }
        public static UnityModManager.ModEntry.ModLogger Logger { get; private set; }

        #endregion

        #region Fields

        private static Vector2 _qualificationScrollPosition;
        private static Vector2 _traitScrollPosition;

        #endregion

        #region Methods

        private static bool Load(UnityModManager.ModEntry modEntry)
        {
            HarmonyInstance.Create(modEntry.Info.Id).PatchAll(Assembly.GetExecutingAssembly());

            Logger = modEntry.Logger;

            modEntry.OnToggle = OnToggle;
            modEntry.OnGUI = OnGUI;
            modEntry.OnShowGUI = OnShowGUI;
            modEntry.OnHideGUI = OnHideGUI;

#if DEBUG
            modEntry.OnUnload = OnUnload;
#endif

            return true;
        }

        #endregion

        #region Event handler

        private static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            Enabled = value;
            return true;
        }

        private static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            try
            {
                var staff = InspectorMenu_Inspect_Patch.SelectedStaff;

                if (staff == null)
                {
                    GUILayout.Label("No staff selected");
                    return;
                }

                GUILayout.Label($"Selected: {staff.NameWithTitle}");

                PrintSetRank(staff);

                PrintRemoveQualifications(staff);
                PrintAddQualifications(staff);

                PrintRemoveTraits(staff);
                PrintAddTraits(staff);

                PrintAssistantButton(staff);
                PrintPerfectGPButton(staff);
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString());
            }
        }

        private static void OnShowGUI(UnityModManager.ModEntry modEntry)
        {
            var manager = GetInputManager();
            if (manager == null)
                return;

            manager.Enabled = false;
        }

        private static void OnHideGUI(UnityModManager.ModEntry modEntry)
        {
            var manager = GetInputManager();
            if (manager == null)
                return;

            manager.Enabled = true;
        }

#if DEBUG

        private static bool OnUnload(UnityModManager.ModEntry modEntry)
        {
            HarmonyInstance.Create(modEntry.Info.Id).UnpatchAll();
            return true;
        }

#endif

        #endregion

        #region GUI Helper

        private static void PrintSetRank(Staff staff)
        {
            GUILayout.Label($"Set rank (Current: {staff.Rank + 1})", UnityModManager.UI.h2);

            GUILayout.BeginHorizontal();

            var labelStyle = new GUIStyle(GUI.skin.GetStyle("Label")) { alignment = TextAnchor.MiddleCenter };

            for (var i = 1; i <= 5; i++)
            {
                if (i >= staff.Qualifications.Count && staff.Rank + 1 != i)
                {
                    if (GUILayout.Button("" + i, GUILayout.Width(50f)))
                    {
                        staff.SetRank(i - 1);
                        staff.SetSalary(staff.GetDesiredSalary(), false);
                    }
                }
                else
                    GUILayout.Label("" + i, labelStyle, GUILayout.Width(50f));
            }

            GUILayout.EndHorizontal();
        }

        private static void PrintRemoveQualifications(Staff staff)
        {
            GUILayout.Label("Remove qualification", UnityModManager.UI.h2);

            if (staff.Qualifications.Count <= 0)
            {
                GUILayout.Label("No qualification found.");
                return;
            }

            GUILayout.BeginHorizontal();

            var qualifications = staff.Qualifications.ToList();
            var allRequiredQualifications = qualifications
                .Select(slot => slot.Definition.RequiredQualifications)
                .SelectMany(instances => instances)
                .Select(instance => (QualificationDefinition)instance.GetInstance)
                .ToArray();

            foreach (var qualification in qualifications)
            {
                var qualificationName = qualification.Definition.NameLocalised.Translation;

                if (allRequiredQualifications.Any(definition => definition == qualification.Definition))
                    GUILayout.Label(qualificationName, GUILayout.ExpandWidth(false));
                else
                {
                    if (GUILayout.Button(qualificationName, GUILayout.ExpandWidth(false)))
                    {
                        staff.Qualifications.Remove(qualification);
                        staff.ModifiersComponent?.RemoveModifiers(qualification.Definition.Modifiers);
                    }
                }
            }

            GUILayout.EndHorizontal();
        }

        private static void PrintAddQualifications(Staff staff)
        {
            GUILayout.Label("Add qualification", UnityModManager.UI.h2);

            if (staff.NumFreeQualificationSlots == 0)
            {
                GUILayout.Label("No qualification slots available.");
                return;
            }

            var level = GetCurrentLoadedLevel();
            var allAvailableQualificationsDefinitions = level.JobApplicantManager.Qualifications.List.Keys;

            _qualificationScrollPosition = GUILayout.BeginScrollView(_qualificationScrollPosition, GUILayout.Height(150f));

            foreach (var definition in allAvailableQualificationsDefinitions)
            {
                if (!definition.ValidFor(staff))
                    continue;

                if (GUILayout.Button(definition.NameLocalised.Translation, GUILayout.Width(200f)))
                {
                    staff.Qualifications.Add(new QualificationSlot(definition, true));
                    staff.ModifiersComponent?.AddModifiers(definition.Modifiers);
                }
            }

            GUILayout.EndScrollView();
        }

        private static void PrintRemoveTraits(Staff staff)
        {
            GUILayout.Label("Remove traits", UnityModManager.UI.h2);

            var activeTraits = GetActiveCharacterTraits(staff);
            if (activeTraits.Count <= 0)
            {
                GUILayout.Label("No traits found.");
                return;
            }

            GUILayout.BeginHorizontal();

            foreach (var activeTrait in activeTraits)
            {
                if (GUILayout.Button(activeTrait.GetShortName(staff.Gender).ToString(), GUILayout.ExpandWidth(false)))
                {
                    staff.Traits.Remove(staff, activeTrait);
                    staff.ModifiersComponent?.RemoveModifiers(activeTrait.Modifiers);
                }
            }

            GUILayout.EndHorizontal();
        }

        private static void PrintAddTraits(Staff staff)
        {
            GUILayout.Label("Add traits (updates when not paused)", UnityModManager.UI.h2);

            var level = GetCurrentLoadedLevel();
            var allTraits = level.CharacterTraitsManager.AllTraits.List.Keys.ToList();
            var activeTraits = GetActiveCharacterTraits(staff);

            var availableTraits = allTraits.Where(x => !activeTraits.Contains(x));

            _traitScrollPosition = GUILayout.BeginScrollView(_traitScrollPosition, GUILayout.Height(200f));

            foreach (var trait in availableTraits)
            {
                if (!trait.IsValidFor(staff.Definition._type) || trait.Conditions.Any(x => !x.IsValid(staff)))
                    continue;

                if (GUILayout.Button(trait.GetShortName(staff.Gender).ToString(), GUILayout.Width(200f)))
                {
                    staff.Traits.Add(trait);
                    staff.ModifiersComponent?.AddModifiers(trait.Modifiers);
                }
            }

            GUILayout.EndScrollView();
        }

        private static void PrintAssistantButton(Staff staff)
        {
            GUILayout.Space(10);
            GUILayout.Label("Assistant", UnityModManager.UI.h2);

            if (GUILayout.Button("Make Perfect Assistant", GUILayout.Height(30f)))
            {
                MakePerfectAssistant(staff);
            }
        }

        private static void PrintPerfectGPButton(Staff staff)
        {
            GUILayout.Space(10);
            GUILayout.Label("General Practitioner", UnityModManager.UI.h2);

            if (GUILayout.Button("Make Perfect GP", GUILayout.Height(30f)))
            {
                MakePerfectGP(staff);
            }
        }




        #endregion

        #region Helper methods

        private static void MakePerfectAssistant(Staff staff)
        {
            try
            {
                // Set rank to 5 (emp rank 5)
                staff.SetRank(4); // Rank is 0-based, so 4 = rank 5
                staff.SetSalary(staff.GetDesiredSalary(), false);

                // Remove all qualifications
                var qualificationsToRemove = staff.Qualifications.ToList();
                foreach (var qualification in qualificationsToRemove)
                {
                    staff.Qualifications.Remove(qualification);
                    staff.ModifiersComponent?.RemoveModifiers(qualification.Definition.Modifiers);
                }

                // Add specific qualifications
                var level = GetCurrentLoadedLevel();
                var allQualifications = level.JobApplicantManager.Qualifications.List.Keys;

                var targetQualifications = new[] { "Customer Service", "Customer Service II", "Customer Service III", "Emotional Int", "Motivation" };

                foreach (var targetQual in targetQualifications)
                {
                    var qualificationDef = allQualifications.FirstOrDefault(q =>
                        q.NameLocalised.Translation.Contains(targetQual) ||
                        q.NameLocalised.Translation.Equals(targetQual, StringComparison.OrdinalIgnoreCase));

                    if (qualificationDef != null && qualificationDef.ValidFor(staff))
                    {
                        staff.Qualifications.Add(new QualificationSlot(qualificationDef, true));
                        staff.ModifiersComponent?.AddModifiers(qualificationDef.Modifiers);
                    }
                }

                // Remove all existing traits
                var activeTraits = GetActiveCharacterTraits(staff);
                foreach (var trait in activeTraits)
                {
                    staff.Traits.Remove(staff, trait);
                    staff.ModifiersComponent?.RemoveModifiers(trait.Modifiers);
                }

                // Add specific positive traits
                var allTraits = level.CharacterTraitsManager.AllTraits.List.Keys.ToList();
                var targetTraits = new[] { "Charming", "Entertainer", "Fast Learner", "Funny", "Healer", "Hygienic", "Inspiring", "Motivated", "Positive", "Teacher", "Tireless" };

                foreach (var targetTraitName in targetTraits)
                {
                    var traitDef = allTraits.FirstOrDefault(t =>
                        t.GetShortName(staff.Gender).ToString().Contains(targetTraitName) ||
                        t.GetShortName(staff.Gender).ToString().Equals(targetTraitName, StringComparison.OrdinalIgnoreCase));

                    if (traitDef != null && traitDef.IsValidFor(staff.Definition._type) && traitDef.Conditions.All(x => x.IsValid(staff)))
                    {
                        staff.Traits.Add(traitDef);
                        staff.ModifiersComponent?.AddModifiers(traitDef.Modifiers);
                    }
                }

                Logger.Log($"Successfully transformed {staff.NameWithTitle} into a perfect assistant!");
            }
            catch (Exception e)
            {
                Logger.Error($"Error making perfect assistant: {e.ToString()}");
            }
        }

        private static void MakePerfectGP(Staff staff)
        {
            try
            {
                // Set rank to 5 (emp rank 5)
                staff.SetRank(4); // Rank is 0-based, so 4 = rank 5
                staff.SetSalary(staff.GetDesiredSalary(), false);

                // Remove all qualifications
                var qualificationsToRemove = staff.Qualifications.ToList();
                foreach (var qualification in qualificationsToRemove)
                {
                    staff.Qualifications.Remove(qualification);
                    staff.ModifiersComponent?.RemoveModifiers(qualification.Definition.Modifiers);
                }

                // Add specific GP qualifications
                var level = GetCurrentLoadedLevel();
                var allQualifications = level.JobApplicantManager.Qualifications.List.Keys;

                var targetQualifications = new[] { "General Practice", "General Practice II", "General Practice III", "General Practice IV", "General Practice V" };

                foreach (var targetQual in targetQualifications)
                {
                    var qualificationDef = allQualifications.FirstOrDefault(q =>
                        q.NameLocalised.Translation.Contains(targetQual) ||
                        q.NameLocalised.Translation.Equals(targetQual, StringComparison.OrdinalIgnoreCase));

                    if (qualificationDef != null && qualificationDef.ValidFor(staff))
                    {
                        staff.Qualifications.Add(new QualificationSlot(qualificationDef, true));
                        staff.ModifiersComponent?.AddModifiers(qualificationDef.Modifiers);
                    }
                }

                // Remove all existing traits
                var activeTraits = GetActiveCharacterTraits(staff);
                foreach (var trait in activeTraits)
                {
                    staff.Traits.Remove(staff, trait);
                    staff.ModifiersComponent?.RemoveModifiers(trait.Modifiers);
                }

                // Add specific positive traits
                var allTraits = level.CharacterTraitsManager.AllTraits.List.Keys.ToList();
                var targetTraits = new[] { "Charming", "Entertainer", "Fast Learner", "Funny", "Healer", "Hygienic", "Inspiring", "Motivated", "Positive", "Teacher", "Tireless" };

                foreach (var targetTraitName in targetTraits)
                {
                    var traitDef = allTraits.FirstOrDefault(t =>
                        t.GetShortName(staff.Gender).ToString().Contains(targetTraitName) ||
                        t.GetShortName(staff.Gender).ToString().Equals(targetTraitName, StringComparison.OrdinalIgnoreCase));

                    if (traitDef != null && traitDef.IsValidFor(staff.Definition._type) && traitDef.Conditions.All(x => x.IsValid(staff)))
                    {
                        staff.Traits.Add(traitDef);
                        staff.ModifiersComponent?.AddModifiers(traitDef.Modifiers);
                    }
                }

                Logger.Log($"Successfully transformed {staff.NameWithTitle} into a perfect GP!");
            }
            catch (Exception e)
            {
                Logger.Error($"Error making perfect GP: {e.ToString()}");
            }
        }


        private static Level GetCurrentLoadedLevel()
        {
            var mainScript = Object.FindObjectOfType<MainScript>();
            if (!mainScript)
                return default;

            var app = Traverse.Create(mainScript).Field("_app").GetValue<App>();
            return app?.Level;
        }

        private static InputManager GetInputManager()
        {
            var mainScript = Object.FindObjectOfType<MainScript>();
            if (!mainScript)
                return default;

            var app = Traverse.Create(mainScript).Field("_app").GetValue<App>();
            return app?.InputManager;
        }

        private static List<CharacterTraitDefinition> GetActiveCharacterTraits(Character staff)
        {
            return Traverse.Create(staff.Traits).Field("_activeTraits").GetValue<List<CharacterTraitDefinition>>().ToList();
        }

        #endregion
    }
}