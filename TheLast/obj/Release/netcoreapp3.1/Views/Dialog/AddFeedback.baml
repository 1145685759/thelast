   M S B A M L   `   `   `  ���� B  >TheLast, Version=1.0.0.2, Culture=neutral, PublicKeyToken=null;"clr-namespace:TheLast.Views.DialogTheLast.Views.Dialog  &     TheLast.Views.Dialog.AddFeedback   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     4local"clr-namespace:TheLast.Views.Dialog     _ [MaterialDesignThemes.Wpf, Version=4.4.0.0, Culture=neutral, PublicKeyToken=df2a72020bd7962a:md1http://materialdesigninxaml.net/winfx/xaml/themes  _ [Microsoft.Xaml.Behaviors, Version=1.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a3i+http://schemas.microsoft.com/xaml/behaviors  &   System.Windows.Media.TextOptions    TextFormattingMode$  Ideal=�   TextRenderingMode$
 Auto=�   MaterialDesignBody#���   $��Regular"�$��13(�  MaterialDesignPaper#���  .��� .+��  � RowDefinitions �� $
��auto ��� �� $
��auto �.+���  �� Padding$ 20,10q�$��配置判断内容��$
��Bold"�$��20(�]� $��1��$��False��$��20 10q�.+���  �� Orientation$ 
Horizontal=�$	��Top=�.+��� $��	站号：��$��Center=��� -   $��StationCombox��$
��10 0q�$��50��$��Center=����� *StationNumList+ �� SelectedItem �� *CurrentStationNum+*  $Microsoft.Xaml.Behaviors.Interaction   Triggers +  %Microsoft.Xaml.Behaviors.EventTrigger     	EventName$ SelectionChanged��*  $Microsoft.Xaml.Behaviors.TriggerBase   Actions. 2  ,Microsoft.Xaml.Behaviors.InvokeCommandAction  	   CommandParameter	 �� 
 �� ElementName$
 StationCombox�� �� Path$ SelectedItem�   Command �� *$"DataContext.StationSelectedCommand+ �� RelativeSource ��  �� Mode$ FindAncestor=� �� AncestorType ItemsControl�� $��寄存器类型：��$��Center=��� -   $��
TypeCombox��$
��10 0q�$��50��$��Center=����� *RegisterTypeDtos+ �� *CurrentRegisterType+   $ SelectionChanged��.   	 �� $
 
TypeCombox��$ SelectedItem� �� *DataContext.SelectedCommand+ �� $ FindAncestor=� ItemsControl� $	��5 0q�.+��� $ 
Horizontal=�.+��� $��外机号：��$��Center=�y��� *	IsOutdoor+�� -   $��Outdoor��$��50������ *OutdoorNums+ �� SelectedValue �� *
OutdoorNum+y��� *	IsOutdoor+   $ SelectionChanged��.   	 �� $
 Outdoor��$ SelectedValue� �� *$"DataContext.SelectedOutdoorCommand+ �� $ FindAncestor=� ItemsControl�� $ 
Horizontal=�.+��� $��内机号：��$��Center=�y��� *
IsIndoor+�� -   $��Indoor��$��50������ *
IndoorNums+ �� *	IndoorNum+y��� *
IsIndoor+   $ SelectionChanged��.   	 �� $
 Indoor��$ SelectedValue� �� *#!DataContext.SelectedIndoorCommand+ �� $ FindAncestor=� ItemsControl�� $��可选寄存器：��$��Center=��� -   $��RegisterCombox�� �� DisplayMemberPath$
 Name��$
��10 0q�$��50��$��Center=����� *RegisterDtos+ �� *CurrentRegister+   $ SelectionChanged��.   	 �� $
 RegisterCombox��$ SelectedItem� �� *%#DataContext.SelectedRegisterCommand+ �� $ FindAncestor=� ItemsControl�� $��运算符：��$��Center=��� $��Center=�$�� 10 0q�$��50������ *Operationals+ �� *CurrentOperational+�� $��目标值：��$��Center=��� $��Center=�$ DisplayValue��$�� 10 0q�$��50������ *ValueDictionaryDtos+ �� 
IsEditable �� *
IsEditable+ �� *CurrentValueDictionary+ �� Text �� *
InputValue+�� $��延时模式：��$��Center=��� $��Center=�$
 Mode��$�� 10 0q�$��50������ *DelayModeList+ �� SelectedIndex �� *CurrentDelayMode+�� $��延时时间：��$��Center=��� $��Center=�$�� 10 0q�$��50������ *	DelatTime+�� $��添加������ *AddFeedback+&   System.Windows.Controls.DataGrid     AutoGenerateColumns$ False��   
IsReadOnly$
 True��   CanUserAddRows$ False��$	��700��$
��0 20q����� *AddFeedbackDtoList+   Columns 0  *System.Windows.Controls.DataGridTextColumn  ,  &System.Windows.Controls.DataGridColumn   Header$ 站号��1	  +System.Windows.Controls.DataGridBoundColumn 	  Binding �� *
StationNum+  $ 	寄存器�� �� *	Address+  $ 	运算符�� �� *Operational+  $ 	目标值�� �� *DisplayTagetValue+  $ 延时模式�� �� *DisplayDelayMode+  $ 延时时间�� �� *	DelayTime+4
  .System.Windows.Controls.DataGridCheckBoxColumn
  $ 是否跳过判断�� �� *IsJump+4  .System.Windows.Controls.DataGridTemplateColumn  $ 操作��   CellTemplate �� .R��� $ 
Horizontal=�.+��� $��删除������ ���� *#!DataContext.DeleteFeedbackCommand+ �� $ FindAncestor=� UserControl�� $��10q�$ 
Horizontal=�$��2��$��Right=�.+��� $��取消��$��0,0,10,0q�   MaterialDesignOutlinedButton#��[  � Cursor$
 Hand������ *CancelCommand+�� $��确定������ *SaveCommand+