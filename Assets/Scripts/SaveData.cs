using System.IO;
using Newtonsoft.Json;

public abstract class SaveData
{
    [JsonIgnore]
    protected abstract string FileName { get; }

    private string _filePath;

    [JsonIgnore]
    private string FilePath
    {
        get
        {
            if (string.IsNullOrEmpty(_filePath))
                _filePath = Path.Combine(PathUtils.ApplicationPath, GetFileName());

            return _filePath;
        }
    }

    private string GetFileName()
    {
        return $"{FileName}.sd";
    }

    private string Serialize()
    {
        var data = JsonConvert.SerializeObject(this);
        return data;
    }

    private void Deserialize(string data)
    {
        JsonConvert.PopulateObject(data, this);
    }

    public void Save()
    {
        var directoryName = Path.GetDirectoryName(FilePath);
        if (string.IsNullOrEmpty(directoryName))
            return;

        if (!Directory.Exists(directoryName))
            Directory.CreateDirectory(directoryName);

        File.WriteAllText(FilePath, Serialize());
    }

    public void Load()
    {
        if (!File.Exists(FilePath))
            return;

        Deserialize(File.ReadAllText(FilePath));
    }
}