public interface IDataObject
{
    public void Save(SaveData data);
    public void Load(SaveData data, bool reset = false);
}
