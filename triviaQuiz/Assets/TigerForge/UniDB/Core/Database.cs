using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using Random = UnityEngine.Random;

/// UniDB v.1.2
namespace TigerForge.UniDB
{

    #region " DATABASE class "

    /// <summary>
    /// The Database engine.
    /// </summary>
    public class Database
    {
        /// <summary>
        /// Activate the debug mode for showing useful information on the Unity Console.
        /// </summary>
        public static bool debugMode = false;

        /// <summary>
        /// Show in the Console the various application related paths.
        /// </summary>
        public static void ShowApplicationPaths()
        {
            string baseDir = System.AppDomain.CurrentDomain.BaseDirectory;
            string currDir = System.Environment.CurrentDirectory;
            string absURL = Application.absoluteURL;
            string dataPath = Application.dataPath;
            string perPath = Application.persistentDataPath;
            string assPath = Application.streamingAssetsPath;

            Debug.Log("BASE URL: " + baseDir);
            Debug.Log("CURRENT URL: " + currDir);
            Debug.Log("ABSOLUTE URL: " + absURL);
            Debug.Log("DATA FOLDER: " + dataPath);
            Debug.Log("PERSISTEN DATA FOLDER: " + perPath);
            Debug.Log("ASSETS FOLDER: " + assPath);
        }

        protected string _ID = "";
        public virtual string ID
        {
            get
            {
                return _ID;
            }
            set
            {
                _ID = value;
            }
        }

        public virtual string GetID()
        {
            return _ID;
        }

        protected string _Type = "";
        public virtual string Type
        {
            get
            {
                return _Type;
            }
            set
            {
                _Type = value;
            }
        }

        public virtual string GetDBType()
        {
            return _Type;
        }

        protected string _Key = "";
        public virtual string Key
        {
            get
            {
                return _Key;
            }
            set
            {
                _Key = value;
            }
        }

        public virtual string GetKey()
        {
            return _Key;
        }
                
    }

    #endregion

    #region " SPECIAL CLASSES "

    /// <summary>
    /// A set of information about a Database operation.
    /// </summary>
    public class Info
    {
        /// <summary>
        /// The Query executed on the Database.
        /// </summary>
        public string query = "";
        /// <summary>
        /// The ID of the affected Record.
        /// </summary>
        public int id = 0;
        /// <summary>
        /// The number of Rows involved in this operation.
        /// </summary>
        public int affectedRows = 0;
        /// <summary>
        /// The numeric result of this operation.
        /// </summary>
        public float result = 0;
        /// <summary>
        /// The boolean result of this operation. It's initialized by the Exists() method.
        /// </summary>
        public bool exists = false;
        /// <summary>
        /// The error message from the Server. It may include an error code.
        /// </summary>
        public string error = "";
        /// <summary>
        /// The operation status (OK or ERROR).
        /// </summary>
        public string status = "";
        /// <summary>
        /// True if the operation returned some Record.
        /// </summary>
        public bool hasData = false;
        /// <summary>
        /// True if the operation returned no Records.
        /// </summary>
        public bool isEmpty = false;
        /// <summary>
        /// True if the operation succeeded.
        /// </summary>
        public bool isOK = false;
        /// <summary>
        /// True if the operation rised an error.
        /// </summary>
        public bool isError = false;
        /// <summary>
        /// The string result of a query. It may contains the raw result to convert in the proper way.
        /// </summary>
        public string rawData = "";
        /// <summary>
        /// The request code.
        /// </summary>
        public string code = "";
    }

    /// <summary>
    /// Represent a condition between a column's value and a given value.
    /// </summary>
    public class Condition
    {
        public string symbol = "";
        public string column = "";
        public object value = null;
    }

    /// <summary>
    /// Represent an ordering criteria.
    /// </summary>
    public class Order
    {
        public Column column;
        public enum Direction
        {
            ASC,
            DESC
        }
        public Direction order = Direction.ASC;
    }

    public class Column
    {
        /// <summary>
        /// The name of the Column.
        /// </summary>
        public string name = "";
        /// <summary>
        /// The name of the Table this Column belongs to.
        /// </summary>
        public string table = "";
        public object _value = null;
        public object _value2 = null;
        public bool _valueJsonEncoded = false;

