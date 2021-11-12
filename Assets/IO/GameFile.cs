using System.Text;
using System.IO;
using UnityEngine;

public class GameFile
{
    public static string fileRootPath = Application.persistentDataPath;//游戏读写文件根目录
    public static string gameConfigFileName = "GameConfigEdit.xml";//游戏配置文件
    public static string gameRecordFileName = "Record.txt";//游戏记录文件

    /// <summary>
    /// 生成游戏配置文件
    /// </summary>
    public static void CreateGameConfigFile(string content)
    {
        string toPath = fileRootPath + "/" + gameConfigFileName;
        CreateFile(toPath, content);
    }

    /// <summary>
    /// 生成游戏记录文件
    /// </summary>
    public static void CreateGameRecordFile()
    {
        CreateFile(fileRootPath + "/" + gameRecordFileName, "1");
    }


    /// <summary>
    /// 读取游戏记录
    /// </summary>
    /// <returns>返回玩家最高关卡记录</returns>
    public static string ReadRecord()
    {
        string filePath = fileRootPath + "/" + gameRecordFileName;
        if (!File.Exists(filePath))
        {
            CreateFile(filePath, "1");
            return "1";
        }

        return File.ReadAllText(filePath);
    }

    /// <summary>
    /// 保存游戏记录【保存玩家最高关卡记录】
    /// </summary>
    /// <param name="content">游戏记录</param>
    public static void WriteRecord(string content)
    {
        string filePath = fileRootPath + "/" + gameRecordFileName;
        CreateFile(filePath, content);
    }

    /// <summary>
    /// 写入内容到指定文件里，如果文件存在就覆盖内容，文件不存在则创建
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <param name="content">要写入文件的内容</param>
    public static void CreateFile(string filePath, string content)
    {
        File.WriteAllText(filePath, content, Encoding.UTF8);
    }
}

