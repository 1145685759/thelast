   M S B A M L   `   `   `  ���� B  >TheLast, Version=1.0.0.2, Culture=neutral, PublicKeyToken=null;"clr-namespace:TheLast.Views.DialogTheLast.Views.Dialog  %    TheLast.Views.Dialog.JumpConfig   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     4local"clr-namespace:TheLast.Views.Dialog     _ [MaterialDesignThemes.Wpf, Version=4.4.0.0, Culture=neutral, PublicKeyToken=df2a72020bd7962a:md1http://materialdesigninxaml.net/winfx/xaml/themes  _ [Microsoft.Xaml.Behaviors, Version=1.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a3i+http://schemas.microsoft.com/xaml/behaviors  &   System.Windows.Media.TextOptions    TextFormattingMode$  Ideal=�   TextRenderingMode$
 Auto=�   MaterialDesignBody#���   $��Regular"�$��13(�  MaterialDesignPaper#���  .��� .+��  � RowDefinitions �� $
��auto ��� $
��auto ��� �� $
��auto �.+���  �� Padding$ 20,10q�$��配置跳过判断条件��$
��Bold"�$��20(�]� $��1��$��False��$
��20 0q�.+���  �� Orientation$ 
Horizontal=�$	��Top=�.+��� $��	站号：��$��Center=��� -   $��StationCombox��$
��10 0q�$��50��$��Center=����� *
StationNum+*  $Microsoft.Xaml.Behaviors.Interaction   Triggers +  %Microsoft.Xaml.Behaviors.EventTrigger     	EventName$ SelectionChanged��*  $Microsoft.Xaml.Behaviors.TriggerBase   Actions. 2  ,Microsoft.Xaml.Behaviors.InvokeCommandAction     CommandParameter �� 	 �� ElementName$	 StationCombox��
 �� Path$
 SelectedItem�   Command �� *$"DataContext.StationSelectedCommand+ �� RelativeSource ��  �� Mode$ FindAncestor=� �� AncestorType ItemsControl�� $��寄存器类型：��$��Center=��� -   $��
TypeCombox��$
��10 0q�$��50��$��Center=����� *RegisterTypes+   $ SelectionChanged��.    �� $	 
TypeCombox��$
 SelectedItem� �� *DataContext.SelectedCommand+ �� $ FindAncestor=� ItemsControl�� $��可选寄存器：��$��Center=��� -   $��RegisterCombox��$��10 0 q�$��50��$��Center=����� *RegisterNames+   $ SelectionChanged��.    �� $	 RegisterCombox��$
 SelectedItem� �� *%#DataContext.SelectedRegisterCommand+ �� $ FindAncestor=� ItemsControl�� $��添加��$
��20 0q����� *Add+���� $	 RegisterCombox��$
 SelectedItem�]� $��2��$��False��$��20 10q�.+�&   System.Windows.Controls.DataGrid     AutoGenerateColumns$ False��   CanUserAddRows$ False��$	��700��$
��0 20q����� *JumpRegister+   Columns 0  *System.Windows.Controls.DataGridTextColumn  ,  &System.Windows.Controls.DataGridColumn   Header$ 站号��1	  +System.Windows.Controls.DataGridBoundColumn 	  Binding �� *
StationNum+  $ 	寄存器�� �� *Name+4
  .System.Windows.Controls.DataGridTemplateColumn
  $ 操作�� 
  CellTemplate �� .R��� $��删除������ ���� *DataContext.Delete+ �� $ FindAncestor=� UserControl�� $��10q�$ 
Horizontal=�$��3��$��Right=�.+��� $��取消��$��0,0,10,0q�   MaterialDesignOutlinedButton#��[ ���� *CancelCommand+�� $��确定������ *SaveCommand+