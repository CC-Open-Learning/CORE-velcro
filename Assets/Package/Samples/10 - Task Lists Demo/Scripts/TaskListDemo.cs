using UnityEngine;
using UnityEngine.UIElements;

namespace VARLab.Velcro.Demos
{
    public class TaskListDemo : Toolbar
    {
        [Header("Task List Demo")]
        [SerializeField]
        private TaskList taskList;

        public TaskListSO taskListSO1;
        public TaskListSO taskListSO2;

        private Button showButton1;
        private Button showButton2;
        private Button addButton;
        private Button removeButton;
        private Button completeButton;

        private void Start()
        {
            SetupBaseToolbar();

            showButton1 = Root.Q("ShowButton1").Q<Button>();
            showButton2 = Root.Q("ShowButton2").Q<Button>();
            addButton = Root.Q("AddButton").Q<Button>();
            removeButton = Root.Q("RemoveButton").Q<Button>();
            completeButton = Root.Q("CompleteButton").Q<Button>();

            showButton1.clicked += () => ShowList1();
            showButton2.clicked += () => ShowList2();
            addButton.clicked += () => AddTask();
            removeButton.clicked += () => RemoveTask();
            completeButton.clicked += () => CompleteTask();
        }

        private void ShowList1()
        {
            taskList.CreateTaskListFromSO(taskListSO1);
            taskList.Show();
        }

        private void ShowList2()
        {
            taskList.CreateTaskListFromSO(taskListSO2);
            taskList.Show();
        }

        private void AddTask()
        {
            taskList.AddTask($"New task {taskList.Count + 1}", $"Hint for new task {taskList.Count + 1}");
        }

        private void RemoveTask()
        {
            taskList.RemoveTask(taskList.Tasks[taskList.Count - 1].Name);
        }

        private void CompleteTask()
        {
            if (taskList.Count <= 0 || taskList.IsComplete)
            {
                return;
            }

            int randomTaskIndex = -1;
            while (true)
            {
                randomTaskIndex = Random.Range(0, taskList.Count);

                if (!taskList.IsTaskComplete(taskList.Tasks[randomTaskIndex].Name))
                {
                    break;
                }
            }

            taskList.SetTaskComplete(taskList.Tasks[randomTaskIndex].Name, true);
        }
    }
}