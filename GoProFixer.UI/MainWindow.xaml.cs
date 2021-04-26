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

namespace GoProFixer.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (result != System.Windows.Forms.DialogResult.OK)
                    return;

                var recursiveSearchOption = SearchOption.TopDirectoryOnly;
                if ((bool)CheckboxRecursive.IsChecked)
                    recursiveSearchOption = SearchOption.AllDirectories;

                textHolder.Content = dialog.SelectedPath;
                var files = Directory.EnumerateFiles(dialog.SelectedPath, "*.*", recursiveSearchOption);
                boxy.Text = files.Select(x => x + "\r\n").Aggregate("", (x, y) => x + y);
            }
        }
    }
}
