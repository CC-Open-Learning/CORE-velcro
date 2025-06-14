using System.Linq;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using NUnit.Framework;
using VARLab.Velcro;

public class TableIntegrationTests
{
    private int sceneCounter;

    private GameObject tableObj;
    private UIDocument tableDoc;
    private Table tableComp;
    private VisualElement tableElement;

    private Sprite checkmarkIcon;

    [UnitySetUp]
    [Category("BuildServer")]
    public IEnumerator SetUp()
    {
        sceneCounter = TestUtils.ClearScene(sceneCounter, "TableScene");

        tableObj = new GameObject("Timer Object");
        tableDoc = tableObj.AddComponent<UIDocument>();
        tableComp = tableObj.AddComponent<Table>();

        VisualTreeAsset tableUXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/VELCRO UI/UI/Table/Table.uxml");
        VisualTreeAsset tableCategoryUXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/VELCRO UI/UI/Table/TableCategory.uxml");
        VisualTreeAsset tableEntryUXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/VELCRO UI/UI/Table/TableEntry.uxml");
        VisualTreeAsset tableElementUXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/VELCRO UI/UI/Table/TableElement.uxml");
        VisualTreeAsset tableHeaderLabelUXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/VELCRO UI/UI/Table/TableHeaderLabel.uxml");
        PanelSettings panelSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>("Assets/VELCRO UI/Settings/Panel Settings.asset");
        checkmarkIcon = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/VELCRO UI/Sprites/Checkmarks/Checkmark_Sprite.png");

        {
            //Reference panel settings and source asset as SerializedFields
            SerializedObject so = new SerializedObject(tableDoc);
            so.FindProperty("m_PanelSettings").objectReferenceValue = panelSettings;
            so.FindProperty("sourceAsset").objectReferenceValue = tableUXML;
            so.ApplyModifiedProperties();
        }
        {
            SerializedObject so = new SerializedObject(tableComp);
            so.FindProperty("categoryTemplate").objectReferenceValue = tableCategoryUXML;
            so.FindProperty("entryTemplate").objectReferenceValue = tableEntryUXML;
            so.FindProperty("elementTemplate").objectReferenceValue = tableElementUXML;
            so.FindProperty("headerLabelTemplate").objectReferenceValue = tableHeaderLabelUXML;
            so.ApplyModifiedProperties();
        }

        tableUXML.CloneTree(tableDoc.rootVisualElement);

        tableElement = tableDoc.rootVisualElement.Q("Table");

        yield return null;
    }

    [Test, Order(1)]
    [Category("BuildServer")]
    public void Start_SetsTableToDisplayNone()
    {
        // Assert
        Assert.AreEqual(new StyleEnum<DisplayStyle>(DisplayStyle.None), tableElement.style.display);
    }

    [Test, Order(2)]
    [Category("BuildServer")]
    public void HandleDisplayUI_SetsTableDisplayToFlex()
    {
        // Act
        tableComp.HandleDisplayUI("Column 1");

        // Assert
        Assert.AreEqual(new StyleEnum<DisplayStyle>(DisplayStyle.Flex), tableElement.style.display);
    }

    [Test, Order(2)]
    [Category("BuildServer")]
    public void HandleDisplayUI_WithThreeColumns_AddsThreeColumns()
    {
        // Arrange
        int expectedCount = 3;

        // Act
        tableComp.HandleDisplayUI("Column 1", "Column 2", "Column 3");

        // Assert
        Assert.AreEqual(expectedCount, tableComp.ColumnCount);
    }

    [Test, Order(4)]
    [Category("BuildServer")]
    public void AddCategory_AddsNewCategoryToList()
    {
        // Arrange
        int oldCount = tableComp.CategoryCount;

        // Act
        tableComp.AddCategory("Category 1");

        // Assert
        Assert.Greater(tableComp.CategoryCount, oldCount);
    }

    [Test, Order(5)]
    [Category("BuildServer")]
    public void AddCategory_SetsCategoryName()
    {
        // Arrange
        string expectedName = "Category 1";

        // Act
        tableComp.AddCategory("Category 1");

        // Assert
        Assert.AreEqual(expectedName, tableComp.Categories.ElementAt(0).Name);
        Assert.AreEqual(expectedName, tableElement.Q<Label>("CategoryNameLabel").text);
    }

    [Test, Order(6)]
    [Category("BuildServer")]
    public void AddCategory_AddsNewCategoryUIToTable()
    {
        // Arrange
        VisualElement categoryHolder = tableElement.Q("CategoryHolder");
        int oldCount = categoryHolder.childCount;

        // Act
        tableComp.AddCategory("Category 1");

        // Assert
        Assert.Greater(categoryHolder.childCount, oldCount);
    }

