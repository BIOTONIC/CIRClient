using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace CIRClient
{
    // 自定义的DatePicker Watermark更改为截止时间
    public class MyEndDatePicker : DatePicker
    {
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            DatePickerTextBox box = GetTemplateChild("PART_TextBox") as DatePickerTextBox;
            box.ApplyTemplate();

            ContentControl watermark = box.Template.FindName("PART_Watermark", box) as ContentControl;
            watermark.Content = "截止时间";
        }
    }
}