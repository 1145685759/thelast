   M S B A M L   `   `   `  ���� B  >TheLast, Version=1.0.0.2, Culture=neutral, PublicKeyToken=null-clr-namespace:TheLast.ViewsTheLast.Views  %    TheLast.Views.Dialog.ComSetting   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     -localclr-namespace:TheLast.Views     _ [MaterialDesignThemes.Wpf, Version=4.4.0.0, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  $	��450��$	��310��&   System.Windows.Media.TextOptions    TextFormattingMode$  Ideal=�   TextRenderingMode$
 Auto=�   MaterialDesignBody#���   $��Regular"�$��13(�  MaterialDesignPaper#���  .��#  MaterialDesignThemes.Wpf.Card  .���  � RowDefinitions �� $
��auto ��� �� $
��auto �.+���  �� Padding$ 20,10q�$��系统设置��$
��Bold"�$��20(��� $��1��$��Center=�$��Center=�.+���  �� Orientation$ 
Horizontal=�$
��Left=�.+��� $��内机数量：��$��Center=��� $��Center=�$	��200������ *IndoorCount+�� $ 
Horizontal=�$
��Left=�$	��0 5q�.+��� $��外机数量：��$��Center=��� $��Center=�$	��200������ *OutdoorCount+�� $ 
Horizontal=�$
��Left=�.+��� $��内机控制方式：��$��Center=���  �� DisplayMemberPath$ DisplayValue��$��Center=�$	��175������ *IMCMS+ �� SelectedIndex �� *IMCM+�� $ 
Horizontal=�$
��Left=�$	��0 5q�.+��� $��机组控制方式：��$��Center=��� $ DisplayValue��$��Center=�$	��175������ *UCMS+ �� *UCM+�� $ 
Horizontal=�$
��Left=�.+��� $��风速挡位：��$��Center=��� $ DisplayValue��$��Center=�$	��200������ *WSGS+ �� *WSG+�� $ 
Horizontal=�$
��Left=�$	��0 5q�.+��� $��端口号：��$��Center=��� $��Center=�$	��215������ *
ComPorts+ �� SelectedItem �� *CurrentComPort+�� $��10q�$ 
Horizontal=�$��2��$��Right=�.+��� $��取消��$��0,0,10,0q�   MaterialDesignOutlinedButton#��[ ���� *CancelCommand+�� $��确定�� � Cursor$
 Hand������ *SaveCommand+