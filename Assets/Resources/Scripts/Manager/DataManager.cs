using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class DataManager : Single<DataManager>
{
    public List<Avatar> NpcPrefabsList = new List<Avatar>();

    public void LoadPrefabsData()
    {
        var man01 = new Avatar()
        {
            id = 1,
            aname = "man-astronaut",
            animatorController = "MoonwalkController"
        };
        var man02 = new Avatar()
        {
            id = 2,
            aname = "man-basketball-player",
            animatorController = "BellyController"
        };
        var man03 = new Avatar()
        {
            id = 3,
            aname = "man-boxer",
            animatorController = "BrooklynuprockController"
        };
        var man04 = new Avatar()
        {
            id = 4,
            aname = "man-business",
            animatorController = "FlairController"
        };
        var man05 = new Avatar()
        {
            id = 5,
            aname = "man-casual",
            animatorController = "HouseController"
        };
        var man06 = new Avatar()
        {
            id = 6,
            aname = "man-chef",
            animatorController = "JazzController"
        };
        var man07 = new Avatar()
        {
            id = 7,
            aname = "man-clown",
            animatorController = "MoonwalkController"
        };
        var man08 = new Avatar()
        {
            id = 8,
            aname = "man-construction-worker",
            animatorController = "NorthsoulController"
        };
        var man09 = new Avatar()
        {
            id = 9,
            aname = "man-cowboy",
            animatorController = "SambaController"
        };
        var man10 = new Avatar()
        {
            id = 10,
            aname = "man-cyclist",
            animatorController = "YmcaController"
        };
        NpcPrefabsList.Add(man01);
        NpcPrefabsList.Add(man02);
        NpcPrefabsList.Add(man03);
        NpcPrefabsList.Add(man04);
        NpcPrefabsList.Add(man05);
        NpcPrefabsList.Add(man06);
        NpcPrefabsList.Add(man07);
        NpcPrefabsList.Add(man08);
        NpcPrefabsList.Add(man09);
        NpcPrefabsList.Add(man10);
    }

}
