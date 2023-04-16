using Common;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;

namespace FileActions
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private HashSet<string> _actions = new HashSet<string>();

        private Dictionary<string, Func<string, string>> _delegates = new Dictionary<string, Func<string, string>>()
        {
            { Constants.Copy, Common.FileActions.Copy },
            { Constants.Hash, Common.FileActions.Hash },
            { Constants.Zip, Common.FileActions.Zip }
        };

        public MainWindow()
        {
            InitializeComponent();
            if (Common.FileActions.IsInAdminRole())
                System.Windows.Application.Current.Shutdown();
        }

        private void UpdateTasks()
        {
            _actions.Clear();
            if(zipCB.IsChecked.HasValue && zipCB.IsChecked.Value)
                _actions.Add(Constants.Zip);
            if(copyCB.IsChecked.HasValue && copyCB.IsChecked.Value)
                _actions.Add(Constants.Copy);
            if(hashCB.IsChecked.HasValue && hashCB.IsChecked.Value)
                _actions.Add(Constants.Hash);
        }

        private void onGoButtonClicked(object sender, RoutedEventArgs e)
        {
            UpdateTasks();
            if (_actions.Count == 0)
            {
                MessageBox.Show("No actions selected!");
                return;
            }

            string? fileSelected = SelectFile();

            if (fileSelected is null)
                return;

            foreach (var action in _actions)
            {
                try
                {
                    _delegates[action](fileSelected!);
                }
                catch (UnauthorizedAccessException)
                {
                    int exitCode = RunActionInIsolatedProcess(action, fileSelected);
                    if(exitCode != 0)
                    {
                        MessageBox.Show("Something went wrong:/");
                        return;
                    }
                }
                catch(Exception)
                {
                    MessageBox.Show("Something went wrong:/");
                    return;
                }
            }
        }

        private string? SelectFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
                return openFileDialog.FileName;

            return null;
        }

        private int RunActionInIsolatedProcess(string action, string fileName)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = "SecuredActions.exe";
            proc.StartInfo.UseShellExecute = true;
            proc.StartInfo.Verb = "runas";
            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            proc.StartInfo.ArgumentList.Add(action);
            proc.StartInfo.ArgumentList.Add(fileName);

            try
            {
                proc.Start();
                proc.WaitForExit();
            }
            catch (Exception)
            {
                return -1;
            }
            return proc.ExitCode;
        }
    }
}