    [Test, Order(7)]
    [Category("BuildServer")]
    public void AddCategory_SetsCategoryOwnerToTable()
    {
        // Arrange
        Table expectedValue = tableComp;

        // Act
        tableComp.AddCategory("Category 1");

        // Assert
        Assert.AreEqual(expectedValue, tableComp.Categories.ElementAt(0).Owner);
    }

    [Test, Order(8)]
    [Category("BuildServer")]
    public void AddColumn_AddsNewColumn()
    {
        // Arrange
        int oldCount = tableComp.ColumnCount;

        // Act
        tableComp.AddColumn("Column 1");

        // Assert
        Assert.Greater(tableComp.ColumnCount, oldCount);
    }

    [Test, Order(9)]
    [Category("BuildServer")]
    public void AddColumn_SetsColumnLabelToName()
    {
        // Arrange
        string expectedName = "Column 1";

        // Act
        tableComp.AddColumn("Column 1");

        // Assert
        Assert.AreEqual(expectedName, tableElement.Q<Label>("HeaderLabel").text);
    }

    [Test, Order(10)]
    [Category("BuildServer")]
    public void RemoveColumn_WithValidColumnIndex_RemovesColumn()
    {
        // Arrange
        int oldCount = tableComp.ColumnCount;
        tableComp.AddColumn("Column 1");
        tableComp.AddColumn("Column 2");

        // Act
        tableComp.RemoveColumn(0);

        // Assert
        Assert.Greater(tableComp.ColumnCount, oldCount);
    }

    [Test, Order(11)]
    [Category("BuildServer")]
    public void RemoveColumn_WithValidColumnIndex_RemovesElementFromEntry()
    {
        // Arrange
        tableComp.AddColumn("Column 1");
        tableComp.AddColumn("Column 2");
        tableComp.AddCategory("Category 1");
        tableComp.Categories.ElementAt(0).AddEntry();

        int oldCount = tableComp.ColumnCount;

        // Act
        tableComp.RemoveColumn(0);

        // Assert
        Assert.Less(tableComp.ColumnCount, oldCount);
    }

    [Test, Order(12)]
    [Category("BuildServer")]
    public void RemoveColumn_WithInvalidColumnIndex_LogsError()
    {
        // Arrange
        string expectedError = "Unable to remove column at invalid index -6.";

        // Act
        tableComp.RemoveColumn(-6);

        // Assert
        LogAssert.Expect(LogType.Error, expectedError);
    }

    [Test, Order(13)]
    [Category("BuildServer")]
    public void RemoveColumn_WithValidName_RemovesColumn()
    {
        // Arrange
        int oldCount = tableComp.ColumnCount;
        tableComp.AddColumn("Column 1");
        tableComp.AddColumn("Column 2");

        // Act
        tableComp.RemoveColumn("Column 2");

        // Assert
        Assert.Greater(tableComp.ColumnCount, oldCount);
    }

    [Test, Order(14)]
    [Category("BuildServer")]
    public void RemoveColumn_WithInvalidName_LogsError()
    {
        // Arrange
        string expectedMessage = "Unable to find and remove column with name: Column 31.60";

        // Act
        tableComp.RemoveColumn("Column 31.60");

        // Assert
        LogAssert.Expect(LogType.Error, expectedMessage);
    }

    [Test, Order(15)]
    [Category("BuildServer")]
    public void RemoveAllCategories_RemovesAllCategoriesFromListAndUI()
    {
        // Arrange
        tableComp.AddCategory("Category 1");
        tableComp.AddCategory("Category 2");
        tableComp.AddCategory("Category 3");

        int oldCount = tableComp.CategoryCount;
        // Act
        tableComp.RemoveAllCategories();

        // Assert
        Assert.NotZero(oldCount);
        Assert.Zero(tableComp.CategoryCount);
    }

    [Test, Order(16)]
    [Category("BuildServer")]
    public void RemoveAllColumns_RemovesAllColumnsFromListAndUI()
    {
        // Arrange
        tableComp.AddColumn("Column 1");
        tableComp.AddColumn("Column 2");
        tableComp.AddColumn("Column 3");

        int oldCount = tableComp.ColumnCount;
        // Act
        tableComp.RemoveAllColumns();

        // Assert
        Assert.NotZero(oldCount);
        Assert.Zero(tableComp.ColumnCount);
    }

