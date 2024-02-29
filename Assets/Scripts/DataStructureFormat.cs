using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataStructureFormat
{
    public abstract class DataType
    {
        public enum DataFormat { Index, Float, Int, String, Bool, List, DataStruct };

        public string dataName;
        public DataFormat dataFormat;
        public string dataDescription;

        public DataType(string name, DataFormat format)
        {
            dataName = name;
            dataFormat = format;
        }
    }

    public class InputDataType : DataType
    {
        public bool isDropdownInput;
        public List<string> dropdownValues;

        public InputDataType(string name, DataFormat format, List<string> dropdownOptions = null) : base(name, format)
        {
            isDropdownInput = (dropdownOptions != null);
            dropdownValues = dropdownOptions;

        }
    }

    public class ListDataType : InputDataType
    {
        public DataType listDataType;

        public ListDataType(string name, DataType newType) : base(name, DataFormat.List)
        {
            listDataType = newType;
        }
    }

    public class DStructDataType : DataType
    {
        public System.Type relatedClassType;
        public List<DataType> content;

        public DStructDataType(string name, System.Type linkedClassType, List<DataType> newContent) : base(name, DataFormat.DataStruct)
        {
            content = newContent;
        }
    }


    public DStructDataType systemVersionDataFormat;
    public DStructDataType threatsDataFormat;

    public DataStructureFormat()
    {
        systemVersionDataFormat = new DStructDataType("SystemVersion", typeof(SystemVersion), new List<DataType>());
        systemVersionDataFormat.content.Add(new InputDataType("Index", DataType.DataFormat.Index));
        systemVersionDataFormat.content.Add(new InputDataType("Version Name", DataType.DataFormat.String));
        systemVersionDataFormat.content.Add(new ListDataType("Threats in version", new InputDataType("Index", DataType.DataFormat.Index)));

        threatsDataFormat = new DStructDataType("Threats", typeof(Threat), new List<DataType>());
        threatsDataFormat.content.Add(new InputDataType("Index", DataType.DataFormat.Index));
        threatsDataFormat.content.Add(new InputDataType("Threat Name", DataType.DataFormat.String));
        threatsDataFormat.content.Add(new InputDataType("timeBetweenSymbolChanging", DataType.DataFormat.Float));
        threatsDataFormat.content.Add(new DStructDataType("threatLocks", typeof(Threat.ThreatLock),new List<DataType>()));
        DStructDataType threatLock = threatsDataFormat.content[threatsDataFormat.content.Count - 1] as DStructDataType;
        threatLock.content.Add(new InputDataType("Threat Lock Name", DataType.DataFormat.String, new List<string> { "MA", "ML" }));
        threatLock.content.Add(new ListDataType("Paths to symbols", new InputDataType("Path to symbol", DataType.DataFormat.String)));
        threatLock.content.Add(new InputDataType("Path to sound", DataType.DataFormat.String));
        threatsDataFormat.content.Add(new InputDataType("defaultUncertaintyRange", DataType.DataFormat.Float));
        threatsDataFormat.content.Add(new InputDataType("defaultUncertaintyTime", DataType.DataFormat.Float));


    }
}
