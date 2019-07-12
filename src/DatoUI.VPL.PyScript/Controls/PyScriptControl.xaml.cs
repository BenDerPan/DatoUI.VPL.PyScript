using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Indentation;
using ICSharpCode.AvalonEdit.Indentation.CSharp;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.IO;
using System.Windows.Threading;
using System.Xml;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;

namespace DatoUI.VPL.PyScript.Controls
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class PyScriptControl : UserControl
    {
        private string currentFileName;
        private FoldingManager foldingManager;
        private BraceFoldingStrategy foldingStrategy;
        PythonAutoCompletionHelper pythonAutoCompletionHelper;

        public PythonAutoCompletionHelper AutoCompletionHelper => pythonAutoCompletionHelper;

        public CompletionWindow CompletionWindow { get; set; }

        public bool IsDebug { get; private set; }

        public event EventHandler StartCompilingEventHandler;

        public PyScriptControl()
        {
            LoadIPyHighlighting();
            InitializeComponent();

            IsDebug = true;
            CurrentFile = new ScriptFile();
            TextEditor.TextChanged += TextEditor_TextChanged;
            TextEditor.TextArea.TextEntering += TextArea_TextEntering;
            TextEditor.TextArea.TextEntered += TextArea_TextEntered;
            TextEditor.MouseWheel += TextEditor_PreviewMouseWheel;

            TextEditor.TextArea.IndentationStrategy = new CSharpIndentationStrategy();
            TextEditor.Options.ConvertTabsToSpaces = true;
            foldingManager = FoldingManager.Install(TextEditor.TextArea);
            foldingStrategy = new BraceFoldingStrategy();
            foldingStrategy.UpdateFoldings(foldingManager, TextEditor.Document);

            pythonAutoCompletionHelper = new PythonAutoCompletionHelper();
            
        }

        void LoadIPyHighlighting()
        {
            IHighlightingDefinition highlightingDefinition;
            using (Stream s = new FileStream("DatoUI.VPL.PyScript.Python.xshd",FileMode.Open,FileAccess.Read))
            {
                if (s == null)
                {
                    throw new InvalidOperationException("找不到IPy语法高亮文件");
                }
                using (XmlReader reader = new XmlTextReader(s))
                {
                    highlightingDefinition = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
            //注册到HighlightingManager
            HighlightingManager.Instance.RegisterHighlighting("Python", new string[] { ".py" }, highlightingDefinition);
        }

        public ScriptFile CurrentFile { get; private set; }

        public void SetError(string errorMessage)
        {
            tbError.Text = $"[{DateTime.Now.ToString("yyyy-dd-mm HH:MM:ss")}]Debug Exception:{Environment.NewLine}{errorMessage}";
            tbError.Visibility = Visibility.Visible;
        }

        public void ClearError()
        {
            tbError.Text = string.Empty;
            tbError.Visibility = Visibility.Collapsed;
        }

        private void TextEditor_TextChanged(object sender, EventArgs e)
        {
            var TextEditor = sender as TextEditor;
            if (TextEditor != null) CurrentFile.ScriptContent = TextEditor.Document.Text;
        }

        private void OpenFileClick(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog { CheckFileExists = true,Multiselect=false,Filter="Python(*.py)|*.py|文本文件(*.txt)|*.txt|所有文件(*.*)|*.*" };
            if (!(dlg.ShowDialog() ?? false)) return;
            currentFileName = dlg.FileName;
            TextEditor.Load(currentFileName);
            TextEditor.SyntaxHighlighting =
                HighlightingManager.Instance.GetDefinitionByExtension(Path.GetExtension(currentFileName));
        }

        private void SaveFileClick(object sender, EventArgs e)
        {
            if (currentFileName == null)
            {
                var dlg = new SaveFileDialog { DefaultExt = ".py",Filter= "Python(*.py)|*.py" };
                if (dlg.ShowDialog() ?? false)
                    currentFileName = dlg.FileName;
                else
                    return;
            }
            TextEditor.Save(currentFileName);
        }

        private void TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            var text = string.Empty;
            if (TextEditor.Text.Length > 0)
            {
                for (int i = TextEditor.CaretOffset - 1; i < TextEditor.CaretOffset && i >= 0; i--)
                {
                    if (PythonAutoCompletionHelper.BreakChars.ContainsKey(TextEditor.Text[i]))
                    {
                        break;
                    }
                    text = TextEditor.Text.Substring(i, TextEditor.CaretOffset - i);
                }
            }
            if (!string.IsNullOrEmpty(text))
            {
                CompletionWindow = new CompletionWindow(TextEditor.TextArea);
                IList<ICompletionData> data = CompletionWindow.CompletionList.CompletionData;
                foreach (var key in pythonAutoCompletionHelper.CustomKeywords.Keys)
                {
                    if (key.StartsWith(text))
                    {
                        data.Add(new PyScriptCompletionData(key));
                    }
                }
                if (data.Count > 0)
                {
                    CompletionWindow.Show();
                    CompletionWindow.Closed += delegate
                    {
                        CompletionWindow = null;
                    };
                }
                else
                {
                    CompletionWindow = null;
                }
            }


            if (TextEditor.Text.Length > 1 && e.Text == ".")
            {
                string methodText = string.Empty;
                var offset = TextEditor.CaretOffset - 1;
                for (int i = offset - 1; i < offset && i >= 0; i--)
                {
                    if (PythonAutoCompletionHelper.BreakChars.ContainsKey(TextEditor.Text[i]))
                    {
                        break;
                    }
                    methodText = TextEditor.Text.Substring(i, offset - i);
                }
                if (!string.IsNullOrEmpty(methodText) && pythonAutoCompletionHelper.CustomKeywords.ContainsKey(methodText))
                {
                    CompletionWindow = new CompletionWindow(TextEditor.TextArea);
                    IList<ICompletionData> data = CompletionWindow.CompletionList.CompletionData;
                    foreach (var com in pythonAutoCompletionHelper.CustomKeywords[methodText])
                    {
                        data.Add(com);
                    }
                    if (data.Count > 0)
                    {
                        CompletionWindow.Show();
                        CompletionWindow.Closed += delegate
                        {
                            CompletionWindow = null;
                        };
                    }
                    else
                    {
                        CompletionWindow = null;
                    }
                }
            }

        }

        private void TextArea_TextEntering(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length > 0 && CompletionWindow != null)
            {
                if (!char.IsLetterOrDigit(e.Text[0]))
                {
                    CompletionWindow.CompletionList.RequestInsertion(e);
                }
            }

        }

        private void TextEditor_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            bool handle = (Keyboard.Modifiers & ModifierKeys.Control) > 0;
            if (!handle)
            {
                return;
            }

            if (e.Delta < 0 && TextEditor.FontSize <= 1)
            {
                return;
            }
            if (e.Delta > 0 && TextEditor.FontSize >= 150)
            {
                return;
            }
            TextEditor.FontSize += e.Delta / 120;
        }
        
        private void FrameworkElement_OnLoaded(object sender, RoutedEventArgs e)
        {
            var toolBar = sender as ToolBar;
            var overflowGrid = toolBar.Template.FindName("OverflowGrid", toolBar) as FrameworkElement;
            if (overflowGrid != null)
                overflowGrid.Visibility = Visibility.Collapsed;

            var mainPanelBorder = toolBar.Template.FindName("MainPanelBorder", toolBar) as FrameworkElement;
            if (mainPanelBorder != null)
                mainPanelBorder.Margin = new Thickness(0);
        }

        private void CompileButton_OnClick(object sender, RoutedEventArgs e)
        {
            ClearError();
            StartCompilingEventHandler?.Invoke(this, new EventArgs());
        }

        private void CbIsLocked_Checked(object sender, RoutedEventArgs e)
        {
            IsDebug = cbIsLocked.IsChecked.Value;
        }
    }
}
