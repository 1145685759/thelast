   M S B A M L   `   `   `  ���� B  >TheLast, Version=1.0.0.2, Culture=neutral, PublicKeyToken=null-clr-namespace:TheLast.ViewsTheLast.Views  /clr-namespace:TheLast.CommonTheLast.Common  E'clr-namespace:TheLast.Common.ConvertersTheLast.Common.Converters  #    TheLast.Views.BasicParameters   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     -localclr-namespace:TheLast.Views     Z VPrism.DryIoc.Wpf, Version=8.0.0.1909, Culture=neutral, PublicKeyToken=40ee6c3a2184dc59S OPrism.Wpf, Version=8.0.0.1909, Culture=neutral, PublicKeyToken=40ee6c3a2184dc59&prismhttp://prismlibrary.com/   _	 [MaterialDesignThemes.Wpf, Version=4.4.0.0, Culture=neutral, PublicKeyToken=df2a72020bd7962a:md1http://materialdesigninxaml.net/winfx/xaml/themes 	 'proxclr-namespace:TheLast.Common 	 _
 [MaterialDesignExtensions, Version=3.3.0.0, Culture=neutral, PublicKeyToken=c5ebccebe084d64fHmde>https://spiegelp.github.io/MaterialDesignExtensions/winfx/xaml 
 _ [Microsoft.Xaml.Behaviors, Version=1.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a3i+http://schemas.microsoft.com/xaml/behaviors  0cv'clr-namespace:TheLast.Common.Converters  !  Prism.Mvvm.ViewModelLocator    AutoWireViewModel  True   MaterialDesignBody#���   $��Regular"�$��13(�&   System.Windows.Media.TextOptions   TextFormattingMode$ Ideal=�   TextRenderingMode$
 Auto=�  MaterialDesignPaper#���   � 	Resources -   'TheLast.Common.Converters.TypeConverter  TypeConverter!   TheLast.Common.BindingProxy 	 proxy   Data�� %'   &	       &	           �� .��� $
��50 0q� � RowDefinitions �� $
��auto ��� .+�]� $��False��.+��� $	��250��$��Center=�) 	 #MaterialDesignThemes.Wpf.HintAssist   Hint$ 查找寄存器...��. 	 (MaterialDesignThemes.Wpf.TextFieldAssist   HasClearButton$
 True�� � Cursor$ Arrow������ *SearchString+	 �� Mode$	 TwoWay=�
 �� UpdateSourceTrigger$
 PropertyChanged=�.�� J� InputBindings �� 
 �� Key$ Enter�� �� Command �� *Search+.��� .+���  �� Orientation$ 
Horizontal=�.+��� $��内机号：��$��Center=�$��20q�y��� *
IsIndoor+�� -   $��	IndoorNum��$��Center=�$��50������ *	Indoors+y��� *
IsIndoor+*  $Microsoft.Xaml.Behaviors.Interaction   Triggers +  %Microsoft.Xaml.Behaviors.EventTrigger     	EventName$ SelectionChanged��*	  $Microsoft.Xaml.Behaviors.TriggerBase 	  Actions. 2
  ,Microsoft.Xaml.Behaviors.InvokeCommandAction
   
  CommandParameter ��  �� ElementName$ 	IndoorNum�� �� Path$ SelectedValue� 
  Command �� *#!DataContext.SelectedIndoorCommand+ �� RelativeSource ��  �� Mode$ FindAncestor=� �� AncestorType ItemsControl�� $ 
Horizontal=�.+��� $��外机号：��$��Center=�$��20q�y��� *	IsOutdoor+�� -   $��
OutdoorNum��$��Center=�$��50������ *
Outdoors+y��� *	IsOutdoor+   $ SelectionChanged��. 
   �� $ 
OutdoorNum��$ SelectedValue� �� *$"DataContext.SelectedOutdoorCommand+ �� $ FindAncestor=� ItemsControl�� $��导出寄存器配置��$��20q�$��Center=�$
 Hand��$��Right=����� *ExportConfig+�� $��加载寄存器配置��$��20q�$��Center=�$
 Hand��$��Right=����� *
LoadConfig+�� $��添 加 寄 存 器��$��20q�$��Center=�$
 Hand��$��Right=����� *AddRegister+&   System.Windows.Controls.DataGrid  -   $��RegisterDataGrid��$��1��   
IsReadOnly$
 True��   AutoGenerateColumns$ False��   CanUserAddRows$ False������ *RegisterDtos+   Columns 4  .System.Windows.Controls.DataGridTemplateColumn  ,  &System.Windows.Controls.DataGridColumn   Header$ 序号��   
IsReadOnly$
 True��   CellTemplate �� .R��� $��Center=�����  �� $ FindAncestor=�)  #System.Windows.Controls.DataGridRow"  $ Header�4  .System.Windows.Controls.DataGridCheckBoxColumn  $ 是否启用��1  +System.Windows.Controls.DataGridBoundColumn    Binding  �� *
IsEnable+0  *System.Windows.Controls.DataGridTextColumn  $ 寄存器命名��  �� *Name+  $ 传感器类型��  �� *Type+! �� 	Converter#! [ "   
Visibility" �� *Data.IsTemp+# �� Source## [   $ 所属站号��  �� *
StationNum+  $ 寄存器地址��  �� *	Address+  $ 操作�� �� .R��� $ 
Horizontal=�.+��� $��查看/编辑��$
��20 0q����� *DataContext.Check+ �� $ FindAncestor=� UserControl���� $ RegisterDataGrid��$ SelectedItem��� $��删除������ *DataContext.Deleted+ �� $ FindAncestor=� UserControl���� $ RegisterDataGrid��$ SelectedItem