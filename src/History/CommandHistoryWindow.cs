﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Text;
using System.Windows;
using DbgX.Interfaces;
using DbgX.Interfaces.Listeners;
using DbgX.Interfaces.Services;
using DbgX.Util;

namespace WinDbgExt.History
{
    [NamedPartMetadata("CommandHistoryWindow"), Export(typeof(IDbgToolWindow))]
    [Export(typeof(IDbgDmlOutputListener))]
    [Export(typeof(IDbgCommandExecutionListener))]
    [Export(typeof(IHistoryManager))]
    public class CommandHistoryWindow : IDbgToolWindow, IDbgDmlOutputListener, IDbgCommandExecutionListener, IHistoryManager
    {
        private StringBuilder _output = new StringBuilder();
        private string _currentCommand;

        [Import]
        private IDbgConsole _console;

        [Import]
        private IDbgToolWindowManager _toolWindowManager;

        public CommandHistoryWindow()
        {
            History = new ObservableCollection<Tuple<string, string>>();
            OpenCommand = new DelegateCommand<string>(Open);
        }

        public DelegateCommand<string> OpenCommand { get; }

        public FrameworkElement GetToolWindowView(object parameter)
        {
            return new HistoryControl { DataContext = this };
        }

        public ObservableCollection<Tuple<string, string>> History { get; }

        public void OnDmlOutput(string text)
        {
            _output.Append(text);
        }

        public void OnCommandCompletion()
        {
            if (_currentCommand != null)
            {
                LogCommand(_currentCommand, _output.ToString());
            }
        }

        public void OnCommandExecuted(string command)
        {
            _output.Clear();
            _currentCommand = command;
        }

        public void LogCommand(string command, string output)
        {
            History.Add(Tuple.Create(command, output));
        }

        private void Open(string commandOutput)
        {
            _toolWindowManager.OpenToolWindow("CommandWindow", commandOutput);
        }
    }
}