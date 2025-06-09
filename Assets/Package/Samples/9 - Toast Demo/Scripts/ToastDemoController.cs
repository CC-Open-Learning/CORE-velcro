using UnityEngine;
using UnityEngine.UIElements;

namespace VARLab.Velcro.Demos
{
    public class ToastDemoController : Toolbar
    {
        [SerializeField] ToastSimpleSO toastSimpleSO;
        [SerializeField] ToastComplexSO toastComplexSO;

        [SerializeField] ToastSimple toastSimple;
        [SerializeField] ToastComplex toastComplex;

        private void Start()
        {
            SetupBaseToolbar();

            Button simpleBtn1 = Root.Q<Button>("Simple1");
            simpleBtn1.clicked += () =>
            {
                toastSimple.HandleDisplayUI("Hint", "You can change all labels of toasts", ToastType.Hint, Align.FlexStart);
            };

            Button simpleBtn2 = Root.Q<Button>("Simple2");
            simpleBtn2.clicked += () =>
            {
                toastSimple.HandleDisplayUI("Hint", "If you don't provide a sprite for your custom toast, it will change to Display.None");
            };

            Button simpleBtn3 = Root.Q<Button>("Simple3");
            simpleBtn3.clicked += () =>
            {
                toastSimple.HandleDisplayUI(toastSimpleSO);
            };

            Button simpleCustomBtn = Root.Q<Button>("SimpleCustom");
            simpleCustomBtn.clicked += () =>
            {
                toastSimple.SetCustomToast(Color.grey, Color.magenta, Color.black, null);
                toastSimple.HandleDisplayUI("Custom", "This is a custom toast without a sprite", ToastType.Custom, Align.Center);
            };

            Button simpleHideBtn = Root.Q<Button>("SimpleHide");
            simpleHideBtn.clicked += () =>
            {
                toastSimple.CloseToast();
            };

            Button complexBtn1 = Root.Q<Button>("Complex1");
            complexBtn1.clicked += () =>
            {
                toastComplex.HandleDisplayUI("Up here", "FlexStart", "You can align the toast to the top (flex start), bottom (flex end) or center (center)", ToastType.Location, Align.FlexStart);
            };

            Button complexBtn2 = Root.Q<Button>("Complex2");
            complexBtn2.clicked += () =>
            {
                toastComplex.HandleDisplayUI("Complex", "Scaling", "The toast will continue to scale vertically. The message container takes grow priority over the header, and the secondary message label takes grow priority over the primary label");
            };

            Button complexBtn3 = Root.Q<Button>("Complex3");
            complexBtn3.clicked += () =>
            {
                toastComplex.HandleDisplayUI(toastComplexSO);
            };

            Button complexCustomBtn = Root.Q<Button>("ComplexCustom");
            complexCustomBtn.clicked += () =>
            {
                toastComplex.SetCustomToast(Color.grey, Color.cyan, Color.black, null);
                toastComplex.HandleDisplayUI("Custom", "Toast", "This is a custom toast without a sprite", ToastType.Custom, Align.Center);
            };

            Button complexHideBtn = Root.Q<Button>("ComplexHide");
            complexHideBtn.clicked += () =>
            {
                toastComplex.CloseToast();
            };
        }
    }
}