        public Column(string name, string table)
        {
            this.name = name;
            this.table = table;
        }
        /// <summary>
        /// "Equal to" condition: column's value = value
        /// </summary>
        public Condition Equal(object value)
        {
            return new Condition { column = name, value = value, symbol = "" };
        }
        /// <summary>
        /// "Greater than" condition: column's value > value
        /// </summary>
        public Condition Greater(object value)
        {
            return new Condition { column = name + "[>]", value = value, symbol = "[>]" };
        }
        /// <summary>
        /// "Less than" condition: column's value < value
        /// </summary>
        public Condition Less(object value)
        {
            return new Condition { column = name + "[<]", value = value, symbol = "[<]" };
        }
        /// <summary>
        /// "Greater than or equal to" condition: column's value >= value
        /// </summary>
        public Condition GreaterEqual(object value)
        {
            return new Condition { column = name + "[>=]", value = value, symbol = "[>=]" };
        }
        /// <summary>
        /// "Less than or equal to" condition: column's value <= value
        /// </summary>
        public Condition LessEqual(object value)
        {
            return new Condition { column = name + "[<=]", value = value, symbol = "[<=]" };
        }
        /// <summary>
        /// "Not equal to" condition: column's value != value
        /// </summary>
        public Condition NotEqual(object value)
        {
            return new Condition { column = name + "[!=]", value = value, symbol = "[!=]" };
        }
        /// <summary>
        /// "Like" condition: column's value LIKE value
        /// </summary>
        public Condition Like(object value)
        {
            return new Condition { column = name + "[~]", value = value, symbol = "[~]" };
        }
        /// <summary>
        /// "Not like" condition: column's value NOT LIKE value
        /// </summary>
        public Condition NotLike(object value)
        {
            return new Condition { column = name + "[!~]", value = value, symbol = "[!~]" };
        }
        /// <summary>
        /// "Between" condition: column's value BETWEEN value1 AND value 2
        /// </summary>
        public Condition Between(object value1, object value2)
        {
            return new Condition { column = name + "[<>]", value = new object[] { value1, value2 }, symbol = "[<>]" };
        }
        /// <summary>
        /// "Not between" condition: column's value NOT BETWEEN value1 AND value 2
        /// </summary>
        public Condition NotBetween(object value1, object value2)
        {
            return new Condition { column = name + "[><]", value = new object[] { value1, value2 }, symbol = "[><]" };
        }
        /// <summary>
        /// "In" condition: column's value IN value1, value2, ... , valueX
        /// </summary>
        public Condition In(params object[] values)
        {
            return new Condition { column = name, value = values, symbol = "" };
        }
        /// <summary>
        /// "Not in" condition: column's value NOT IN value1, value2, ... , valueX
        /// </summary>
        public Condition NotIn(params object[] values)
        {
            return new Condition { column = name + "[!]", value = values, symbol = "[!]" };
        }
        /// <summary>
        /// "Is null" condition: column's value IS NULL
        /// </summary>
        public Condition IsNull()
        {
            return new Condition { column = name, value = null, symbol = "" };
        }
        /// <summary>
        /// "Is not null" condition: column's value IS NOT NULL
        /// </summary>
        public Condition NotNull()
        {
            return new Condition { column = name + "[!]", value = null, symbol = "[!]" };
        }
        /// <summary>
        /// "Is true" condition: column's value = 1 (true)
        /// </summary>
        public Condition IsTrue()
        {
            return new Condition { column = name, value = 1, symbol = "" };
        }
        /// <summary>
        /// "Is false" condition: column's value = 0 (false)
        /// </summary>
        public Condition IsFalse()
        {
            return new Condition { column = name, value = 0, symbol = "" };
        }

        /// <summary>
        /// Assign an "alias" to this Column's name and return that column's value under the given new name.
        /// </summary>
        public string AS(string newName)
        {
            return name + " (" + newName + ")";
        }

        /// <summary>
        /// Assign the DISTINCT clausule to this column.
        /// </summary>
        public string Distinct()
        {
            return "@" + name;
        }

        /// <summary>
        /// Assign a value to this Column. This value will be used in the Insert/Update operations.
        /// <para>Insert : the value of this Column.</para>
        /// <para>Update : the new value of this Column or a mathematical operation using the symbols {+}, {-}, {*}, {/}.</para>
        /// </summary>
        public Column Value(object value, bool jsonEncode = false)
        {
            this._value = value;
            _valueJsonEncoded = jsonEncode;
            return this;
        }

        /// <summary>
        /// Assign a replacement rule to this Column. The given list must be a sequence of old string / new string. The given old string/substring will be replaced by the given new string.
        /// <para>Note: this method is involved into the Replace operation.</para>
        /// </summary>
        public Column Replace(params string[] values)
        {
            _value = string.Join("|", values);
            return this;
        }
    }

