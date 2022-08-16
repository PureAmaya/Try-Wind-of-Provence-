public class Settings
{
    public static YamlReadWrite.SettingsContent SettingsContent;

    /// <summary>
    /// 读取设置文件
    /// </summary>
    public static void ReadSettings()
    {
        SettingsContent = YamlReadWrite.Read<YamlReadWrite.SettingsContent>(YamlReadWrite.FileName.Settings);
    }

    /// <summary>
    /// 保存设置至文件
    /// </summary>
    public static void SaveSettings()
    {
        YamlReadWrite.Write(SettingsContent,YamlReadWrite.FileName.Settings,"#游戏设置");
    }
    
    /// <summary>
    /// 初始化设置（但不写入文件中）
    /// </summary>
    public static void Default()
    {
        SettingsContent.MusicVolume = 1f;
        SettingsContent.SoundEffectVolume = 1f;
        SettingsContent.lag = 0;
        

    }
}
