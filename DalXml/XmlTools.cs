namespace Dal;

using DO;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

static class XMLTools
{// Static constructor: Ensures the XML directory exists, creates it if missing.

    const string s_xmlDir = @"..\xml\";
    static XMLTools()
    {
        if (!Directory.Exists(s_xmlDir))
            Directory.CreateDirectory(s_xmlDir);
    }
    // SaveListToXMLSerializer: Serializes and saves a list of objects to an XML file using XmlSerializer.

    #region SaveLoadWithXMLSerializer
    [MethodImpl(MethodImplOptions.Synchronized)]

    public static void SaveListToXMLSerializer<T>(List<T> list, string xmlFileName) where T : class
    {
        string xmlFilePath = s_xmlDir + xmlFileName;

        try
        {
            using FileStream file = new(xmlFilePath, FileMode.Create, FileAccess.Write, FileShare.None);
            new XmlSerializer(typeof(List<T>)).Serialize(file, list);
        }
        catch (Exception ex)
        {
            throw new DO.Exceptions.DalXMLFileLoadCreateException($"fail to create xml file: {s_xmlDir + xmlFilePath}, {ex.Message}");
        }
    }
    // LoadListFromXMLSerializer: Loads and deserializes a list of objects from an XML file; returns empty list if file missing.
    [MethodImpl(MethodImplOptions.Synchronized)]

    public static List<T> LoadListFromXMLSerializer<T>(string xmlFileName) where T : class
    {
        string xmlFilePath = s_xmlDir + xmlFileName;

        try
        {
            if (!File.Exists(xmlFilePath)) return new();
            using FileStream file = new(xmlFilePath, FileMode.Open);
            XmlSerializer x = new(typeof(List<T>));
            return x.Deserialize(file) as List<T> ?? new();
        }
        catch (Exception ex)
        {
            throw new DO.Exceptions.DalXMLFileLoadCreateException($"fail to load xml file: {xmlFilePath}, {ex.Message}");
        }
    }
    #endregion
    // SaveListToXMLElement: Saves an XElement (XML tree) to an XML file.

    #region SaveLoadWithXElement
    [MethodImpl(MethodImplOptions.Synchronized)]

    public static void SaveListToXMLElement(XElement rootElem, string xmlFileName)
    {
        string xmlFilePath = s_xmlDir + xmlFileName;

        try
        {
            rootElem.Save(xmlFilePath);
        }
        catch (Exception ex)
        {
            throw new DO.Exceptions.DalXMLFileLoadCreateException($"fail to create xml file: {s_xmlDir + xmlFilePath}, {ex.Message}");
        }
    }
    // LoadListFromXMLElement: Loads an XElement from an XML file; creates and saves a new empty XElement if file missing.
    [MethodImpl(MethodImplOptions.Synchronized)]

    public static XElement LoadListFromXMLElement(string xmlFileName)
    {
        string xmlFilePath = s_xmlDir + xmlFileName;

        try
        {
            if (File.Exists(xmlFilePath))
                return XElement.Load(xmlFilePath);
            XElement rootElem = new(xmlFileName);
            rootElem.Save(xmlFilePath);
            return rootElem;
        }
        catch (Exception ex)
        {
            throw new DO.Exceptions.DalXMLFileLoadCreateException($"fail to load xml file: {s_xmlDir + xmlFilePath}, {ex.Message}");
        }
    }
    #endregion

    // GetAndIncreaseConfigIntVal: Retrieves an integer config value from XML, increments it, saves, and returns the original value.

    #region XmlConfig
    [MethodImpl(MethodImplOptions.Synchronized)]

    public static int GetAndIncreaseConfigIntVal(string xmlFileName, string elemName)
    {
        XElement root = XMLTools.LoadListFromXMLElement(xmlFileName);
        int nextId = root.ToIntNullable(elemName) ?? throw new FormatException($"can't convert:  {xmlFileName}, {elemName}");
        root.Element(elemName)?.SetValue((nextId + 1).ToString());
        XMLTools.SaveListToXMLElement(root, xmlFileName);
        return nextId;
    }
    // GetConfigIntVal: Retrieves an integer config value from XML without changing it.
    [MethodImpl(MethodImplOptions.Synchronized)]

    public static int GetConfigIntVal(string xmlFileName, string elemName)
    {
        XElement root = XMLTools.LoadListFromXMLElement(xmlFileName);
        int num = root.ToIntNullable(elemName) ?? throw new FormatException($"can't convert:  {xmlFileName}, {elemName}");
        return num;
    }
    // GetConfigDateVal: Retrieves a DateTime config value from XML.
    [MethodImpl(MethodImplOptions.Synchronized)]

