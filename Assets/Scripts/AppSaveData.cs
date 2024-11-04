using Newtonsoft.Json;

public class AppSaveData : SaveData
{
    protected override string FileName => "app_data";

    [JsonProperty("SoundsEnabled")]
    public bool SoundsEnabled { get; set; } = true;

    [JsonProperty("BestScore")]
    public int BestScore { get; set; }
}