    [Test, Order(17)]
    [Category("BuildServer")]
    public void GetCategoryByName_WithValidName_ReturnsCategory()
    {
        // Arrange
        tableComp.AddCategory("Category One");
        TableCategory expectedCategory = tableComp.Categories.First();

        // Act
        TableCategory actual = tableComp.FindCategoryByName("Category One");

        // Assert
        Assert.AreEqual(expectedCategory, actual);
    }

    [Test, Order(18)]
    [Category("BuildServer")]
    public void GetCategoryByName_WithInvalidName_ReturnsNull()
    {
        // Act
        TableCategory actual = tableComp.FindCategoryByName("Category 1000");

        // Assert
        Assert.IsNull(actual);
    }

    [Test, Order(19)]
    [Category("BuildServer")]
    public void Show_SetsTableToDisplayFlex()
    {
        // Act
        tableComp.Show();

        // Assert
        Assert.AreEqual(new StyleEnum<DisplayStyle>(DisplayStyle.Flex), tableElement.style.display);
    }

    [Test, Order(20)]
    [Category("BuildServer")]
    public void Hide_SetsTableToDisplayNone()
    {
        // Arrange
        tableComp.Show();

        // Act
        tableComp.Hide();

        // Assert
        Assert.AreEqual(new StyleEnum<DisplayStyle>(DisplayStyle.None), tableElement.style.display);
    }

    [Test, Order(21)]
    [Category("BuildServer")]
    public void Show_InvokesTableShownEvent()
    {
        // Arrange
        tableComp.OnTableShown.AddListener(Ping);
        string expectedMessage = "Table event triggered!";

        // Act
        tableComp.Show();

        // Assert
        LogAssert.Expect(LogType.Log, expectedMessage);

        // Cleanup
        tableComp.OnTableShown.RemoveListener(Ping);
    }

    [Test, Order(22)]
    [Category("BuildServer")]
    public void Hide_InvokesTableHideEvent()
    {
        // Arrange
        tableComp.OnTableHidden.AddListener(Ping);
        string expectedMessage = "Table event triggered!";

        // Act
        tableComp.Hide();

        // Assert
        LogAssert.Expect(LogType.Log, expectedMessage);

        // Cleanup
        tableComp.OnTableHidden.RemoveListener(Ping);
    }

    [Test, Order(23)]
    [Category("BuildServer")]
    public void AddCategory_InvokesCategoryAddedEvent()
    {
        // Arrange
        tableComp.OnCategoryAdded.AddListener(Ping);
        string expectedMessage = "Table event triggered!";

        // Act
        tableComp.AddCategory("Category 1");

        // Assert
        LogAssert.Expect(LogType.Log, expectedMessage);

        // Cleanup
        tableComp.OnCategoryAdded.RemoveListener(Ping);
    }

    [Test, Order(24)]
    [Category("BuildServer")]
    public void TableCategory_RemoveFromTable_InvokesCategoryRemovedEvent()
    {
        // Arrange
        tableComp.OnCategoryRemoved.AddListener(Ping);
        string expectedMessage = "Table event triggered!";
        tableComp.AddCategory("Category 1");

        // Act
        tableComp.Categories.First().RemoveFromTable();

        // Assert
        LogAssert.Expect(LogType.Log, expectedMessage);

        // Cleanup
        tableComp.OnCategoryRemoved.RemoveListener(Ping);
    }

    [Test, Order(25)]
    [Category("BuildServer")]
    public void AddColumn_InvokesColumnAddedEvent()
    {
        // Arrange
        tableComp.OnColumnAdded.AddListener(Ping);
        string expectedMessage = "Table event triggered!";

        // Act
        tableComp.AddColumn("Column 1");

        // Assert
        LogAssert.Expect(LogType.Log, expectedMessage);

        // Cleanup
        tableComp.OnColumnAdded.RemoveListener(Ping);
    }

    [Test, Order(26)]
    [Category("BuildServer")]
    public void RemoveColumn_InvokesColumnRemovedEvent()
    {
        // Arrange
        tableComp.OnColumnRemoved.AddListener(Ping);
        string expectedMessage = "Table event triggered!";
        tableComp.AddColumn("Column 1");

        // Act
        tableComp.RemoveColumn(0);

        // Assert
        LogAssert.Expect(LogType.Log, expectedMessage);

        // Cleanup
        tableComp.OnColumnRemoved.RemoveListener(Ping);
    }

