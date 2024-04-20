using System.Collections;
using System.Collections.Generic;
using TigerForge.UniDB;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    private UniDB.Trivia triviaDB;
    public Transform categoryButtonParent; // Parent object to hold instantiated buttons
    public GameObject categoryButtonPrefab; // Prefab for category buttons

    // Start is called before the first frame update
    void Start()
    {
        triviaDB = new UniDB.Trivia();
        OnLoadCategories();
    }

    private void OnLoadCategories()
    {
        var categories = triviaDB.GetTable_Categories();
        _ = categories
            .Select()
            .Run(
                (List<UniDB.Trivia.Categories.Record> data, Info info) =>
                {
                    if (info.isOK && data !=null)
                    {
                        foreach (var d in data)
                        {
                            GameObject newButton = Instantiate(categoryButtonPrefab, categoryButtonParent);
                            newButton.name = "CategoryButton_" + d.name; // Naming the button for easier identification
                            CategoryButton buttonScript = newButton.GetComponent<CategoryButton>();
                            if (buttonScript != null)
                            {
                                buttonScript.categoryNameTxt.text = d.name;
                                buttonScript.categoryID = (int)d.code;
                            }
                            else
                            {
                                Debug.LogError("CategoryButton script not found on the instantiated prefab!");
                            }
                        }
                    }
                    else
                    {
                        Debug.LogError("Error: " + info.error);
                    }
                }
            );
    }
}