    public class AND
    {
        /// <summary>
        /// Generate a Query where the couples column/value are linked by AND.
        /// </summary>
        public static Dictionary<string, object> Q(params Condition[] condition)
        {
            var c = new Dictionary<string, object>();
            for (var i = 0; i < condition.Length; i++) c.Add(condition[i].column + " #" + i, condition[i].value);
            var AND = new Dictionary<string, object>();
            AND.Add("AND", c);

            return AND;
        }

        /// <summary>
        /// Generate a Query where the given conditions are linked by AND.
        /// </summary>
        public static Dictionary<string, object> Q(params Dictionary<string, object>[] conditions)
        {
            var COND = new Dictionary<string, object>();
            for (var i = 0; i < conditions.Length; i++) COND.Add("AND #" + i, conditions[i]);
            var AND = new Dictionary<string, object>();
            AND.Add("AND #X", COND);

            return AND;
        }
    }

    public class OR
    {
        /// <summary>
        /// Generate a Query where the couples column/value are linked by OR.
        /// </summary>
        public static Dictionary<string, object> Q(params Condition[] condition)
        {
            var c = new Dictionary<string, object>();
            for (var i = 0; i < condition.Length; i++) c.Add(condition[i].column + " #C_" + i, condition[i].value);
            var OR = new Dictionary<string, object>();
            OR.Add("OR #" + Random.Range(0, 10000), c);

            return OR;
        }

        /// <summary>
        /// Generate a Query where the given conditions are linked by OR.
        /// </summary>
        public static Dictionary<string, object> Q(params Dictionary<string, object>[] conditions)
        {
            var COND = new Dictionary<string, object>();
            for (var i = 0; i < conditions.Length; i++) COND.Add("AND #" + i, conditions[i]);

            var OR = new Dictionary<string, object>();
            OR.Add("OR #X", COND);

            return OR;
        }
    }

    #endregion

    public class Table
    {
        #region " PROPERTIES "

        private string table;
        private string columns;
        private Dictionary<string, object> where;
        private Dictionary<string, object> join;
        private List<Dictionary<string, object>> recordData;
        private string extra;

        private string action;
        private string dbKey;
        private string token;
        private string key;
        private string dbURL;
        private string dbID;
        private string dbType;
        private Database db;

        protected string _name = "";
        public virtual string name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }
        public virtual string GetName()
        {
            return _name;
        }


        protected Database _parent;
        public virtual Database parent
        {
            get
            {
                return _parent;
            }
            set
            {
                _parent = value;
            }
        }
        public virtual Database GetParent()
        {
            return _parent;
        }

        #endregion

        #region " - SELECT "

        /// <summary>
        /// Initialize a SELECT operation on this Table for reading just one Record. The returned Record will have only the listed columns' values.
        /// <para>columns : the list of the columns to read.</para>
        /// </summary>
        public Table SelectOne(params Column[] columns)
        {
            action = "SELECTONE";
            this.columns = GetColumns(columns);
            where = new Dictionary<string, object>();
            join = new Dictionary<string, object>();
            return this;
        }
        /// <summary>
        /// Initialize a SELECT operation on this Table for reading just one Record. The returned Record will have all the available columns' values.
        /// </summary>
        public Table SelectOne()
        {
            action = "SELECTONE";
            columns = "*";
            where = new Dictionary<string, object>();
            join = new Dictionary<string, object>();
            return this;
        }

        /// <summary>
        /// Initialize a SELECT operation on this Table. Returned Records will have only the listed columns' values.
        /// <para>columns : the list of the columns to read.</para>
        /// </summary>
        public Table Select(params Column[] columns)
        {
            action = "SELECT";
            this.columns = GetColumns(columns);
            where = new Dictionary<string, object>();
            join = new Dictionary<string, object>();
            return this;
        }
        /// <summary>
        /// Initialize a SELECT operation on this Table. Returned Records will have all the available columns' values.
        /// </summary>
        public Table Select()
        {
            action = "SELECT";
            columns = "*";
            where = new Dictionary<string, object>();
            join = new Dictionary<string, object>();
            return this;
        }

        /// <summary>
        /// Initialize a SELECT operation on this Table, fetching Records randomly. Returned Records will have only the listed columns' values.
        /// <para>columns : the list of the columns to read.</para>
        /// </summary>
        public Table SelectRand(params Column[] columns)
        {
            action = "RANDSELECT";
            this.columns = GetColumns(columns);
            where = new Dictionary<string, object>();
            join = new Dictionary<string, object>();
            return this;
        }
        /// <summary>
        /// Initialize a SELECT operation on this Table, fetching Records randomly. Returned Records will have all the available columns' values.
        /// </summary>
        public Table SelectRand()
        {
            action = "RANDSELECT";
            columns = "*";
            where = new Dictionary<string, object>();
            join = new Dictionary<string, object>();
            return this;
        }

