using System;
using UnityEngine;
using TMPro;
using CSFramework;

public class LocalizeTextMeshProUGUI : MonoBehaviour
{
    [SerializeField] private string m_Matrial;
    [SerializeField] protected string Term;

    private TextMeshProUGUI _text;
    public bool UseFormat = false;
    public string MaterialName => m_Matrial;

    public TextMeshProUGUI Text 
    { 
        get 
        {
            if (_text == null)
                _text = GetComponent<TextMeshProUGUI>();
            return _text;
        }
    }

    public string GetKey ()
    {
        if (string.IsNullOrEmpty(Term))
            return string.Empty;

        return Term.Trim();
    }

    public void ClearTerm ()
    {
        Term = string.Empty;
    }

    public void SetText (string str)
    {
        var text = Text;
        if (text)
            text.SetText(str);
    }

    public void SetLocalizeText (string key, LocalizeManager localize_manager, string local_name = "")
    {
        Term = key;
        key = GetKey();
        
        if (!string.IsNullOrEmpty(key))
        {
            SetText(localize_manager.GetLocalString(key, local_name));
            RefreshFont(localize_manager, local_name);
            UseFormat = false;
        }
    }

    public void SetLocalizeTextWithFormats (string key, LocalizeManager localize_manager, string local_name, params object[] values)
    {
        Term = key;

        key = GetKey();
        if (!string.IsNullOrEmpty(key))
        {
            SetText(localize_manager.GetLocalStringWithFormats(key, local_name, values));
            RefreshFont(localize_manager, local_name);
            UseFormat = true;
        }
    }

    public void RefreshFont (LocalizeManager localize_manager, string local_name = "")
    {
        var text = Text;
        if (text != null && !string.IsNullOrEmpty(m_Matrial))
        {
            if (localize_manager.TryGetLocalizeResource(text.font, local_name, m_Matrial, out var font_asset, out var material))
                SetFont(font_asset, material);
            else
                UnityEngine.Debug.LogWarning("RefreshFont: TryGetLocalizeResource Failed", gameObject);
        }
    }

    public bool SetFont (TMP_FontAsset font_asset, Material material)
    {
        var set_dirty = false;
        var text = Text;
        if (text.font != font_asset)
        {
            text.font = font_asset;
            set_dirty = true;
        }
        
        if (material != null && text.fontSharedMaterial != material)
        {
            text.fontSharedMaterial = material;
            set_dirty = true;
        }

        if (set_dirty)
            text.UpdateFontAsset();

        return set_dirty;
    }
}