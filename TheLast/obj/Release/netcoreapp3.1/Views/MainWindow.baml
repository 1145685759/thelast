   M S B A M L   `   `   `  ���� B  >TheLast, Version=1.0.0.2, Culture=neutral, PublicKeyToken=null7 clr-namespace:TheLast.ExtensionsTheLast.Extensions      TheLast.Views.MainWindow   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Z VPrism.DryIoc.Wpf, Version=8.0.0.1909, Culture=neutral, PublicKeyToken=40ee6c3a2184dc59S OPrism.Wpf, Version=8.0.0.1909, Culture=neutral, PublicKeyToken=40ee6c3a2184dc59&prismhttp://prismlibrary.com/   _	 [MaterialDesignThemes.Wpf, Version=4.4.0.0, Culture=neutral, PublicKeyToken=df2a72020bd7962a:md1http://materialdesigninxaml.net/winfx/xaml/themes 	 *ext clr-namespace:TheLast.Extensions 	 _
 [MaterialDesignExtensions, Version=3.3.0.0, Culture=neutral, PublicKeyToken=c5ebccebe084d64fHmde>https://spiegelp.github.io/MaterialDesignExtensions/winfx/xaml 
 !  Prism.Mvvm.ViewModelLocator    AutoWireViewModel  True   MaterialDesignBody#���   $��Regular"�$��13(� � WindowStyle$
 None=� � WindowStartupLocation$ CenterScreen=�&   System.Windows.Media.TextOptions   TextFormattingMode$ Ideal=�   TextRenderingMode$
 Auto=� � AllowsTransparency$
 True�� � Icon$ /Images/favicon.ico��  MaterialDesignPaper#���  $	��768��$
��1280�� � Title �� *Title+.���  � RowDefinitions �� $
��auto ��� .+�' 	 !MaterialDesignThemes.Wpf.Snackbar  -   	  Name$	 Snackbar��
 K� ZIndex$
 1��   MessageQueue 4 	 .MaterialDesignThemes.Wpf.MessageQueueExtension  ( 	 "MaterialDesignThemes.Wpf.ColorZone  -     Name$ 	ColorZone��$��16q�+ 	 %MaterialDesignThemes.Wpf.ShadowAssist   ShadowDepth$ Depth2=�$	��Top=�   Mode$ 
PrimaryMid=�.��]� $
��True��.+��� $��Right=� �� Orientation$ 
Horizontal=�.+�' 	 !MaterialDesignThemes.Wpf.PopupBox     PopupContent. o� .@�m� -   $��检查更新���� -    ��Name$ btnMin��$	��—�� ! MaterialDesignFlatMidBgButton#��[ �� -   $ btnMax��$	��☐��#��[ �� -   $ btnClose��$	��✕��#��[ �� $ 
Horizontal=�.+��� $��2,0,0,0q�  MaterialDesignToolButton#���   � ToolTip$ Previous Item�� � Cursor$
 Hand��+ 	 %MaterialDesignThemes.Wpf.RippleAssist   Feedback ��  �� RelativeSource �� *Self+ �� Path$ 
Foreground�  BrushRoundConverter �� 	Converter# [ ���� *GoBackCommand+��0	 	 *MaterialDesignThemes.Wpf.PackIconExtension	   	  Kind$ 	ArrowLeft=� 	  Size 24����  ��  �� AncestorType" �$ (TextElement.Foreground)��� $��16,0,0,0q�#���  $ 	Next Item�� ��  �� *Self+$ 
Foreground�# [ ���� *GoForwardCommand+��	  $ 
ArrowRight=� 24����  �� " �$ (TextElement.Foreground)��� $
��16,0q�$��Center=�$��Center=�4
  .System.Windows.Automation.AutomationProperties 
  Name$% Material Design In XAML Toolkit��$��22(�$��AUX自动化测试软件��� $��1�� � ColumnDefinitions �� $	��260 ��� .+�6 
 0MaterialDesignExtensions.Controls.SideNavigation  -   $
��menu��#���      Items �� *NavigationItems+&   WillSelectNavigationItemCommand �� *!WillSelectNavigationItemCommand+) 	 #MaterialDesignThemes.Wpf.DialogHost  -     Name$ 
DialogHost��$��1��    DialogTheme$  Inherit=�!   
Identifier$
! Root��.���� %   TheLast.Extensions.PrismManager"   MainViewRegionName!  Prism.Regions.RegionManager#   
RegionName## Z" 