using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace VARLab.Velcro.Demos {
    public class ProgressIndicatorDemoController : Toolbar
    {
        [SerializeField] private ProgressIndicatorV1 progressIndicator;

        public UnityEvent DisplayTemplateOne;
        public UnityEvent HideTemplateOne;
        public UnityEvent DisplayTemplateTwo;
        public UnityEvent HideTemplateTwo;

        private bool isTemplateOneVisible = false;
        private bool isTemplateTwoVisible = false;
        private bool isProgressIndicatorVisible = false;

        private void Start()
        {
            SetupBaseToolbar();

            DisplayTemplateOne ??= new UnityEvent();
            HideTemplateOne ??= new UnityEvent();
            DisplayTemplateTwo ??= new UnityEvent();
            HideTemplateTwo ??= new UnityEvent();

            Button indicatorOneBtn = Root.Q("IndicatorOne").Q<Button>();
            indicatorOneBtn.clicked += () =>
            {
                if (isTemplateOneVisible)
                {
                    HideTemplateOne?.Invoke();
                }
                else
                {
                    DisplayTemplateOne?.Invoke();
                }

                isTemplateOneVisible = !isTemplateOneVisible;
            };

            Button indicatorTwoBtn = Root.Q("IndicatorTwo").Q<Button>();
            indicatorTwoBtn.clicked += () =>
            {
                if (isTemplateTwoVisible)
                {
                    HideTemplateTwo?.Invoke();
                }
                else
                {
                    DisplayTemplateTwo?.Invoke();
                }

                isTemplateTwoVisible = !isTemplateTwoVisible;
            };

            Button indicatorThreeBtn = Root.Q("IndicatorThree").Q<Button>();
            indicatorThreeBtn.clicked += () =>
            {
                if (isProgressIndicatorVisible)
                {
                    progressIndicator.Hide();

                    //Remove all categories so the next time it's populated, they aren't duplicated
                    for (int i = progressIndicator.CategoryCount - 1; i >= 0; i--)
                    {
                        progressIndicator.RemoveCategory(i);
                    }
                }
                else
                {
                    PopulateIndicator();
                    progressIndicator.Show();
                }

                isProgressIndicatorVisible = !isProgressIndicatorVisible;
            };

            Button addProgressBtn = Root.Q("AddProgress").Q<Button>();
            addProgressBtn.clicked += () =>
            {
                AddProgress();
            };

            Button removeProgressBtn = Root.Q("RemoveProgress").Q<Button>();
            removeProgressBtn.clicked += () =>
            {
                RemoveProgress();
            };

            Button removeCategoryBtn = Root.Q("RemoveCategory").Q<Button>();
            removeCategoryBtn.clicked += () =>
            {
                RemoveCategory();
            };

            Button removeTaskBtn = Root.Q("RemoveTask").Q<Button>();
            removeTaskBtn.clicked += () =>
            {
                RemoveTask();
            };
        }

        private void PopulateIndicator()
        {
            //You can use the inspector or add at runtime
            progressIndicator.AddCategory("Category 1");
            progressIndicator.AddTask(0, "Task 1-1", Random.Range(1, 2));
            progressIndicator.AddTask(0, "Task 1-2", Random.Range(1, 2));
            progressIndicator.AddTask(0, "Task 1-3", Random.Range(1, 2));
            progressIndicator.AddTask(0, "Task 1-4", Random.Range(1, 2));
            progressIndicator.AddCategory("Category 2");
            progressIndicator.AddTask(1, "Task 2-1", Random.Range(2, 4));
            progressIndicator.AddTask(1, "Task 2-2", Random.Range(2, 4));
            progressIndicator.AddCategory("Category 3");
            progressIndicator.AddTask(2, "Task 3-1", Random.Range(3, 5));
            progressIndicator.AddTask(2, "Task 3-2", Random.Range(2, 5));
        }

        private void AddProgress()
        {
            int categoryCount = progressIndicator.CategoryCount;
            int category = Random.Range(0, categoryCount);

            int taskCount = progressIndicator.GetTaskCount(category);
            int task = Random.Range(0, taskCount);
            progressIndicator.AddProgressToTask(category, task);
        }

        private void RemoveProgress()
        {
            int categoryCount = progressIndicator.CategoryCount;
            int category = Random.Range(0, categoryCount);

            int taskCount = progressIndicator.GetTaskCount(category);
            int task = Random.Range(0, taskCount);
            progressIndicator.RemoveProgressFromTask(category, task);
        }

        private void RemoveCategory()
        {
            if (progressIndicator.CategoryCount > 0)
            {
                int last = progressIndicator.CategoryCount - 1;
                progressIndicator.RemoveCategory(last);
            }
        }

        private void RemoveTask()
        {
            int lastCat = progressIndicator.CategoryCount - 1;
            if (progressIndicator.CategoryCount > 0 && progressIndicator.GetTaskCount(lastCat) > 0)
            {
                int lastTask = progressIndicator.GetTaskCount(lastCat) - 1;
                progressIndicator.RemoveTask(lastCat, lastTask);
            }
        }
    }
}