        #endregion

        #region " - WHERE "

        /// <summary>
        /// Initialize a WHERE condition with the given single Condition.
        /// </summary>
        public Table Where(Condition condition)
        {          
            var c = new Dictionary<string, object>();
            c.Add(condition.column, condition.value);
            where = c;

            return this;
        }

        /// <summary>
        /// Initialize a complex WHERE condition with the given Conditions combination. The AND.Q() and OR.Q() methods must be used for creating the condition.
        /// </summary>
        public Table Where(Dictionary<string, object> condition)
        {
            where = condition;

            return this;
        }

        /// <summary>
        /// Initialize a WHERE condition with the given conditions linked by an OR.
        /// </summary>
        public Table WhereOR(params Condition[] condition)
        {
            var c = new Dictionary<string, object>();
            for (var i = 0; i < condition.Length; i++) c.Add(condition[i].column + " #" + i, condition[i].value);
            var OR = new Dictionary<string, object>();
            OR.Add("OR", c);
            
            where = OR;

            return this;
        }

        /// <summary>
        /// Initialize a WHERE condition with the given conditions linked by an AND.
        /// </summary>
        public Table WhereAND(params Condition[] condition)
        {
            var c = new Dictionary<string, object>();
            for (var i = 0; i < condition.Length; i++) c.Add(condition[i].column + " #" + i, condition[i].value);
            var AND = new Dictionary<string, object>();
            AND.Add("AND", c);

            where = AND;

            return this;
        }
        #endregion

        #region " - ORDER BY, LIMIT "

        /// <summary>
        /// Initialize a criteria for ordering the results of a SELECT.
        /// </summary>
        public Table OrderBy(params Order[] order)
        {
            var c = new Dictionary<string, object>();
            for (var i = 0; i < order.Length; i++) c.Add(order[i].column.name, order[i].order.ToString());

            where.Add("ORDER", c);

            return this;
        }

        /// <summary>
        /// Initialize a criteria for limiting the number of Records from a SELECT.
        /// </summary>
        public Table Limit(int rows, int offset = 0)
        {      
            where.Add("LIMIT", new int[] { offset, rows });

            return this;
        }

        #endregion

        #region " - JOIN "

        public enum JoinType
        {
            Left,
            Right,
            Full,
            Inner
        }

        /// <summary>
        /// Initialize a (LEFT / RIGHT / FULL / INNER) JOIN condition for selecting Records in combination with another Table.
        /// </summary>
        public Table Join(JoinType joinType, Table joinTable, params Column[] column)
        {
            var jt = "";
            if (joinType == JoinType.Left)
                jt = "[>]";
            else if (joinType == JoinType.Right)
                jt = "[<]";
            else if (joinType == JoinType.Full)
                jt = "[<>]";
            else
                jt = "[><]";

            var jtName = joinTable.GetName();

            var c = new Dictionary<string, string>();
            for (var i = 0; i < column.Length; i++)
            {
                if (i % 2 == 0)
                {
                    var col1 = (column[i].table == jtName) ? column[i].name : column[i].table + "." + column[i].name;
                    var col2 = (column[i + 1].table == jtName) ? column[i + 1].name : column[i + 1].table + "." + column[i + 1].name;
                    c.Add(col1, col2);
                }
            }

            join = new Dictionary<string, object>();
            join.Add(jt + joinTable.GetName(), c);

            return this;
        }

        #endregion

        #region " - DELETE "

        /// <summary>
        /// Initialize a DELETE operation. A WHERE condition is mandatory.
        /// </summary>
        public Table Delete()
        {
            action = "DELETE";
            where = new Dictionary<string, object>();
            return this;
        }

        #endregion

        #region " - INSERT "

        /// <summary>
        /// Initialize an INSERT operation. The Data() method must be used for providing the data to be inserted.
        /// <para>Note: you can use the Data() method multiple times in order to insert two or more Records with one Insert() call.</para>
        /// </summary>
        public Table Insert()
        {
            action = "INSERT";
            recordData = new List<Dictionary<string, object>>();
            return this;
        }

        #endregion

        #region " - INSERT OR UPDATE "

