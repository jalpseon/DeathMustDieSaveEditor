using Newtonsoft.Json;
using DeathMustDieSaveEditor.Core.Models.SaveStructure;
using System.Linq;

namespace DeathMustDieSaveEditor.Core.Models.SaveStructure
{
    [Serializable]
    public class SerializedSaveData
    {
        public IList<string> keys { get; set; }
        public IList<string> values { get; set; }

        private int GetProfileStateIndex()
        {
            if (keys == null)
                return 0;

            int index = keys.ToList().FindIndex(k => k != null && k.Contains("Profile+ProfileState"));
            return index >= 0 ? index : 0;
        }

        public Progression GetProgression()
        {
            int index = GetProfileStateIndex();
            string prog = values[index];
            var res = JsonConvert.DeserializeObject<Progression>(prog);
            return res;
        }

        public string GetNotParsedProgression()
        {
            int index = GetProfileStateIndex();
            string prog = values[index];
            return prog;
        }

        public void SaveProgression(Progression progression)
        {
            int index = GetProfileStateIndex();
            var serializedProgression = JsonConvert.SerializeObject(progression);
            this.values[index] = serializedProgression;
        }
    }

    [Serializable]
    public class SaveData
    {
        public int Version { get; set; }
        public bool IsAutosave { get; set; }
        public SerializedSaveData serializedSaveData { get; set; }
        public string thumbnailBase64 { get; set; }
        public string playthroughGuidString { get; set; }
    }

    [Serializable]
    public class InventoryData
    {
        public string CharacterCode { get; set; }
        public string Json { get; set; }
    }

    [Serializable]
    public class EquipmentStateWrapper
    {
        public EquipmentState EquipmentState { get; set; }
    }

    [Serializable]
    public class EquipmentState
    {
        public IList<Item> Items { get; set; }
    }

    [Serializable]
    public class Affix
    {
        public string Code { get; set; }
        public int Levels { get; set; }
    }

    [Serializable]
    public class StashState
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public IList<Item> Items { get; set; }
    }

    [Serializable]
    public class Item
    {
        public string Code { get; set; }
        public int Type { get; set; }
        public int Rarity { get; set; }
        public int TierIndex { get; set; }
        public bool IsUnique { get; set; }
        public string SubtypeCode { get; set; }
        public string IconVariant { get; set; }
        public string DropVariant { get; set; }
        public IList<Affix> Affixes { get; set; }
        public bool WasOwnedByPlayer { get; set; }
        public string BoundToCharacterCode { get; set; }
    }

    [Serializable]
    public class LibraryState
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public IList<Item> Items { get; set; }
    }

    [Serializable]
    public class BackpackState
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public IList<Item> Items { get; set; }
    }

    [Serializable]
    public class Progression
    {
        public string ProgressionJson { get; set; }
        public string DarknessJson { get; set; }
        public string AchievementsJson { get; set; }
        public string TalentsJson { get; set; }
        public string UnlocksJson { get; set; }
        public IList<InventoryData> InventoryData { get; set; }
        public string ShopJson { get; set; }
        public string StashJson { get; set; }
        public StashState StashState { get; set; }
        public LibraryState LibraryState { get; set; }
        public BackpackState BackpackState { get; set; }
        public int Gold { get; set; }

        public IEnumerable<Item> GetEquippedItems(string characterCode)
        {
            var charEquipped = this.InventoryData.Where(x => x.CharacterCode == characterCode).FirstOrDefault();

            var res = JsonConvert.DeserializeObject<EquipmentStateWrapper>(charEquipped.Json);
            return res.EquipmentState.Items;
        }

        public void SetEquippedItems(string characterCode, IEnumerable<Item> items)
        {
            var charEquipped = this.InventoryData.Where(x => x.CharacterCode == characterCode).FirstOrDefault();

            var res = JsonConvert.DeserializeObject<EquipmentStateWrapper>(charEquipped.Json);
            res.EquipmentState.Items = items.ToList();

            var serializedEquippment = JsonConvert.SerializeObject(res);
            charEquipped.Json = serializedEquippment;
        }
    }
}
