using UnityEngine.Events;
using UnityEngine.UIElements;

namespace VARLab.Velcro.Demos
{
    /// <summary>
    /// This script and the associated UI Document is for the sample scene only. It is not intended to be pulled
    /// into any DLX besides for learning and testing purposes
    /// </summary>
    public class TemplateDemoController : Toolbar
    {
        public UnityEvent DisplayIcons;
        public UnityEvent DisplayButtons;
        public UnityEvent DisplayButtonsWithIcons;
        public UnityEvent DisplayControls;
        public UnityEvent DisplayWindow1;
        public UnityEvent DisplayWindow2;
        public UnityEvent DisplayBoxShadows;
        public UnityEvent DisplayCards1;
        public UnityEvent DisplayCards2;
        public UnityEvent DisplayCards3;
        public UnityEvent DisplayNodeSteppers;

        void Start()
        {
            SetupBaseToolbar();

            DisplayIcons ??= new UnityEvent();
            DisplayButtons ??= new UnityEvent();
            DisplayButtonsWithIcons ??= new UnityEvent();
            DisplayControls ??= new UnityEvent();
            DisplayWindow1 ??= new UnityEvent();
            DisplayWindow2 ??= new UnityEvent();
            DisplayBoxShadows ??= new UnityEvent();
            DisplayCards1 ??= new UnityEvent();
            DisplayCards2 ??= new UnityEvent();
            DisplayCards3 ??= new UnityEvent();
            DisplayNodeSteppers ??= new UnityEvent();

            Button iconsBtn = Root.Q<Button>("Icons");
            iconsBtn.clicked += () =>
            {
                DisplayIcons?.Invoke();
            };

            Button buttonsBtn = Root.Q<Button>("Buttons");
            buttonsBtn.clicked += () =>
            {
                DisplayButtons?.Invoke();
            };

            Button buttonsWithIconsBtn = Root.Q<Button>("ButtonsWithIcons");
            buttonsWithIconsBtn.clicked += () =>
            {
                DisplayButtonsWithIcons?.Invoke();
            };

            Button controlsBtn = Root.Q<Button>("Controls");
            controlsBtn.clicked += () =>
            {
                DisplayControls?.Invoke();
            };

            Button window1Btn = Root.Q<Button>("Window1");
            window1Btn.clicked += () =>
            {
                DisplayWindow1?.Invoke();
            };

            Button window2Btn = Root.Q<Button>("Window2");
            window2Btn.clicked += () =>
            {
                DisplayWindow2?.Invoke();
            };

            Button boxShadowBtn = Root.Q<Button>("BoxShadows");
            boxShadowBtn.clicked += () =>
            {
                DisplayBoxShadows?.Invoke();
            };

            Button card1Btn = Root.Q<Button>("Cards1");
            card1Btn.clicked += () =>
            {
                DisplayCards1?.Invoke();
            };

            Button card2Btn = Root.Q<Button>("Cards2");
            card2Btn.clicked += () =>
            {
                DisplayCards2?.Invoke();
            };

            Button card3Btn = Root.Q<Button>("Cards3");
            card3Btn.clicked += () =>
            {
                DisplayCards3?.Invoke();
            };

            Button nodeStepperBtn = Root.Q<Button>("NodeSteppers");
            nodeStepperBtn.clicked += () =>
            {
                DisplayNodeSteppers?.Invoke();
            };
        }
    }
}