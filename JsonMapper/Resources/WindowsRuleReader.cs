using System.Collections.Generic;
using System.IO;
using JsonMapper.Interfaces;
using Microsoft.Win32;

namespace JsonMapper.Resources
{
    internal class WindowsRuleReader : IRuleReader
    {
        public List<string> LoadRules()
        {
            OpenFileDialog ruleDialog = new OpenFileDialog();
            List<string> ruleList = new List<string>();
            if (ruleDialog.ShowDialog() == true)
            {
                using (StreamReader sr = new StreamReader(ruleDialog.FileName))
                {
                    while (!sr.EndOfStream) ruleList.Add(sr.ReadLine());
                }
            }
            return ruleList;
        }

        public void SaveRules(List<string> rulesToSave)
        {
            SaveFileDialog save = new SaveFileDialog
            {
                FileName = "JsonMapperRules.txt",
                Filter = "Text File | *.txt"
            };
            if (save.ShowDialog() == true)
            {
                StreamWriter writer = new StreamWriter(save.OpenFile());
                rulesToSave.ForEach(rule => writer.WriteLine(rule));
                writer.Dispose();
                writer.Close();
            }
        }
    }
}