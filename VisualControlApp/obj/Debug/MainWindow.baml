   M S B A M L   `   `   `  ����K  GVisualControlApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:VisualControlAppVisualControlApp  5   	   !    VisualControlApp.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   R NOxyPlot.Wpf, Version=2.0.0.0, Culture=neutral, PublicKeyToken=75e952ba404cdbb0 oxyhttp://oxyplot.org/wpf  5   	   *localclr-namespace:VisualControlApp  5   	     � Title$  
VertexCode��5	   	   $
��Auto��6   $	��530��6*   $
��Auto��6:   $	��800��6G    � 	Resources 5         warning 	 check��%�   &	        &	 k     �� 5   
   $7��1/VisualControlApp;component/Resources/warning.png��6    $
��Auto��6[   $
��Auto��6i   �� 5   
   $5��//VisualControlApp;component/Resources/check.png��6   $
��Auto��6W   $
��Auto��6e   5      .��5      � 
������  � ColumnDefinitions 5   
   �� 5      $	��600��6   $
��auto �6.   5      �� 5      5      .+�5   
   � $��0��6   
������  � RowDefinitions 5      �� 5      $��30 �6    5      �� 5       $��30 �6    5!      �� 5"      $��50 �6    5#      �� 5$      $��* �6    $	��150��6,   5%      �� 5&      $��* �6    $	��150��6+   5'      �� 5(      $	��100 �6    5)      .+�5,      �� $��0��6   $��5��6,   $��Stretch=�5-      $	��Top=�5.       �� ShowsPreview . $��2��50      �� 52      $��0��6   $��4��6,   $��Stretch=�53      $	��Top=�54       . $��2��56      �� 59      $��Cabin��6   $��10,7,0,0q�6$   $	��Top=�66   $
��Left=�6N   $��62��6i   $��0��6t   $��0��6�   �� 5:      -   5        $��CabinNameField��5:      $��23��6,   $��77,7,0,0q�68    �� TextWrapping$ NoWrap=�6J   $�� ��6`   $	��Top=�6h   $
��Left=�6�   $	��182��6�   $��0��6�   $��0��6�   �� 5;      $��Frame��6   $
��Left=�6$   $��10,7,0,0q�6?   $	��Top=�6Q   $��62��6i   $��0��6t   $��1��6�   �� 5<      -   5        $��FrameNameField��5<      $��23��6,   $��77,7,0,0q�68   $ NoWrap=�6J   $�� ��6`   $	��Top=�6h   $
��Left=�6�   $	��182��6�   $��0��6�   $��1��6�   �� 5=      -   5         ��Name$ unlockButton��5=      $��Unlock Tables��6+   $��
10,10,0,10q�6C   $��2��6W   $	��Top=�6d   $
��Left=�6|   $	��100��6�   $��30��6�   �� 5>      -   5        $��
messageBut��5>      {�. $��30��68   $��0q�6D    J� RenderTransformOrigin$ 
5.76,0.573&�6X   $��115,10,0,10q�6{   $��2��6�   $
��Left=�6�   $��30��6�   $��0��6�   ��6�   Z� ��6�   Z� .��5?      C�  C� ResourceKey$ warning��6"   5A      &   System.Windows.Controls.DataGrid  5B      -   5        $��sectionTable��5B      $��10q�6+   $��3��67   	   CanUserSortColumns	 .  
   
IsReadOnly
 . "   VerticalScrollBarVisibility$ Visible=�6�      Columns 5C      0  *System.Windows.Controls.DataGridTextColumn  5D      ,  &System.Windows.Controls.DataGridColumn   Header$ PR��6)   1  +System.Windows.Controls.DataGridBoundColumn   Binding 65   ��  �� Path$ SectionName�  5E      $ GA��6)    65   �� $ GA�  5F      $ QT��6)    65   �� $ QT�  5G      $ MM��6)    65   �� $ Width�  5H      $ DE��6)    65   �� $ DE�  5I      $ X1��6)    65   �� $ X1�  5J      $ Y1��6)    65   �� $ Y1�  5K      $ X2��6)    65   �� $ X2�  5L      $ Y2��6)    65   �� $ Y2�  5M      $ HI��6)    65   �� $ HI�4  .System.Windows.Controls.DataGridTemplateColumn  5N      $ LR��6-      CellTemplate 5O      �� 5P      .R�5Q   "   �� $��errBut��6)   {�.  $��20��6I   $��20��6T   
������ 
������ $��0q�6�   ��5R   &   �� 5S   *    ��Name$ error��60   $7��1/VisualControlApp;component/Resources/warning.png��6?   5U   #    �� Triggers 5V   "   �� 5W   &    �� Value$ False��6[    �� Binding 62   �� $ LoadWithError�.a�5X   *   ��  �� 
TargetName$ error��61    �� Property � �� �� Value$5 //VisualControlApp;component/Resources/check.png��6V   5Y   '   �� 5Z   &   $
 True��6[    62   �� $ LoadWithError�.a�5[   *   �� $ error��61    � ��$7 1/VisualControlApp;component/Resources/warning.png��6V   �� 5\   *    6D   �� $ ExtensionsString� �@�ToolTip5]   '   5_      5a         RowStyle 5d      �� 5e       �� 
TargetType)  #System.Windows.Controls.DataGridRow"  6    �� Triggers 5f      �� 5g       6*   �� $
 Type�	 Lower6J   .a�5h   "   ��  � ��
 ��·�6Q   5i      �� 5j       6*   �� $
 Type�
 Higher6J   .a�5k   "   ��  � ��
 �����6V   5l      5n         	CellStyle 5p      �� 5q      *  $System.Windows.Controls.DataGridCell"  6    5r      �� 5s       6*   �� $ 	HaveError� True6O   .a�5t   "   ��  � y�$ Visible=�6?   6Q   5u      �� 5v       6*   �� $ 	HaveError�	 False6O   .a�5w   "   ��  � y�$ Hidden=�6?   6P   5x      5z      5}        5~      -   5        $��commandTable��5~      $��10q�6+   $��4��67   	 .  
 . $ Visible=�6r    5        5�      $
 Name��6)    68   �� $ 	Operation�  5�      $ Ordinate��6)    6<   �� $ Ordinate�5�      �� 5�      -   5        $
��Save��5�      $ 0.235,0.084&�6&   $��0��6_   $��5��6o   $
��Left=�6|   $��66��6�   $��	10,0,0,20q�6�   $��70��6�   $��Bottom=�6�   5�      � 5�   
   $��1��6    5�      �� 5�      5�      .+�5�        OxyPlot.Wpf.PlotView  -   5        $��	PlotModel��5�      $��0��6,   $��10q�69   5�      5�      5�      