   M S B A M L   `   `   `  ���� B  >TheLast, Version=1.0.0.2, Culture=neutral, PublicKeyToken=null-clr-namespace:TheLast.ViewsTheLast.Views  7 clr-namespace:TheLast.ExtensionsTheLast.Extensions      TheLast.Views.LoginView   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     _ [Microsoft.Xaml.Behaviors, Version=1.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a3i+http://schemas.microsoft.com/xaml/behaviors  'localclr-namespace:TheLast.Views  Dmc;http://schemas.openxmlformats.org/markup-compatibility/2006  _ [MaterialDesignThemes.Wpf, Version=4.4.0.0, Culture=neutral, PublicKeyToken=df2a72020bd7962a:md1http://materialdesigninxaml.net/winfx/xaml/themes  +pass clr-namespace:TheLast.Extensions  b	 ^SharpVectors.Converters.Wpf, Version=1.7.7.0, Culture=neutral, PublicKeyToken=b532964b8548be771svgc&http://sharpvectors.codeplex.com/svgc/ 	 Z
 VPrism.DryIoc.Wpf, Version=8.0.0.1909, Culture=neutral, PublicKeyToken=40ee6c3a2184dc59S OPrism.Wpf, Version=8.0.0.1909, Culture=neutral, PublicKeyToken=40ee6c3a2184dc59&prismhttp://prismlibrary.com/ 
  &   System.Windows.Media.TextOptions    TextFormattingMode$  Ideal=�   TextRenderingMode$
 Auto=�   LightInfoBrush#���   $��Regular"�$��13(�  MaterialDesignPaper#���  $	��600��$	��350��#  Prism.Services.Dialogs.Dialog   WindowStyle ��  �� 
TargetType" �.���  �� Property � �� �� Value$	 600����  � ��$	 350����  �@�SizeToContent$ WidthAndHeight=���  �@�
ResizeMode$ NoResize=���  �@ WindowStartupLocation$ CenterScreen=�.��� ����  �� ImageSource$ ../Images/a.jpg�� � ColumnDefinitions �� $
��1.5* ��� .+�'  !MaterialDesignThemes.Wpf.Snackbar  -     Name$ LoginSnakeBar��$��2��	 K� ZIndex$	 1��
   MessageQueue
 4  .MaterialDesignThemes.Wpf.MessageQueueExtension  7  1MaterialDesignThemes.Wpf.Transitions.Transitioner  $��1�� �� SelectedIndex �� *SelectIndex+ �� FallbackValue$ 0��.@�<  6MaterialDesignThemes.Wpf.Transitions.TransitionerSlide  .��]� $��15q�$��Center=�.+��� $
��0,10q�$	��Top=�$��22(�$
��Bold"�$��欢迎使用���� $
��0,10q�$	��Top=�$��22(�$
��Bold"�$��AUX自动化测试软件���� $
��0,10q�)  #MaterialDesignThemes.Wpf.HintAssist   Hint$ 请输入账号��$	��Top=����� *
UserName+G� $
��0,10q�$ 请输入密码��$	��Top=�+   %TheLast.Extensions.PassWordExtensions   PassWord �� *
PassWord+ �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=�*	  $Microsoft.Xaml.Behaviors.Interaction 	  	Behaviors )
   #TheLast.Extensions.PasswordBehavior
  �� $��Login��$��登录系统��$	��Top=����� *ExecuteCommand+]� $	��0,5q�$��False��.+��� $��注册账号��$
��Bold"� 	  Triggers +  %Microsoft.Xaml.Behaviors.EventTrigger     	EventName$ MouseLeftButtonDown��*  $Microsoft.Xaml.Behaviors.TriggerBase   Actions. 2  ,Microsoft.Xaml.Behaviors.InvokeCommandAction     CommandParameter$ ResgiterPage��   Command �� *ExecuteCommand+  .��]� $��15q�$��Center=�.+��� $
��0,10q�$	��Top=�$��22(�$
��Bold"�$��注册账号���� $	��0,5q�$ 请输入用户名��$	��Top=����� *UserDto.Account+�� $	��0,5q�$ 请输入账号��$	��Top=� � Cursor$ Arrow������ *UserDto.UserName+G� $	��0,5q�$ 请输入密码��$	��Top=�$ IBeam�� �� *UserDto.Password+$ TwoWay=�$ PropertyChanged=� 
  G� $	��0,5q�$ 请再次输入密码��$	��Top=�$ Arrow�� �� *UserDto.NewPassWord+$ TwoWay=�$ PropertyChanged=� 
  �� $��Resgiter��$��注册账号��$	��Top=����� *ExecuteCommand+�� $
��0,10q�$��Return��$��返回登录��$	��Top=�   MaterialDesignOutlinedButton#��[ ���� *ExecuteCommand+