using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace CSFramework
{
    public class CommonWindowView : MonoBehaviour
    {
        public RectTransform Content;
        public Button CloseButton;
        public Button ConfirmButton;
        public Button CancelButton;
        public TextMeshProUGUI ConfirmText;
        public TextMeshProUGUI CancelText;
        public TextMeshProUGUI TitleText;
        public TextMeshProUGUI ContentText;
    }
}