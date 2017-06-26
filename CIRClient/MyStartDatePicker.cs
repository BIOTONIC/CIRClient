using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace CIRClient
{
    // 自定义的DatePicker Watermark更改为开始时间
    public class MyStartDatePicker : DatePicker
    {
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            DatePickerTextBox box = GetTemplateChild("PART_TextBox") as DatePickerTextBox;
            box.ApplyTemplate();

            ContentControl watermark = box.Template.FindName("PART_Watermark", box) as ContentControl;
            watermark.Content = "起始时间";
        }
    }
}
