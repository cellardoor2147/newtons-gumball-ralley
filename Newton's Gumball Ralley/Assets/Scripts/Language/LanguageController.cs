using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class LanguageController : MonoBehaviour
{
    public void SelectEnglish()
    {
        if (LocalizationSettings.SelectedLocale != LocalizationSettings.AvailableLocales.Locales[0])
        {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[0];
        }
    }
    public void SelectSpanish()
    {
        if (LocalizationSettings.SelectedLocale != LocalizationSettings.AvailableLocales.Locales[1])
        {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[1];
        }
    }
}
