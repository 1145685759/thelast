   M S B A M L   `   `   `  ���� B  >TheLast, Version=1.0.0.2, Culture=neutral, PublicKeyToken=null;"clr-namespace:TheLast.Views.DialogTheLast.Views.Dialog  &     TheLast.Views.Dialog.AddRegister   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     4local"clr-namespace:TheLast.Views.Dialog     _ [MaterialDesignThemes.Wpf, Version=4.4.0.0, Culture=neutral, PublicKeyToken=df2a72020bd7962a:md1http://materialdesigninxaml.net/winfx/xaml/themes  Z VPrism.DryIoc.Wpf, Version=8.0.0.1909, Culture=neutral, PublicKeyToken=40ee6c3a2184dc59S	 OPrism.Wpf, Version=8.0.0.1909, Culture=neutral, PublicKeyToken=40ee6c3a2184dc59&prismhttp://prismlibrary.com/  	 &   System.Windows.Media.TextOptions    TextFormattingMode$  Ideal=�   TextRenderingMode$
 Auto=�   MaterialDesignBody#���   $��Regular"�$��13(�  MaterialDesignPaper#���  $	��450��.��� .+��  � RowDefinitions �� $
��auto ��� �� $
��auto �.+���  �� Padding$ 20,10q�$��添加寄存器��$
��Bold"�$��20(�]� $��1��$��False��.+���  �� Orientation$ 
Horizontal=�$	��Top=�$��20 10q�.+��� $��所属站号：��$��Center=��� )  #MaterialDesignThemes.Wpf.HintAssist   Hint$ 请输入所属站号��$
��10 0q�$��Center=� � Cursor$ IBeam������ *Model.StationNum+�� $ 
Horizontal=�$	��Top=�$��20 10q�.+��� $��寄存器地址：��$��Center=��� $
��10,0q�$ 请输入寄存器地址��$ IBeam������ *Model.Address+�� $ 
Horizontal=�$��20,10q�$	��Top=�.+��� $��寄存器命名：��$��Center=��� $
��10 0q� �� AcceptsReturn$
 True�� �� TextWrapping$
 Wrap=�$��Center=�$ 请给寄存器命名��$	��Top=�$ IBeam������ *
Model.Name+�� $ 
Horizontal=�$��20,10q�$	��Top=�.+��� $'��!是否需要显示实时曲线：��$��Center=��� $
��10 0q�$��Center=�$��是/否 显示��$ IBeam��	 d� 	IsChecked	 �� *Model.IsDisplay+�� $ 
Horizontal=�$��20,10q�$	��Top=�y��� *
Visibility+.+��� $��起始地址：��$��Center=��� $
��10 0q�$
 True��$
 Wrap=�$��Center=�$ 请输入起始地址��$	��Top=�$ IBeam������ *Model.StartAddress+�� $ 
Horizontal=�$��20,10q�$	��Top=�y��� *
Visibility+.+��� $��传感器类型：��$��Center=��� $ 请选择传感器类型������ *Types+
 �� SelectedIndex
 �� *
Model.Type+�� $ 
Horizontal=�$��20,10q�.+��� $!��是否需要历史记录：��$��Center=��� $
��10 0q�$��Center=�$��是/否��$ IBeam��	 �� *Model.IsHsData+�� $��10q�$ 
Horizontal=�$��2��$��Right=�.+��� $��取消��$��0,0,10,0q�   MaterialDesignOutlinedButton#��[ ���� *CancelCommand+�� $��确定������ *SaveCommand+