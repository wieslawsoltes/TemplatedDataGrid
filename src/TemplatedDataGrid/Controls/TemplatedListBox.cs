using System;
using Avalonia.Controls;
using Avalonia.Styling;

namespace TemplatedDataGrid.Controls
{
    public class TemplatedListBox : ListBox, IStyleable
    {
        Type IStyleable.StyleKey => typeof(ListBox);

        protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
        {
            return new TemplatedListBoxItem();
        }

        protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
        {
            return NeedsContainer<TemplatedListBoxItem>(item, out recycleKey);
        }
    }
}
