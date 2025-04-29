using CSFramework;
using UnityEngine;
using UnityEngine.UI;

public class CellEditControlUI : BaseUI
{
    public M3Editor Editor;
    public Text RandomSeedText;
    public InputField DifficultyLevelInput;

    protected override void on_create()
    {
        RandomSeedText = find_component<Text>("RandomGroup/RandomSeedText");
        RandomSeedText.text = "";
        DifficultyLevelInput = find_component<InputField>("RandomGroup/DifficultyLevel");
        DifficultyLevelInput.onEndEdit.AddListener(OnDifficultyLevelEndInput);

        register_toggle("ToggleGroup/EditModeToggle", OnEditModeChanged).isOn = false;
        register_button("RandomGroup/RandomButton", OnRandomButtonClicked);
        register_button("OffsetGroup/ButtonGroup/UpButton", on_offset_up_clicked);
        register_button("OffsetGroup/ButtonGroup/RightButton", on_offset_right_clicked);
        register_button("OffsetGroup/ButtonGroup/DownButton", on_offset_down_clicked);
        register_button("OffsetGroup/ButtonGroup/LeftButton", on_offset_left_clicked);
    }

    public CellEditControlUI Init (M3Editor editor)
    {
        Editor = editor;
        return this;
    }

    public void SelectPanel (M3Panel panel)
    {
        if (panel == null)
        {
            RandomSeedText.text = "";
            DifficultyLevelInput.text = "";
        }
        else
        {
            RandomSeedText.text = $"{panel.RandomSeed}";
            DifficultyLevelInput.text = $"{panel.DifficultyLevel}";
        }
    }

    public void OnEditModeChanged (bool selected)
    {
        if (Editor)
        {
            Editor.LayerUI.SetEditMode(selected);
            Editor.CellEditUI.SetEditMode(selected);
        }
    }

    public void OnDifficultyLevelEndInput (string content)
    {
        if (int.TryParse(content, out var difficulty_level))
        {
            if (Editor.LayerUI.CurrentPanel == null)
            {
                Editor.ShowError("需要先选中一个布局");
                return;
            }
            difficulty_level = Mathf.Clamp(difficulty_level, 1, M3Const.CellTypeCount);
            DifficultyLevelInput.text = $"{difficulty_level}";
            Editor.LayerUI.CurrentPanel.DifficultyLevel = difficulty_level;
        }
    }

    public void OnRandomButtonClicked ()
    {
        if (Editor.LayerUI.CurrentPanel == null)
        {
            Editor.ShowError("需要先选中一个布局");
            return;
        }

        Editor.LayerUI.CurrentPanel.RandomAllCellType();
        RandomSeedText.text = $"{Editor.LayerUI.CurrentPanel.RandomSeed}";
        OnEditModeChanged(false);
        Editor.LayerUI.OnRefreshLayerList();
    }

    private void on_offset_up_clicked ()
    {
        if (Editor.LayerUI.CurrentLayer != null)
        {
            Editor.LayerUI.CurrentLayer.MoveUp();
            Editor.CellEditUI.RefreshLayer(Editor.LayerUI.CurrentLayer);
        }
    }

    private void on_offset_right_clicked ()
    {
        if (Editor.LayerUI.CurrentLayer != null)
        {
            Editor.LayerUI.CurrentLayer.MoveRight();
            Editor.CellEditUI.RefreshLayer(Editor.LayerUI.CurrentLayer);
        }
    }

    private void on_offset_down_clicked ()
    {
        if (Editor.LayerUI.CurrentLayer != null)
        {
            Editor.LayerUI.CurrentLayer.MoveDown();
            Editor.CellEditUI.RefreshLayer(Editor.LayerUI.CurrentLayer);
        }
    }

    private void on_offset_left_clicked ()
    {
        if (Editor.LayerUI.CurrentLayer != null)
        {
            Editor.LayerUI.CurrentLayer.MoveLeft();
            Editor.CellEditUI.RefreshLayer(Editor.LayerUI.CurrentLayer);
        }
    }
}