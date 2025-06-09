using System.Linq;
using System.Collections;
using UnityEngine;

namespace VARLab.Velcro.Demos
{
    public class TableDemoController : Toolbar
    {
        [Header("Table Demo")]
        [SerializeField]
        private Table table;

        [SerializeField]
        private Sprite icon;

        private IEnumerator Start()
        {
            yield return null;
            SetupBaseToolbar();

            table.HandleDisplayUI("Column 1", "Column 2", "Column 3", "Column 4", "Column 5");
            table.AddCategory("Category 1");

            TableCategory category = table.FindCategoryByName("Category 1");
            category.AddEntry();
            category.AddEntry();
            category.AddEntry();
            category.AddEntry();

            TableEntry entry = category.Entries.ElementAt(0);
            entry.Elements.ElementAt(0).Text = "First";
            entry.Elements.ElementAt(1).Text = "Second";
            entry.Elements.ElementAt(1).Icon = icon;
            entry.Elements.ElementAt(2).Text = "Third";
            entry.Elements.ElementAt(2).TextColour = Color.cyan;

            table.AddCategory("Category 2");
            TableCategory category2 = table.FindCategoryByName("Category 2");
            category2.AddEntry();
            category2.AddEntry();
            category2.AddEntry();
            category2.AddEntry();
        }
    }
}