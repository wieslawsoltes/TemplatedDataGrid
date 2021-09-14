using System;
using Avalonia.Controls;
using Avalonia.Controls.Generators;
using Avalonia.Styling;

namespace TemplatedDataGrid.Controls
{
    public class TemplatedListBox : ListBox, IStyleable
    {
        Type IStyleable.StyleKey => typeof(ListBox);

        protected override IItemContainerGenerator CreateItemContainerGenerator()
        {
            return new ItemContainerGenerator<TemplatedListBoxItem>(
                this,
                ContentControl.ContentProperty,
                ContentControl.ContentTemplateProperty);
        }
    }
}
