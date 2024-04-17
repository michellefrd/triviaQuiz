
// ****************************************************************************************
//
// HOW TO SET UP THIS DEMO
// =======================
//
// This is a working Demo. However, to make this C# code work, you must follow this
// intructions (if possibile, use the provided names, exactly as they are written):
//
// 1) Create a Database named 'testdb'
//
// 2) Create a Table named 'users' with this two Columns:
//    a) One column named 'name', set as TEXT
//    b) One column named 'age', set as INT
//
// 3) Using the UninDB Admin Panel:
//    a) Create a connection Profile to your 'testdb' Database
//    b) Generate the "Unity Settings Script"
//    c) Copy the Script and paste it inside the UniDBConfig.cs file of this Unity Project
//
// 4) Uncomment the lines of C# code below.
// 
// If, for some reason, you can't set one of the requested names above, just update
// the C# code below with the correct names.
//
// ****************************************************************************************    

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TigerForge.UniDB;

namespace TigerForge.UniDB
{
    public class UniDBDemo : MonoBehaviour
    {

        void Start()
        {
            // This setting is pretty useful for checking what's happening inside the system.
            // It should always be set as 'true' during the whole development process.
            // Then, it can be deleted or set to 'false' in production.
            
            //Database.debugMode = true;

            // The first thing to do is to create an Instance of a Database. All the configured Databases are under the UniDB class.
            
            //var myDB = new UniDB.Testdb();

            // For working with a Table, you have to create an Instance of that Table. Just use the built-in GetTable_*() methods.
            
            //var users = myDB.GetTable_Users();

            // To INSERT a new Record in the 'users' Table, just use the INSERT method.
            // Using the Data() method more times, you can INSERT more Records with one call.

            //_ = users
            //    .Insert()
            //    .Data(users.C.name.Value("Steven"), users.C.age.Value(18))
            //    .Data(users.C.name.Value("Mary"), users.C.age.Value(32))
            //    .Data(users.C.name.Value("John"), users.C.age.Value(56))
            //    .Run((Info info) =>
            //    {
            //        if (info.isOK)
            //        {
            //            Debug.Log("Some Records have been added to your 'users' Table.");

            //            // To read the 'users' Table content, just use the SELECT method.
            //            _ = users
            //            .Select()
            //            .Run((List<UniDB.Testdb.Users.Record> records, Info info2) => 
            //            {
            //                if (info2.isOK)
            //                {
            //                    foreach (var r in records)
            //                    {
            //                        Debug.Log("- " + r.name + " is " + r.age + " yo.");
            //                    }
            //                }
            //                else
            //                {
            //                    Debug.LogWarning("Something gones wrong with the SELECT query!");
            //                }
                        
            //            });

            //        }
            //        else
            //        {
            //            Debug.LogWarning("Something gones wrong with the INSERT query!");
            //        }
            //    });

        }

    }

}
