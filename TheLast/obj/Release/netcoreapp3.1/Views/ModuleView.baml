   M S B A M L   `   `   `  ���� B  >TheLast, Version=1.0.0.2, Culture=neutral, PublicKeyToken=null-clr-namespace:TheLast.ViewsTheLast.Views  E'clr-namespace:TheLast.Common.ConvertersTheLast.Common.Converters      TheLast.Views.ModuleView   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     -localclr-namespace:TheLast.Views     6cv'clr-namespace:TheLast.Common.Converters     _ [MaterialDesignThemes.Wpf, Version=4.4.0.0, Culture=neutral, PublicKeyToken=df2a72020bd7962a:md1http://materialdesigninxaml.net/winfx/xaml/themes  _ [Microsoft.Xaml.Behaviors, Version=1.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a3i+http://schemas.microsoft.com/xaml/behaviors  &   System.Windows.Media.TextOptions    TextFormattingMode$  Ideal=�   TextRenderingMode$
 Auto=�   MaterialDesignBody#���   $��Regular"�$��13(�  MaterialDesignPaper#���   � 	Resources 7   1TheLast.Common.Converters.IntToVisibilityConveter  IntToVisibility��%   &	         .��)  #MaterialDesignThemes.Wpf.DialogHost  .��)  #MaterialDesignThemes.Wpf.DrawerHost     IsRightDrawerOpen �� *IsRightDrawerOpen+   RightDrawerContent ]� $	��300��$��False��.+���  �� Padding$ 20,10q�$	��Top=�$��20(�$
��Bold"�$��添加模块���� $
��20,0q�)  #MaterialDesignThemes.Wpf.HintAssist   Hint$ 请输入模块名称��$	��Top=����� *CurrentDto.ModuleName+�� $	��100��$��20q� �� TextWrapping$
 Wrap=�$ 请输入规格书要求��$	��Top=����� *&$CurrentDto.SpecificationRequirements+�� $��保存��$
��20,0q�$��添加/编辑 模块��$	��Top=� � Cursor$
 Hand������ *ExecuteCommand+.��� 	 � RowDefinitions	 �� $
��auto ��� $
��283* ��� $
��125* �.+��� $��15,0,0,0q�
 �� Orientation$
 
Horizontal=�.+��� $	��250��$��Center=�$ 查找模块...��.  (MaterialDesignThemes.Wpf.TextFieldAssist   HasClearButton$
 True������ *Search+ �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=�.�� J� InputBindings �� 
 �� Key$ Enter�� �� CommandParameter$ 查询�� �� Command �� *ExecuteCommand+.���� $��
687,5,20,5q�$��Right=�$��新增��$��+ 添加模块��$
 Hand������ *ExecuteCommand+�� $��1��$	��Top=�$��	0,123,0,0q�$��2��y��� *ModuleDtos.Count+ �� 	Converter# [ .+��� $
��0,10q�$��18(�$��Center=�$?��9尝试添加一些模块，以便在此处查看它们。���� $��1��$��2��.���� $��Center=����� *
ModuleDtos+���� .R�� ���� .R�?  9MaterialDesignThemes.Wpf.Transitions.TransitioningContent  C  =MaterialDesignThemes.Wpf.Transitions.TransitioningContentBase   OpeningEffect D	  >MaterialDesignThemes.Wpf.Transitions.TransitionEffectExtension	   	  Kind$ ExpandIn=�.��� $	��220��$	��180��$	��250��$��8q�*
  $Microsoft.Xaml.Behaviors.Interaction 
  Triggers +  %Microsoft.Xaml.Behaviors.EventTrigger     	EventName$ MouseLeftButtonUp��*  $Microsoft.Xaml.Behaviors.TriggerBase   Actions. 2  ,Microsoft.Xaml.Behaviors.InvokeCommandAction     CommandParameter ��    Command �� *" DataContext.CheckTestStepCommand+ �� RelativeSource ��  �� Mode$ FindAncestor=� �� AncestorType ItemsControl	 �� $
��auto ��� .+�'  !MaterialDesignThemes.Wpf.PopupBox  $��Right=� K� ZIndex$ 1��   PopupContent. �� .+��� $��删除������ ���� *DataContext.DeleteCommand+ �� $ FindAncestor=� ItemsControl�� $��修改模块������ ���� *DataContext.SelectedCommand+ �� $ FindAncestor=� ItemsControl��   PrimaryHueMidBrush#���  $��2�� �� CornerRadius$ 3���� $
 10,5q�$
��Bold"����� *
ModuleName+�� $��1��.+��� $
 
Horizontal=�.+��� $��总步骤数:��$
 10,5q��� $��Center=����� *TestStepCount+�� $
 
Horizontal=�.+��� $��异常步骤数:��$
 10,5q��� $��Center=����� *AbnormalTestStep+�� $��2��$
}�True��.+��� 
  �� Top$  10��! �� Right$	! -50��$	��120��$	��120�����#FFFFFF$	 100��" J� Opacity$	" 0.1Q��� $  80��$	! -30��$	��120��$	��120�����#FFFFFF$	 100��$	" 0.1Q