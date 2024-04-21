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
        public class Trivia : Database
        {
            public readonly new string Key =
                "gDfSLxUcuO|pyiUGSfVkaBx64yyuCMzPYghmF1phGLP|https://axlplay.net/ServerApp/bF68zoXtqr/";

            public readonly new string ID = "I34117fYfI1FY3V642b4O87";
            public readonly new string Type = "mysql";

            public override string GetKey()
            {
                return Key;
            }

            public override string GetID()
            {
                return ID;
            }

            public override string GetDBType()
            {
                return Type;
            }


//------------------------------------------------------------------------------------
// TABLE: categories
//------------------------------------------------------------------------------------
            public class Categories : Table
            {
                public class Record
                {
                    /// <summary>
                    /// [trivia » categories] code (int | Nullable Int32)
                    /// </summary>
                    public Nullable<Int32> code;

                    /// <summary>
                    /// [trivia » categories] name (varchar | string)
                    /// </summary>
                    public string name;
                }

                /// <summary>
                /// This Table Record structure.
                /// </summary>
                public Record R = new();

                public class Columns
                {
                    /// <summary>
                    /// [trivia » categories] code (int | Nullable Int32)
                    /// </summary>
                    public Column code = new("code", "categories");

                    /// <summary>
                    /// [trivia » categories] name (varchar | string)
                    /// </summary>
                    public Column name = new("name", "categories");
                }

                /// <summary>
                /// This Table Columns with built-in functionalities.
                /// </summary>
                public Columns C = new();

                public readonly new string name = "categories";
                public readonly new Database parent;

                public override string GetName()
                {
                    return name;
                }

                public override Database GetParent()
                {
                    return parent;
                }

                public Categories(Database p)
                {
                    parent = p;
                }
            }


//------------------------------------------------------------------------------------
// TABLE: leaderboard
//------------------------------------------------------------------------------------
            public class Leaderboard : Table
            {
                public class Record
                {
                    /// <summary>
                    /// [trivia » leaderboard] email (varchar | string)
                    /// </summary>
                    public string email;

                    /// <summary>
                    /// [trivia » leaderboard] score (int | Nullable Int32)
                    /// </summary>
                    public Int32 score;
                }

                /// <summary>
                /// This Table Record structure.
                /// </summary>
                public Record R = new();

                public class Columns
                {
                    /// <summary>
                    /// [trivia » leaderboard] email (varchar | string)
                    /// </summary>
                    public Column email = new("email", "leaderboard");

                    /// <summary>
                    /// [trivia » leaderboard] score (int | Nullable Int32)
                    /// </summary>
                    public Column score = new("score", "leaderboard");
                }

                /// <summary>
                /// This Table Columns with built-in functionalities.
                /// </summary>
                public Columns C = new();

                public readonly new string name = "leaderboard";
                public readonly new Database parent;

                public override string GetName()
                {
                    return name;
                }

                public override Database GetParent()
                {
                    return parent;
                }

                public Leaderboard(Database p)
                {
                    parent = p;
                }
            }


//------------------------------------------------------------------------------------
// TABLE: questions
//------------------------------------------------------------------------------------
            public class Questions : Table
            {
                public class Record
                {
                    /// <summary>
                    /// [trivia » questions] id (int | Nullable Int32)
                    /// </summary>
                    public Nullable<Int32> id;

                    /// <summary>
                    /// [trivia » questions] category (int | Nullable Int32)
                    /// </summary>
                    public Nullable<Int32> category;

                    /// <summary>
                    /// [trivia » questions] question (varchar | string)
                    /// </summary>
                    public string question;

                    /// <summary>
                    /// [trivia » questions] option0 (varchar | string)
                    /// </summary>
                    public string option0;

                    /// <summary>
                    /// [trivia » questions] option1 (varchar | string)
                    /// </summary>
                    public string option1;

                    /// <summary>
                    /// [trivia » questions] option2 (varchar | string)
                    /// </summary>
                    public string option2;

                    /// <summary>
                    /// [trivia » questions] option3 (varchar | string)
                    /// </summary>
                    public string option3;

                    /// <summary>
                    /// [trivia » questions] correctAnswer (int | Nullable Int32)
                    /// </summary>
                    public Nullable<Int32> correctAnswer;

                    /// <summary>
                    /// [trivia » questions] bonus (int | Nullable Int32)
                    /// </summary>
                    public Nullable<Int32> bonus;

                    /// <summary>
                    /// [trivia » questions] time (int | Nullable Int32)
                    /// </summary>
                    public Nullable<Int32> time;
                }

                /// <summary>
                /// This Table Record structure.
                /// </summary>
                public Record R = new();

                public class Columns
                {
                    /// <summary>
                    /// [trivia » questions] id (int | Nullable Int32)
                    /// </summary>
                    public Column id = new("id", "questions");

                    /// <summary>
                    /// [trivia » questions] category (int | Nullable Int32)
                    /// </summary>
                    public Column category = new("category", "questions");

                    /// <summary>
                    /// [trivia » questions] question (varchar | string)
                    /// </summary>
                    public Column question = new("question", "questions");

                    /// <summary>
                    /// [trivia » questions] option0 (varchar | string)
                    /// </summary>
                    public Column option0 = new("option0", "questions");

                    /// <summary>
                    /// [trivia » questions] option1 (varchar | string)
                    /// </summary>
                    public Column option1 = new("option1", "questions");

                    /// <summary>
                    /// [trivia » questions] option2 (varchar | string)
                    /// </summary>
                    public Column option2 = new("option2", "questions");

                    /// <summary>
                    /// [trivia » questions] option3 (varchar | string)
                    /// </summary>
                    public Column option3 = new("option3", "questions");

                    /// <summary>
                    /// [trivia » questions] correctAnswer (int | Nullable Int32)
                    /// </summary>
                    public Column correctAnswer = new("correctAnswer", "questions");

                    /// <summary>
                    /// [trivia » questions] bonus (int | Nullable Int32)
                    /// </summary>
                    public Column bonus = new("bonus", "questions");

                    /// <summary>
                    /// [trivia » questions] time (int | Nullable Int32)
                    /// </summary>
                    public Column time = new("time", "questions");
                }

                /// <summary>
                /// This Table Columns with built-in functionalities.
                /// </summary>
                public Columns C = new();

                public readonly new string name = "questions";
                public readonly new Database parent;

                public override string GetName()
                {
                    return name;
                }

                public override Database GetParent()
                {
                    return parent;
                }

                public Questions(Database p)
                {
                    parent = p;
                }
            }


//------------------------------------------------------------------------------------
// TABLE: users
//------------------------------------------------------------------------------------
            public class Users : Table
            {
                public class Record
                {
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

                public class Columns
                {
                    /// <summary>
                    /// [trivia » users] email (varchar | string)
                    /// </summary>
                    public Column email = new("email", "users");

                    /// <summary>
                    /// [trivia » users] name (varchar | string)
                    /// </summary>
                    public Column name = new("name", "users");

                    /// <summary>
                    /// [trivia » users] last_name (varchar | string)
                    /// </summary>
                    public Column last_name = new("last_name", "users");

                    /// <summary>
                    /// [trivia » users] password (varchar | string)
                    /// </summary>
                    public Column password = new("password", "users");

                    /// <summary>
                    /// [trivia » users] code (int | Int32)
                    /// </summary>
                    public Column code = new("code", "users");
                }

                /// <summary>
                /// This Table Columns with built-in functionalities.
                /// </summary>
                public Columns C = new();

                public readonly new string name = "users";
                public readonly new Database parent;

                public override string GetName()
                {
                    return name;
                }

                public override Database GetParent()
                {
                    return parent;
                }

                public Users(Database p)
                {
                    parent = p;
                }
            }


//====================================================================================
            /// <summary>
            /// [MYSQL] trivia » Categories
            /// </summary>
            public Categories GetTable_Categories()
            {
                return new Categories(this);
            }

            /// <summary>
            /// [MYSQL] trivia » Leaderboard
            /// </summary>
            public Leaderboard GetTable_Leaderboard()
            {
                return new Leaderboard(this);
            }

            /// <summary>
            /// [MYSQL] trivia » Questions
            /// </summary>
            public Questions GetTable_Questions()
            {
                return new Questions(this);
            }

            /// <summary>
            /// [MYSQL] trivia » Users
            /// </summary>
            public Users GetTable_Users()
            {
                return new Users(this);
            }
//====================================================================================
        }
    }
}