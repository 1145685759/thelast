   M S B A M L   `   `   `  ���� B  >TheLast, Version=1.0.0.2, Culture=neutral, PublicKeyToken=nullE'clr-namespace:TheLast.Common.ConvertersTheLast.Common.Converters  -clr-namespace:TheLast.ViewsTheLast.Views  "    TheLast.Views.ProjectManager   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     6cv'clr-namespace:TheLast.Common.Converters     -localclr-namespace:TheLast.Views     _ [MaterialDesignThemes.Wpf, Version=4.4.0.0, Culture=neutral, PublicKeyToken=df2a72020bd7962a:md1http://materialdesigninxaml.net/winfx/xaml/themes  _ [Microsoft.Xaml.Behaviors, Version=1.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a3i+http://schemas.microsoft.com/xaml/behaviors  &   System.Windows.Media.TextOptions    TextFormattingMode$  Ideal=�   TextRenderingMode$
 Auto=�   MaterialDesignBody#���   $��Regular"�$��13(�  MaterialDesignPaper#���   � 	Resources 7   1TheLast.Common.Converters.IntToVisibilityConveter  IntToVisibility��%   &	         .��)  #MaterialDesignThemes.Wpf.DialogHost  .��)  #MaterialDesignThemes.Wpf.DrawerHost     IsRightDrawerOpen �� *IsRightDrawerOpen+   RightDrawerContent ]� $	��300��$��False��.+���  �� Padding$ 20,10q�$	��Top=�$��20(�$
��Bold"�$��添加项目���� $
��20,0q�)  #MaterialDesignThemes.Wpf.HintAssist   Hint$ 请输入项目名称��$	��Top=����� *CurrentDto.ProjectName+�� $	��100��$��20q�$ 请输入项目描述��$	��Top=����� *CurrentDto.ProjectDescribe+�� $��保存��$
��20,0q�$��添加/编辑 项目��$	��Top=� � Cursor$
 Hand������ *ExecuteCommand+.���  � RowDefinitions �� $
��auto ��� $
��283* ��� $
��125* �.+��� $��15,0,0,0q�	 �� Orientation$	 
Horizontal=�.+��� $	��250��$��Center=�$ 查找项目...��.  (MaterialDesignThemes.Wpf.TextFieldAssist
   HasClearButton$

 True������ *Search+ �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=�.�� J� InputBindings �� 
 �� Key$ Enter�� �� CommandParameter$ 查询�� �� Command �� *ExecuteCommand+.���� $��
687,5,20,5q�$��Right=�$��新增��$��+ 添加项目��$
 Hand������ *ExecuteCommand+�� $��1��$	��Top=�$��	0,123,0,0q�$��2��y��� *ProjectDtos.Count+ �� 	Converter# [ .+��� $��/Images/wind.png��$	��120��$	��120���� $
��0,10q�$��18(�$��Center=�$?��9尝试添加一些项目，以便在此处查看它们。���� $��1��$��2��.���� $��Center=����� *ProjectDtos+���� .R�� ���� .R�?  9MaterialDesignThemes.Wpf.Transitions.TransitioningContent  C  =MaterialDesignThemes.Wpf.Transitions.TransitioningContentBase   OpeningEffect D	  >MaterialDesignThemes.Wpf.Transitions.TransitionEffectExtension	   	  Kind$ ExpandIn=�.��� $	��220��$	��180��$	��250��$��8q�*
  $Microsoft.Xaml.Behaviors.Interaction 
  Triggers +  %Microsoft.Xaml.Behaviors.EventTrigger     	EventName$ MouseLeftButtonUp��*  $Microsoft.Xaml.Behaviors.TriggerBase   Actions. 2  ,Microsoft.Xaml.Behaviors.InvokeCommandAction     CommandParameter ��    Command �� * DataContext.CheckModuleCommand+ �� RelativeSource ��  �� Mode$ FindAncestor=� �� AncestorType ItemsControl �� $
��auto ��� .+�'  !MaterialDesignThemes.Wpf.PopupBox  $��Right=� K� ZIndex$ 1��   PopupContent. �� .+��� $��删除������ ���� *DataContext.DeleteCommand+ �� $ FindAncestor=� ItemsControl�� $��修改模块������ ���� *DataContext.SelectedCommand+ �� $ FindAncestor=� ItemsControl�� $��自动测试������ ���� *%#DataContext.AutomaticTestingCommand+ �� $ FindAncestor=� ItemsControl��   PrimaryHueMidBrush#���  $��2�� �� CornerRadius$ 3���� $
 10,5q�$
��Bold"����� *ProjectName+�� $��1��.+��� $
 10,5q����� *ProjectDescribe+�� $	 
Horizontal=�.+��� $��总模块数:��$
 10,5q��� $��Center=����� *ModuleCount+�� $��2��$
}�True��.+��� 
 �� Top$ 10��  �� Right$	  -50��$	��120��$	��120�����#FFFFFF$	 100��! J� Opacity$	! 0.1Q��� $ 80��$	  -30��$	��120��$	��120�����#FFFFFF$	 100��$	! 0.1Q