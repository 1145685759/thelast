   M S B A M L   `   `   `  ���� B  >TheLast, Version=1.0.0.2, Culture=neutral, PublicKeyToken=null-clr-namespace:TheLast.ViewsTheLast.Views  I Eunvell.ReoGrid, Version=3.0.0.0, Culture=neutral, PublicKeyToken=nullG4clr-namespace:unvell.ReoGrid;assembly=unvell.ReoGridunvell.ReoGrid     TheLast.Views.TestStep   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     -localclr-namespace:TheLast.Views     _ [MaterialDesignThemes.Wpf, Version=4.4.0.0, Culture=neutral, PublicKeyToken=df2a72020bd7962a:md1http://materialdesigninxaml.net/winfx/xaml/themes  =rg4clr-namespace:unvell.ReoGrid;assembly=unvell.ReoGrid  _	 [Microsoft.Xaml.Behaviors, Version=1.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a3i+http://schemas.microsoft.com/xaml/behaviors 	 &   System.Windows.Media.TextOptions    TextFormattingMode$  Ideal=�   TextRenderingMode$
 Auto=�   MaterialDesignBody#���   $��Regular"�$��13(�  MaterialDesignPaper#���  .��� $��50q� � ColumnDefinitions �� $
��0.2* ��� $��* �.+���  ! MaterialDesignOutlinedTextBox#��[ $��Stretch=� �� AcceptsReturn$
 True�� �� TextWrapping$
 Wrap=�" �� VerticalScrollBarVisibility$
 Auto=�)  #MaterialDesignThemes.Wpf.HintAssist   Hint$ 规格书要求������ *&$CurrentDto.SpecificationRequirements+� $
��20 0q�$��1�� � RowDefinitions �� $
��0.2* ��� .+�]� $��False��.+��� $��添加步骤��$	��100��$��10 0 0 0q� � Cursor$
 Hand��$��Right=����� *AddTestStep+�� $��加载测试步骤��$	��120��$��10 0 0 0q�$��Right=����� *LoadTestStep+�� $!��配置寄存器例外条件��$	��200��$��10 0 0 0q�$��Right=�$
 Hand������ *
JumpConfig+�� $��检查寄存器��$	��100��$��10 0 0 0q�$��Right=����� *CheckRegisters+&   System.Windows.Controls.DataGrid  -   $
��grid��	   SelectionUnit$	 FullRow=�$��1��$	��400��$��Stretch=�"
   VerticalScrollBarVisibility$

 Auto=�$   HorizontalScrollBarVisibility$
 Auto=�   AutoGenerateColumns$ False��   CanUserAddRows$ False��   CanUserDeleteRows$ False�� �� SelectedItem �� *CurrentItem+���� *TestStepDtos+   Columns 4  .System.Windows.Controls.DataGridTemplateColumn  ,  &System.Windows.Controls.DataGridColumn   Header$ 序号��   
IsReadOnly$
 True��   CellTemplate �� .R��� $��Center=�����  �� RelativeSource ��  �� Mode$ FindAncestor=� �� AncestorType)  #System.Windows.Controls.DataGridRow"   �� Path$ Header�  $ 应用条件��   Width5  /System.Windows.Controls.DataGridLengthConverter$	 200  �� .R��� $��
Conditions��$
 Wrap=����� *
Conditions+.��* 	 $Microsoft.Xaml.Behaviors.Interaction   Triggers +	 	 %Microsoft.Xaml.Behaviors.EventTrigger	   	  	EventName$ 	LostFocus��*
 	 $Microsoft.Xaml.Behaviors.TriggerBase 
  Actions. 2 	 ,Microsoft.Xaml.Behaviors.InvokeCommandAction     CommandParameter ��  �� ElementName$ 
Conditions��$
 Text�   Command �� *!DataContext.ConditionsLostFocus+ �� $ FindAncestor=� UserControl.��  $ 测试内容��$	 200  �� .R��� $��TestContent��$
 Wrap=����� *TestContent+.�� 	  $ 	LostFocus��.    �� $ TestContent��$
 Text� �� *" DataContext.TestContentLostFocus+ �� $ FindAncestor=� UserControl.��0  *System.Windows.Controls.DataGridTextColumn   ) %MaterialDesignDataGridTextColumnStyle1  +System.Windows.Controls.DataGridBoundColumn   ElementStyle# �  $ 测试过程��$
 True��    Binding  �� *TestProcess+  # �  $ 判断内容��$
 True��  �� *JudgmentContent+  $ 备注��$	 200  �� .R��� $��Remark��$
 Wrap=����� *Remark+.�� 	  $ 	LostFocus��.    �� $ Remark��$
 Text� �� *DataContext.RemarkLostFocus+ �� $ FindAncestor=� UserControl.��  $ 操作�� �� .R��� ! �� Orientation$! 
Horizontal=�.+��� $��配置测试过程������ $
 grid��$ CurrentCell.Item����� *DataContext.ConfigInit+ �� $ FindAncestor=� UserControl�� $��配置判断内容��$	��5 0q����� $
 grid��$ CurrentCell.Item����� *DataContext.ConfigFeedback+ �� $ FindAncestor=� UserControl�� $��删除������ $
 grid��$ CurrentCell.Item����� *DataContext.DeleteTestStep+ �� $ FindAncestor=� UserControl