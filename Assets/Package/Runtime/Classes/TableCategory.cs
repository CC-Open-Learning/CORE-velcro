using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace VARLab.Velcro
{
    /// <summary>
    /// Class that represents a category in a table.
    /// </summary>
    public class TableCategory
    {
        /// <summary>
        /// The table which the category is contained in.
        /// </summary>
        public Table Owner { get; private set; }

        private string name;

        /// <summary>
        /// The name of the category.
        /// </summary>
        public string Name
        {
            get => name;
            set
            {
                name = value;
                if (nameLabel != null)
                {
                    nameLabel.text = name;
                }
            }
        }

        // List of entries
        private List<TableEntry> entries = new List<TableEntry>();
        
        /// <summary>
        /// The list of entries contained inside the category.
        /// </summary>
        public IEnumerable<TableEntry> Entries => entries;

        /// <summary>
        /// The amount of entries contained in the category.
        /// </summary>
        public int EntryCount => entries.Count;

        // UI references
        private VisualElement categoryElement;
        private Label nameLabel;
        internal VisualElement entryHolder;

        /// <summary>
        /// Invoked when a new entry has been added to the category.
        /// </summary>
        public UnityEvent<TableEntry> OnEntryAdded;

        /// <summary>
        /// Invoked when an entry is removed from the category.
        /// </summary>
        public UnityEvent<TableEntry> OnEntryRemoved;

        /// <summary>
        /// Table category constructor.
        /// </summary>
        public TableCategory()
        {
            OnEntryAdded ??= new UnityEvent<TableEntry>();
            OnEntryRemoved ??= new UnityEvent<TableEntry>();
        }

        // Sets the owner table of the category
        internal void SetOwner(Table owner)
        {
            Owner = owner;
        }

        // Sets the UI references for the category 
        internal void SetUIReferences(VisualElement categoryElement, Label nameLabel, VisualElement entryHolder)
        {
            this.categoryElement = categoryElement;
            this.nameLabel = nameLabel;
            this.entryHolder = entryHolder;
        }

        /// <summary>
        /// Adds a new entry to the category.
        /// </summary>
        public void AddEntry()
        {
            VisualElement newEntry = Owner.EntryTemplate.CloneTree();
            newEntry.name = $"Element {EntryCount + 1}";
            VisualElement elementHolder = newEntry.Q("Entry");
            Button deleteButton = elementHolder.Q<Button>("DeleteButton");
            entryHolder.Add(newEntry);

            TableEntry entry = new TableEntry();
            entry.SetOwner(Owner);
            entry.SetCategory(this);
            entry.SetUIReferences(newEntry, elementHolder, deleteButton);
            entry.GenerateElements();
            entries.Add(entry);

            deleteButton.RegisterCallback<ClickEvent, TableEntry>(RemoveEntryFromListAndUI, entry);

            OnEntryAdded.Invoke(entry);
        }

        // Button callback
        private void RemoveEntryFromListAndUI(ClickEvent evt, TableEntry entry)
        {
            entry.RemoveFromCategory();
        }

        /// <summary>
        /// Removes all entries from the category.
        /// </summary>
        public void RemoveAllEntries()
        {
            for (int i = entries.Count - 1; i > -1; i--)
            {
                entries[i].RemoveFromCategory();
            }
        }

        /// <summary>
        /// Removes the category UI element from the table
        /// </summary>
        public void RemoveFromTable()
        {
            Owner.RemoveCategory(this);
            categoryElement.RemoveFromHierarchy();
        }

        // Internal use only
        // Used for the TableEntry.RemoveFromCategory() method.
        internal void RemoveEntry(TableEntry entry)
        {
            entries.Remove(entry);
            OnEntryRemoved.Invoke(entry);
            entry.deleteButton?.UnregisterCallback<ClickEvent, TableEntry>(RemoveEntryFromListAndUI);
        }
    }
}