        /// <summary>
        /// Initialize an INSERT/UPDATE operation. The Data() and the Where() methods must be used for providing the data to be wrote and the condition for determining if UniDB has to insert a new record or update an existing one.
        /// <para>Note: this method is intended to be used for managing one record only (insert one new record, update one existing record).</para>
        /// </summary>
        public Table InsertOrUpdate()
        {
            action = "INSERTORUPDATE";
            recordData = new List<Dictionary<string, object>>();
            where = new Dictionary<string, object>();
            return this;
        }

        #endregion

        #region " - DATA "

        /// <summary>
        /// Set the columns and their values for an insert/update operation.
        /// <para>Insert : the Data() method can be used multiple times for inserting more records with one call.</para>
        /// <para>Update : the Data() method must be used only once for providing the data to update.</para>
        /// </summary>
        public Table Data(params Column[] column)
        {
            var insertData = new Dictionary<string, object>();
            foreach (var col in column)
            {
                var colName = col._valueJsonEncoded ? col.name + " [JSON]" : col.name;
                var colValue = col._value;
                try
                {
                    var textValue = col._value.ToString();
                    if (textValue.StartsWith("{+}")) { colValue = int.Parse(textValue.Replace("{+}", ""));  colName += "[+]"; }
                    if (textValue.StartsWith("{-}")) { colValue = int.Parse(textValue.Replace("{-}", ""));  colName += "[-]"; }
                    if (textValue.StartsWith("{*}")) { colValue = int.Parse(textValue.Replace("{*}", ""));  colName += "[*]"; }
                    if (textValue.StartsWith("{/}")) { colValue = int.Parse(textValue.Replace("{/}", "")); colName += "[/]"; }
                }
                catch (Exception) {  }

                insertData.Add(colName, colValue);
            }

            recordData.Add(insertData);
            return this;
        }

        #endregion

        #region " - UPDATE "

        /// <summary>
        /// Initialize an UPDATE operation. The Data() method must be used (only once) for providing the data to be updated.
        /// </summary>
        public Table Update()
        {
            action = "UPDATE";
            recordData = new List<Dictionary<string, object>>();
            where = new Dictionary<string, object>();
            return this;
        }

        #endregion

        #region " - COUNT, MAX, MIN, AVG, SUM, EXISTS, DROP "

        /// <summary>
        /// Initialize a COUNT operation for counting the number of Records of a Table or the Records which respect a Where condition.
        /// </summary>
        public Table Count()
        {
            action = "COUNT";
            where = new Dictionary<string, object>();
            return this;
        }

        /// <summary>
        /// Initialize a MAX operation for finding the higher value in the given Column.
        /// </summary>
        public Table Max(Column column)
        {
            action = "MAX";
            where = new Dictionary<string, object>();
            columns = column.name;
            return this;
        }

        /// <summary>
        /// Initialize a MIN operation for finding the lower value in the given Column.
        /// </summary>
        public Table Min(Column column)
        {
            action = "MIN";
            where = new Dictionary<string, object>();
            columns = column.name;
            return this;
        }

        /// <summary>
        /// Initialize an AVG operation for calculating the average value of the given Column.
        /// </summary>
        public Table Avg(Column column)
        {
            action = "AVG";
            where = new Dictionary<string, object>();
            columns = column.name;
            return this;
        }

        /// <summary>
        /// Initialize an SUM operation for calculating the total value of the given Column.
        /// </summary>
        public Table Sum(Column column)
        {
            action = "SUM";
            where = new Dictionary<string, object>();
            columns = column.name;
            return this;
        }

        /// <summary>
        /// Initialize a verification control for determining whether the target data exists. A Where condition is mandatory.
        /// </summary>
        public Table Exists()
        {
            action = "EXISTS";
            where = new Dictionary<string, object>();
            return this;
        }

        /// <summary>
        /// Initialize a DROP operation on this Table. The Table content will be erased.
        /// <para>Note: use this method with caution. Erased data may not be recoverable.</para>
        /// </summary>
        public Table Drop()
        {
            action = "DROP";
            where = new Dictionary<string, object>();
            return this;
        }

        #endregion

        #region " - REPLACE "

        /// <summary>
        /// Initialize a REPLACE operation for replacing an old string/substring with a new one, in the given Column.
        /// </summary>
        public Table Replace(params Column[] column)
        {
            action = "REPLACE";
            where = new Dictionary<string, object>();
            join = new Dictionary<string, object>();

            foreach(var col in column)
            {               
                var key = col.name;
                var values = col._value.ToString().Split('|');
                var newDict = new Dictionary<string, object>();
                for (var i = 0; i < values.Length; i++) if (i % 2 == 0) newDict[values[i]] = values[i + 1];
                join.Add(key, newDict);
            }

            return this;
        }


