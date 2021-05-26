using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;

using System.Windows.Input;
using System.Windows.Media;

using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;

//https://stackoverflow.com/a/57710850/969070


namespace AltUpTest
{
    public class Cmd : ICommand
    {

        private readonly Action<object> _execute;

        public Cmd( Action<object> execute)
        { 
            _execute = execute;
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }




    public class AltUpTestCommand : Command
    {
        public AltUpTestCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static AltUpTestCommand Instance
        {
            get; private set;
        }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName
        {
            get { return "AltUpTestCommand"; }
        }



        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            Window win = new Window();
            win.Title = "Testing Alt + Up keys";
            TextBlock tb = new TextBlock();
            ScrollViewer sc = new ScrollViewer();
            sc.Content = tb;
            win.Content = sc;            

            tb.Text = "To test:\r\nFirst press Ctrl + Up key\r\nThen press Alt + X key\r\nThen press Alt + Up key\r\n\r\n";
            

            KeyGesture altUpGesture  = new KeyGesture(Key.Up, ModifierKeys.Alt);
            KeyGesture altXGesture   = new KeyGesture(Key.X,  ModifierKeys.Alt);
            KeyGesture ctrlUpGesture = new KeyGesture(Key.Up, ModifierKeys.Control);

            ICommand altUpCmd =  new Cmd(x => tb.Text = tb.Text + "Alt + Up was pressed \r\n");
            ICommand altXCmd =   new Cmd(x => tb.Text = tb.Text + "Alt + X was pressed \r\n");
            ICommand crtlUpCmd = new Cmd(x => tb.Text = tb.Text + "Ctrl + Up was pressed \r\n");


            win.InputBindings.Add(new InputBinding(altUpCmd,  altUpGesture));
            win.InputBindings.Add(new InputBinding(altXCmd,   altXGesture));
            win.InputBindings.Add(new InputBinding(crtlUpCmd, ctrlUpGesture));



            win.PreviewKeyDown += new KeyEventHandler(delegate (object sender, KeyEventArgs a)
                {
                    tb.Text = tb.Text + "PreviewKeyDown: " + a.Key.ToString() + ", SystemKey:" + a.SystemKey.ToString() + "\r\n";
                    a.Handled = true;
                    sc.ScrollToEnd();
                });

            win.KeyDown += new KeyEventHandler(delegate (object sender, KeyEventArgs a)
            {
                tb.Text = tb.Text + "KeyDown: " + a.Key.ToString() + ", SystemKey:" + a.SystemKey.ToString() + "\r\n";
                a.Handled = true;
                sc.ScrollToEnd();
            });



            win.Height = 600;
            win.Width = 400;
            win.Show();

            return Result.Success;
        }
    }
}
