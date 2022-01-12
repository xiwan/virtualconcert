using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class DataManager : Single<DataManager>
{
    public List<Avatar> NpcPrefabsList = new List<Avatar>();

    public string[] PlayerAvatars = new string[] { "woman-zombie", "woman-soldier", "woman-swimsuit", "woman-skate", "woman-prehistoric",
        "woman-police", "woman-lumberjack", "woman-fire", "woman-farm", "woman-doctor", "woman-cyclist", "woman-cowgirl", "woman-construction-worker",
        "woman-clown", "woman-chef", "woman-casual", "woman-business", "woman-basketball-player", "woman-astronaut"};
    public string[] NPCAvatars = new string[] {
        "man-astronaut", "man-basketball-player", "man-boxer", "man-business", "man-casual", "man-chef", "man-clown",
         "man-construction-worker", "man-cowboy", "man-cyclist", "man-diving", "man-doctor", "man-farm", "man-fire",
         "man-hazard", "man-knight", "man-lumberjack", "man-ninja", "man-pirate", "man-police", "man-prehistoric",
         "man-skate", "man-skeleton", "man-ski", "man-soldier", "man-swimsuit", "man-wizard", "man-zombie"};

    public string[] DanceAnimations = new string[] { "MoonwalkController", "BellyController", "BrooklynuprockController", "FlairController",
        "HouseController", "JazzController", "NorthsoulController", "SambaController", "YmcaController" };

    public void LoadPrefabsData()
    {
        
    }

    public List<Avatar> GenerateAvatarList()
    {
        for (int i = 0; i < 20; i++)
        {
            var man = new Avatar()
            {
                id = i,
                aname = MathHelper.RandomValue(NPCAvatars),
                animatorController = MathHelper.RandomValue(DanceAnimations)
            };
            NpcPrefabsList.Add(man);
        }
        return NpcPrefabsList;
    }

}