        #endregion

        #region " - MATH "

        /// <summary>
        /// Initialize an UPDATE operation for recalculating the value of a Column. The Data() method must be used (only once) for providing the Column and the mathematical formula to use.
        /// </summary>
        public Table Math()
        {
            action = "MATH";
            recordData = new List<Dictionary<string, object>>();
            where = new Dictionary<string, object>();
            return this;
        }

        #endregion

        #region " - QUERY "

        /// <summary>
        /// Initialize a custom QUERY with the provided query string (SQL-92 standard) and data (a List of values). Use {0...n} placeholders for placing the values inside the query string.
        /// <para>query : a query string. Names must be written inside angular brakets. Data must be placed inside the string with {0...n} placeholders.</para>
        /// <para>data : a generic list of values. If the query contains placeholders, these values will be placed inside the query.</para>
        /// </summary>
        public Table Query(string query, List<object> data)
        {
            action = "QUERY";
            extra = query;
            where = new Dictionary<string, object>();
            for (var i = 0; i < data.Count; i++)
            {
                where.Add(":D" + i, data[i]);
                extra = extra.Replace("{" + i + "}", ":D" + i);
            }
            return this;
        }

        #endregion

        #region " RUN "

        /// <summary>
        /// Execute the built Query.
        /// <para>Note: this Run() method override must be used with the SelectOne operation only.</para>
        /// </summary>
        public async UniTask Run<T>(Action<T, Info> onDone, Action<Info> onError = null)
        {
            if (action != "SELECTONE")
            {
                Debug.LogWarning("[UniDB] You must use this Run() method override with the SelectOne() method only.");
                return;
            }

            RunInitialize();

            var http = new HTTP(token, dbURL, key);

            switch (action)
            {
                case "SELECTONE":

                    await http.
                          Post("selectone").
                          Data(new HTTP.Package { 
                              dbID = dbID, 
                              dbTable = table, 
                              what = columns, 
                              where = (where.Count > 0) ? JsonHelper.Stringify(where) : "",
                              join = (join.Count > 0) ? JsonHelper.Stringify(join) : ""
                          }).
                          Done((HttpRequest result) => {

                              debugResponse(result);

                              var data = JsonHelper.Parse<T>(result.response.data);
                              var info = GenericResult(result);
                              info.hasData = true;
                              info.isEmpty = false;
                              onDone?.Invoke(data, info); 
                          }).
                          Error((HttpRequest result) => {
                              onError?.Invoke(GenericError(result));
                          }).
                          Call();

                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Execute the built Query.
        /// <para>Note: this Run() method override must be used with the Select/SelectRand operation only.</para>
        /// </summary>
        public async UniTask Run<T>(Action<List<T>, Info> onDone, Action<Info> onError = null)
        {
            if (action != "SELECT" && action != "RANDSELECT")
            {
                Debug.LogWarning("[UniDB] You must use this Run() method override with the Select()/SelectRand() method only.");
                return;
            }

            RunInitialize();

            var http = new HTTP(token, dbURL, key);

            switch (action)
            {
                case "SELECT":
                case "RANDSELECT":

                    await http.
                          Post(action == "SELECT" ? "select" : "randselect").
                          Data(new HTTP.Package
                          {
                              dbID = dbID,
                              dbTable = table,
                              what = columns,
                              where = (where.Count > 0) ? JsonHelper.Stringify(where) : "",
                              join = (join.Count > 0) ? JsonHelper.Stringify(join) : ""
                          }).
                          Done((HttpRequest result) => {

                              debugResponse(result);

                              var data = JsonHelper.Parse<List<T>>(result.response.data);
                              var info = GenericResult(result);
                              info.hasData = data.Count > 0;
                              info.isEmpty = data.Count == 0;
                              onDone?.Invoke(data, info);
                          }).
                          Error((HttpRequest result) => {
                              onError?.Invoke(GenericError(result));
                          }).
                          Call();

                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Execute the built Query.
        /// </summary>
        public async UniTask Run(Action<Info> onDone, Action<Info> onError = null)
        {
            RunInitialize();

            var http = new HTTP(token, dbURL, key);

            switch (action)
            {
                case "INSERT":

                    if (recordData.Count <= 0)
                    {
                        Debug.LogWarning("[UniDB] You must provide data for the Insert operation.");
                        return;
                    }

                    await http.
                          Post("insert").
                          Data(new HTTP.Package
                          {
                              dbID = dbID,
                              dbTable = table,
                              data = JsonHelper.Stringify(recordData)
                          }).
                          Done((HttpRequest result) => {

                              debugResponse(result);

                              var info = GenericResult(result);
                              info.id = intParse(result.response.data);
                              onDone?.Invoke(info);
                          }).
                          Error((HttpRequest result) => {
                              onError?.Invoke(GenericError(result));
                          }).
                          Call();

                    break;

                case "UPDATE":

                    if (where.Count <= 0)
                    {
                        Debug.LogWarning("[UniDB] You can't use the Update operation without a Where condition.");
                        return;
                    }
                    if (recordData.Count <= 0)
                    {
                        Debug.LogWarning("[UniDB] You must provide data for the Update operation.");
                        return;
                    }

                    await http.
                          Post("update").
                          Data(new HTTP.Package
                          {
                              dbID = dbID,
                              dbTable = table,
                              data = JsonHelper.Stringify(recordData[0]),
                              where = JsonHelper.Stringify(where)
                          }).
                          Done((HttpRequest result) => {

                              debugResponse(result);

                              var info = GenericResult(result);
                              info.affectedRows = intParse(result.response.data);
                              onDone?.Invoke(info);
                          }).
                          Error((HttpRequest result) => {
                              onError?.Invoke(GenericError(result));
                          }).
                          Call();

                    break;

                case "INSERTORUPDATE":

                    if (where.Count <= 0)
                    {
                        Debug.LogWarning("[UniDB] You can't use the InsertOrUpdate operation without a Where condition.");
                        return;
                    }
                    if (recordData.Count <= 0)
                    {
                        Debug.LogWarning("[UniDB] You must provide data for the InsertOrUpdate operation.");
                        return;
                    }

                    await http.
                          Post("insertorupdate").
                          Data(new HTTP.Package
                          {
                              dbID = dbID,
                              dbTable = table,
                              data = JsonHelper.Stringify(recordData[0]),
                              where = JsonHelper.Stringify(where)
                          }).
                          Done((HttpRequest result) => {

                              debugResponse(result);

                              var info = GenericResult(result);
                              var ipTmp = result.response.data.Replace("\"", "").Split('|');
                              if (ipTmp[0] == "U")
                              {
                                  info.rawData = "UPDATE";
                                  info.affectedRows = intParse(result.response.data);
                              }
                              else
                              {
                                  info.rawData = "INSERT";
                                  info.id = intParse(result.response.data);
                              }
                              onDone?.Invoke(info);
                          }).
                          Error((HttpRequest result) => {
                              onError?.Invoke(GenericError(result));
                          }).
                          Call();

                    break;

                case "MATH":

                    if (where.Count <= 0)
                    {
                        Debug.LogWarning("[UniDB] You can't use the Math operation without a Where condition.");
                        return;
                    }
                    if (recordData.Count <= 0)
                    {
                        Debug.LogWarning("[UniDB] You must provide one Column, with a formula, for the Math operation.");
                        return;
                    }
                    else
                    {
                        if (recordData[0].Count != 1)
                        {
                            Debug.LogWarning("[UniDB] You must provide one Column, with a formula, for the Math operation.");
                            return;
                        }
                    }

                    await http.
                          Post("math").
                          Data(new HTTP.Package
                          {
                              dbID = dbID,
                              dbTable = table,
                              data = JsonHelper.Stringify(recordData[0]),
                              where = JsonHelper.Stringify(where)
                          }).
                          Done((HttpRequest result) => {

                              debugResponse(result);

                              var info = GenericResult(result);
                              info.affectedRows = intParse(result.response.data);
                              onDone?.Invoke(info);
                          }).
                          Error((HttpRequest result) => {
                              onError?.Invoke(GenericError(result));
                          }).
                          Call();

                    break;

                case "DELETE":

                    if (where.Count <= 0)
                    {
                        Debug.LogWarning("[UniDB] You can't use the Delete operation without a Where condition.");
                        return;
                    }

                    await http.
                          Post("delete").
                          Data(new HTTP.Package
                          {
                              dbID = dbID,
                              dbTable = table,
                              where = JsonHelper.Stringify(where)
                          }).
                          Done((HttpRequest result) => {

                              debugResponse(result);

                              var info = GenericResult(result);
                              info.affectedRows = intParse(result.response.data);
                              onDone?.Invoke(info);
                          }).
                          Error((HttpRequest result) => {
                              onError?.Invoke(GenericError(result));
                          }).
                          Call();

                    break;

                case "COUNT":
                case "MAX":
                case "MIN":
                case "AVG":
                case "SUM":
                case "EXISTS":
                case "REPLACE":
                case "DROP":

                    await http.
                          Post("utils").
                          Data(new HTTP.Package
                          {
                              dbID = dbID,
                              dbTable = table,
                              column = columns,
                              where = (where.Count > 0) ? JsonHelper.Stringify(where) : "",
                              join = (join.Count > 0) ? JsonHelper.Stringify(join) : "",
                              extra = action
                          }).
                          Done((HttpRequest result) => {

                              debugResponse(result);

                              var info = GenericResult(result);
                              info.result = floatParse(result.response.data);
                              info.affectedRows = (action == "REPLACE") ? (int)floatParse(result.response.data) : 0;
                              info.exists = (action == "EXISTS") ? floatParse(result.response.data) == 1 : false;
                              onDone?.Invoke(info);
                          }).
                          Error((HttpRequest result) => {
                              onError?.Invoke(GenericError(result));
                          }).
                          Call();
                    break;

                case "QUERY":

                    await http.
                          Post("query").
                          Data(new HTTP.Package
                          {
                              dbID = dbID,
                              dbTable = table,
                              where = (where.Count > 0) ? JsonHelper.Stringify(where) : "",
                              extra = extra
                          }).
                          Done((HttpRequest result) => {

                              debugResponse(result);

                              var info = GenericResult(result);
                              info.rawData = result.response.data;
                              onDone?.Invoke(info);
                          }).
                          Error((HttpRequest result) => {
                              onError?.Invoke(GenericError(result));
                          }).
                          Call();
                    break;

                default:
                    break;
            }
        }

        void RunInitialize()
        {
            table = GetName();
            db = GetParent();
            dbKey = db.GetKey();
            var tmp = dbKey.Split('|');
            token = tmp[0];
            key = tmp[1];
            dbURL = tmp[2].EndsWith("/") ? tmp[2] + "core/" : tmp[2] + "/core/";
            dbID = db.GetID();
            dbType = db.GetDBType();
        }

        Info GenericResult(HttpRequest result)
        {
            return new Info
            {
                query = result.response.query,
                hasData = true,
                isEmpty = false,
                error = result.response.error,
                code = (result.response.code == "" ? "000" : result.response.code),
                status = result.response.status,
                isOK = result.response.status == "OK",
                isError = result.response.status != "OK",
                rawData = ""
            };
        }

        Info GenericError(HttpRequest result)
        {
            return new Info
            {
                query = "",
                hasData = false,
                isEmpty = true,
                error = result.httpError,
                code = result.code.ToString(),
                status = "ERROR",
                isOK = false,
                isError = true,
                rawData = ""
            };
        }

        float floatParse(string value)
        {
            var valid = "0123456789.";
            var newString = "";
            foreach (char c in value)
            {
                if (valid.Contains(c)) newString += c;
            }
            return float.Parse(newString, NumberStyles.Any, CultureInfo.InvariantCulture);
        }

        int intParse(string value)
        {
            var valid = "0123456789.";
            var newString = "";
            foreach (char c in value)
            {
                if (valid.Contains(c)) newString += c;
            }
            return int.Parse(newString, NumberStyles.Any, CultureInfo.InvariantCulture);
        }

        void debugResponse(HttpRequest result)
        {
            if (Database.debugMode)
            {
                Debug.Log("<color=#FFA500>[" + dbType + "] » " + table + "</color>\n");
                if (result.response.status == "OK")
                {
                    Debug.Log("<color=#008000>[STATUS] <b>OK</b></color>\n");
                }
                else
                {
                    Debug.Log("<color=#FF0000>[STATUS] " + result.response.status + " " + result.response.code + "</color>\n");
                    Debug.Log("[ERROR] " + result.response.error);
                }
                Debug.Log("<color=#808080>[QUERY]</color> " + result.response.query + "\n");
                Debug.Log("<color=#808080>[RESULT]</color> " + result.response.data + "\n");
                Debug.Log("<color=#808080>[CODE]</color> " + result.response.code + "\n");
            }
        }

        #endregion


        #region " Helpers "

        string GetColumns(params Column[] columns)
        {
            var thisTable = GetName();
            var cols = new List<string>();

            foreach (var c in columns)
            {
                var colTable = c.table;
                cols.Add(colTable + "." + c.name);
            }

            return string.Join(",", cols);
        }

        #endregion
    }

}
