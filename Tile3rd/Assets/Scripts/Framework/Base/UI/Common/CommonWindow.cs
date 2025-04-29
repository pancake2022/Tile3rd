using CSFramework;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace CSFramework
{
    public class CommonWindow : WindowUI
    {
        public class Data
        {
            public bool ShowClose = true;
            public bool ShowConfirm = true;
            public bool ShowCancel = true;
            public Action OnConfirm = null;
            public Action OnCancel = null;
            public string Confirm = "Confirm";
            public string Cancel = "Cancel";
            public string Title = "";
            public string Content = "";
            public bool CloseWhenClickOutside = true;
        }

        private Button _close_button;
        private Button _confirm_button;
        private Button _cancel_button;
        private TextMeshProUGUI _confirm_text;
        private TextMeshProUGUI _cancel_text;
        private TextMeshProUGUI _title_text;
        private TextMeshProUGUI _content_text;
        private Data _data;

        protected override void on_create()
        {
            var view = GetComponent<CommonWindowView>();
            if (view)
            {
                _close_button = register_button(view.CloseButton, on_cancel_clicked);
                _confirm_button = register_button(view.ConfirmButton, on_confirm_clicked);
                _cancel_button = register_button(view.CancelButton, on_cancel_clicked);
                _confirm_text = view.ConfirmText;
                _cancel_text = view.CancelText;
                _title_text = view.TitleText;
                _content_text = view.ContentText;
                Property.CloseWhenClickOutsideWindowRect = view.Content;
            }
            else
            {
                _close_button = register_button("Content/CloseButton", on_cancel_clicked);
                _confirm_button = register_button("Content/Bottom/ConfirmButton", on_confirm_clicked);
                _cancel_button = register_button("Content/Bottom/CancelButton", on_cancel_clicked);
                _confirm_text = find_component<TextMeshProUGUI>("Text", _confirm_button.transform);
                _cancel_text = find_component<TextMeshProUGUI>("Text", _cancel_button.transform);
                _title_text = find_component<TextMeshProUGUI>("Content/Title");
                _content_text = find_component<TextMeshProUGUI>("Content/Content");
                Property.CloseWhenClickOutsideWindowRect = find_component<RectTransform>("Content");
            }
            Property.ClickOutsideWindowRectCallback = on_click_outside;
        }

        private void on_confirm_clicked ()
        {
            var cb = _data.OnConfirm;
            Close();
            cb?.Invoke();
        }

        private void on_cancel_clicked ()
        {
            var cb = _data.OnCancel;
            Close();
            cb?.Invoke();
        }

        private bool on_click_outside ()
        {
            on_cancel_clicked();
            return false;
        }

        public void Refresh (Data data)
        {
            _data = data;

            refresh();
        }

        private void refresh ()
        {
            _close_button.gameObject.SetActive(_data.ShowClose);
            _confirm_button.gameObject.SetActive(_data.ShowConfirm);
            _cancel_button.gameObject.SetActive(_data.ShowCancel);
            _confirm_text.SetText(_data.Confirm);
            _cancel_text.SetText(_data.Cancel);
            _title_text.SetText(_data.Title);
            _content_text.SetText(_data.Content);
            if (!_data.CloseWhenClickOutside)
                Property.CloseWhenClickOutsideWindowRect = null;
        }
    }
}