    public static DateTime GetConfigDateVal(string xmlFileName, string elemName)
    {
        XElement root = XMLTools.LoadListFromXMLElement(xmlFileName);
        DateTime dt = root.ToDateTimeNullable(elemName) ?? throw new FormatException($"can't convert:  {xmlFileName}, {elemName}");
        return dt;
    }
    // GetConfigTimeVal: Retrieves a TimeSpan config value from XML; returns TimeSpan.MinValue if missing.
    [MethodImpl(MethodImplOptions.Synchronized)]

    public static TimeSpan GetConfigTimeVal(string xmlFileName, string elemName)
    {
        XElement root = XMLTools.LoadListFromXMLElement(xmlFileName);

        TimeSpan dateTimeValue = root.ToTimeSpanNullable(elemName) ?? TimeSpan.MinValue;


        // Assuming the TimeSpan is the difference from now to the dateTimeValue
        return dateTimeValue;
    }

    //public static TimeSpan GetConfigRiskRenge(string xmlFileName, string elemName)
    //{
    //    XElement root = XMLTools.LoadListFromXMLElement(xmlFileName);
    //    DateTime? dateTimeValue = root.ToDateTimeNullable(elemName);

    //    if (!dateTimeValue.HasValue)
    //    {
    //        throw new FormatException($"Can't convert: {xmlFileName}, {elemName}");
    //    }

    //    // Assuming the TimeSpan is the difference from now to the dateTimeValue
    //    return DateTime.Now - dateTimeValue.Value;
    //}

    // SetConfigDateVal: Sets and saves a DateTime config value in the XML file.
    [MethodImpl(MethodImplOptions.Synchronized)]

    public static void SetConfigIntVal(string xmlFileName, string elemName, int elemVal)
    {
        XElement root = XMLTools.LoadListFromXMLElement(xmlFileName);
        root.Element(elemName)?.SetValue((elemVal).ToString());
        XMLTools.SaveListToXMLElement(root, xmlFileName);
    }
    [MethodImpl(MethodImplOptions.Synchronized)]

    public static void SetConfigDateVal(string xmlFileName, string elemName, DateTime elemVal)
    {
        XElement root = XMLTools.LoadListFromXMLElement(xmlFileName);
        root.Element(elemName)?.SetValue((elemVal).ToString());
        XMLTools.SaveListToXMLElement(root, xmlFileName);
    }
    // SetConfigTimeVal: Sets and saves a TimeSpan config value in the XML file.
    [MethodImpl(MethodImplOptions.Synchronized)]

    public static void SetConfigTimeVal(string xmlFileName, string elemName, TimeSpan elemVal)
    {
        XElement root = XMLTools.LoadListFromXMLElement(xmlFileName);
        root.Element(elemName)?.SetValue((elemVal).ToString());
        XMLTools.SaveListToXMLElement(root, xmlFileName);
    }
    #endregion
    // Extension functions (ToEnumNullable, ToDateTimeNullable, ToTimeSpanNullable, ToDoubleNullable, ToIntNullable):
    //    Safely parse XML element values to nullable types, returning null if parsing fai

    #region ExtensionFuctions
    [MethodImpl(MethodImplOptions.Synchronized)]

    public static T? ToEnumNullable<T>(this XElement element, string name) where T : struct, Enum =>
        Enum.TryParse<T>((string?)element.Element(name), out var result) ? (T?)result : null;
    [MethodImpl(MethodImplOptions.Synchronized)]

    public static DateTime? ToDateTimeNullable(this XElement element, string name) =>
        DateTime.TryParse((string?)element.Element(name), out var result) ? (DateTime?)result : null;
    [MethodImpl(MethodImplOptions.Synchronized)]

    public static TimeSpan? ToTimeSpanNullable(this XElement element, string name) =>
        TimeSpan.TryParse((string?)element.Element(name), out var result) ? (TimeSpan?)result : null;
    [MethodImpl(MethodImplOptions.Synchronized)]

    public static double? ToDoubleNullable(this XElement element, string name) =>
        double.TryParse((string?)element.Element(name), out var result) ? (double?)result : null;
    [MethodImpl(MethodImplOptions.Synchronized)]

    public static int? ToIntNullable(this XElement element, string name) =>
        int.TryParse((string?)element.Element(name), out var result) ? (int?)result : null;
    #endregion

}