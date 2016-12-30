using System;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;

namespace Ultra.PrepareData.Utils
{
    [MarkupExtensionReturnType(typeof(ICommand))]
    public class CommandBindingExtension : MarkupExtension
    {
        public CommandBindingExtension()
        {
        }

        public CommandBindingExtension(string commandName)
        {
            CommandName = commandName;
        }

        [ConstructorArgument("commandName")]
        public string CommandName { get; set; }

        private object _targetObject;
        private object _targetProperty;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var provideValueTarget =
                serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            if (provideValueTarget != null)
            {
                _targetObject = provideValueTarget.TargetObject;
                _targetProperty = provideValueTarget.TargetProperty;
            }

            if (string.IsNullOrEmpty(CommandName)) return new RelayCommand<object>(o => { }, o => false);

            // The serviceProvider is actually a ProvideValueServiceProvider, which has a private field "_context" of type ParserContext
            var parserContext = GetPrivateFieldValue<ParserContext>(serviceProvider, "_context");
            if (parserContext == null) return new RelayCommand<object>(o => { }, o => false);
            // A ParserContext has a private field "_rootElement", which returns the root element of the XAML file
            var rootElement = GetPrivateFieldValue<FrameworkElement>(parserContext, "_rootElement");
            if (rootElement == null) return new RelayCommand<object>(o => { }, o => false);
            // Now we can retrieve the DataContext
            var dataContext = rootElement.DataContext;

            // The DataContext may not be set yet when the FrameworkElement is first created, and it may change afterwards,
            // so we handle the DataContextChanged event to update the Command when needed
            if (!_dataContextChangeHandlerSet)
            {
                rootElement.DataContextChanged += rootElement_DataContextChanged;
                _dataContextChangeHandlerSet = true;
            }

            if (dataContext == null) return new RelayCommand<object>(o => { }, o => false);
            var command = GetCommand(dataContext, CommandName);
            return command ?? new RelayCommand<object>(o => { }, o => false);

            // The Command property of an InputBinding cannot be null, so we return a dummy extension instead
        }

        private ICommand GetCommand(object dataContext, string commandName)
        {
            var prop = dataContext.GetType().GetProperty(commandName);
            var command = prop?.GetValue(dataContext, null) as ICommand;
            return command;
        }

        private void AssignCommand(ICommand command)
        {
            if (_targetObject == null || _targetProperty == null) return;
            var depProp = _targetProperty as DependencyProperty;
            if (depProp != null)
            {
                var depObj = _targetObject as DependencyObject;
                depObj?.SetValue(depProp, command);
            }
            else
            {
                var prop = _targetProperty as PropertyInfo;
                prop?.SetValue(_targetObject, command, null);
            }
        }

        private bool _dataContextChangeHandlerSet;

        private void rootElement_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var rootElement = sender as FrameworkElement;
            var dataContext = rootElement?.DataContext;
            if (dataContext == null) return;
            var command = GetCommand(dataContext, CommandName);
            if (command != null)
                AssignCommand(command);
        }

        private T GetPrivateFieldValue<T>(object target, string fieldName)
        {
            var field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            if (field != null)
            {
                return (T) field.GetValue(target);
            }
            return default(T);
        }
    }
}