   M S B A M L   `   `   `  ���� B  >TheLast, Version=1.0.0.2, Culture=neutral, PublicKeyToken=null;"clr-namespace:TheLast.Views.DialogTheLast.Views.Dialog  (    "TheLast.Views.Dialog.CheckRegister   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     4local"clr-namespace:TheLast.Views.Dialog     _ [MaterialDesignThemes.Wpf, Version=4.4.0.0, Culture=neutral, PublicKeyToken=df2a72020bd7962a:md1http://materialdesigninxaml.net/winfx/xaml/themes  &   System.Windows.Media.TextOptions    TextFormattingMode$  Ideal=�   TextRenderingMode$
 Auto=�   MaterialDesignBody#���   $��Regular"�$��13(�  MaterialDesignPaper#���  $	��450��.��� .+��  � RowDefinitions �� $
��auto ��� �� $
��auto �.+���  �� Padding$ 20,10q�$��查看/编辑寄存器��$
��Bold"�$��20(�]� $��1��$��False��.+��� $��是否启用:��$	��Top=�$��20 10q��� $	��Top=�$��20 10q� �� SelectedIndex �� *SelectedIndex+.@��� $��关闭���� $��启用���� $��所属站号：��$	��Top=�$��20 10q��� $��20,10q�)  #MaterialDesignThemes.Wpf.HintAssist   Hint$ 请输入所属站号��$	��Top=� � Cursor$ IBeam������ *Model.StationNum+�� $��寄存器地址：��$	��Top=�$��20 10q��� $��20,10q�$ 请输入寄存器地址��$	��Top=�$ IBeam������ *Model.Address+�� $��寄存器描述：��$	��Top=�$��20 10q��� $��20,10q�$��Center=� �� AcceptsReturn$
 True�� �� TextWrapping$
 Wrap=�$ 请给寄存器命名��$	��Top=�$ IBeam������ *
Model.Name+�� $!��是否显示实时曲线：��$	��Top=�$��20 10q��� $��20,10q�$	��Top=�$��Center=�$��是/否 显示��	 d� 	IsChecked	 �� *Model.IsDisplay+�� 
 �� Orientation$
 
Horizontal=�$��20,10q�$	��Top=�y��� *
Visibility+.+��� $��接入设备地址：��$��Center=��� $
��10 0q�$
 True��$
 Wrap=�$��Center=�$ 接入设备地址��$	��Top=�$ IBeam������ *Model.AccessAddress+�� $
 
Horizontal=�$��20,10q�$	��Top=�y��� *
Visibility+.+��� $��传感器类型：��$��Center=��� $ 请选择传感器类型������ *Types+ �� *
Model.Type+�� $!��是否需要历史记录：��$	��Top=�$��20 10q��� $��20,10q�$��Center=�$��是/否��	 �� *Model.IsHsData+�� $	�� 10q�$
 
Horizontal=�$��2��$��Right=�.+��� $��取消��$��0,0,10,0q�   MaterialDesignOutlinedButton#��[ ���� *CancelCommand+�� $��确定������ *SaveCommand+