    [Test, Order(27)]
    [Category("BuildServer")]
    public void TableCategory_Names_SetsNameInLabel()
    {
        // Arrange
        tableComp.AddCategory("Category");
        TableCategory category = tableComp.Categories.First();
        string expectedName = "Category 1";

        // Act
        category.Name = "Category 1";

        // Assert
        Assert.AreEqual(expectedName, tableElement.Q<Label>("CategoryNameLabel").text);
    }

    [Test, Order(28)]
    [Category("BuildServer")]
    public void TableCategory_AddEntry_AddsNewEntryToListAndUI()
    {
        // Arrange
        tableComp.AddCategory("Category 1");
        TableCategory category = tableComp.Categories.First();
        VisualElement entry = tableElement.Q("EntryHolder");
        int oldEntryCount = category.EntryCount;
        int oldElementCount = entry.childCount;
        
        // Act
        category.AddEntry();

        // Assert
        Assert.Greater(category.EntryCount, oldEntryCount);
        Assert.Greater(entry.childCount, oldElementCount);
    }

    [Test, Order(29)]
    [Category("BuildServer")]
    public void TableCategory_AddEntry_InvokesEntryAddedEvent()
    {
        // Arrange
        tableComp.AddCategory("Category 1");
        TableCategory category = tableComp.Categories.First();
        category.OnEntryAdded.AddListener(Ping);
        string expectedMessage = "Table event triggered!";

        // Act
        category.AddEntry();

        // Assert
        LogAssert.Expect(LogType.Log, expectedMessage);

        // Cleanup
        category.OnEntryAdded.RemoveListener(Ping);
    }

    [Test, Order(30)]
    [Category("BuildServer")]
    public void TableCategory_RemoveAllEntries_RemovesAllEntriesFromListAndUI()
    {
        // Arrange
        tableComp.AddCategory("Category 1");
        TableCategory category = tableComp.Categories.First();
        VisualElement entry = tableElement.Q("EntryHolder");
        category.AddEntry();
        category.AddEntry();

        // Act
        category.RemoveAllEntries();

        // Assert
        Assert.Zero(category.EntryCount);
        Assert.Zero(entry.childCount);
    }

    [Test, Order(31)]
    [Category("BuildServer")]
    public void TableCategory_RemoveFromTable_RemovesCategoryFromList()
    {
        // Arrange
        tableComp.AddCategory("Category 1");
        int oldCount = tableComp.CategoryCount;

        // Act
        tableComp.Categories.First().RemoveFromTable();

        // Assert
        Assert.Less(tableComp.CategoryCount, oldCount);
    }
    
    [Test, Order(32)]
    [Category("BuildServer")]
    public void TableCategory_RemoveFromTable_RemovesCategoryFromUI()
    {
        // Arrange
        tableComp.AddCategory("Category 1");
        VisualElement categoryHolder = tableElement.Q("CategoryHolder");
        int oldCount = categoryHolder.childCount;

        // Act
        tableComp.Categories.First().RemoveFromTable();

        // Assert
        Assert.Less(categoryHolder.childCount, oldCount);
    }

