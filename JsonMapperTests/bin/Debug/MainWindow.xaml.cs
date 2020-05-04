using JsonMapper.Services;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Xml;

namespace JsonMapper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ValueMapper valMapper;
        private JObject ParameterTemplate;
        private bool isJson;
        private string templateAsXml;
        private readonly MapSyntaxWriter syntaxWriter;
        private readonly RuleService ruleService;
        private List<string> displayValue;
        private string templateAsJson;

        private List<char> SeparatorComboBoxContent { get; set; }
        private List<string> Instructions { get; set; }
        private ObservableCollection<string> UIInstructions { get; set; }
        private List<string> Excluders { get; set; }
        public List<JObject> LatestResults { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            valMapper = new ValueMapper();
            syntaxWriter = new MapSyntaxWriter();
            UIInstructions = new ObservableCollection<string>();
            Instructions = new List<string>();
            lbInstructions.ItemsSource = UIInstructions;
            Excluders = new List<string>();
            ruleService = new RuleService();
            SeparatorComboBoxContent = new List<char>() { ',', ';', '|' };
            SeparatorComboBox.ItemsSource = SeparatorComboBoxContent;
            SeparatorComboBox.SelectedIndex = 0;
        }

        private void ChangeBetweenJsonXml_Click(object sender, RoutedEventArgs e)
        {
            if (ParameterTemplate == null) return;
            if (isJson)
            {
                XmlDocument doc = JsonConvert.DeserializeXmlNode(ParameterTemplate.ToString(Newtonsoft.Json.Formatting.None));
                TemplateBox.Document.Blocks.Clear();
                TemplateBox.Document.Blocks.Add(new Paragraph(new Run(doc.OuterXml)));
                isJson = false;
            }
            else
            {
                TemplateBox.Document.Blocks.Clear();
                TemplateBox.Document.Blocks.Add(new Paragraph(new Run(ParameterTemplate.ToString(Newtonsoft.Json.Formatting.Indented))));
                isJson = true;
            }
        }
        private void TemplateButton_Click(object sender, RoutedEventArgs e)
        {
            string template = new TextRange(TemplateBox.Document.ContentStart, TemplateBox.Document.ContentEnd).Text;

            if (template.StartsWith("{"))
            {
                ParameterTemplate = JObject.Parse(template);
                valMapper.SetTemplate(ParameterTemplate);
                TemplateBox.Document.Blocks.Clear();
                TemplateBox.Document.Blocks.Add(new Paragraph(new Run(ParameterTemplate.ToString(Newtonsoft.Json.Formatting.Indented))));
                isJson = true;
            }
            else if (template.StartsWith("<"))
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(template);
                ParameterTemplate = JObject.Parse(JsonConvert.SerializeXmlNode(doc));
                valMapper.SetTemplate(ParameterTemplate);
                TemplateBox.Document.Blocks.Clear();
                TemplateBox.Document.Blocks.Add(new Paragraph(new Run(doc.OuterXml)));
                isJson = false;
            }
            if (ParameterTemplate != null)
            {
                templateAsJson = ParameterTemplate.ToString(Newtonsoft.Json.Formatting.Indented);
                TryEnablingMappers();
            }
        }

        private void LoadCsv_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog csvDialog = new OpenFileDialog();
            if (csvDialog.ShowDialog() == true)
            {   
                valMapper.SetValues(csvDialog.FileName, (char)SeparatorComboBox.SelectedValue);
                TryEnablingMappers();

            }
        }

        private void TryEnablingMappers()
        {
            if (ParameterTemplate != null && valMapper.NodeValues != null && valMapper.valuesByColumnName != null &&
                  valMapper.NodeValues.Count != 0 && valMapper.valuesByColumnName.Count != 0)
            {
                PropertyMapStartBtn.IsEnabled = true;
                SiblingMapStartBtn.IsEnabled = true;
                PropertyInserterStartBtn.IsEnabled = true;
                SiblingColumnValueStartBtn.IsEnabled = true;
                PropertyExcluderStartBtn.IsEnabled = true;
                LoadRuleBtn.IsEnabled = true;
                SaveRuleBtn.IsEnabled = true;
                deleteRuleBtn.IsEnabled = true;
            }
        }

        private void PropertyMap_Click(object sender, RoutedEventArgs e)
        {
            if (ParameterTemplate is null || valMapper.NodeValues is null || valMapper.valuesByColumnName is null ||
                valMapper.NodeValues.Count == 0 || valMapper.valuesByColumnName.Count == 0) return;
            MapperGrid.Visibility = Visibility.Collapsed;
            PropertyMapGrid.Visibility = Visibility.Visible;
            PropertyMapProp_Cb.ItemsSource = valMapper.NodeValues;
            PropertyMapCSV_Cb.ItemsSource = new List<string>(valMapper.valuesByColumnName.Keys);
        }

        private void PropertyMapReturn_Click(object sender, RoutedEventArgs e)
        {
            MapperGrid.Visibility = Visibility.Visible;
            PropertyMapGrid.Visibility = Visibility.Collapsed;
        }

        private void PropertyMapTrigger_Click(object sender, RoutedEventArgs e)
        {
            if (PropertyMapCSV_Cb.SelectedIndex != -1 && PropertyMapProp_Cb.SelectedIndex != -1)
            {
                Instructions.Add(syntaxWriter.WritePropertyMap(PropertyMapProp_Cb.Text, PropertyMapCSV_Cb.Text));
                UIInstructions.Add(syntaxWriter.WritePropertyMap(PropertyMapProp_Cb.Text, PropertyMapCSV_Cb.Text));
                PropertyMapCSV_Cb.ItemsSource = new List<string>();
                PropertyMapProp_Cb.ItemsSource = new List<string>();
                MapperGrid.Visibility = Visibility.Visible;
                PropertyMapGrid.Visibility = Visibility.Hidden;
                MapBtn.IsEnabled = true;
            }
        }

        private void MapBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Instructions.Count == 0) return;
            ExecuteMappingRules(out string template, out List<JObject> resultObjects);
            if (ResultBox.Visibility == Visibility.Hidden && RuleListGrid.Visibility == Visibility.Visible)
            {
                RuleListGrid.Visibility = Visibility.Hidden;
                ResultBox.Visibility = Visibility.Visible;
            }
            LatestResults = resultObjects;
            if (template.StartsWith("{")) DisplayJsonResult(resultObjects);
            else if (template.StartsWith("<")) DisplayXMLResult(resultObjects);
        }


        private void ResultXmlJsonBtn_Click(object sender, RoutedEventArgs e)
        {
            if (LatestResults == null) return;
            if (isJson)
            {
                DisplayXMLResult(LatestResults);
            }
            else
            {
                DisplayJsonResult(LatestResults);
            }
        }

        private void ExecuteMappingRules(out string template, out List<JObject> resultObjects)
        {
            template = new TextRange(TemplateBox.Document.ContentStart, TemplateBox.Document.ContentEnd).Text;
            List<string> actualInstructions = new List<string>(Instructions);
            actualInstructions.AddRange(Excluders);
            valMapper.SetInstructions(actualInstructions);
            resultObjects = valMapper.MapValues();
        }

        private void DisplayXMLResult(List<JObject> resultObjects)
        {
            isJson = false;
            string resultString = "";
            resultObjects.ForEach(result =>
            {
                XmlDocument doc = JsonConvert.DeserializeXmlNode(result.ToString(Newtonsoft.Json.Formatting.None));
                resultString += doc.OuterXml;
            });
            ResultBox.Document.Blocks.Clear();
            ResultBox.Document.Blocks.Add(new Paragraph(new Run(resultString)));
        }

        private void DisplayJsonResult(List<JObject> resultObjects)
        {
            isJson = true;
            string resultString = "";
            resultObjects.ForEach(result => resultString += result.ToString(Newtonsoft.Json.Formatting.Indented) + ",");
            if (!string.IsNullOrEmpty(resultString))
            {
                resultString = resultString.Remove(resultString.Length - 1, 1);
                ResultBox.Document.Blocks.Clear();
                ResultBox.Document.Blocks.Add(new Paragraph(new Run(resultString)));
            }
            else ResultBox.Document.Blocks.Clear();
        }

        private void SiblingPropertyMap_Click(object sender, RoutedEventArgs e)
        {
            if (ParameterTemplate is null || valMapper.NodeValues is null || valMapper.valuesByColumnName is null ||
                valMapper.NodeValues.Count == 0 || valMapper.valuesByColumnName.Count == 0) return;
            MapperGrid.Visibility = Visibility.Collapsed;
            SiblingMapGrid.Visibility = Visibility.Visible;
            SiblingMapProp_Cb.ItemsSource = valMapper.NodeValues;
            SiblingMapCSV_Cb.ItemsSource = new List<string>(valMapper.valuesByColumnName.Keys);
            SiblingName_Cb.ItemsSource = valMapper.NodeValues;
        }
        private void SiblingMapReturn_Click(object sender, RoutedEventArgs e)
        {
            MapperGrid.Visibility = Visibility.Visible;
            SiblingMapGrid.Visibility = Visibility.Hidden;
        }

        private void SiblingColumnMapReturn_Click(object sender, RoutedEventArgs e)
        {
            MapperGrid.Visibility = Visibility.Visible;
            SiblingColumnMapGrid.Visibility = Visibility.Hidden;
        }

        private void SiblingColumnBtn_Click(object sender, RoutedEventArgs e)
        {
            MapperGrid.Visibility = Visibility.Hidden;
            SiblingColumnMapGrid.Visibility = Visibility.Visible;
            SiblingColumnMapCSV_Cb.ItemsSource = new List<string>(valMapper.valuesByColumnName.Keys);
            SiblingColumnNameCSV_Cb.ItemsSource = new List<string>(valMapper.valuesByColumnName.Keys);
            SiblingColumnMapProp_Cb.ItemsSource = valMapper.NodeValues;
        }
        private void PropertyValueInserterReturn_Click(object sender, RoutedEventArgs e)
        {
            MapperGrid.Visibility = Visibility.Visible;
            PropertyValueInserterGrid.Visibility = Visibility.Hidden;
        }

        private void SiblingMapTrigger_Click(object sender, RoutedEventArgs e)
        {
            if (SiblingName_Cb.SelectedIndex != -1 && SiblingMapCSV_Cb.SelectedIndex != -1
                && SiblingMapProp_Cb.SelectedIndex != -1)
            {
                string[] siblings = SiblingName_Cb.Text.Split('.');
                string sibling = siblings[siblings.Length - 1];
                Instructions.Add(syntaxWriter.WriteSiblingPropertyMap(SiblingMapProp_Cb.Text, SiblingMapCSV_Cb.Text, sibling, SiblingMapReplaceValue.Text));
                UIInstructions.Add(syntaxWriter.WriteSiblingPropertyMap(SiblingMapProp_Cb.Text, SiblingMapCSV_Cb.Text, sibling, SiblingMapReplaceValue.Text));

                SiblingMapCSV_Cb.ItemsSource = new List<string>();
                SiblingMapProp_Cb.ItemsSource = new List<string>();
                SiblingName_Cb.ItemsSource = new List<string>();
                SiblingMapReplaceValue.Text = "";
                MapperGrid.Visibility = Visibility.Visible;
                SiblingMapGrid.Visibility = Visibility.Hidden;
                MapBtn.IsEnabled = true;
            }
        }

        private void SiblingColumnMapTrigger_Click(object sender, RoutedEventArgs e)
        {
            if (SiblingColumnMapCSV_Cb.SelectedIndex != -1 && SiblingColumnMapProp_Cb.SelectedIndex != -1
                && SiblingColumnNameCSV_Cb.SelectedIndex != -1 && !string.IsNullOrEmpty(SiblingColumnMapReplaceValue.Text))
            {
                Instructions.Add(syntaxWriter.WriteSiblingColumnPropertyMap(SiblingColumnMapProp_Cb.Text, SiblingColumnMapCSV_Cb.Text, SiblingColumnNameCSV_Cb.Text, SiblingColumnMapReplaceValue.Text));
                UIInstructions.Add(syntaxWriter.WriteSiblingColumnPropertyMap(SiblingColumnMapProp_Cb.Text, SiblingColumnMapCSV_Cb.Text, SiblingColumnNameCSV_Cb.Text, SiblingColumnMapReplaceValue.Text));

                SiblingColumnMapCSV_Cb.ItemsSource = new List<string>();
                SiblingColumnMapProp_Cb.ItemsSource = new List<string>();
                SiblingColumnNameCSV_Cb.ItemsSource = new List<string>();
                SiblingColumnMapReplaceValue.Text = "";
                MapperGrid.Visibility = Visibility.Visible;
                SiblingColumnMapGrid.Visibility = Visibility.Hidden;
                MapBtn.IsEnabled = true;
            }
        }
        private void PropertyInserterStartBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ParameterTemplate is null || valMapper.NodeValues is null || valMapper.valuesByColumnName is null ||
                valMapper.NodeValues.Count == 0 || valMapper.valuesByColumnName.Count == 0) return;
            PropertyValueInserterGrid.Visibility = Visibility.Visible;
            MapperGrid.Visibility = Visibility.Collapsed;
            PropertyValueInserterCSV_Cb.ItemsSource = new List<string>(valMapper.valuesByColumnName.Keys);
            PropertyValueInserterProp_Cb.ItemsSource = valMapper.NodeValues;
        }

        private void PropertyValueInserterTrigger_Click(object sender, RoutedEventArgs e)
        {
            if (PropertyValueInserterProp_Cb.SelectedIndex != -1 && PropertyValueInserterCSV_Cb.SelectedIndex != -1
                && !string.IsNullOrEmpty(PropertyValueInserterBase_tb.Text) && !string.IsNullOrEmpty(PropertyValueInserterReplaceKey_tb.Text))
            {
                Instructions.Add(syntaxWriter.WritePropertyValueInserter(PropertyValueInserterProp_Cb.Text, PropertyValueInserterCSV_Cb.Text, PropertyValueInserterBase_tb.Text, PropertyValueInserterReplaceKey_tb.Text));
                UIInstructions.Add(syntaxWriter.WritePropertyValueInserter(PropertyValueInserterProp_Cb.Text, PropertyValueInserterCSV_Cb.Text, PropertyValueInserterBase_tb.Text, PropertyValueInserterReplaceKey_tb.Text));

                PropertyValueInserterCSV_Cb.ItemsSource = new List<string>();
                PropertyValueInserterProp_Cb.ItemsSource = new List<string>();
                PropertyValueInserterBase_tb.Text = "";
                PropertyValueInserterReplaceKey_tb.Text = "";
                MapperGrid.Visibility = Visibility.Visible;
                PropertyValueInserterGrid.Visibility = Visibility.Hidden;
                MapBtn.IsEnabled = true;
            }
        }

        private void PropertyExcluderStartBtn_Click(object sender, RoutedEventArgs e)
        {
            MapperGrid.Visibility = Visibility.Collapsed;
            PropertyValueExcluderGrid.Visibility = Visibility.Visible;
            PropertyValueExcluder_Cb.ItemsSource = new List<string>(valMapper.valuesByColumnName.Keys);
        }

        private void PropertyValueExcluderReturnBtn_Click(object sender, RoutedEventArgs e)
        {
            MapperGrid.Visibility = Visibility.Visible;
            PropertyValueExcluderGrid.Visibility = Visibility.Hidden;
        }

        private void PropertyValueExcluderTrigger_Click(object sender, RoutedEventArgs e)
        {
            if (PropertyValueExcluder_Cb.SelectedIndex != -1 && !string.IsNullOrEmpty(PropertyToExclude_tb.Text))
            {
                Excluders.Add(syntaxWriter.WritePropertyValueExcluder(PropertyValueExcluder_Cb.Text, PropertyToExclude_tb.Text));
                UIInstructions.Add(syntaxWriter.WritePropertyValueExcluder(PropertyValueExcluder_Cb.Text, PropertyToExclude_tb.Text));
                PropertyValueExcluder_Cb.ItemsSource = new List<string>();
                PropertyToExclude_tb.Text = "";
                MapperGrid.Visibility = Visibility.Visible;
                PropertyValueExcluderGrid.Visibility = Visibility.Hidden;
                MapBtn.IsEnabled = true;
            }
        }

        private void DeleteRule_Click(object sender, RoutedEventArgs e)
        {
            List<string> rulesToRemove = new List<string>(lbInstructions.SelectedItems.Cast<string>().ToList());
            foreach (string rule in rulesToRemove)
            {
                UIInstructions.Remove(rule);
                Instructions.Remove(rule);
                Excluders.Remove(rule);
            }
        }

        private void LoadRule_Click(object sender, RoutedEventArgs e)
        {
            List<string> loadedRules = new List<string>(ruleService.LoadRules());
            UIInstructions.Clear();
            loadedRules.ForEach(rule => UIInstructions.Add(rule));
            Excluders = loadedRules.FindAll(excludeRule => excludeRule.Contains("Excluder"));
            loadedRules.RemoveAll(rule => Excluders.Contains(rule));
            Instructions = loadedRules;
            if (UIInstructions.Count > 0) MapBtn.IsEnabled = true;
        }

        private void SaveRule_Click(object sender, RoutedEventArgs e)
        {
            List<string> ruleList = new List<string>(Instructions);
            ruleList.AddRange(Excluders);
            ruleService.SaveRules(ruleList);
        }

        private void RuleBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ResultBox.Visibility.Equals(Visibility.Visible))
            {
                ResultBox.Visibility = Visibility.Hidden;
                RuleListGrid.Visibility = Visibility.Visible;
            }
            else
            {
                ResultBox.Visibility = Visibility.Visible;
                RuleListGrid.Visibility = Visibility.Hidden;
            }
        }
    }
}
