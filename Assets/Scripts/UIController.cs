using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    private string currentPage = "main-menu";
    private readonly Dictionary<string, string> previousPages = new Dictionary<string, string>
    {
        { "play-type-menu", "main-menu" },
        { "settings-menu", "main-menu" }
    };
    private Dictionary<string, string> nextPages = new Dictionary<string, string> {
        { "play-button", "play-type-menu" },
        { "settings-button", "settings-menu" }
    };

    void Start()
    {
        UIDocument doc = GetComponent<UIDocument>();
        var root = doc.rootVisualElement;
        var back = root.Q<Button>("back-button");

        back.style.left = new StyleLength(new Length(110, LengthUnit.Percent));


        foreach (KeyValuePair<string, string> entry in nextPages)
        {
            root.Q<Button>(entry.Key).clicked += () =>
            {
                root.Q<VisualElement>(currentPage).style.left = new StyleLength(new Length(-100, LengthUnit.Percent));

                if (currentPage == "main-menu")
                {
                    back.style.left = new StyleLength(new Length(back.style.left.value.value - 100, LengthUnit.Percent));
                }
                currentPage = entry.Value;

                root.Q<VisualElement>(currentPage).style.left = new StyleLength(new Length(0, LengthUnit.Percent));
            };
        }

        root.Q<GroupBox>("play-type-menu").Query<Button>().ForEach((b) =>
        {
            b.clicked += () =>
            {
                GameController.NUM_PLAYERS = int.Parse(b.name.Split("-")[0]);

                SceneManager.LoadScene("Game", LoadSceneMode.Single);
            };
        });

        back.clicked += () =>
        {
            root.Q<VisualElement>(currentPage).style.left = new StyleLength(new Length(100, LengthUnit.Percent));

            currentPage = previousPages[currentPage];
            if (currentPage == "main-menu")
            {
                back.style.left = new StyleLength(new Length(back.style.left.value.value + 100, LengthUnit.Percent));
            }

            root.Q<VisualElement>(currentPage).style.left = new StyleLength(new Length(0, LengthUnit.Percent));
        };

        root.Q<Button>("exit-button").clicked += Application.Quit;
    }
}