    [Test, Order(33)]
    [Category("BuildServer")]
    public void TableEntry_FindElementByText_WithValidText_ReturnsElement()
    {
        // Arrange
        tableComp.HandleDisplayUI("1", "2", "3", "4", "5");
        tableComp.AddCategory("Category 1");
        TableCategory category = tableComp.Categories.First();
        category.AddEntry();

        TableEntry entry = category.Entries.First();
        entry.Elements.ElementAt(3).Text = "Element 4";
        TableElement expected = entry.Elements.ElementAt(3);

        // Act
        TableElement actual = entry.FindElementByText("Element 4");

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [Test, Order(34)]
    [Category("BuildServer")]
    public void TableEntry_FindElementByText_WithInvalidText_ReturnsNull()
    {
        // Arrange
        tableComp.AddCategory("Category 1");
        TableCategory category = tableComp.Categories.First();
        category.AddEntry();
        TableEntry entry = category.Entries.First();

        // Act
        TableElement element = entry.FindElementByText("Element 4");

        // Assert
        Assert.IsNull(element);
    }

    [Test, Order(35)]
    [Category("BuildServer")]
    public void TableEntry_RemoveFromCategory_RemovesEntryFromList()
    {
        // Arrange
        tableComp.AddCategory("Category 1");
        TableCategory category = tableComp.Categories.First();
        category.AddEntry();
        category.AddEntry();
        int oldCount = category.EntryCount;

        // Act
        category.Entries.First().RemoveFromCategory();

        // Assert
        Assert.Less(category.EntryCount, oldCount);
    }

    [Test, Order(36)]
    [Category("BuildServer")]
    public void TableEntry_RemoveFromCategory_RemovesEntryFromUI()
    {
        // Arrange
        tableComp.AddCategory("Category 1");
        TableCategory category = tableComp.Categories.First();
        category.AddEntry();
        category.AddEntry();
        VisualElement entryHolder = tableElement.Q("EntryHolder");
        int oldCount = entryHolder.childCount;

        // Act
        category.Entries.First().RemoveFromCategory();

        // Assert
        Assert.Less(entryHolder.childCount, oldCount);
    }

    [Test, Order(37)]
    [Category("BuildServer")]
    public void TableElement_Text_SetsTextInLabel()
    {
        // Arrange
        tableComp.AddColumn("Column 1");
        tableComp.AddCategory("Category 1");
        TableCategory category = tableComp.Categories.First();
        category.AddEntry();
        TableEntry entry = category.Entries.First();
        string expectedText = "Element 1";

        // Act
        entry.Elements.First().Text = "Element 1";

        // Assert
        Assert.AreEqual(expectedText, tableElement.Q("EntryHolder").Q<Label>("Label").text);
    }

    [Test, Order(38)]
    [Category("BuildServer")]
    public void TableElement_TextColour_SetsLabelColour()
    {
        // Arrange
        tableComp.AddColumn("Column 1");
        tableComp.AddCategory("Category 1");
        TableCategory category = tableComp.Categories.First();
        category.AddEntry();
        TableEntry entry = category.Entries.First();
        Color expectedColour = Color.red;

        // Act
        entry.Elements.First().TextColour = Color.red;

        // Assert
        Assert.AreEqual(expectedColour, tableElement.Q("EntryHolder").Q<Label>("Label").style.color.value);
    }

    [Test, Order(39)]
    [Category("BuildServer")]
    public void TableElement_Icon_EnablesAndSetsLabelIcon()
    {
        // Arrange
        tableComp.AddColumn("Column 1");
        tableComp.AddCategory("Category 1");
        TableCategory category = tableComp.Categories.First();
        category.AddEntry();
        TableEntry entry = category.Entries.First();
        Sprite expectedIcon = checkmarkIcon;

        VisualElement icon = tableElement.Q("EntryHolder").Q("Icon");

        // Act
        entry.Elements.First().Icon = checkmarkIcon;

        // Assert
        Assert.AreEqual(new StyleEnum<DisplayStyle>(DisplayStyle.Flex), icon.style.display);
        Assert.AreEqual(new StyleBackground(expectedIcon), icon.style.backgroundImage);
    }

    [Test, Order(40)]
    [Category("BuildServer")]
    public void TableCategory_AddEntry_SetsEntryOwners()
    {
        // Arrange
        tableComp.AddCategory("Category 1");
        TableCategory category = tableComp.Categories.First();
        Table expectedOwner = tableComp;

        // Act
        category.AddEntry();

        // Assert
        Assert.AreEqual(expectedOwner, category.Entries.First().Owner);
        Assert.AreEqual(category, category.Entries.First().Category);
    }

    [Test, Order(41)]
    [Category("BuildServer")]
    public void TableCategory_AddEntry_SetsEntryElementsOwners()
    {
        // Arrange
        tableComp.AddColumn("Column 1");
        tableComp.AddCategory("Category 1");
        TableCategory category = tableComp.Categories.First();
        Table expectedOwner = tableComp;

        // Act
        category.AddEntry();
        TableEntry entry = category.Entries.First();
        TableElement element = entry.Elements.First();
        TableEntry expectedEntry = entry;

        // Assert
        Assert.AreEqual(expectedOwner, element.Owner);
        Assert.AreEqual(expectedEntry, element.Entry);
    }

    [Test, Order(41)]
    [Category("BuildServer")]
    public void TableElement_ClearTextColour_SetsTextLabelColourToStyleKeywordNull()
    {
        // Arrange
        tableComp.AddColumn("Column 1");
        tableComp.AddCategory("Category 1");
        TableCategory category = tableComp.Categories.First();
        category.AddEntry();
        TableElement element = category.Entries.First().Elements.First();
        element.TextColour = Color.red;
        StyleColor expectedValue = StyleKeyword.Null;

        // Act
        element.ClearTextColour();

        // Assert
        Assert.AreEqual(expectedValue, tableElement.Q("EntryHolder").Q<Label>("Label").style.color);
    }

    private void Ping()
    {
        Debug.Log("Table event triggered!");
    }

    private void Ping(int n)
    {
        Ping();
    }

    private void Ping(TableCategory c)
    {
        Ping();
    }

    private void Ping(TableEntry e)
    {
        Ping();
    }
}
