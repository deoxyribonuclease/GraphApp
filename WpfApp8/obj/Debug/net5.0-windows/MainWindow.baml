   M S B A M L   `   `   `  ����  WpfApp8    WpfApp8.MainWindow   5   	   \ XPresentationFramework, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5          � Title$  Oriented Graph��5   	   $	��572��6    $	��967��6-   $	��954��69   $	��450��6H   .��5      � -   5         �Name$ matGrid��5      ���	#FFB4B4B4 � ColumnDefinitions 5   
   �� 5      $	��77* �6   �� 5      $	��29* �6    � RowDefinitions 5
   
   �� 5      $
��Auto �6   $��58��6*   �� 5      $
��285* �6   �� 5      $	��91* �6   .+�5   
   � $��0��6   ���	#FF7A7A7A$��2��63    5      �� 5      $��* �6#   �� 5      $��* �6#   �� 5      $��* �6#   .+�5      �� -   5         ��Name$ AddVertexButton��5      $��
Add Vertex��6.   $
��10,5q�6a   $��18(�6o   �� 5      -   5        $ RemoveVertexButton��5      $��Remove Vertex��61   $
��10,5q�6j   $��1��6x   $��18(�6�   d� 5      -   5         d�Name$ ToggleEdgeCreationButton��5      $��Create Edge��6=   $��	10,5,10,5q�6�   $��2��6�   $��18(�6�   5      &   System.Windows.Controls.DataGrid  5   
   -   5          Name$ matrixDataGrid��5      $��1��6,      AutoGenerateColumns$ False��69      CanUserAddRows$ False��6U   	   CanUserDeleteRows$	 False��6l   $��10,37,10,14q�6�   $��1��6�   
���   �$��24(�6�   $��Normal"�6�   $��Algerian)�6�   
   Columns
 5      0  *System.Windows.Controls.DataGridTextColumn  5      ,  &System.Windows.Controls.DataGridColumn   Header$  ��6%      FontSize$ 20Q�6Q      
FontWeight$
 Bold"�6_   1  +System.Windows.Controls.DataGridBoundColumn   Binding 60   �� *VertexNumber+6_   5       �� 5"   
   -   5         ��Name$ graphCanvas��5"      $��1��6&   $��10,10,10,10q�63   $��2��6H   ��5#      �� 5$       �� ImageSource$ /FonKurs.jpg��6   5&      �� 5'   
   -   5         ��Name$ label��5'      $��Vertex Info��6   $��2��65   $��1��6B   $��20(�5(      $��Center=�6   $��Center=�63   $��	0,0,10,10q�5)      �� 5*   
   -	   5        $ label2��5*      $��Adjacency Matrix��6    $��1��6;   $��1��6H   $��20(�5+      $��Center=�6   $	��Top=�63   $��	0,0,10,10q�5,      5-      5.      