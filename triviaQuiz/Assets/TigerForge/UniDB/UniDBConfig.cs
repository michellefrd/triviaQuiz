// ===========================================
//     UniDB - Unity Settings script (1.2)
// ===========================================

using System;

namespace TigerForge.UniDB
{
/// <summary>
/// The collection of all your Databases.
/// </summary>
public class UniDB
{

//====================================================================================
// DATABASE: trivia
//====================================================================================

/// <summary>
/// [MYSQL] trivia
/// </summary>
public class Trivia : Database {
public readonly new string Key = "gDfSLxUcuO|pyiUGSfVkaBx64yyuCMzPYghmF1phGLP|https://axlplay.net/ServerApp/bF68zoXtqr/";
public readonly new string ID = "I34117fYfI1FY3V642b4O87";
public readonly new string Type = "mysql";
public override string GetKey() { return Key; }
public override string GetID() { return ID; }
public override string GetDBType() { return Type; }


//------------------------------------------------------------------------------------
// TABLE: users
//------------------------------------------------------------------------------------
public class Users : Table {
public class Record {
/// <summary>
/// [trivia » users] email (varchar | string)
/// </summary>
public string email;
/// <summary>
/// [trivia » users] name (varchar | string)
/// </summary>
public string name;
/// <summary>
/// [trivia » users] last_name (varchar | string)
/// </summary>
public string last_name;
/// <summary>
/// [trivia » users] password (varchar | string)
/// </summary>
public string password;
/// <summary>
/// [trivia » users] code (int | Int32)
/// </summary>
public Int32 code;
}
/// <summary>
/// This Table Record structure.
/// </summary>
public Record R = new();

public class Columns {
/// <summary>
/// [trivia » users] email (varchar | string)
/// </summary>
public Column email = new ("email", "users");
/// <summary>
/// [trivia » users] name (varchar | string)
/// </summary>
public Column name = new ("name", "users");
/// <summary>
/// [trivia » users] last_name (varchar | string)
/// </summary>
public Column last_name = new ("last_name", "users");
/// <summary>
/// [trivia » users] password (varchar | string)
/// </summary>
public Column password = new ("password", "users");
/// <summary>
/// [trivia » users] code (int | Int32)
/// </summary>
public Column code = new ("code", "users");
}
/// <summary>
/// This Table Columns with built-in functionalities.
/// </summary>
public Columns C = new();
public readonly new string name = "users";
public readonly new Database parent;
public override string GetName() { return name; }
public override Database GetParent() { return parent; }
public Users(Database p) { parent = p; }
}



//====================================================================================
/// <summary>
/// [MYSQL] trivia » Users
/// </summary>
public Users GetTable_Users() { return new Users(this); }
//====================================================================================
}


}
}
