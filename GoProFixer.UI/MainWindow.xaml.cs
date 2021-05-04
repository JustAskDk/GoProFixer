using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;
using GoProFixer;
using Path = System.IO.Path;

namespace GoProFixer.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GoProFileRename _program;
        private string _path;
        private SearchOption _recursiveSearchOption = SearchOption.TopDirectoryOnly;
        private IEnumerable<FileRenameInfo> _files;

        public MainWindow()
        {
            InitializeComponent();
            _program = new GoProFileRename();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (result != System.Windows.Forms.DialogResult.OK)
                    return;

                _path = dialog.SelectedPath;
                pathHolder.Content = _path;

                UpdateRenames();
            }
        }

        private void UpdateRenames()
        {
            if (String.IsNullOrEmpty(_path)) return;

            _files = _program.GetAllFileRenameInfo(_path, _recursiveSearchOption).ToArray();

            boxy.Text = _files
                .Select(x => $"Path: {Path.GetDirectoryName(x.OriginalName)} Orig: {Path.GetFileName(x.OriginalName)} - New:{Path.GetFileName(x.NewName)}\r\n")
                .Aggregate("", (x, y) => x + y);

            renameButton.IsEnabled = _files.Any();
        }

        private void CheckBoxChanged(object sender, RoutedEventArgs e)
        {
            if ((bool) CheckboxRecursive.IsChecked)
                _recursiveSearchOption = SearchOption.AllDirectories;
            else
                _recursiveSearchOption = SearchOption.TopDirectoryOnly;


            UpdateRenames();
        }

        private void renameButton_Click(object sender, RoutedEventArgs e)
        {
            string messageBoxText = $"{_files.Count()} files renamed";
            string caption = "Rename not possible";
            MessageBoxButton button = MessageBoxButton.OK;
            MessageBoxImage icon = MessageBoxImage.Information;
            MessageBoxResult result;


            foreach (var fileRenameInfo in _files)
                fileRenameInfo.PerformRename();

            result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);
        }
    }
}
