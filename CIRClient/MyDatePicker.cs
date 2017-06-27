using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace CIRClient
{
    // 自定义的DatePicker Watermark更改为当前时间
    public class MyDatePicker : DatePicker
    {
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            DatePickerTextBox box = GetTemplateChild("PART_TextBox") as DatePickerTextBox;
            box.ApplyTemplate();

            ContentControl watermark = box.Template.FindName("PART_Watermark", box) as ContentControl;
            watermark.Content = DateTime.Now.ToString("yyyy/MM/dd");
        }
    }
}
