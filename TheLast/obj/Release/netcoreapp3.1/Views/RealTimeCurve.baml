   M S B A M L   `   `   `  ���� B  >TheLast, Version=1.0.0.2, Culture=neutral, PublicKeyToken=null-clr-namespace:TheLast.ViewsTheLast.Views  !    TheLast.Views.RealTimeCurve   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     -localclr-namespace:TheLast.Views     R NOxyPlot.Wpf, Version=2.1.0.0, Culture=neutral, PublicKeyToken=75e952ba404cdbb0Y UOxyPlot.Wpf.Shared, Version=2.1.0.0, Culture=neutral, PublicKeyToken=75e952ba404cdbb0"oxyhttp://oxyplot.org/wpf   _	 [MaterialDesignThemes.Wpf, Version=4.4.0.0, Culture=neutral, PublicKeyToken=df2a72020bd7962a:md1http://materialdesigninxaml.net/winfx/xaml/themes 	 _
 [Microsoft.Xaml.Behaviors, Version=1.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a3i+http://schemas.microsoft.com/xaml/behaviors 
 &   System.Windows.Media.TextOptions    TextFormattingMode$  Ideal=�   TextRenderingMode$
 Auto=�   MaterialDesignBody#���   $��Regular"�$��13(�  MaterialDesignPaper#���  $	��800��$	��600��.���  � RowDefinitions �� �� $
��0.1* ��� $
��0.1* �.+�  OxyPlot.Wpf.PlotView  $��50q�  OxyPlot.Wpf.PlotViewBase   Model �� *	MyModel+�� $��1�� �� Orientation$ 
Horizontal=�$��Center=�$��Center=�.+��� $ 
Horizontal=�.+��� $��	站号：��$��Center=��� -   $��Station��$��Center=����� *StationNums+* 
 $Microsoft.Xaml.Behaviors.Interaction   Triggers + 
 %Microsoft.Xaml.Behaviors.EventTrigger     	EventName$ SelectionChanged��* 
 $Microsoft.Xaml.Behaviors.TriggerBase   Actions. 2 
 ,Microsoft.Xaml.Behaviors.InvokeCommandAction     CommandParameter �� 	 �� ElementName$	 Station��
 �� Path$
 SelectedItem�   Command �� *StationSelected+�� $ 
Horizontal=�$��20 0 0 0q�.+��� $��寄存器类型：��$��Center=��� -   $��RegisterTypes��$��Center=����� *RegisterTypes+   $ SelectionChanged��.    �� $	 RegisterTypes��$
 SelectedItem� �� *TypesSelected+�� $ 
Horizontal=�$��20 0 0 0q�y��� *
IsIndoor+.+��� $��内机号：��$��Center=�y��� *
IsIndoor+�� -   $��Indoor��$��Center=����� *	Indoors+ �� SelectedIndex �� *Index+y��� *
IsIndoor+   $ SelectionChanged��.    �� $	 Indoor��$
 SelectedItem� �� *SelectedIndoors+�� $ 
Horizontal=�$��20 0 0 0q�y��� *	IsOutdoor+.+��� $��外机号：��$��Center=�y��� *	IsOutdoor+�� -   $��Outdoor��$��Center=����� *
Outdoors+ �� *Index+y��� *	IsOutdoor+   $ SelectionChanged��.    �� $	 Outdoor��$
 SelectedItem� �� *SelectedOutdoors+�� $��2��.���� $��Center=����� *RegisterDtosDisplay+���� .R�� ���� .R��� $ 
Horizontal=�$
��10 0q�.+��� ���� *Name+��  d� 	IsChecked �� *	IsVisible+���� ���� *DataContext.Check+ �� RelativeSource ��  �� Mode$ FindAncestor=� �� AncestorType ItemsControl